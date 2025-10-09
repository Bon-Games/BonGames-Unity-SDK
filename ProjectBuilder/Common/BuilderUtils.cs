namespace BonGames.EasyBuilder
{
    using UnityEditor;
    using UnityEditor.Build;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BonGames.EasyBuilder.Enum;
    using BonGames.EasyBuilder.Argument;
  using BonGames.CommandLine;

  public static class BuilderUtils
    {
        private const string Tag = "[" + nameof(BuilderUtils) + "]";

        public static string GetBuildTargetAppExtension(BuildTarget target, EEnvironment env)
        {
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return ".exe";
                case BuildTarget.Android:
                    return (env == EEnvironment.Distribution || BuildArguments.Android.IsBuildAAB()) ? ".aab" : ".apk";
                case BuildTarget.StandaloneOSX:
                    return ".app";
                case BuildTarget.StandaloneLinux64:
                    return ".x86_64";
                case BuildTarget.iOS:
                    return string.Empty; // to be exported to xcode project, not a file
            }

            EasyBuilder.LogE($"{Tag} Get extension for build target {target} is not supported yet");
            return string.Empty;
        }

        public static NamedBuildTarget GetNamedBuildTarget(EAppTarget appTarget, BuildTarget target)
        {
            NamedBuildTarget result = default;
            if (appTarget == EAppTarget.Server)
            {
                result = NamedBuildTarget.Server;
            }
            else
            {
                if (IsStandalone(target))
                {
                    result = NamedBuildTarget.Standalone;
                }
                else if (target == BuildTarget.iOS)
                {
                    result = NamedBuildTarget.iOS;
                }
                else if (target == BuildTarget.Android)
                {
                    result = NamedBuildTarget.Android;
                }
                else if (target == BuildTarget.WebGL)
                {
                    result = NamedBuildTarget.WebGL;
                }
                else
                {
                    EasyBuilder.LogE($"{Tag} {target} is not supported to get named build target");
                }
            }
            return result;
        }

        public static BuildTarget GetActiveBuildTarget()
        {
            return EditorUserBuildSettings.activeBuildTarget;
        }

        public static BuildTargetGroup GetActiveBuildTargetGroup()
        {
            return EditorUserBuildSettings.selectedBuildTargetGroup;
        }

        public static NamedBuildTarget GetNamedBuildTargetForActiveBuildTarget()
        {
            return GetNamedBuildTarget(GetActiveAppTarget(), GetActiveBuildTarget());
        }

        public static bool IsStandalone(BuildTarget buildTarget)
        {
            string buildTargetName = buildTarget.ToString();
            return buildTargetName.StartsWith("Standalone");
        }

        public static EAppTarget GetActiveAppTarget()
        {
            bool isStandalone = IsStandalone(EditorUserBuildSettings.activeBuildTarget);
            if (!isStandalone)
            {
                return EAppTarget.Client;
            }

            bool isServerSubTarget = EditorUserBuildSettings.standaloneBuildSubtarget == StandaloneBuildSubtarget.Server;
            bool isDedicatedServer = isServerSubTarget && isStandalone;
            return isDedicatedServer ? EAppTarget.Server : EAppTarget.Client;
        }

        public static void SetScriptingDefineSymbols(NamedBuildTarget namedTarget, string[] defines)
        {
            PlayerSettings.SetScriptingDefineSymbols(namedTarget, defines);
        }

        public static void SetScriptingDefineSymbolsToActiveBuildTarget(string[] defines)
        {
            PlayerSettings.SetScriptingDefineSymbols(GetNamedBuildTargetForActiveBuildTarget(), defines != null ? defines : new string[0]);
        }

        public static string[] GetActiveBuildTargetDefineSymbols()
        {
            NamedBuildTarget target = GetNamedBuildTargetForActiveBuildTarget();
            PlayerSettings.GetScriptingDefineSymbols(target, out string[] defines);
            return defines;
        }

        public static string GetRootBuiltFolder()
        {
            string rootFolder = System.IO.Path.Combine(UnityEngine.Application.dataPath, "..", "Build");
            return rootFolder;
        }

        public static string GetPlatformBuildFolder(BuildTarget platformTarget, EAppTarget appTarget)
        {
            string root = GetRootBuiltFolder();
            string outPath = null;
            switch (appTarget)
            {
                case EAppTarget.Client:
                    outPath = System.IO.Path.Combine(root, "Client");
                    break;
                case EAppTarget.Server:
                    outPath = System.IO.Path.Combine(root, "Server");
                    break;
                default:
                    throw new System.Exception($"The app tartget {appTarget} is not supported");
            }

            switch (platformTarget)
            {
                case BuildTarget.Android:
                    outPath = System.IO.Path.Combine(outPath, "android");
                    break;
                case BuildTarget.iOS:
                    outPath = System.IO.Path.Combine(outPath, "iOS");
                    break;
                case BuildTarget.StandaloneWindows64:
                    outPath = System.IO.Path.Combine(outPath, "windows64");
                    break;
                case BuildTarget.StandaloneOSX:
                    outPath = System.IO.Path.Combine(outPath, "osx-intel64");
                    break;
                case BuildTarget.StandaloneLinux64:
                    outPath = System.IO.Path.Combine(outPath, "linux64");
                    break;
                default: throw new System.Exception($"The target built folder is not supported yet target:{platformTarget}");
            }

            return outPath;
        }

        public static string GetDefaultProductName()
        {
            return UnityEngine.Application.productName;
        }

        public static string GetProductName()
        {
            string overrideProdName = BuildArguments.GetProductName();
            return string.IsNullOrEmpty(overrideProdName) ? GetDefaultProductName() : overrideProdName;
        }

        public static EEnvironment GetBuildEnvironment()
        {
            if (System.Enum.TryParse<EEnvironment>(BonGames.CommandLine.ArgumentsResolver.GetEnvironmentArgument(BuildArguments.Key.BuildEnvironment), true, out EEnvironment env))
            {
                return env;
            }
            return EEnvironment.Development;
        }

        public static BuildTargetGroup GetBuildTargetGroup(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                case BuildTarget.iOS:
                    return BuildTargetGroup.iOS;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneOSX:
                case BuildTarget.StandaloneLinux64:
                    return BuildTargetGroup.Standalone;
                default:
                    throw new System.Exception($"The build target {target} is not supported to get group");
            }
        }

        public static int GetSubBuildTarget(EAppTarget appTarget, BuildTarget buildTarget)
        {
            if (BuilderUtils.IsStandalone(buildTarget))
            {
                switch (appTarget)
                {
                    case EAppTarget.Server:
                        return (int)StandaloneBuildSubtarget.Server;
                    case EAppTarget.Client:
                        return (int)StandaloneBuildSubtarget.Player;
                }
            }
            else
            {
                UnityEngine.Debug.Log($"The build target {buildTarget} is not supported sub-target");
            }
            return -1;
        }

        public static List<string> GetAllScriptSymbols()
        {
            return typeof(BuildDefines)
                .GetFields(BindingFlags.Default | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Select(p => p?.GetRawConstantValue().ToString())
                .Where(symbol => !string.IsNullOrEmpty(symbol))
                .ToList();
        }

        public static string BuildInformationDirectory()
        {
            string dir = System.IO.Path.Combine(UnityEngine.Application.dataPath, "..", "BuildInformation");
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            return dir;
        }

        public static string BuildCacheDirectory()
        {
            return System.IO.Path.Combine(UnityEngine.Application.dataPath, "..", "BuildCache");
        }

        public static string BuildProfileDirectory()
        {
            string dir = System.IO.Path.Combine(BuildInformationDirectory(), "BuildProfiles");
            if (!System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }
            return dir;
        }

        public static string GetActiveBuildProfileFilePath(EEnvironment env)
        {
            string envBasedFile = System.IO.Path.Combine(BuildProfileDirectory(), $".args.{env}".ToLower());
            if (System.IO.File.Exists(envBasedFile))
            {
                return envBasedFile;
            }
            return ArgumentsResolver.DefaultArgumentsFilePath();
        }

        public static List<string> GetDefaultScriptSymbols(EAppTarget appTarget, BuildTarget buildTarget, EEnvironment buildEnv)
        {
            NamedBuildTarget namedBuildTarget = BuilderUtils.GetNamedBuildTarget(appTarget, buildTarget);
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] currentSymbols);

            BuildOptions options = BuildOptions.None;
            List<string> defines = null;
            switch (buildEnv)
            {
                case EEnvironment.Debug:
                case EEnvironment.Development:
                    {
                        options = BuildOptions.Development;
                        defines = new List<string>()
                        {
                            BuildDefines.EnableLog,
                            BuildDefines.DevelopmentBuild,
                            BuildDefines.InternalBuild,
                            BuildDefines.DebugLog,
                        };
                    }
                    break;
                case EEnvironment.Staging:
                    {
                        defines = new List<string>()
                        {
                            BuildDefines.EnableLog,
                            BuildDefines.StagingBuild,
                            BuildDefines.InternalBuild,
                            BuildDefines.DebugLog,
                        };
                    }
                    break;
                case EEnvironment.Release:
                case EEnvironment.Distribution:
                    {
                        options |= BuildOptions.CleanBuildCache;
                        defines = new List<string>()
                        {
                            BuildDefines.ReleaseBuild,
                        };
                    }
                    break;
            }

            if (appTarget == EAppTarget.Server)
            {
                defines.Add(BuildDefines.DedicatedServerBuild);
            }

            if (currentSymbols != null && currentSymbols.Length > 0)
            {
                List<string> allInternalSymbols = BuilderUtils.GetAllScriptSymbols();

                for (int i = 0; i < currentSymbols.Length; i++)
                {
                    string symbol = currentSymbols[i];

                    if (allInternalSymbols.Contains(symbol))
                        continue; // Ingnore Internal Symbols

                    // Keep the symbols from external
                    defines.Add(symbol);
                }
            }
            
            return defines;
        }

        public static BuildPlayerOptions GetDefaultBuildPlayerOptions(EAppTarget appTarget, EEnvironment buildEnv)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            BuildOptions options = BuildOptions.None;
            List<string> defines = GetDefaultScriptSymbols(appTarget, GetActiveBuildTarget(), buildEnv);   
            buildPlayerOptions.options = options;
            buildPlayerOptions.extraScriptingDefines = defines.ToArray();
            return buildPlayerOptions;
        }

        public static string Shorten(this EEnvironment env)
        {
            switch (env)
            {
                case EEnvironment.Debug: return "debug";
                case EEnvironment.Development: return "dev";
                case EEnvironment.Staging: return "stg";
                case EEnvironment.Release: return "release";
                case EEnvironment.Distribution: return "dist";
                default: return "undefined";
            }
        }

        public static string ShortenSimplified(this EEnvironment env)
        {
            switch (env)
            {
                case EEnvironment.Debug:
                case EEnvironment.Development:
                    return "dev";
                case EEnvironment.Staging:
                    return "stg";
                case EEnvironment.Release:
                case EEnvironment.Distribution:
                    return "release";
                default: return "undefined";
            }
        }

        public static string GetOutputFileName(string defValue)
        {
            string nameWithtouExtension = BuildArguments.GetOutputFileName();
            return string.IsNullOrEmpty(nameWithtouExtension) ? defValue : nameWithtouExtension;
        }
    }
}
