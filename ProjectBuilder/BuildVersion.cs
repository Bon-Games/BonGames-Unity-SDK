namespace BonGames.EasyBuilder
{
    public class BuildVersion
    {
        private string VersionFile => System.IO.Path.Combine(BuilderUtils.BuildInformationDirectory(), "version.txt");

        private string[] _versionParts = new string[] { "0", "0", "1", "1" };

        public int Major => ParseVersionPartNumberAt(0);
        public int Minor => ParseVersionPartNumberAt(1);
        public int Revision => ParseVersionPartNumberAt(2);
        public int Build
        {
            get
            {
                int buildNumber = BuildArguments.GetBuildNumber(-1);
                if (buildNumber > 0)
                {
                    return buildNumber;
                }
                return ParseVersionPartNumberAt(3);
            }
        }

        public string BundleVersion => $"{Major}.{Minor}.{Revision}";
        public string FullVersion => $"{Major}.{Minor}.{Revision}.{Build}";

        public BuildVersion()
        {
            LoadVersion();
        }

        public void LoadVersion()
        {
            if (System.IO.File.Exists(VersionFile))
            {
                _versionParts = System.IO.File.ReadAllText(VersionFile).Split('.');
            }
            else
            {
                // Create if it does not exist
                System.IO.File.WriteAllText(VersionFile, FullVersion);
            }
        }

        private int ParseVersionPartNumberAt(int index)
        {
            if (_versionParts != null && index < _versionParts.Length && int.TryParse(_versionParts[index], out int res))
            {
                return res;
            }

            return 0;
        }
    }
}
