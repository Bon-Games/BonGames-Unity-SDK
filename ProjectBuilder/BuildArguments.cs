using BonGames.Tools;

namespace BonGames.EasyBuilder
{
    public static class BuildArguments
    {
        public static class Key
        {
            // Common
            public const string BuildDestination    = "-buildDestination"; // Build folder
            public const string BuildApp            = "-buildApp";
            

            // Build Arguments
            public const string BuildAppTarget      = "-buildAppTarget";
            public const string BuildEnvironment    = "-buildEnv";
            public const string BuildPlatformTarget = "-buildPlatformTarget";
            public const string BuildNumber         = "-buildNumber";
            public const string Product             = "-product";
            public const string BundleId            = "-bundleId";
            public const string ProductCode         = "-productCode";

            // Android specified
            public const string KeystorePath        = "-ks";
            public const string KeystorePassword    = "-ksPassword";
            public const string Alias               = "-alias";
            public const string AliasPassword       = "-aliasPassword";

            // iOS specified
            public const string ProvisioningId      = "-iOSProvisioning";
            public const string DevelopmentTeamId   = "-iOSTeamId";

            // Dlc
            public const string BuildDlc            = "-buildDlc";
            public const string DlcDestination      = "-dlcDestination";
            public const string DlcProfileName      = "-buildDlcProfile";

        }

        public static class Android
        {
            public static string GetKeystorePath()
            {
                string relativePath = EnvironmentArguments.GetEnvironmentArgument(Key.KeystorePath);
                if (!string.IsNullOrEmpty(relativePath))
                {
                    return System.IO.Path.Combine(UnityEngine.Application.dataPath, relativePath);
                }
                return null;
            }
            public static string GetKeystorePassword() => EnvironmentArguments.GetEnvironmentArgument(Key.KeystorePassword);
            public static string GetAlias() => EnvironmentArguments.GetEnvironmentArgument(Key.Alias);
            public static string GetAliasPassword() => EnvironmentArguments.GetEnvironmentArgument(Key.AliasPassword);
        }

        public static class IOS
        {
            public static string GetProvisioningId() => EnvironmentArguments.GetEnvironmentArgument(Key.ProvisioningId);
            public static string GetTeamId() => EnvironmentArguments.GetEnvironmentArgument(Key.DevelopmentTeamId);
        }

        public static int GetBuildNumber(int defValue = 1)
        {
            string input = EnvironmentArguments.GetEnvironmentArgument(Key.BuildNumber);
            if (int.TryParse(input, out int buildNumber))
            {
                return buildNumber;
            }
            return defValue;
        }
        public static string GetProductName() => EnvironmentArguments.GetEnvironmentArgument(Key.Product);      
        public static string GetProductNameCode() => EnvironmentArguments.GetEnvironmentArgument(Key.ProductCode);
        public static string GetBundleId() => EnvironmentArguments.GetEnvironmentArgument(Key.BundleId);
        public static string GetBuildDestination() => EnvironmentArguments.GetEnvironmentArgument(Key.BuildDestination);
        public static string GetDlcDestination() => EnvironmentArguments.GetEnvironmentArgument(Key.DlcDestination);
        public static string GetDlcProfileName(string def)
        {
            string profileName = EnvironmentArguments.GetEnvironmentArgument(Key.DlcProfileName);
            return string.IsNullOrEmpty(profileName) ? def : profileName;
        }
        public static bool IsDlcBuildEnable() => EnvironmentArguments.GetEnvironmentArgument(Key.BuildDlc) == "true";
        /// <summary> If you're not in batch mode (mean editor mode), then Player Build is intented even BuildApp param is null, otherwise lets set it to false in args.default config file </summary>
        public static bool IsPlayerBuildEnable() => (!UnityEngine.Application.isBatchMode && string.IsNullOrEmpty(EnvironmentArguments.GetEnvironmentArgument(Key.BuildApp))) || EnvironmentArguments.GetEnvironmentArgument(Key.BuildApp) == "true";
    }
}
