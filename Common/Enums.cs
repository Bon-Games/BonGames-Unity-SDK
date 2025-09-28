namespace BonGames.Tools.Enum
{
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
}
