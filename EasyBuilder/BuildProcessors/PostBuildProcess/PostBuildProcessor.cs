#if UNITY_EDITOR
//#define UNITY_IOS
using UnityEditor;
using System.IO;
using UnityEditor.Build.Reporting;


#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
namespace BonGames.EasyBuilder
{
	public class PostBuildProcessor
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
			ReplaceDefaultLocalization(plistPath);
			AddLSApplicationQueriesSchemes(plistPath);
			ReplaceDefaultLocalizationInPbxproj(System.IO.Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj/project.pbxproj"));
#endif
			UnityEngine.Debug.Log("OnIOSPostBuild finished, plist info below");
			UnityEngine.Debug.Log(File.ReadAllText(plistPath));
		}

#if UNITY_IOS
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
#endif
	}
}
#endif