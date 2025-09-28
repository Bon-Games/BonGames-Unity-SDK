namespace BonGames.EasyBuilder
{
    using UnityEditor;
    using System.Collections.Generic;
    using BonGames.Tools;
    using BonGames.Tools.Enum;

    public static class ProjectSwitcher
    {
        public static bool SwitchTo(EAppTarget app, EEnvironment env, BuildTarget buildTarget)
        {
            bool switchRes = true;
            try
            {
                BuildTargetGroup buildGroup = BuilderUtils.GetBuildTargetGroup(buildTarget);
                if (buildGroup != BuilderUtils.GetActiveBuildTargetGroup() || buildTarget != BuilderUtils.GetActiveBuildTarget())
                {
                    EasyBuilder.LogI($"Switching build target to BuildGroup:{buildGroup} BuildTarget:{buildTarget}");
                    switchRes = EditorUserBuildSettings.SwitchActiveBuildTarget(buildGroup, buildTarget);
                }

                if (BuilderUtils.IsStandalone(buildTarget))
                {
                    EditorUserBuildSettings.standaloneBuildSubtarget = app == EAppTarget.Server ? StandaloneBuildSubtarget.Server : StandaloneBuildSubtarget.Player;
                }
            }
            catch (System.Exception e)
            {
                switchRes = false;
                Domain.LogE($"[ProjectSwitcher] SwitchTo {buildTarget} Error:\n{e}");
            }
            return switchRes;
        }

        public static bool SetDefaultScriptSymbols(EAppTarget app, EEnvironment env)
        {
            bool setRes = true;
            try
            {
                BuildPlayerOptions op = BuilderUtils.GetDefaultBuildPlayerOptions(app, env);
                BuilderUtils.SetScriptingDefineSymbolsToActiveBuildTarget(op.extraScriptingDefines);
                AssetDatabase.Refresh();
            }
            catch (System.Exception e)
            {
                setRes = false;
                Domain.LogE($"[ProjectSwitcher] SetDefaultScriptSymbols Error:\n{e}");
            }
            return setRes;
        }
    }
}