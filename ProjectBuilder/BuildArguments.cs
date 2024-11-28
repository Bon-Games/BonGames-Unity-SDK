using BonGames.Tools;

namespace BonGames.EasyBuilder
{
    public static class BuildArguments
    {
        public static class Key
        {
            public const string BuildNumber         = "-buildNumber";

            // Android specified
            public const string KeystorePath        = "-ks";
            public const string KeystorePassword    = "-ksPassword";
            public const string Alias               = "-alias";
            public const string AliasPassword       = "-aliasPassword";

            // iOS specified
            public const string ProvisioningId    = "-iOSProvisioning";
            public const string DevelopmentTeamId   = "-iOSTeamId";

        }

        public static class Android
        {
            public static string GetKeystorePath() => EnvironmentArguments.GetEnvironmentArgument(Key.KeystorePath);
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
    }
}
