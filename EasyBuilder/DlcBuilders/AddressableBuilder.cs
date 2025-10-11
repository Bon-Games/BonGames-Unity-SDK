// #define UNITY_ADDRESSABLE

using UnityEditor;
using System.Text;
using System.Linq;
using BonGames.EasyBuilder.Enum;
using System.Collections.Generic;


#if UNITY_ADDRESSABLE
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using static UnityEditor.AddressableAssets.Settings.AddressableAssetSettings;
#endif

namespace BonGames.EasyBuilder
{
    public class AddressableBuilder : DlcBuilder
    {
        public AddressableBuilder(BuildTarget target, EEnvironment environment, string profileName, string buildDestination)
            : base(target, environment, profileName, buildDestination)
        {
#if !UNITY_ADDRESSABLE
            throw new System.Exception("Addressable is disabled");
#else
            AddressableAssetSettingsDefaultObject.Settings.BuildAddressablesWithPlayerBuild = PlayerBuildOption.BuildWithPlayer;
#endif
        }

        protected override void Setup()
        {
            #if UNITY_ADDRESSABLE
            string dlcDestination = this.BuildDestination;

            // The build argument can be lower case, so have to search for it
            List<string> profileNames = AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetAllProfileNames();
            string profileName = profileNames.Contains(this.ProfileName) ? this.ProfileName : profileNames.FirstOrDefault(p => this.ProfileName.ToLower().Equals(p.ToLower()));
            string buildProfileId = AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetProfileId(profileName);

            BonGames.Tools.Domain.ThrowIf(string.IsNullOrEmpty(buildProfileId), $"Dlc profile name couldn't be found: {profileName}");
            AddressableAssetSettingsDefaultObject.Settings.activeProfileId = buildProfileId;
            LogI($"Set Dlc active profile to {profileName}:{buildProfileId}");

            if (!string.IsNullOrEmpty(dlcDestination))
            {
                dlcDestination = $"{dlcDestination}/[Environment]/[UnityEditor.PlayerSettings.bundleVersion]/[BuildTarget]";
                LogI($"Set Dlc build destination to {dlcDestination}");

                AddressableAssetSettingsDefaultObject.Settings.profileSettings.SetValue(buildProfileId, "Remote.BuildPath", dlcDestination);
                AddressableAssetSettingsDefaultObject.Settings.profileSettings.SetValue(buildProfileId, "Environment", this.Environment.ShortenSimplified());
                EditorUtility.SetDirty(AddressableAssetSettingsDefaultObject.Settings);
                AssetDatabase.SaveAssets();
            }

            StringBuilder logger = new StringBuilder();
            logger.AppendLine($"Start building dlc with ProfileId:{buildProfileId}");
            foreach (string varName in AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetVariableNames())
            {
                logger.AppendLine($"with {varName}={AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetValueByName(buildProfileId, varName)}");
            }
            this.LogI($"{logger}");
            #endif
        }

        protected override bool StartBuildDlc(out string report)
        {
#if !UNITY_ADDRESSABLE
            throw new System.Exception("Addressable is disabled");
#else
            AddressableAssetSettings.CleanPlayerContent();
            AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult rst);
            report = rst.Error;
            return string.IsNullOrEmpty(rst.Error);
#endif
        }
    }
}
