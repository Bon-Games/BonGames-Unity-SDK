using BonGames.Tools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BonGames.EasyBuilder.Enum;
using UnityEditor;
using BonGames.EasyBuilder.Argument;

#if UNITY_ADDRESSABLE
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
#endif

namespace BonGames.EasyBuilder
{
    public abstract partial class ProjectBuilder : IProjectBuilder
    {
        public virtual IPreProcessBuildWithReportPreset PreProcessBuildWithReport { get; protected set; }

        public virtual IPostProcessBuildWithReportPreset PostProcessingWithReport { get; protected set; }

        public virtual IPreBuildProcess PreBuildProcess { get; protected set; }

        public virtual IPostBuildProcess PostBuildProcess { get; protected set; }

        public EAppTarget AppTarget { get; private set; }

        public BuildTarget BuildTarget { get; private set; }

        public EEnvironment Environment { get; private set; }

        protected string BuildInformationDirectory => BuilderUtils.BuildInformationDirectory();

        protected virtual List<string> PlatformSpecifiedSymbols { get; private set; } = new List<string>()
        {
            BuildDefines.EnableIL2CppScriptBackend,
        };

        protected BuildVersion Version { get; } = new BuildVersion();

        public BuildPlayerOptions BuildPlayerOptions { get; set; }

        public ProjectBuilder(EAppTarget appTarget, BuildTarget buildTarget, EEnvironment environment)
        {
            AppTarget = appTarget;
            BuildTarget = buildTarget;
            Environment = environment;
        }


        public UnityEditor.Build.Reporting.BuildReport Build()
        {
            // Switch to build target
            if (!ProjectSwitcher.SwitchTo(AppTarget, Environment, BuildTarget))
            {
                BonGames.Tools.Domain.ThrowIf(true, $"Failed to switch AppTarget:{AppTarget} BuildTarget:{BuildTarget} Environment:{Environment}");
            }

            Prepare();
            string buildProfileFilePath = BuilderUtils.GetBuildProfileFilePath(Environment);
            if (!UnityEngine.Application.isBatchMode && !string.IsNullOrEmpty(buildProfileFilePath))
            {
                // Uses .args.[Environment] as default
                BonGames.CommandLine.ArgumentsResolver.Load(buildProfileFilePath);
            }
            else
            {
                // Uses .args.default as default
                BonGames.CommandLine.ArgumentsResolver.Load();
            }

            // Create build options
            Version.LoadVersion();
            BuildPlayerOptions = CreateBuildPlayerOptions();
            Domain.ThrowIf(!BuildPlayerOptions.scenes.Any(), "There's no scene inlcuded in the build, ensure you have at least 1 scene in either ProjectSettings or passed argument");

            // Pre Build
            if (PreBuildProcess != null)
            {
                PreBuildProcess.OnPreBuild(this);
            }

            // Setup general product info
            SetProductInformation();

            // Behide the scene informations
            SetupInternally();

            // Clean up current symbols, use buildPlayerOptions.extraScriptingDefines for testing or does not really create an impact            
            BuilderUtils.SetScriptingDefineSymbolsToActiveBuildTarget(BuildPlayerOptions.extraScriptingDefines);

            // Build DLC

            if (BuildArguments.IsDlcBuildEnable())
            {
                string dlcDestination = BuildArguments.GetDlcDestination();
                string profileName = BuildArguments.GetDlcProfileName("Remote") ?? string.Empty;
                bool dlcBuildResult = false;
                string report;
                IDlcBuilder dlcBuilder = DlcBuilder.CreateBuilder(this.BuildTarget, this.Environment, profileName, dlcDestination);
                try
                {
                    dlcBuildResult = dlcBuilder.Build(out report);
                }
                catch (System.Exception e)
                {
                    dlcBuildResult = false;
                    report = e.ToString();
                }
                BonGames.Tools.Domain.ThrowIf(!dlcBuildResult, $"Dlc build failed:\n{report}");
            }

            // Player Build
            if (BuildArguments.IsPlayerBuildEnable())
            {
                UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(BuildPlayerOptions);

                if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
                {
                    // Post Build
                    if (PostBuildProcess != null)
                    {
                        PostBuildProcess.OnPostBuild(this);
                    }
                }
                else
                {
                    BonGames.Tools.Domain.LogW("Skip Post Build Process due to build failed");
                }
                return report;
            }
            else
            {
                return default;
            }
        }

        protected virtual void Prepare()
        {
            PreProcessBuildWithReport = new DefaultPreProcessBuildWithReportPreset();
            PreProcessBuildWithReport.Tasks.Add(new IL2CppPreprocessBuild());

            PostProcessingWithReport = new DefaultPostProcessBuildWithReportPreset();
        }
        protected void SetProductInformation()
        {
            SetProductName();
            UpdateAppVersion();
            SignApp();
        }

        protected virtual void SetupInternally()
        {

        }

        protected virtual void UpdateAppVersion()
        {
            // Set all in once here, reduce complexity, if you want to set by your own logic, lets override this method
            PlayerSettings.bundleVersion = Version.BundleVersion;
            PlayerSettings.Android.bundleVersionCode = Version.Build;
            PlayerSettings.iOS.buildNumber = $"{Version.Build}";
            PlayerSettings.WSA.packageVersion = new System.Version(Version.Major, Version.Minor, Version.Build, Version.Revision);
        }

        protected virtual void SetProductName()
        {
            string product = BuildArguments.GetProductName();
            if (!string.IsNullOrEmpty(product))
            {
                switch (Environment)
                {
                    case EEnvironment.Debug:
                    case EEnvironment.Development:
                        product = $"{product} (Dev)";
                        break;
                    case EEnvironment.Staging:
                        product = $"{product} (Stag)";
                        break;
                    case EEnvironment.Release:
                    case EEnvironment.Distribution:
                        break;
                }
                PlayerSettings.productName = product;
            }

            // Setting for bundle id
            string bundleId = BuildArguments.GetBundleId();
            // If the bundle id is passed as an argument
            if (!string.IsNullOrEmpty(bundleId))
            {
                BuildTargetGroup buildGroup = BuilderUtils.GetBuildTargetGroup(BuildTarget);
                PlayerSettings.SetApplicationIdentifier(buildGroup, bundleId);
                EasyBuilder.LogE($"Set BundleId: {bundleId} for BuildTarget: {BuildTarget} BuildGroup: {buildGroup}");
            }
        }

        protected virtual void SignApp() { }

        protected virtual BuildPlayerOptions CreateBuildPlayerOptions()
        {
            string buildLocation = string.IsNullOrEmpty(BuildArguments.GetBuildDestination()) ? BuilderUtils.GetPlatformBuildFolder(BuildTarget, AppTarget) : BuildArguments.GetBuildDestination();
            BuildPlayerOptions buildPlayerOptions = GetDefaultBuildPlayerOptions();
            buildPlayerOptions.locationPathName = Path.Combine(buildLocation, BuildFileName());
            buildPlayerOptions.targetGroup = BuilderUtils.GetBuildTargetGroup(BuildTarget);
            buildPlayerOptions.subtarget = BuilderUtils.GetSubBuildTarget(AppTarget, BuildTarget);
            buildPlayerOptions.scenes = GetActiveScenes();
            buildPlayerOptions.target = BuildTarget;
            OnBuildPlayerOptionCreate(ref buildPlayerOptions);

            return buildPlayerOptions;
        }

        protected virtual void OnBuildPlayerOptionCreate(ref BuildPlayerOptions ops)
        {

        }

        private string BuildFileName()
        {
            string outputFileName = string.IsNullOrEmpty(BuildArguments.GetProductNameCode()) ? BuilderUtils.GetProductName() : BuildArguments.GetProductNameCode();
            outputFileName = $"{outputFileName}-{Environment.Shorten()}-{Version.BundleVersion}({Version.Build})";
            return $"{BuilderUtils.GetOutputFileName(outputFileName)}{BuilderUtils.GetBuildTargetAppExtension(BuildTarget, Environment)}";
        }

        public BuildPlayerOptions GetDefaultBuildPlayerOptions()
        {
            BuildPlayerOptions buildPlayerOptions = BuilderUtils.GetDefaultBuildPlayerOptions(AppTarget, Environment);
            
            List<string> symbols = new List<string>(buildPlayerOptions.extraScriptingDefines);
            if (PlatformSpecifiedSymbols != null && PlatformSpecifiedSymbols.Count > 0)
            {
                symbols.AddRange(PlatformSpecifiedSymbols);
            }
            symbols.AddRange(BuildArguments.GetAdditionalSymbols());

            buildPlayerOptions.extraScriptingDefines = symbols.ToArray();
            return buildPlayerOptions;
        }

        protected virtual string[] GetActiveScenes()
        {
            string[] allScenes = AssetDatabase.FindAssets("t:scene");
            List<string> activeScenes = new List<string>();
            string[] customScenePaths = BuildArguments.GetScenePaths();
            if (customScenePaths.Length > 0)
            {
                activeScenes.AddRange(customScenePaths.Where(path => allScenes.Contains(path)));
            }
            else
            {
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                for (int i = 0; i < scenes.Length; i++)
                {
                    EditorBuildSettingsScene sceneSettings = scenes[i];
                    if (sceneSettings.enabled)
                    {
                        activeScenes.Add(sceneSettings.path);
                    }
                }
            }
            return activeScenes.ToArray();
        }
    }

    internal class DefaultPreProcessBuildWithReportPreset : IPreProcessBuildWithReportPreset
    {
        public List<IPreProcessBuildWithReportTask> Tasks { get; private set; } = new();
    }

    internal class DefaultPostProcessBuildWithReportPreset : IPostProcessBuildWithReportPreset
    {
        public List<IPostProcessBuildWithReportTask> Tasks { get; private set; } = new();
    }
}
