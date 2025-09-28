namespace BonGames.EasyBuilder
{
    using UnityEditor;
    using System.Collections.Generic;
  using BonGames.Tools;
  using BonGames.Tools.Enum;

  public static class ProjectSwitcher
    {
        public static void ClearScriptingDefineSymbols()
        {
            BuilderUtils.SetScriptingDefineSymbolsToActiveBuildTarget(new string[] { });
        }

        public static void SetEditorRunAsRemoteClient()
        {
            BuildPlayerOptions op = BuilderUtils.GetDefaultBuildPlayerOptions(EAppTarget.Client, EEnvironment.Staging);
            BuilderUtils.SetScriptingDefineSymbolsToActiveBuildTarget(op.extraScriptingDefines);
        }

        public static void SetEditorRunAsLocalClient()
        {
            ClearScriptingDefineSymbols();
        }

        public static void SetEditorRunAsLocalServer()
        {
            BuildPlayerOptions op = BuilderUtils.GetDefaultBuildPlayerOptions(EAppTarget.Server, EEnvironment.Staging);            
            List<string> defines = new List<string>(op.extraScriptingDefines);
            for (int i = defines.Count - 1; i >= 0; i--)
            {
                if (defines[i] == BuildDefines.EnableUnityMultiplayAgent ||
                    defines[i] == BuildDefines.EnableHostingAgent)
                {
                    defines.RemoveAt(i);
                }
            }
            BuilderUtils.SetScriptingDefineSymbolsToActiveBuildTarget(defines.ToArray());
        }
    
        public static void SetEditorRunAsOffline()
        {
            BuilderUtils.SetScriptingDefineSymbolsToActiveBuildTarget(new string[] { BuildDefines.OfflineGameMode });
        }
    }
}