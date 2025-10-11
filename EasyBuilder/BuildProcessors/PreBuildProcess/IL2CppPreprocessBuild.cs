namespace BonGames.EasyBuilder
{
    using UnityEditor;
    using BonGames.EasyBuilder.Enum;

    public class IL2CppPreprocessBuild : IPreBuildProcess
    {
        private const string Tag = "[" + nameof(IL2CppPreprocessBuild) + "]";
        public string Guid { get; set; }

        public IL2CppPreprocessBuild()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        public void OnPreBuild(IProjectBuilder builder)
        {
            BuildTargetGroup activeGroup = BuilderUtils.GetActiveBuildTargetGroup();
#if ENABLE_IL2CPP_SCRIPT_BACKEND
            PlayerSettings.SetScriptingBackend(activeGroup, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetAdditionalIl2CppArgs("--generic-virtual-method-iterations=1");
            bool isClientBuild = true;
#if DEDICATED_SERVER
            isClientBuild = false;
#endif // -- DEDICATED_SERVER

#if UNITY_2022_1_OR_NEWER
            UnityEditor.Build.NamedBuildTarget namedBuildTarget = BuilderUtils.GetNamedBuildTarget(isClientBuild ? EAppTarget.Client : EAppTarget.Server, builder.BuildTarget);
            PlayerSettings.SetIl2CppCodeGeneration(namedBuildTarget, UnityEditor.Build.Il2CppCodeGeneration.OptimizeSize);
#else
            EasyBuilder.LogW($"{Tag} You might have to set UnityEditor.Build.Il2CppCodeGeneration `Faster (smaller) builds` manually.");
#endif // -- UNITY_2022_1_OR_NEWER
#endif // -- ENABLE_IL2CPP_SCRIPT_BACKEND
        }
    }
}
