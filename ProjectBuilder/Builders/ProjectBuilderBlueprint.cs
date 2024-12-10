using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace BonGames.EasyBuilder
{
    public interface IProjectBuilder
    {
        public IPreProcessBuildWithReportPreset PreProcessBuildWithReport { get; }
        public IPostProcessBuildWithReportPreset PostProcessingWithReport { get; }
        public IPreBuildProcess PreBuildProcess { get; }
        public IPostBuildProcess PostBuildProcess { get; }
        
        public EAppTarget AppTarget { get; }
        public BuildTarget BuildTarget { get; }
        public EEnvironment Environment { get; }

        public UnityEditor.Build.Reporting.BuildReport Build();
    }

    public interface IPreBuildProcess
    {
        public void OnPreBuild(IProjectBuilder builder);
    }

    public interface IPostBuildProcess
    {
        public void OnPostBuild(IProjectBuilder builder);
    }


    public interface IPreProcessBuildWithReportTask
    {
        public void OnPreprocessBuild(BuildReport report);
    }

    public interface IPreProcessBuildWithReportPreset
    {
        public List<IPreProcessBuildWithReportTask> Tasks { get; }
    }

    public interface IPostProcessBuildWithReportTask
    {
        public void OnPostprocessBuild(BuildReport report);
    }

    public interface IPostProcessBuildWithReportPreset
    {
        public List<IPostProcessBuildWithReportTask> Tasks { get; }
    }
}
