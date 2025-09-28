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

#if UNITY_ADDRESSABLE
        [MenuItem(MenuRunTools + "/Dlc/Build")]
        public static void AddressableBuild()
        {
            AddressableAssetSettings.BuildPlayerContent();            
        }

        public static void TestAddressable()
        {
            string buildProfileId = AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetProfileId("Remote");
            AddressableAssetSettingsDefaultObject.Settings.profileSettings.SetValue(buildProfileId, "Remote.BuildPath", "abc");
            EditorUtility.SetDirty(AddressableAssetSettingsDefaultObject.Settings);
        }
#endif

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