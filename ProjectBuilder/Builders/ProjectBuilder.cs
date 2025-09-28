using BonGames.Tools;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BonGames.Tools.Enum;
using UnityEditor;
using UnityEditor.Build;

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
            Prepare();
            BonGames.CommandLine.ArgumentsResolver.Load();
            // Switch to build target
            BuildTargetGroup buildGroup = BuilderUtils.GetBuildTargetGroup(BuildTarget);
            if (buildGroup != BuilderUtils.GetActiveBuildTargetGroup() || BuildTarget != BuilderUtils.GetActiveBuildTarget())
            {
                BonGames.Tools.Domain.LogI($"Switching build target to BuildGroup:{buildGroup} BuildTarget:{BuildTarget}");
                bool switchRes = EditorUserBuildSettings.SwitchActiveBuildTarget(buildGroup, BuildTarget);
                BonGames.Tools.Domain.ThrowIf(!switchRes, $"SwitchingError: Switching build target to BuildGroup:{buildGroup} BuildTarget:{BuildTarget}");
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
            // TODO: This could be sepeated from build player
#if UNITY_ADDRESSABLE
            if (BuildArguments.IsDlcBuildEnable())
            {
                string dlcDestination = BuildArguments.GetDlcDestination();
                string profileName = BuildArguments.GetDlcProfileName("Remote") ?? string.Empty;
                // The build argument is lower case, so have to search for it
                List<string> profileNames = AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetAllProfileNames();
                profileName = profileNames.Contains(profileName) ? profileName : profileNames.FirstOrDefault(p => profileName.ToLower().Equals(p.ToLower()));
                string buildProfileId = AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetProfileId(profileName);

                BonGames.Tools.Domain.ThrowIf(string.IsNullOrEmpty(buildProfileId), $"Dlc profile name couldn't be found: {profileName}");
                AddressableAssetSettingsDefaultObject.Settings.activeProfileId = buildProfileId;
                BonGames.Tools.Domain.LogI($"Set Dlc active profile to {profileName}:{buildProfileId}");
                if (!string.IsNullOrEmpty(dlcDestination))
                {
                    dlcDestination = $"{dlcDestination}/[Environment]/[UnityEditor.PlayerSettings.bundleVersion]/[BuildTarget]";
                    BonGames.Tools.Domain.LogI($"Set Dlc build destination to {dlcDestination}");

                    AddressableAssetSettingsDefaultObject.Settings.profileSettings.SetValue(buildProfileId, "Remote.BuildPath", dlcDestination);
                    AddressableAssetSettingsDefaultObject.Settings.profileSettings.SetValue(buildProfileId, "Environment", this.Environment.ShortenSimplified());
                    EditorUtility.SetDirty(AddressableAssetSettingsDefaultObject.Settings);
                    AssetDatabase.SaveAssets();
                }
                BonGames.Tools.Domain.LogI($"Start building dlc with ProfileId:{buildProfileId}");
                foreach (string varName in AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetVariableNames()) 
                {
                    BonGames.Tools.Domain.LogI($"with {varName}={AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetValueByName(buildProfileId, varName)}");
                }
                AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult rst);
                BonGames.Tools.Domain.ThrowIf(!string.IsNullOrEmpty(rst.Error), $"Dlc build failed:\n{rst.Error}");
            }
#endif

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
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            NamedBuildTarget namedBuildTarget = BuilderUtils.GetNamedBuildTarget(AppTarget, BuildTarget);
            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] currentSymbols);

            BuildOptions options = BuildOptions.None;
            List<string> defines = null;
            switch (Environment)
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

            if (PlatformSpecifiedSymbols != null)
            {
                defines.AddRange(PlatformSpecifiedSymbols);
            }

            defines.AddRange(BuildArguments.GetAdditionalSymbols());            

            buildPlayerOptions.options = options;
            buildPlayerOptions.extraScriptingDefines = defines.ToArray();
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
