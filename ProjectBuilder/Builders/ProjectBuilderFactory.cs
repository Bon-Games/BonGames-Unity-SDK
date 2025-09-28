using BonGames.Tools.Enum;
using UnityEditor;
using static BonGames.EasyBuilder.BuildArguments;

namespace BonGames.EasyBuilder
{
    public partial class ProjectBuilder
    {
        private static IProjectBuilder s_instance = null;
        public static IProjectBuilder GetInstance() { return s_instance; }


        public static IProjectBuilder CreateBuilder(EAppTarget appTarget, BuildTarget target, EEnvironment buildEvn)
        {
            if (s_instance != null && s_instance.BuildTarget == target && s_instance.AppTarget == appTarget)
            {
                return s_instance;
            }

            switch (target)
            {
                case BuildTarget.Android:
                    s_instance = new AndroidBuilder(buildEvn);
                    break;
                case BuildTarget.iOS:
                    s_instance = new IOSBuilder(buildEvn);
                    break;
            }

            if (BuilderUtils.IsStandalone(target))
            {
                s_instance = new StandaloneBuilder(appTarget, target, buildEvn);
            }
            return s_instance;
        }

    }
}
