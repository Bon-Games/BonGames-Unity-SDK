namespace BonGames.EasyBuilder
{
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;

    public class PostProcessBuildWithReport : IPostprocessBuildWithReport
    {
        private const string Tag = "[" + nameof(PostProcessBuildWithReport) +"]";

        public int callbackOrder => 10000;

        public void OnPostprocessBuild(BuildReport report)
        {
            IProjectBuilder activeBuilder = ProjectBuilder.GetInstance();

            if (activeBuilder == null) return;

            EasyBuilder.LogI($"{Tag} Postprocessing build with report ...");
            
            if (activeBuilder.PostProcessingWithReport == null) return;
            
            for (int i = 0; i < activeBuilder.PostProcessingWithReport.Tasks.Count; i++)
            {
                IPostProcessBuildWithReportTask processor = activeBuilder.PostProcessingWithReport.Tasks[i];
                processor.OnPostprocessBuild(report);
            }
        }
    }
}