using BonGames.EasyBuilder.Argument;

namespace BonGames.EasyBuilder
{
    public class BuildVersion
    {
        private string VersionFile => System.IO.Path.Combine(BuilderUtils.BuildInformationDirectory(), "version.txt");

        private string[] _versionParts = new string[] { "0", "0", "1", "1" };
        private readonly bool _allowEnvVersion;

        public int Major => ParseVersionPartNumberAt(0);
        public int Minor => ParseVersionPartNumberAt(1);
        public int Revision => ParseVersionPartNumberAt(2);
        public int Build
        {
            get
            {
                if (_allowEnvVersion)
                {
                    int buildNumber = BuildArguments.GetBuildNumber(-1);
                    if (buildNumber > 0)
                    {
                        return buildNumber;
                    }
                }
                return ParseVersionPartNumberAt(3);
            }
        }
        public string BundleVersion
        {
            get
            {
                string def = $"{Major}.{Minor}.{Revision}";
                return _allowEnvVersion ? BuildArguments.GetVersionString(def) : def;
            }
        }
        public string FullVersion => $"{BundleVersion}.{Build}";

        public BuildVersion(bool allowEnvVersion = true)
        {
            _allowEnvVersion = allowEnvVersion;
            LoadVersion();
        }

        public void LoadVersion()
        {
            string bundleVersion = BundleVersion;
            if (_allowEnvVersion && !string.IsNullOrEmpty(bundleVersion))
            {
                _versionParts = bundleVersion.Split('.');
            }
            else if (System.IO.File.Exists(VersionFile))
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
