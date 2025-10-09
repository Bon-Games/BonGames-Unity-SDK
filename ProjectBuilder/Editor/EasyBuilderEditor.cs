#if UNITY_EDITOR
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

        private void InitializeWindow()
        {
            if (_pages == null)
            {
                _pages = new()
                {
                    { EWindow.AppBuilder, new PlayerBuilderWindow() },
                    { EWindow.DlcBuilder, new DlcBuilderWindow() },
                    { EWindow.BuildProfileCreator, new BuildProfileCreatorWindow() },
                };
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Tab: ", GUILayout.MaxWidth(220));
            _activeWindow = (EWindow)EditorGUILayout.EnumPopup(_activeWindow);
            GUILayout.EndHorizontal();

            if (_pages != null && _pages.TryGetValue(_activeWindow, out IEditorWindow p) && p != null)
            {
                p.DrawGUI(this);
            }
            GUILayout.EndVertical();
        }
    }
}
#endif