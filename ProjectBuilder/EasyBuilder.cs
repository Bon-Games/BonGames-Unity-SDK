namespace BonGames.EasyBuilder
{
    using UnityEditor;
    using UnityEngine;
    using BonGames.Tools;

    public enum EEnvironment
    {
        Debug,
        /// <summary> All features/ cheat/ tools are unclocked</summary>
        Development,
        /// <summary> Same as the Development build but the Staging is the most stable version during development phase</summary>
        Staging,
        /// <summary> Release build basically all development features will be disabled, it same as the build to end user</summary>
        Release,
        /// <summary> Build is used to upload to store/distribute. Functional is the same as Release </summary>
        Distribution
    }

    public enum EAppTarget
    {
        Client,
        Server,
    }

    public static class EasyBuilder
    {
        public const string Tag = "[" + nameof(EasyBuilder) + "]";
        public const string MenuParent = "BonGames";
        public const string MenuRunTools = "BonGames/Run";

        public static void LogI(string message) => BonGames.Tools.Domain.LogI(message);
        public static void LogW(string message) => BonGames.Tools.Domain.LogW(message);
        public static void LogE(string message) => BonGames.Tools.Domain.LogE(message);


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
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.Android, EEnvironment.Release).Build();
        }

        [MenuItem(MenuRunTools + "/Build/iOS/Staging")]
        public static void BuildIOSStaging()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.Android, EEnvironment.Staging).Build();
        }

        [MenuItem(MenuRunTools + "/Build/iOS/Development")]
        public static void BuildIOSDevelopment()
        {
            ProjectBuilder.CreateBuilder(EAppTarget.Client, BuildTarget.Android, EEnvironment.Development).Build();
        }

        public static void Build()
        {
            try
            {
                if (!Application.isBatchMode) throw new System.Exception("This method only supports batmode for now");

                if (!System.Enum.TryParse<EEnvironment>(EnvironmentArguments.GetEnvironmentArgument(BuildArguments.Key.BuildEnvironment), true, out EEnvironment env))
                    throw new System.Exception($"Build Environment is invalid with value {EnvironmentArguments.GetEnvironmentArgument(BuildArguments.Key.BuildEnvironment)}");


                if (!System.Enum.TryParse<EAppTarget>(EnvironmentArguments.GetEnvironmentArgument(BuildArguments.Key.BuildAppTarget), true, out EAppTarget appTarget))
                    throw new System.Exception($"App Target is invalid with value {EnvironmentArguments.GetEnvironmentArgument(BuildArguments.Key.BuildAppTarget)}");


                if (!System.Enum.TryParse<BuildTarget>(EnvironmentArguments.GetEnvironmentArgument(BuildArguments.Key.BuildPlatformTarget), true, out BuildTarget buildTarget))
                    throw new System.Exception($"Build Platform Target is invalid with value {EnvironmentArguments.GetEnvironmentArgument(BuildArguments.Key.BuildPlatformTarget)}");

                // ProjectBuilder.CreateBuilder(appTarget, buildTarget, env).Build();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Build Failed with Exception\n{e}");
            }
            finally
            {
                EditorApplication.Exit(0);
            }
        }
    }
}