using BonGames.EasyBuilder.Enum;
using UnityEditor;

namespace BonGames.EasyBuilder
{
    public class WebGLBuilder : ProjectBuilder
    {
        public WebGLBuilder(EEnvironment environment) : base(EAppTarget.Client, BuildTarget.WebGL, environment)
        {

        }
    }
}
