#if UNITY_EDITOR
using BonGames.Shared;
using BonGames.Tools;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace BonGames.UniConfigurator
{
    public class UniDatabaseEditor<T> : Editor where T : IUniRecord
    {
        private UniDatabase<T> _target;
        private UniRecordTypes<T> _typeFactory;

        private void OnEnable()
        {
            _target = serializedObject.targetObject as UniDatabase<T>;
            _typeFactory ??= new UniRecordTypes<T>();
        }

        public override void OnInspectorGUI()
        {
            if (_target == null) return;
            
            GUILayout.BeginVertical();
            for (int i = 0; i < _target.Count; i++)
            {
                DrawConfigJsonArea(i);
            }
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(EditorContents.IconClear, EditorCustomStyles.CriticalIcon, EditorUISize.M.Icon))
            {
                _target.Clear();
            }
            if (GUILayout.Button(EditorContents.IconRefresh, EditorUISize.M.Icon))
            {
                _typeFactory = new UniRecordTypes<T>();
            }
            if (GUILayout.Button(EditorContents.IconAdd, EditorUISize.M.Icon))
            {
                GenericMenu typeMenu = new GenericMenu() { allowDuplicateNames = false };
                foreach (string id in _typeFactory.CreatableIds())
                {
                    string captureId = id;
                    typeMenu.AddItem(new GUIContent(captureId), false, () =>
                    {
                        T config = _typeFactory.Create(captureId);
                        _target.Add(config);
                        _target.SetThisDirty();
                    });
                }
                typeMenu.ShowAsContext();
            }
            if (GUILayout.Button(EditorContents.IconRemove, EditorUISize.M.Icon))
            {
                if (_target.Count > 0)
                {
                    _target.RemoveAt(_target.Count - 1);
                    _target.SetThisDirty();
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawConfigJsonArea(int index)
        {
            string json = _target.GetRawValue(index);
            // Reserve space
            GUIStyle style = GUI.skin.textArea;
            float height = style.CalcHeight(new GUIContent(json), EditorGUIUtility.currentViewWidth - 30);
            Rect rect = GUILayoutUtility.GetRect(new GUIContent(json), style, GUILayout.Height(height));


            bool isEnable = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.TextArea(rect, json, style);
            GUI.enabled = isEnable;

            GUILayout.BeginHorizontal();

            GUILayout.Label($"Item {index}:");

            if (GUILayout.Button(EditorContents.IconDelete, EditorUISize.S.MaxButtonWidth))
            {
                _target.RemoveAt(index);
                _target.SetThisDirty();
            }

            if (GUILayout.Button(EditorContents.IconCopy, EditorUISize.S.MaxButtonWidth))
            {
                EditorGUIUtility.systemCopyBuffer = json;
            }

            if (GUILayout.Button(EditorContents.IconEdit, EditorUISize.S.MaxButtonWidth))
            {
                EditConfigurationJson(index);
            }

            if (GUILayout.Button(EditorContents.IconFormat, EditorUISize.S.MaxButtonWidth))
            {
                string jsonFormat = _target.GetRawValue(index);                
                _target[index] = UniDatabase<T>.Deserialize(jsonFormat);
                _target.SetThisDirty();
            }

            if (GUILayout.Button(EditorContents.IconDuplicate, EditorUISize.S.MaxButtonWidth))
            {
                JObject currentObject = JObject.Parse(_target.GetRawValue(index));
                if (currentObject.SelectToken(nameof(IUniRecord.Guid)) != null)
                {
                    currentObject[nameof(IUniRecord.Guid)] = System.Guid.NewGuid().ToString();
                }
                string json2Duplicate = currentObject.ToString();
                _target.Add(UniDatabase<T>.Deserialize(json2Duplicate));
                _target.SetThisDirty();
            }

            GUILayout.EndHorizontal();

            //// Handle click
            if (UnityEngine.Event.current.type == EventType.MouseUp && rect.Contains(UnityEngine.Event.current.mousePosition))
            {
                EditConfigurationJson(index);
            }
        }

        private void EditConfigurationJson(int index)
        {
            string json = _target.GetRawValue(index);

            TextEditorWindow.GetOrCreateWindow()
                    .SetText(json)
                    .OnFinishEvent += (newJson) =>
                    {
                        T config = UniDatabase<T>.Deserialize(newJson);
                        if (config != null)
                        {
                            _target[index] = config;
                            _target.SetThisDirty();
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Invalid JSON", "The JSON you entered is not valid.", "OK");
                        }
                    };
        }
    }
}
#endif