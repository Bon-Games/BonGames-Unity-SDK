namespace BonGames.EasyBuilder
{
    using UnityEditor;
    using BonGames.Tools;
    using BonGames.EasyBuilder.Enum;
    using BonGames.CommandLine;
    using System.Collections.Generic;
    using System;

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

        public static bool SetScriptSymbolsTo(EAppTarget app, EEnvironment env)
        {
            bool setRes = true;
            try
            {
                string buildProfileFilePath = BuilderUtils.GetActiveBuildProfileFilePath(env);
                Dictionary<string, string> targetArgs = ArgumentsResolver.LoadArguments(buildProfileFilePath);
                BuildPlayerOptions op = BuilderUtils.GetDefaultBuildPlayerOptions(app, env);
                List<string> symbols = new List<string>(op.extraScriptingDefines);
                if (targetArgs.TryGetValue(nameof(Argument.BuildArguments.Key.AdditionalSymbols), out string addSymbols) && !string.IsNullOrEmpty(addSymbols))
                {
                    symbols.AddRange(addSymbols.Split(';'));
                }
                Domain.LogI($"[ProjectSwitcher] Set Script Symbols To:\n{string.Join(";", symbols)}");
                BuilderUtils.SetScriptingDefineSymbolsToActiveBuildTarget(symbols.ToArray());
                AssetDatabase.Refresh();
            }
            catch (System.Exception e)
            {
                setRes = false;
                Domain.LogE($"[ProjectSwitcher] Set Script Symbols Error:\n{e}");
            }
            return setRes;
        }
    }
}