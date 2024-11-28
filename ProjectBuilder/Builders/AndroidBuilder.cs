using UnityEditor;

namespace BonGames.EasyBuilder
{
    public class AndroidBuilder : ProjectBuilder
    {
        public AndroidBuilder(EEnvironment environment) : base(EAppTarget.Client, BuildTarget.Android, environment)
        {
            
        }

        protected override void SetupInternally()
        {
            base.SetupInternally();
            /// EditorUserBuildSettings
            bool isReleaseBuild = Environment == EEnvironment.Release || Environment == EEnvironment.Distribution;
            EditorUserBuildSettings.development = Environment == EEnvironment.Debug || Environment == EEnvironment.Development;
            EditorUserBuildSettings.connectProfiler = Environment == EEnvironment.Debug;
            EditorUserBuildSettings.buildWithDeepProfilingSupport = Environment == EEnvironment.Debug;
            EditorUserBuildSettings.allowDebugging = Environment == EEnvironment.Debug;
            EditorUserBuildSettings.buildAppBundle = Environment == EEnvironment.Distribution;
            EditorUserBuildSettings.androidBuildType = GetBuildType();
            EditorUserBuildSettings.androidCreateSymbols = isReleaseBuild ? AndroidCreateSymbols.Public : AndroidCreateSymbols.Disabled;
            /// -- EditorUserBuildSettings
            

        }

        protected override void SignApp()
        {
            base.SignApp();
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = System.IO.Path.Combine(BuildInformationDirectory, "And", "bon-games-ks.keystore"); 
            PlayerSettings.Android.keystorePass = "BonGames@123";
            PlayerSettings.Android.keyaliasName = Environment == EEnvironment.Release ? "bon-games-release" : "bon-games-dev";
            PlayerSettings.Android.keyaliasPass = "BonGames@123";
        }

        private AndroidBuildType GetBuildType()
        {
            switch (Environment)
            {
                case EEnvironment.Debug:
                    return AndroidBuildType.Debug;
                case EEnvironment.Development:                    
                case EEnvironment.Staging:
                    return AndroidBuildType.Development;                    
                case EEnvironment.Release:
                case EEnvironment.Distribution:
                    return AndroidBuildType.Release;
            }
            return AndroidBuildType.Development;
        }
    }
}
