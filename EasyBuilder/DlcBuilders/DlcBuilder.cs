using BonGames.Tools;
using BonGames.EasyBuilder.Enum;
using UnityEditor;

namespace BonGames.EasyBuilder
{
    public abstract partial class DlcBuilder : IDlcBuilder
    {
        private const string Tag = "[" + nameof(DlcBuilder) + "]";

        public readonly string ProfileName;
        public readonly string BuildDestination;
        public BuildTarget BuildTarget { get; private set; }
        public EEnvironment Environment { get; private set; }

        public DlcBuilder(BuildTarget target, EEnvironment buildEvn, string profileName, string buildDestination)
        {
            Environment = buildEvn;
            BuildTarget = target;
            ProfileName = profileName;
            BuildDestination = buildDestination;
        }

        public bool Build(out string report)
        {
            Setup();
            return StartBuildDlc(out report);
        }

        protected abstract bool StartBuildDlc(out string report);
        
        protected virtual void Setup() { } 
        
        protected void LogI(string msg)
        {
            Domain.LogI($"{Tag}: {msg}");
        }

        protected void LogW(string msg)
        {
            Domain.LogW($"{Tag}: {msg}");
        }

        protected void LogE(string msg)
        {
            Domain.LogE($"{Tag}: {msg}");
        }
    }
}
