
namespace BonGames.EasyBuilder
{
    using UnityEditor;    
    using UnityEditor.Build.Reporting;

    public class IL2CppPreprocessBuild : IPreProcessBuildWithReportTask
    {
        private const string Tag = "[" + nameof(IL2CppPreprocessBuild) +"]";

        public void OnPreprocessBuild(BuildReport report)
        {
            BuildSummary summary = report.summary;

            PlayerSettings.SetScriptingBackend(summary.platformGroup, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetAdditionalIl2CppArgs("--generic-virtual-method-iterations=1");
            bool isClientBuild = true;
#if DEDICATED_SERVER
            isClientBuild = false;
#endif

#if UNITY_2022_1_OR_NEWER
            UnityEditor.Build.NamedBuildTarget namedBuildTarget = BuilderUtils.GetNamedBuildTarget(isClientBuild ? EAppTarget.Client : EAppTarget.Server, summary.platform);
            PlayerSettings.SetIl2CppCodeGeneration(namedBuildTarget, UnityEditor.Build.Il2CppCodeGeneration.OptimizeSize);
#else
            EasyBuilder.LogW($"{Tag} You might have to set UnityEditor.Build.Il2CppCodeGeneration `Faster (smaller) builds` manually.");
#endif
        }
    }
}
