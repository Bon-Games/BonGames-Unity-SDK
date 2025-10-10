#define UNITY_ADDRESSABLE

using BonGames.EasyBuilder.Enum;
using BonGames.Shared;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace BonGames.EasyBuilder
{
    internal interface IDlcBuildProfileCollection
    {
        List<string> GetProfiles();
    }

    internal class AddressableBuildProfile : IDlcBuildProfileCollection
    {
        private readonly List<string> _profiles;
        
        public AddressableBuildProfile()
        {
#if UNITY_ADDRESSABLE
            _profiles = new List<string>(AddressableAssetSettingsDefaultObject.Settings?.profileSettings?.GetAllProfileNames() ?? new List<string>());
#else
            _profiles = new List<string>();
#endif
        }

        List<string> IDlcBuildProfileCollection.GetProfiles()
        {
            return _profiles;
        }
    }

    public class DlcBuilderWindow : IEditorWindow
    {
        private EAppTarget _appTarget = EAppTarget.Client;
        private EEnvironment _environment = EEnvironment.Development;
        private BuildTarget _buildTarget = BuildTarget.Android;
        
        private int _dlcProfileId = 0;
        private IDlcBuildProfileCollection _dlcProfile;
        private GUIContent[] _profileOptions;
        public DlcBuilderWindow()
        {
            _dlcProfileId = 0;
#if UNITY_ADDRESSABLE
            _dlcProfile = new AddressableBuildProfile();
#endif
            if (_dlcProfile != null)
            {
                _profileOptions = _dlcProfile.GetProfiles().Select(p => new GUIContent(p)).ToArray();
            }
            if (_profileOptions == null || _profileOptions.Length == 0)
            {
                _profileOptions = new GUIContent[1] { new GUIContent("None") };
            }
        }

        public void DrawGUI(IEasyBuilderEditor parent)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextDlcProfile, EditorUISize.S.MaxLabelWidth);
            _dlcProfileId = EditorGUILayout.Popup(_dlcProfileId, _profileOptions, EditorUISize.S.MaxButtonWidth);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextBuildEnvironment, EditorUISize.S.MaxLabelWidth);
            _environment = (EEnvironment)EditorGUILayout.EnumPopup(_environment, EditorUISize.S.MaxButtonWidth);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextBuildTarget, EditorUISize.S.MaxLabelWidth);
            _buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(_buildTarget, EditorUISize.S.MaxButtonWidth);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(EditorContents.CTABuild, EditorUISize.S.MaxLabelWidth) && _dlcProfileId < _profileOptions.Length)
            {
                EasyBuilder.BuildDlc(_buildTarget, _environment, _dlcProfile.GetProfiles()[_dlcProfileId]);
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();
        }
    }

}
