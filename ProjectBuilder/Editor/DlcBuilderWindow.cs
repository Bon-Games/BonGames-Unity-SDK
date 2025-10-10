using BonGames.EasyBuilder.Enum;
using BonGames.Shared;
using UnityEditor;
using UnityEngine;

namespace BonGames.EasyBuilder
{
    public class DlcBuilderWindow : IEditorWindow
    {
        private EAppTarget _appTarget = EAppTarget.Client;
        private EEnvironment _environment = EEnvironment.Development;
        private BuildTarget _buildTarget = BuildTarget.Android;
        private string _dlcProfile = null;

        public DlcBuilderWindow()
        {
            _dlcProfile = "Local";
        }

        public void DrawGUI(IEasyBuilderEditor parent)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextDlcProfile, EditorUISize.S.MaxLabelWidth);
            _dlcProfile = GUILayout.TextField(_dlcProfile, EditorUISize.S.MinOnelineInputWidth);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextBuildEnvironment, EditorUISize.S.MaxLabelWidth);
            _environment = (EEnvironment)EditorGUILayout.EnumPopup(_environment);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextBuildTarget, EditorUISize.S.MaxLabelWidth);
            _buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(_buildTarget);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(EditorContents.CTABuild, EditorUISize.S.MaxLabelWidth))
            {
                EasyBuilder.BuildDlc(_buildTarget, _environment, _dlcProfile);
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();
        }
    }

}
