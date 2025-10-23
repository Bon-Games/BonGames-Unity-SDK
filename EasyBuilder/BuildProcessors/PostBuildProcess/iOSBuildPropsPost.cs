#if UNITY_EDITOR
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace BonGames.EasyBuilder
{
    public class iOSBuildPropsPost : IPostBuildProcess
    {
        public string Guid { get; set; }
        [JsonProperty]
        private Dictionary<string, bool> _flags = new Dictionary<string, bool>()
        {
            { "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", true },
            { "ENABLE_BITCODE", false }
        };

        public iOSBuildPropsPost()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        public void OnPostBuild(BuildReport report, IProjectBuilder builder, string outputBuiltProject)
        {
            if (builder.BuildTarget != UnityEditor.BuildTarget.iOS) return;

#if UNITY_IOS
            string projPath = PBXProject.GetPBXProjectPath(outputBuiltProject);
            PBXProject project = new PBXProject();
            project.ReadFromFile(projPath);

            string mainTargetGuid = project.GetUnityMainTargetGuid();
            string unityFrameworkTargetGuid = project.GetUnityMainTargetGuid();
            foreach (KeyValuePair<string, bool> pair in _flags)
            {
                project.SetBuildProperty(mainTargetGuid, pair.Key, pair.Value ? "YES" : "NO");
                project.SetBuildProperty(unityFrameworkTargetGuid, pair.Key, "NO");
            }
            project.WriteToFile(projPath);
#endif
        }
    }
}
#endif