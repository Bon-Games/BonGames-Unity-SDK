namespace BonGames.EasyBuilder
{
    using UnityEditor;
    using UnityEngine;
    using BonGames.Tools;
    using System.Text;
    using BonGames.Tools.Enum;
    using BonGames.CommandLine;
#if UNITY_ADDRESSABLE
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;
#endif

  public static class EasyBuilder
    {
        public const string Tag = "[" + nameof(EasyBuilder) + "]";
        public const string MenuParent = "BonGames";
        public const string MenuRunTools = "BonGames/Run";

        public static void LogI(string message) => BonGames.Tools.Domain.LogI(message);
        public static void LogW(string message) => BonGames.Tools.Domain.LogW(message);
        public static void LogE(string message) => BonGames.Tools.Domain.LogE(message);

        [MenuItem(MenuRunTools + "/Build/Android/Distribution")]
        public static void BuildAndroidDistribution()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.Android, EEnvironment.Distribution).Build();
        }

        [MenuItem(MenuRunTools + "/Build/Android/Release")]
        public static void BuildAndroidRelease()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.Android, EEnvironment.Release).Build();
        }

        [MenuItem(MenuRunTools + "/Build/Android/Staging")]
        public static void BuildAndroidStaging()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.Android, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/Android/Development")]
        public static void BuildAndroidDevelopment()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.Android, EEnvironment.Development).Build();
        }

        [MenuItem(MenuRunTools + "/Build/iOS/Release")]
        public static void BuildIOSRelease()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.iOS, EEnvironment.Release).Build();
        }

        [MenuItem(MenuRunTools + "/Build/iOS/Staging")]
        public static void BuildIOSStaging()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.iOS, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/iOS/Development")]
        public static void BuildIOSDevelopment()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.iOS, EEnvironment.Development).Build();
        }

        [MenuItem(MenuRunTools + "/Build/Windows/Development")]
        public static void BuildWindowsDevelopment()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.StandaloneWindows, EEnvironment.Development).Build();
        }

        [MenuItem(MenuRunTools + "/Build/Windows/Staging")]
        public static void BuildWindowsStaging()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.StandaloneWindows, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/Windows/Release")]
        public static void BuildWindowsRelease()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.StandaloneWindows, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/MacOS/Development")]
        public static void BuildMacOSDevelopment()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.StandaloneOSX, EEnvironment.Development).Build();
        }

        [MenuItem(MenuRunTools + "/Build/MacOS/Staging")]
        public static void BuildMacOSStaging()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.StandaloneOSX, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/MacOS/Release")]
        public static void BuildMacOSRelease()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.StandaloneOSX, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/Linux/Development")]
        public static void BuildLinuxDevelopment()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.StandaloneLinux64, EEnvironment.Development).Build();
        }

        [MenuItem(MenuRunTools + "/Build/Linux/Staging")]
        public static void BuildLinuxStaging()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.StandaloneLinux64, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/Linux/Release")]
        public static void BuildLinuxRelease()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.StandaloneLinux64, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/ServerLinux/Development")]
        public static void BuildServerLinuxDevelopment()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Server, BuildTarget.StandaloneLinux64, EEnvironment.Development).Build();
        }

        [MenuItem(MenuRunTools + "/Build/ServerLinux/Staging")]
        public static void BuildServerLinuxStaging()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Server, BuildTarget.StandaloneLinux64, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/ServerLinux/Release")]
        public static void BuildServerLinuxRelease()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Server, BuildTarget.StandaloneLinux64, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/ServerWindows/Development")]
        public static void BuildServerWindowsDevelopment()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Server, BuildTarget.StandaloneWindows, EEnvironment.Development).Build();
        }

        [MenuItem(MenuRunTools + "/Build/ServerWindows/Staging")]
        public static void BuildServerWindowsStaging()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Server, BuildTarget.StandaloneWindows, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/ServerWindows/Release")]
        public static void BuildServerWindowsRelease()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Server, BuildTarget.StandaloneWindows, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/ServerMacOS/Development")]
        public static void BuildServerMacOSDevelopment()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Server, BuildTarget.StandaloneOSX, EEnvironment.Development).Build();
        }

        [MenuItem(MenuRunTools + "/Build/ServerMacOS/Staging")]
        public static void BuildServerMacOSStaging()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Server, BuildTarget.StandaloneOSX, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/ServerMacOS/Release")]
        public static void BuildServerMacOSRelease()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Server, BuildTarget.StandaloneOSX, EEnvironment.Staging).Build();
        }


        [MenuItem(MenuRunTools + "/Dlc/Build For Active Target")]
        public static void AddressableBuild()
        {
            DlcBuilder.CreateBuilder(BuilderUtils.GetActiveBuildTarget(), EEnvironment.Development, "Local", null);
        }

        public static void Build()
        {
            bool isSuccess = false;
            try
            {
                if (!Application.isBatchMode) throw new System.Exception("This method only supports batmode for now");

                if (!System.Enum.TryParse<EEnvironment>(ArgumentsResolver.GetEnvironmentArgument(BuildArguments.Key.BuildEnvironment), true, out EEnvironment env))
                    throw new System.Exception($"Build Environment is invalid with value {ArgumentsResolver.GetEnvironmentArgument(BuildArguments.Key.BuildEnvironment)}");


                if (!System.Enum.TryParse<EAppTarget>(ArgumentsResolver.GetEnvironmentArgument(BuildArguments.Key.BuildAppTarget), true, out EAppTarget appTarget))
                    throw new System.Exception($"App Target is invalid with value {ArgumentsResolver.GetEnvironmentArgument(BuildArguments.Key.BuildAppTarget)}");


                if (!System.Enum.TryParse<BuildTarget>(ArgumentsResolver.GetEnvironmentArgument(BuildArguments.Key.BuildPlatformTarget), true, out BuildTarget buildTarget))
                    throw new System.Exception($"Build Platform Target is invalid with value {ArgumentsResolver.GetEnvironmentArgument(BuildArguments.Key.BuildPlatformTarget)}");

                // If report is Null, then Player Build was intended to be ignored
                // So ONLY error could be arised in that case is Dlc building process which would throw an exception if an error occurs, then the catch block would be invoke
                UnityEditor.Build.Reporting.BuildReport report = ProjectBuilder.CreateBuilder(appTarget, buildTarget, env).Build();
                isSuccess = !BuildArguments.IsPlayerBuildEnable() || report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded;
                if (isSuccess)
                {
                    LogI("Build process completed without error");
                }
                else
                {
                    StringBuilder stepMessageLogger = new();
                    if (report.steps != null && report.steps.Length > 0)
                    {
                        stepMessageLogger.AppendLine("Build Steps:");
                        for (int i = 0; i < report.steps.Length; i++)
                        {
                            stepMessageLogger.AppendLine(report.steps[i].name);
                            stepMessageLogger.AppendLine(string.Join("\n", report.steps[i].messages));
                        }
                    }
                    else
                    {
                        stepMessageLogger.AppendLine("No step was executed ??");
                    }
                    throw new System.Exception($"Build Failed totalErrors: {report.summary.totalErrors}\n{stepMessageLogger.ToString()}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Build Failed with Exception\n{e}");
                EditorApplication.Exit(1);
            }
            
            EditorApplication.Exit(isSuccess ? 0 : 1);
        }
    }
}