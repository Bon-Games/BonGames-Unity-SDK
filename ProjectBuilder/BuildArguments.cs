using BonGames.Tools;

namespace BonGames.EasyBuilder
{
    public static class BuildArguments
    {
        public static class Key
        {
            // Common
            public const string BuildDestination    = "-buildDestination"; // Build folder            

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
                Domain.LogW($"Keystore does't exist at relativePath {relativePath}");
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
    }
}
