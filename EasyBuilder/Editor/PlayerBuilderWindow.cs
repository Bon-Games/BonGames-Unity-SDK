using BonGames.EasyBuilder.Enum;
using BonGames.Shared;
using UnityEditor;
using UnityEngine;

namespace BonGames.EasyBuilder
{
    public class PlayerBuilderWindow : BaseEditorWindow
    {    
        private EAppTarget _appTarget = EAppTarget.Client;
        private EEnvironment _environment = EEnvironment.Development;
        private BuildTarget _buildTarget = BuildTarget.Android;
        private string _activeBuildProfile = null;
        private bool _doesProfileExist = false;
        private BuildVersion _buildVersion;
        
        public PlayerBuilderWindow()
        {
            _buildTarget = BuilderUtils.GetActiveBuildTarget();
            LoadBuildProfile();
        }

        public override void DrawGUI(IEasyBuilderEditor parent)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextBuildProfile, Shared.EditorUISize.S.MaxLabelWidth);
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(_activeBuildProfile, Shared.EditorUISize.S.MinOnelineInputWidth, Shared.EditorUISize.ExpandWidth);
            EditorGUI.EndDisabledGroup();
            if (!_doesProfileExist)
            {
                GUILayout.Label(EditorContents.IconDoesntExistWarning, EditorCustomStyles.WarningIcon, Shared.EditorUISize.S.Icon);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextOutput, Shared.EditorUISize.S.MaxLabelWidth);
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(BuilderUtils.GetBuildLocationWithExecFile(_appTarget, _buildTarget, _environment, _buildVersion), Shared.EditorUISize.S.MinOnelineInputWidth, Shared.EditorUISize.ExpandWidth);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextAppTarget, Shared.EditorUISize.S.MaxLabelWidth);
            _appTarget = (EAppTarget)EditorGUILayout.EnumPopup(_appTarget, EditorUISize.S.MaxButtonWidth);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextBuildEnvironment, Shared.EditorUISize.S.MaxLabelWidth);
            _environment = (EEnvironment)EditorGUILayout.EnumPopup(_environment, EditorUISize.S.MaxButtonWidth);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextBuildTarget, Shared.EditorUISize.S.MaxLabelWidth);
            _buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(_buildTarget, EditorUISize.S.MaxButtonWidth);
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                LoadBuildProfile();
            }
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(EditorContents.CTAEditorSetup, Shared.EditorUISize.S.MaxButtonWidth))
            {
                ProjectSwitcher.SwitchTo(_appTarget, _environment, _buildTarget);
                ProjectSwitcher.SetScriptSymbolsTo(_appTarget, _environment);
                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button(EditorContents.CTABuild, Shared.EditorUISize.S.MaxButtonWidth))
            {
                EasyBuilder.Build(_appTarget, _environment, _buildTarget);
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();
        }

        public override void OnFocus(IEasyBuilderEditor parent)
        {
            LoadBuildProfile();
        }

        private void LoadBuildProfile()
        {
            _activeBuildProfile = BuilderUtils.GetActiveBuildProfileFilePath(_environment);
            BonGames.CommandLine.ArgumentsResolver.Load(_activeBuildProfile);
            _doesProfileExist = System.IO.File.Exists(_activeBuildProfile);
            _buildVersion = new BuildVersion(true);
        }
    }

}
