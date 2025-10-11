using UnityEngine;

namespace BonGames.EasyBuilder
{
#if UNITY_EDITOR
    using UnityEditor;
    using BonGames.UniConfigurator;

    [CustomEditor(typeof(PreBuildProcessors))]
    public class PreBuildProcessorsEditor : UniDatabaseEditor<IPreBuildProcess> { }
#endif

    [CreateAssetMenu(fileName = "PreBuildProcessors", menuName = "Easy Builder/PreBuildProcessors", order = 0)]
    public class PreBuildProcessors : UniConfigurator.UniDatabase<IPreBuildProcess>
    {
        public void Execute(IProjectBuilder builder)
        {
            for (int i = 0; i < Count; i++)
            {
                EasyBuilder.LogI($"Pre Build Processing .. {this[i]}");
                this[i].OnPreBuild(builder);
            }
        }    
    }
}
