using BonGames.EasyBuilder.Enum;
using UnityEditor;

namespace BonGames.EasyBuilder
{
    public interface IDlcBuilder
    {
        public BuildTarget BuildTarget { get; }
        public EEnvironment Environment { get; }
        public bool Build(out string report);
    }
}
