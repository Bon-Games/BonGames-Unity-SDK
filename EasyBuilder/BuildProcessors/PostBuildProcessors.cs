using UnityEngine;

namespace BonGames.EasyBuilder
{
#if UNITY_EDITOR
    using UnityEditor;
    using BonGames.UniConfigurator;

    [CustomEditor(typeof(PostBuildProcessors))]
    public class PostBuildProcessorsEditor : UniDatabaseEditor<IPostBuildProcess> { }
#endif

    [CreateAssetMenu(fileName = "PostBuildProcessors", menuName = "Easy Builder/PostBuildProcessors", order = 1)]
    public class PostBuildProcessors : UniConfigurator.UniDatabase<IPostBuildProcess>
    {
        public void Execute(UnityEditor.Build.Reporting.BuildReport report, IProjectBuilder builder)
        {
            for (int i = 0; i < Count; i++)
            {
                EasyBuilder.LogI($"Post Build Processing .. {this[i]}");
                this[i].OnPostBuild(report, builder);
            }
        }
    }
}
