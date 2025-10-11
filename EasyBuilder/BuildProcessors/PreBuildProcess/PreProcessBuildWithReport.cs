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

            EasyBuilder.LogI($"{Tag} Pre Processing build with report ...");    
        }
    }
}