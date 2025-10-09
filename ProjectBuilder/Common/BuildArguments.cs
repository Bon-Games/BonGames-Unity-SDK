using System;
using BonGames.CommandLine;

namespace BonGames.EasyBuilder.Argument
{
    public static class BuildArguments
    {
        public static class Key
        {
            // Common
            public const string BuildDestination    = "-buildDestination"; // Build folder
            public const string BuildApp            = "-buildApp";
            public const string CI                  = "-ci"; // True if the build is trigged from Jenkins or a build pipline
            public const string OutputFileName      = "-outputFileName"; // Eg: [ProductCode]-[BuildEnvironment]-[BuildVersionString]-([BuildNumber])

            // Build Arguments
            public const string BuildAppTarget      = "-buildAppTarget";
            public const string BuildEnvironment    = "-buildEnv";
            public const string BuildPlatformTarget = "-buildPlatformTarget";
            public const string BuildNumber         = "-buildNumber";
            public const string BuildVersionString  = "-buildVersionString";
            public const string Product             = "-product";
            public const string BundleId            = "-bundleId";
            public const string ProductCode         = "-productCode";
            public const string ScenePaths          = "-scenes";
            public const string AdditionalSymbols   = "-symbols";
            public const string ReleaseAlias        = "-releaseAlias";
            public const string PreProcessorsBuild  = "-preProcessorsBuild";
            public const string PostProcessorsBuild = "-postProcessorsBuild";

            // Android specified
            public const string KeystorePath        = "-ks";
            public const string KeystorePassword    = "-ksPassword";
            public const string Alias               = "-alias";
            public const string AliasPassword       = "-aliasPassword";
            public const string BuildAAB            = "-buildAAB";

            // iOS specified
            public const string ProvisioningId      = "-iOSProvisioning";
            public const string DevelopmentTeamId   = "-iOSTeamId";

            // Dlc
            public const string BuildDlc            = "-buildDlc";
            public const string DlcDestination      = "-dlcDestination";
            public const string DlcProfileName      = "-buildDlcProfile";

            // Git Information
            public const string GitRevision         = "-gitRevision"; // Git commit hash
            public const string GitBranch           = "-gitBranch";
        }

        public static class Android
        {
            public static string GetKeystorePath()
            {
                string relativePath = ArgumentsResolver.GetEnvironmentArgument(Key.KeystorePath);
                if (!string.IsNullOrEmpty(relativePath))
                {
                    return System.IO.Path.Combine(UnityEngine.Application.dataPath, "..", relativePath);
                }
                return null;
            }
            public static string GetKeystorePassword() => ArgumentsResolver.GetEnvironmentArgument(Key.KeystorePassword);
            public static string GetAlias() => ArgumentsResolver.GetEnvironmentArgument(Key.Alias);
            public static string GetAliasPassword() => ArgumentsResolver.GetEnvironmentArgument(Key.AliasPassword);
            public static bool IsBuildAAB() => ArgumentsResolver.GetEnvironmentArgument(Key.BuildAAB) == "true";
        }

        public static class IOS
        {
            public static string GetProvisioningId() => ArgumentsResolver.GetEnvironmentArgument(Key.ProvisioningId);
            public static string GetTeamId() => ArgumentsResolver.GetEnvironmentArgument(Key.DevelopmentTeamId);
        }

        public static class Standalone { }        

        public static int GetBuildNumber(int defValue = 1)
        {
            string input = ArgumentsResolver.GetEnvironmentArgument(Key.BuildNumber);
            if (int.TryParse(input, out int buildNumber))
            {
                return buildNumber;
            }
            return defValue;
        }
        public static string GetProductName() => ArgumentsResolver.GetEnvironmentArgument(Key.Product);      
        public static string GetProductNameCode() => ArgumentsResolver.GetEnvironmentArgument(Key.ProductCode);
        public static string GetBundleId() => ArgumentsResolver.GetEnvironmentArgument(Key.BundleId);
        public static string GetBuildDestination() => ArgumentsResolver.GetEnvironmentArgument(Key.BuildDestination);
        public static string GetDlcDestination() => ArgumentsResolver.GetEnvironmentArgument(Key.DlcDestination);
        public static string GetDlcProfileName(string def)
        {
            string profileName = ArgumentsResolver.GetEnvironmentArgument(Key.DlcProfileName);
            return string.IsNullOrEmpty(profileName) ? def : profileName;
        }
        public static bool IsDlcBuildEnable() => ArgumentsResolver.GetEnvironmentArgument(Key.BuildDlc) == "true";
        /// <summary> If you're not in batch mode (mean editor mode), then Player Build is intented even BuildApp param is null, otherwise lets set it to false in args.default config file </summary>
        public static bool IsPlayerBuildEnable() => (!UnityEngine.Application.isBatchMode && string.IsNullOrEmpty(ArgumentsResolver.GetEnvironmentArgument(Key.BuildApp))) || ArgumentsResolver.GetEnvironmentArgument(Key.BuildApp) == "true";
        public static string[] GetAdditionalSymbols()
        {
            string args = ArgumentsResolver.GetEnvironmentArgument(Key.AdditionalSymbols);

            if (string.IsNullOrEmpty(args)) return Array.Empty<string>();

            return args.Trim().Split(';');
        }
        public static string[] GetScenePaths()
        {
            string args = ArgumentsResolver.GetEnvironmentArgument(Key.ScenePaths);

            if (string.IsNullOrEmpty(args)) return Array.Empty<string>();

            return args.Trim().Split(';');
        }
        public static bool IsCIBuild() => ArgumentsResolver.GetEnvironmentArgument(Key.CI) == "true";
        public static string GetVersionString(string defValue)
        {
            string version = ArgumentsResolver.GetEnvironmentArgument(Key.BuildVersionString);
            return string.IsNullOrEmpty(version) ? defValue : version;
        }
        public static string GetGitRevision() => ArgumentsResolver.GetEnvironmentArgument(Key.GitRevision);
        public static string GetGitBranch() => ArgumentsResolver.GetEnvironmentArgument(Key.GitBranch);
        public static string GetOutputFileName() => ArgumentsExpander.ExpandArguments(ArgumentsResolver.GetEnvironmentArgument(Key.OutputFileName));
        public static string GetPreProcessorsBuild() => ArgumentsResolver.GetEnvironmentArgument(Key.PreProcessorsBuild);
        public static string GetPostProcessorsBuild() => ArgumentsResolver.GetEnvironmentArgument(Key.PostProcessorsBuild);
    }
}
