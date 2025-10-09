using BonGames.EasyBuilder.Enum;
using UnityEditor;
using UnityEngine;

namespace BonGames.EasyBuilder
{
    public class DlcBuilderWindow : IEditorWindow
    {
        private const uint LabelWidth = 220;

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
            GUILayout.Label("Dlc Profile", GUILayout.Width(LabelWidth));
            _dlcProfile = GUILayout.TextField(_dlcProfile, GUILayout.MinWidth(400));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Environment", GUILayout.Width(LabelWidth));
            _environment = (EEnvironment)EditorGUILayout.EnumPopup(_environment);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Build Target", GUILayout.Width(LabelWidth));
            _buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(_buildTarget);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build", GUILayout.Width(LabelWidth)))
            {
                EasyBuilder.BuildDlc(_buildTarget, _environment, _dlcProfile);
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();
        }
    }

}
