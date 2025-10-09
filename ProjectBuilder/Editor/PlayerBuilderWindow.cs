using BonGames.EasyBuilder.Enum;
using UnityEditor;
using UnityEngine;

namespace BonGames.EasyBuilder
{
    public class PlayerBuilderWindow : IEditorWindow
    {
        private const uint LabelWidth = 220;

        private EAppTarget _appTarget = EAppTarget.Client;
        private EEnvironment _environment = EEnvironment.Development;
        private BuildTarget _buildTarget = BuildTarget.Android;
        private string _activeBuildProfile = null;

        public PlayerBuilderWindow()
        {
            _activeBuildProfile = BuilderUtils.GetActiveBuildProfileFilePath(_environment);
        }

        public void DrawGUI(IEasyBuilderEditor parent)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Profile", GUILayout.Width(LabelWidth));
            GUI.enabled = false;
            GUILayout.TextField(_activeBuildProfile, GUILayout.MinWidth(400));
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            GUILayout.Label("App Target", GUILayout.Width(LabelWidth));
            _appTarget = (EAppTarget)EditorGUILayout.EnumPopup(_appTarget);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Environment", GUILayout.Width(LabelWidth));
            _environment = (EEnvironment)EditorGUILayout.EnumPopup(_environment);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Build Target", GUILayout.Width(LabelWidth));
            _buildTarget = (BuildTarget)EditorGUILayout.EnumPopup(_buildTarget);
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                _activeBuildProfile = BuilderUtils.GetActiveBuildProfileFilePath(_environment);
            }
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Setup Editor", GUILayout.Width(LabelWidth)))
            {
                ProjectSwitcher.SwitchTo(_appTarget, _environment, _buildTarget);
                ProjectSwitcher.SetScriptSymbolsTo(_appTarget, _environment);
                GUIUtility.ExitGUI();
            }

            if (GUILayout.Button("Build", GUILayout.Width(LabelWidth)))
            {
                EasyBuilder.Build(_appTarget, _environment, _buildTarget);
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();
        }
    }

}
