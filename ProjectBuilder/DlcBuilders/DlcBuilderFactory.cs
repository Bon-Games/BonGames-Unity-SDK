using UnityEditor;
using BonGames.Tools.Enum;

namespace BonGames.EasyBuilder
{
    public partial class DlcBuilder
    {
        private static IDlcBuilder s_instance = null;
        
        public static IDlcBuilder GetInstance() { return s_instance; }

        public static IDlcBuilder CreateBuilder(BuildTarget target, EEnvironment buildEvn, string profileName, string buildDestination)
        {
            if (s_instance != null && s_instance.BuildTarget == target && s_instance.Environment == buildEvn)
            {
                return s_instance;
            }

#if UNITY_ADDRESSABLE
            s_instance = new AddressableBuilder(target, buildEvn, profileName, buildDestination);
#else
            s_instance = null;
#endif
            return s_instance;
        }
    }
}
