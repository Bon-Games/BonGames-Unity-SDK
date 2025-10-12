using BonGames.EasyBuilder;
using UnityEditor.Build.Reporting;


//#define UNITY_IOS
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using System.Xml;
using System.IO;
#if UNITY_IOS

using UnityEditor.iOS.Xcode;
#endif

public class PostBuildProcessor : IPostBuildProcess
{
    public string Guid { get; set; }

    public PostBuildProcessor() 
	{
		Guid = System.Guid.NewGuid().ToString();
    }

    public void OnPostBuild(BuildReport report, IProjectBuilder builder, string pathToBuiltProject)
    {
		OnIOSPostBuild(builder.BuildTarget, pathToBuiltProject);
    }

    private static void OnIOSPostBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS) return;

        UnityEngine.Debug.Log("OnIOSPostBuild started");

        string plistPath = Path.Combine(pathToBuiltProject, "Info.plist");

        if (!File.Exists(plistPath)) return;

#if UNITY_IOS
		ModifyFrameworks(pathToBuiltProject);
		IOSIgnoreEncryption(plistPath);
		ReplaceDefaultLocalization(plistPath);
		AddLSApplicationQueriesSchemes(plistPath);
		ReplaceDefaultLocalizationInPbxproj(System.IO.Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj/project.pbxproj"));
		AddAppleSignInCapability(target, pathToBuiltProject);
        AddPushNotificationCap(target, pathToBuiltProject, plistPath);
#endif

        // #if ALLOW_HTTP_REQUEST
        //         IOSAllowHTTPRequest(plistPath);
        // #endif
        UnityEngine.Debug.Log("OnIOSPostBuild finished, plist info below");
        UnityEngine.Debug.Log(File.ReadAllText(plistPath));
    }

    private static void IOSAllowHTTPRequest(string plistPath)
    {
        UnityEngine.Debug.Log("OnIOSPostBuild IOSAllowHTTPRequest");

        XmlDocument plist = new XmlDocument();
        plist.Load(plistPath);

        // Add NSAppTransportSecurity if it doesn't exist
        XmlNode appTransportSecurity = plist.SelectSingleNode("//dict/key[. = 'NSAppTransportSecurity']");
        if (appTransportSecurity == null)
        {
            // Create NSAppTransportSecurity node
            XmlNode dictNode = plist.SelectSingleNode("//dict");
            if (dictNode != null)
            {
                XmlElement atsElement = plist.CreateElement("key");
                atsElement.InnerText = "NSAppTransportSecurity";
                dictNode.AppendChild(atsElement);

                XmlElement atsDictElement = plist.CreateElement("dict");
                dictNode.AppendChild(atsDictElement);

                // Add Allow Arbitrary Loads
                XmlElement allowArbitraryLoadsKey = plist.CreateElement("key");
                allowArbitraryLoadsKey.InnerText = "NSAllowsArbitraryLoads";
                atsDictElement.AppendChild(allowArbitraryLoadsKey);

                XmlElement allowArbitraryLoadsValue = plist.CreateElement("true");
                atsDictElement.AppendChild(allowArbitraryLoadsValue);
            }
        }

        plist.Save(plistPath);
    }


#if UNITY_IOS
	private static void ModifyFrameworks(string path)
	{
		string projPath = PBXProject.GetPBXProjectPath(path);
           
		var project = new PBXProject();
		project.ReadFromFile(projPath);

		string mainTargetGuid = project.GetUnityMainTargetGuid();
           
		foreach (var targetGuid in new[] { mainTargetGuid, project.GetUnityFrameworkTargetGuid() })
		{
			project.SetBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
		}
           
		project.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");

		project.WriteToFile(projPath);
	}
	
	private static void AddAppleSignInCapability(BuildTarget target, string pathToBuiltProject)
	{
		try
		{
			UnityEngine.Debug.Log("[BuildPostProcessing] Adding Apple Sign-In capability...");

			if (target != BuildTarget.iOS)
				return;

			var projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
			var project = new PBXProject();
			project.ReadFromString(System.IO.File.ReadAllText(projectPath));
			var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
			manager.AddSignInWithApple();
			manager.WriteToFile();


			UnityEngine.Debug.Log("[BuildPostProcessing] Xcode project updated with Apple Sign-In capability");
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError($"[BuildPostProcessing] Failed to add Apple Sign-In capability: {ex.Message}");
			UnityEngine.Debug.LogError($"[BuildPostProcessing] Exception details: {ex}");
		}
	}

	private static void IOSIgnoreEncryption(string plistPath)
	{
		UnityEngine.Debug.Log("OnIOSPostBuild IOSIgnoreEncryption");

		PlistDocument plist = new PlistDocument();
		plist.ReadFromString(File.ReadAllText(plistPath));

		PlistElementDict rootDict = plist.root;

		string encryptKey = "ITSAppUsesNonExemptEncryption";
		rootDict.SetBoolean(encryptKey, false);

		File.WriteAllText(plistPath, plist.WriteToString());
	}

	private static void ReplaceDefaultLocalization(string plistPath)
	{
		const string defaultLanguageKey = "CFBundleDevelopmentRegion";
		const string japaneseLanguageCode = "ja";

		string plistContent = File.ReadAllText(plistPath);

		if (plistContent.Contains(defaultLanguageKey))
		{
			int keyIndex = plistContent.IndexOf(defaultLanguageKey);
			int valueStartIndex = plistContent.IndexOf("<string>", keyIndex) + "<string>".Length;
			int valueEndIndex = plistContent.IndexOf("</string>", valueStartIndex);

			string originalLanguage = plistContent.Substring(valueStartIndex, valueEndIndex - valueStartIndex);

			plistContent = plistContent.Remove(valueStartIndex, valueEndIndex - valueStartIndex)
				.Insert(valueStartIndex, japaneseLanguageCode);

			File.WriteAllText(plistPath, plistContent);
			UnityEngine.Debug.Log($"Replaced default language '{originalLanguage}' with '{japaneseLanguageCode}' in Info.plist.");
		}
		else
		{
			UnityEngine.Debug.LogWarning($"'{defaultLanguageKey}' not found in Info.plist. Skipping language replacement.");
		}
	}

	private static void ReplaceDefaultLocalizationInPbxproj(string pbxprojPath)
	{
		const string developmentRegionKey = "developmentRegion";
		const string japaneseLanguageName = "Japanese";
		UnityEngine.Debug.LogWarning($"ReplaceDefaultLocalizationInPbxproj {pbxprojPath}");

		string pbxprojContent = File.ReadAllText(pbxprojPath);
		if (pbxprojContent.Contains(developmentRegionKey))
		{
			int keyIndex = pbxprojContent.IndexOf(developmentRegionKey);
			int valueStartIndex = pbxprojContent.IndexOf("= ", keyIndex) + "= ".Length;
			int valueEndIndex = pbxprojContent.IndexOf(";", valueStartIndex);

			string originalLanguage = pbxprojContent.Substring(valueStartIndex, valueEndIndex - valueStartIndex).Trim();

			pbxprojContent = pbxprojContent.Remove(valueStartIndex, valueEndIndex - valueStartIndex)
				.Insert(valueStartIndex, japaneseLanguageName);

			File.WriteAllText(pbxprojPath, pbxprojContent);
			UnityEngine.Debug.Log($"Replaced developmentRegion '{originalLanguage}' with '{japaneseLanguageName}' in project.pbxproj.");
		}
		else
		{
			UnityEngine.Debug.LogWarning($"'{developmentRegionKey}' not found in project.pbxproj. Skipping.");
		}
	}

	private static void AddLSApplicationQueriesSchemes(string plistPath)
	{
		// Load the plist file
		PlistDocument plist = new PlistDocument();
		plist.ReadFromFile(plistPath);

		// Get the root dictionary
		PlistElementDict rootDict = plist.root;

		// Check if LSApplicationQueriesSchemes exists
		PlistElementArray queriesSchemes;
		if (rootDict.values.ContainsKey("LSApplicationQueriesSchemes"))
		{
			queriesSchemes = rootDict["LSApplicationQueriesSchemes"].AsArray();
		}
		else
		{
			queriesSchemes = rootDict.CreateArray("LSApplicationQueriesSchemes");
		}

		// Add "twitter" scheme if not already present
		if (!queriesSchemes.values.Exists(x => x.AsString() == "twitter"))
		{
			queriesSchemes.AddString("twitter");
		}

		// Add "https" scheme if not already present
		if (!queriesSchemes.values.Exists(x => x.AsString() == "https"))
		{
			queriesSchemes.AddString("https");
		}

		// Write the updated plist back to the file
		plist.WriteToFile(plistPath);
		UnityEngine.Debug.Log("Successfully updated Info.plist with LSApplicationQueriesSchemes.");
	}
    private static void AddPushNotificationCap(BuildTarget target, string path, string plistPath)
    {
        try
		{
			if (target != BuildTarget.iOS) return;

			UnityEngine.Debug.Log("[BuildPostProcessing] Adding Push notification ...");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            PlistElementDict rootDict = plist.root;


            var projectPath = PBXProject.GetPBXProjectPath(path);
			var project = new PBXProject();
			project.ReadFromString(System.IO.File.ReadAllText(projectPath));
			var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
#if RELEASE_BUILD
			manager.AddPushNotifications(false);
#else
            manager.AddPushNotifications(true);
#endif
            manager.WriteToFile();

			// Remote notification
            PlistElementArray currentBackgroundModes = (PlistElementArray)rootDict["UIBackgroundModes"];
            if (currentBackgroundModes == null)
			{
                currentBackgroundModes = rootDict.CreateArray("UIBackgroundModes");
			}

            PlistElementString remoteNotificationElement = new PlistElementString("remote-notification");
            if (!currentBackgroundModes.values.Contains(remoteNotificationElement))
            {
                currentBackgroundModes.values.Add(remoteNotificationElement);
                File.WriteAllText(plistPath, plist.WriteToString());
            }

            UnityEngine.Debug.Log("[BuildPostProcessing] Xcode project updated with Push Notification");
		}
		catch (System.Exception ex)
		{
			UnityEngine.Debug.LogError($"[BuildPostProcessing] Failed to add Push Notification:\n{ex}");			
		}
    }
#endif
}
#endif