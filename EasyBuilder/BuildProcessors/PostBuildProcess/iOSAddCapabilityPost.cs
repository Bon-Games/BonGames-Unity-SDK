#if UNITY_EDITOR
using System.IO;
using Newtonsoft.Json;
using UnityEditor.Build.Reporting;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace BonGames.EasyBuilder
{
    public class iOSAddCapabilityPost : IPostBuildProcess
    {
        public string Guid { get; set; }
        [JsonProperty("LocalPushNotification")] private bool _enableLocalPushNotification = false;
        [JsonProperty("RemotePushNotification")] private bool _enableRemotePushNotification = false;
        [JsonProperty("AppleSignIn")] private bool _enableAppleSignIn = false;
        [JsonProperty("IgnoreEncryption")] private bool _isIgnoreEncryption = true;

        public iOSAddCapabilityPost()
        {
            Guid = System.Guid.NewGuid().ToString();
        }

        public void OnPostBuild(BuildReport report, IProjectBuilder builder, string outputBuiltProject)
        {
            if (builder.BuildTarget != UnityEditor.BuildTarget.iOS) return;
#if UNITY_IOS
            string projPath = PBXProject.GetPBXProjectPath(outputBuiltProject);
            string plistPath = Path.Combine(outputBuiltProject, "Info.plist");

            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            PlistElementDict rootDict = plist.root;

            // Load PBX project
            PBXProject project = new PBXProject();
            project.ReadFromFile(projPath);

            // Get or reuse entitlements file
            string mainTarget = project.GetUnityMainTargetGuid();
            string existing = project.GetBuildPropertyForAnyConfig(mainTarget, "CODE_SIGN_ENTITLEMENTS");
            string entitlementsFile = string.IsNullOrEmpty(existing) ? "Unity-iPhone.entitlements" : existing;

            // Add Apple Sign In capability
            ProjectCapabilityManager manager = new ProjectCapabilityManager(projPath, entitlementsFile, null, mainTarget);
            if (_enableAppleSignIn)
            {
                manager.AddSignInWithApple();
            }

            if (_enableLocalPushNotification || _enableRemotePushNotification)
            {
                manager.AddPushNotifications(
                    builder.Environment == Enum.EEnvironment.Debug ||
                    builder.Environment == Enum.EEnvironment.Development ||
                    builder.Environment == Enum.EEnvironment.Staging);
            }

            if (_enableRemotePushNotification)
            {
                PlistElementArray currentBackgroundModes = (PlistElementArray)rootDict["UIBackgroundModes"];
                if (currentBackgroundModes == null)
                {
                    currentBackgroundModes = rootDict.CreateArray("UIBackgroundModes");
                }

                PlistElementString remoteNotificationElement = new PlistElementString("remote-notification");
                if (!currentBackgroundModes.values.Contains(remoteNotificationElement))
                {
                    currentBackgroundModes.values.Add(remoteNotificationElement);
                }
            }

            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", !_isIgnoreEncryption);

            manager.WriteToFile();
            project.SetBuildProperty(mainTarget, "CODE_SIGN_ENTITLEMENTS", entitlementsFile);
            project.WriteToFile(projPath);
            File.WriteAllText(plistPath, plist.WriteToString());
#endif
        }
    }
}
#endif