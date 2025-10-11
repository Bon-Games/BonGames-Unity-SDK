namespace BonGames.EasyBuilder
{
  using System.Linq;
  using UnityEditor;
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

            EasyBuilder.LogI($"{Tag} Post processing build with report ...");
        }
    }
}