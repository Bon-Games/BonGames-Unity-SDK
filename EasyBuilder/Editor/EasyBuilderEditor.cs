#if UNITY_EDITOR
using BonGames.Shared;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BonGames.EasyBuilder
{
    public class EasyBuilderEditor : EditorWindow, IEasyBuilderEditor
    {
        enum EWindow
        {
            AppBuilder = 0,
            DlcBuilder = 1,
            BuildProfileCreator = 2,
            BuildProcessorsEditor = 3,
        }
        [MenuItem("BonGames/Easy Builder Editor")]
        public static void ShowWindow()
        {
            GetOrCreateWindow();
        }

        public static EasyBuilderEditor GetOrCreateWindow()
        {
            EasyBuilderEditor win = GetWindow<EasyBuilderEditor>("Easy Builder");
            win.InitializeWindow();
            return win;
        }

        private EWindow _activeWindow;
        private Dictionary<EWindow, IEditorWindow> _pages;
        private Vector2 _scrollPosition = Vector2.zero;

        private void InitializeWindow()
        {
            if (_pages == null)
            {
                _pages = new()
                {
                    { EWindow.AppBuilder, new PlayerBuilderWindow() },
                    { EWindow.DlcBuilder, new DlcBuilderWindow() },
                    { EWindow.BuildProfileCreator, new BuildProfileCreatorWindow() },
                    { EWindow.BuildProcessorsEditor, new BuildProcesssorsEditorWindow() },
                };
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextWindow, EditorUISize.S.MaxLabelWidth);
            _activeWindow = (EWindow)EditorGUILayout.EnumPopup(_activeWindow, EditorUISize.S.MaxButtonWidth);
            GUILayout.EndHorizontal();

            if (_pages != null && _pages.TryGetValue(_activeWindow, out IEditorWindow p) && p != null)
            {
                p.DrawGUI(this);
            }
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}
#endif