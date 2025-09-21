using BonGames.Tools;
using System.Collections;
using System.Collections.Generic;

namespace BonGames.EasyBuilder
{
    public class BuildVariables
    {
        private readonly Dictionary<string, string> Variables = new Dictionary<string, string>()
        {
            { $"[{nameof(BuildArguments.Key.BuildNumber)}]" ,   $"{BuildArguments.GetBuildNumber()}"    },
            { $"[BundleVersion]"                            ,   $"{new BuildVersion().BundleVersion}"   },
            { $"[Environment]"                              ,   $"{BuilderUtils.GetBuildEnvironment()}" },
            { $"[ProductName]"                              ,   $"{BuilderUtils.GetProductName()}" },
        };
    }
}
