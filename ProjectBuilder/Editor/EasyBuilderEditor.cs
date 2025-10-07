using BonGames.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BonGames.EasyBuilder
{
    public class EasyBuilderEditor : EditorWindow
    {
        private string _text = "Enter your text here...";
        private Vector2 _scrollPosition = Vector2.zero;

        public event System.Action<string> OnFinishEvent;

        [MenuItem("BonGames/Easy Builder Editor")]
        public static void ShowWindow()
        {
            GetOrCreateWindow();
        }

        public static EasyBuilderEditor GetOrCreateWindow()
        {
            return GetWindow<EasyBuilderEditor>("Easy Builder");
        }


        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Clear"))
            {
                _text = "";
                _scrollPosition = Vector2.zero;
            }

            GUILayout.EndHorizontal();
        }
    }
}
