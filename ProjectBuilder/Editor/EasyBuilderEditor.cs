#if UNITY_EDITOR
using BonGames.CommandLine;
using BonGames.EasyBuilder.Argument;
using BonGames.EasyBuilder.Enum;
using BonGames.Tools;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BonGames.EasyBuilder
{
    public class EasyBuilderEditor : EditorWindow, IEasyBuilderEditor
    {
        enum EWindow
        {
            Builder = 0,
            BuildProfileCreator = 1,
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
                    { EWindow.Builder, new PlayerBuilderWindow() },
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