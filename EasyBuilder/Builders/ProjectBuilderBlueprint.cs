using System.Collections.Generic;
using BonGames.EasyBuilder.Enum;
using BonGames.UniConfigurator;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace BonGames.EasyBuilder
{
    public interface IProjectBuilder
    {
        public EAppTarget AppTarget { get; }
        public BuildTarget BuildTarget { get; }
        public EEnvironment Environment { get; }

        public UnityEditor.Build.Reporting.BuildReport Build();
    }

    public interface IPreBuildProcess : IUniRecord
    {
        public void OnPreBuild(IProjectBuilder builder);
    }

    public interface IPostBuildProcess : IUniRecord
    {
        public void OnPostBuild(BuildReport report, IProjectBuilder builder, string outputBuiltProject);
    }
}
