using UnityEditor;

namespace BonGames.EasyBuilder
{
    public class IOSBuilder : ProjectBuilder
    {
        public IOSBuilder(EEnvironment environment) : base(EAppTarget.Client, BuildTarget.iOS, environment)
        {
            
        }

        protected override void SetupInternally()
        {
            base.SetupInternally();
            /// EditorUserBuildSettings
            bool isDebugBuild = Environment == EEnvironment.Debug;
            EditorUserBuildSettings.connectProfiler = isDebugBuild;
            EditorUserBuildSettings.buildWithDeepProfilingSupport = isDebugBuild;
            EditorUserBuildSettings.iOSXcodeBuildConfig = isDebugBuild ? XcodeBuildConfig.Debug : XcodeBuildConfig.Release;
            EditorUserBuildSettings.macOSXcodeBuildConfig = isDebugBuild ? XcodeBuildConfig.Debug : XcodeBuildConfig.Release;
            /// -- EditorUserBuildSettings
        }

        protected override void SignApp()
        {
            base.SignApp();
            PlayerSettings.iOS.iOSManualProvisioningProfileID = BuildArguments.IOS.GetProvisioningId();
            PlayerSettings.iOS.appleEnableAutomaticSigning = string.IsNullOrEmpty(PlayerSettings.iOS.iOSManualProvisioningProfileID);
            PlayerSettings.iOS.appleDeveloperTeamID = BuildArguments.IOS.GetTeamId();
            PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Automatic;
        }
    }
}
