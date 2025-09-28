using UnityEngine;
using UnityEditor;

namespace BonGames.Tools
{
    public class TextEditorWindow : EditorWindow
    {
        private string _text = "Enter your text here...";
        private Vector2 _scrollPosition = Vector2.zero;

        public event System.Action<string> OnFinishEvent;

        [MenuItem("BonGames/Text Editor")]
        public static void ShowWindow()
        {
            GetOrCreateWindow();
        }

        public static TextEditorWindow GetOrCreateWindow()
        {
            return GetWindow<TextEditorWindow>("Text Editor");
        }

        public TextEditorWindow SetText(string text)
        {
            _text = text;
            return this;
        }

        private void OnGUI()
        {
            GUILayout.Label("Text Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            _text = EditorGUILayout.TextArea(_text, GUILayout.ExpandHeight(true));

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Clear"))
            {
                _text = "";
                _scrollPosition = Vector2.zero;
            }

            if (GUILayout.Button("Copy to Clipboard"))
            {
                EditorGUIUtility.systemCopyBuffer = _text;
            }

            if (GUILayout.Button("Complete"))
            {
                if (OnFinishEvent != null)
                {
                    OnFinishEvent.Invoke(_text);
                }
                this.Close();
            }

            GUILayout.EndHorizontal();
        }
    }
}
