namespace BonGames.EasyBuilder
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using UnityEngine;
    using BonGames.Tools;

    public enum EEnvironment
    {
        Debug,
        /// <summary> All features/ cheat/ tools are unclocked</summary>
        Development,
        /// <summary> Same as the Development build but the Staging is the most stable version during development phase</summary>
        Staging,
        /// <summary> Release build basically all development features will be disabled, it same as the build to end user</summary>
        Release,
        /// <summary> Build is used to upload to store/distribute. Functional is the same as Release </summary>
        Distribution
    }

    public enum EAppTarget
    {
        Client,
        Server,
    }

    public static class EasyBuilder
    {
        public const string Tag = "[" + nameof(EasyBuilder) + "]";
        public const string MenuParent = "Bon Games";
        public const string MenuRunTools = "Bon Games/Run";

        public static void LogI(string message) => BonGames.Tools.Domain.LogI(message);
        public static void LogW(string message) => BonGames.Tools.Domain.LogW(message);
        public static void LogE(string message) => BonGames.Tools.Domain.LogE(message);

        public static void BuildPlayer(BuildPlayerOptions buildPlayerOptions)
        {
            string buildLocation = buildPlayerOptions.locationPathName + "/..";
            BonGames.Tools.UniIO.DeleteFolderIfExist(buildLocation, true);
            BonGames.Tools.UniIO.CreateFolderIfNotExist(buildLocation);
            EditorUserBuildSettings.SwitchActiveBuildTarget(buildPlayerOptions.targetGroup, buildPlayerOptions.target);

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            LogI($"{Tag} Build result {report.summary.result} with duration {report.summary.totalTime}");
        }

        public static void BuildPlayer(EAppTarget appTarget, BuildTarget target, EEnvironment buildEvn)
        {
            BuildPlayerOptions options = CreateBuildPlayerOptions(appTarget, target, buildEvn);
            List<string> allInternalSymbols = BuilderUtils.GetAllScriptSymbols();
            Debug.Log($"{string.Join("\n", allInternalSymbols)}");
            /*
                        PlayerSettings.SetScriptingBackend(options.targetGroup, ScriptingImplementation.IL2CPP);
                        PlayerSettings.SetAdditionalIl2CppArgs("--generic-virtual-method-iterations=1");
            #if UNITY_2022_1_OR_NEWER
                        if (appTarget == EAppTarget.Client)
                        {
                            if (target == BuildTarget.StandaloneWindows)
                            {
                                PlayerSettings.SetIl2CppCodeGeneration(UnityEditor.Build.NamedBuildTarget.Standalone, UnityEditor.Build.Il2CppCodeGeneration.OptimizeSize);
                            }
                            else if (target == BuildTarget.Android)
                            {
                                PlayerSettings.SetIl2CppCodeGeneration(UnityEditor.Build.NamedBuildTarget.Android, UnityEditor.Build.Il2CppCodeGeneration.OptimizeSize);
                            }
                        }
                        else
                        {
                            PlayerSettings.SetIl2CppCodeGeneration(UnityEditor.Build.NamedBuildTarget.Server, UnityEditor.Build.Il2CppCodeGeneration.OptimizeSize);
                        }
            #endif
            */
            // BuildPlayer(options);
        }

        public static BuildPlayerOptions CreateBuildPlayerOptions(EAppTarget appTarget, BuildTarget target, EEnvironment buildEvn)
        {
            string buildLocation = GetPlatformBuildFolder(target, appTarget);
            BuildPlayerOptions buildPlayerOptions = GetDefaultBuildPlayerOptions(appTarget, buildEvn);
            buildPlayerOptions.locationPathName = Path.Combine(buildLocation, GetDefaultProductName() + BuilderUtils.GetBuildTargetAppExtension(target));
            buildPlayerOptions.targetGroup = GetBuildTargetGroup(target);
            buildPlayerOptions.subtarget = GetSubBuildTarget(appTarget, target);
            buildPlayerOptions.scenes = GetActiveScenes();
            buildPlayerOptions.target = target;

            return buildPlayerOptions;
        }

        public static BuildPlayerOptions GetDefaultBuildPlayerOptions(EAppTarget appTarget, EEnvironment buildEnv)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

            BuildOptions options = BuildOptions.None;
            List<string> defines = null;
            switch (buildEnv)
            {
                case EEnvironment.Development:
                    {
                        options = BuildOptions.Development;
                        defines = new List<string>()
                        {
                            BuildDefines.EnableLog,
                            BuildDefines.DevelopmentBuild,
                            BuildDefines.InternalBuild,
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
                        };
                    }
                    break;
                case EEnvironment.Release:
                    {
                        defines = new List<string>()
                        {
                            BuildDefines.ReleaseBuild,
                        };
                    }
                    break;
            }

            string[] currentSymbols = BuilderUtils.GetActiveBuildTargetDefineSymbols();
            List<string> internalDefineSymbols = BuilderUtils.GetAllScriptSymbols();

            for (int i = 0; i < currentSymbols.Length; i++)
            {
                string symbol = currentSymbols[i];
                // Ignore interal symbol as them will be added/re-added by internal logic
                if (string.IsNullOrEmpty(symbol) || internalDefineSymbols.Contains(symbol)) continue;

                // Keep the external ones such as DOTWEEN
                defines.Add(symbol);
            }

            buildPlayerOptions.options = options;
            buildPlayerOptions.extraScriptingDefines = defines.ToArray();
            return buildPlayerOptions;
        }

        private static BuildTargetGroup GetBuildTargetGroup(BuildTarget target)
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
                    throw new System.Exception($"{Tag} The build target {target} is not supported to get group");
            }
        }

        private static string[] GetActiveScenes()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            List<string> activeScenes = new List<string>();
            for (int i = 0; i < scenes.Length; i++)
            {
                EditorBuildSettingsScene sceneSettings = scenes[i];
                if (sceneSettings.enabled)
                {
                    activeScenes.Add(sceneSettings.path);
                }
            }
            return activeScenes.ToArray();
        }

        private static int GetSubBuildTarget(EAppTarget appTarget, BuildTarget buildTarget)
        {
            string buildTargetName = buildTarget.ToString();
            if (buildTargetName.StartsWith("Standalone"))
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
                LogI($"{Tag} The build target {buildTarget} is not supported sub-target");
            }
            return -1;
        }

        public static string GetRootBuiltFolder()
        {
            string rootFolder = System.IO.Path.Combine(Application.dataPath, "../../Build");
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
                default: throw new System.Exception($"{Tag} The target built folder is not supported yet target:{platformTarget}");
            }

            return outPath;
        }

        public static string GetDefaultProductName()
        {
            return Application.productName;
        }

        [MenuItem(MenuRunTools + "Build/Client/Windows64")]
        public static void BuildClientWindows64()
        {
            BuildPlayer(EAppTarget.Client, BuildTarget.StandaloneWindows64, EEnvironment.Development);
        }

        [MenuItem(MenuRunTools + "Build/Client/Windows64 Testnet")]
        public static void BuildClientWindows64TestNet()
        {
            BuildPlayerOptions options = CreateBuildPlayerOptions(EAppTarget.Client, BuildTarget.StandaloneWindows64, EEnvironment.Development);
            options.scenes = new string[] { @"Assets/BlueEye.Thirteen/Scenes/TestNet.unity" };
            BuildPlayer(options);
        }

        [MenuItem(MenuRunTools + "Build/Client/MacOS Testnet")]
        public static void BuildClientOSXTestnet()
        {
            BuildPlayerOptions options = CreateBuildPlayerOptions(EAppTarget.Client, BuildTarget.StandaloneOSX, EEnvironment.Development);
            options.scenes = new string[] { @"Assets/BlueEye.Thirteen/Scenes/TestNet.unity" };
            BuildPlayer(options);
        }


        [MenuItem(MenuRunTools + "Build/Client/Android")]
        public static void BuildClientAndroid()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.Android, EEnvironment.Release).Build();
        }

        [MenuItem(MenuRunTools + "Build/Client/iOS")]
        public static void BuildIOS()
        {
            BuildPlayer(EAppTarget.Client, BuildTarget.iOS, EEnvironment.Staging);
        }


        [MenuItem(MenuRunTools + "Build/Server/Windows64")]
        public static void BuildServerWindows64()
        {
            BuildPlayer(EAppTarget.Server, BuildTarget.StandaloneWindows64, EEnvironment.Staging);
        }

        [MenuItem(MenuRunTools + "Build/Server/OSX Intel")]
        public static void BuildServerOSX()
        {
            BuildPlayer(EAppTarget.Server, BuildTarget.StandaloneOSX, EEnvironment.Staging);
        }

        [MenuItem(MenuRunTools + "Build/Server/Linux64")]
        public static void BuildServerLinux64()
        {
            BuildPlayerOptions options = CreateBuildPlayerOptions(EAppTarget.Server, BuildTarget.StandaloneLinux64, EEnvironment.Staging);
            options.scenes = new string[]
            {
                @"Assets/BlueEye.Thirteen/Scenes/Init.unity",
                @"Assets/BlueEye.Thirteen/Scenes/Net.unity"
            };
            BuildPlayer(options);
        }


        [MenuItem(MenuRunTools + "Build/All/Win64 Client-Server")]
        public static void BuildAllClientServerWindows64()
        {
            BuildServerWindows64();
            BuildClientWindows64();
        }


        [MenuItem(MenuRunTools + "Build/All/Win64 Client-Server Testnet")]
        public static void BuildAllClientServerWindows64Testnet()
        {
            BuildServerWindows64();
            BuildClientWindows64TestNet();
        }


        public static void Build()
        {
            try
            {
                if (!Application.isBatchMode) throw new System.Exception("This method only supports batmode for now");

                if (!System.Enum.TryParse<EEnvironment>(EnvironmentArguments.GetEnvironmentArgument(BuildArguments.Key.BuildEnvironment), true, out EEnvironment env))
                    throw new System.Exception($"Build Environment is invalid with value {EnvironmentArguments.GetEnvironmentArgument(BuildArguments.Key.BuildEnvironment)}");


                if (!System.Enum.TryParse<EAppTarget>(EnvironmentArguments.GetEnvironmentArgument(BuildArguments.Key.BuildAppTarget), true, out EAppTarget appTarget))
                    throw new System.Exception($"App Target is invalid with value {EnvironmentArguments.GetEnvironmentArgument(BuildArguments.Key.BuildAppTarget)}");


                if (!System.Enum.TryParse<BuildTarget>(EnvironmentArguments.GetEnvironmentArgument(BuildArguments.Key.BuildPlatformTarget), true, out BuildTarget buildTarget))
                    throw new System.Exception($"Build Platform Target is invalid with value {EnvironmentArguments.GetEnvironmentArgument(BuildArguments.Key.BuildPlatformTarget)}");

                ProjectBuilder.CreateBuilder(appTarget, buildTarget, env).Build();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Build Failed with Exception\n{e}");
            }
            finally
            {
                EditorApplication.Exit(0);
            }
        }
    }
}