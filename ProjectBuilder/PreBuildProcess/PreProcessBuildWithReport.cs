namespace BonGames.EasyBuilder
{
    using UnityEditor.Build;
    using UnityEditor.Build.Reporting;    

    public class PreProcessBuildWithReport : IPreprocessBuildWithReport
    {
        private const string Tag = "[" + nameof(PreProcessBuildWithReport) +"]";

        public int callbackOrder => 10000;

        public void OnPreprocessBuild(BuildReport report)
        {
            IProjectBuilder activeBuilder = ProjectBuilder.GetInstance();

            if (activeBuilder == null) return;

            EasyBuilder.LogI($"{Tag} Preprocessing build with report ...");
            
            if (activeBuilder.PreProcessBuildWithReport == null) return;

            for (int i = 0; i < activeBuilder.PreProcessBuildWithReport.Tasks.Count; i++)
            {
                IPreProcessBuildWithReportTask processor = activeBuilder.PreProcessBuildWithReport.Tasks[i];
                processor.OnPreprocessBuild(report);
            }
        }
    }
}