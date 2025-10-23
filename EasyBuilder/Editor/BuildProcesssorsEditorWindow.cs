using BonGames.EasyBuilder.Enum;
using BonGames.Shared;
using BonGames.Tools;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BonGames.EasyBuilder
{
    enum Processor
    {
        PreProcessor = 0,
        PostProcessor = 1,
    }

    class ObjectSelector
    {
        private readonly System.Type _targetType;
        private readonly bool _includeSceneObject;
        private readonly GUIContent _label;

        private UnityEngine.Object _unityObject = null;
        private UnityEditor.Editor _objectEditor = null;

        public ObjectSelector(GUIContent label, System.Type targetType, bool includeSceneObject)
        {
            _targetType = targetType;
            _includeSceneObject = includeSceneObject;
            _label = label;
        }

        public void DrawGUI()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextSelected, EditorUISize.S.MaxLabelWidth);
            EditorGUI.BeginChangeCheck();
            _unityObject = EditorGUILayout.ObjectField(obj: _unityObject, objType: _targetType, allowSceneObjects: _includeSceneObject, EditorUISize.S.MinOnelineInputWidth);
            if (EditorGUI.EndChangeCheck())
            {
                Editor.CreateCachedEditor(_unityObject, null, ref _objectEditor);
            }
            GUILayout.EndHorizontal();

            if (_unityObject != null)
            {
                _objectEditor.OnInspectorGUI();
            }
            EditorGUILayout.EndVertical();
        }

        public string ObjectName() => _unityObject != null ? _unityObject.name : string.Empty;

        public void SetSelected(UnityEngine.Object selected)
        {
            _unityObject = selected;
            Editor.CreateCachedEditor(_unityObject, null, ref _objectEditor);
        }
    }

    public class BuildProcesssorsEditorWindow : BaseEditorWindow
    {
        private Processor _processorType;
        private ObjectSelector _processorSelector;
        private string _newProcessorName;

        public BuildProcesssorsEditorWindow()
        {
            _processorType = Processor.PreProcessor;
            _processorSelector = new ObjectSelector(EditorContents.TextSelected, typeof(PreBuildProcessors), false);
        }

        public override void DrawGUI(IEasyBuilderEditor parent)
        {
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            GUILayout.Label(EditorContents.TextType, EditorUISize.S.MaxLabelWidth);
            _processorType = (Processor)EditorGUILayout.EnumPopup(_processorType, EditorUISize.S.MaxButtonWidth);
            if (EditorGUI.EndChangeCheck())
            {
                _processorSelector = _processorType == Processor.PreProcessor ? new ObjectSelector(EditorContents.TextSelected, typeof(PreBuildProcessors), false)
                    : new ObjectSelector(EditorContents.TextSelected, typeof(PostBuildProcessors), false);
            }
            GUILayout.EndHorizontal();

            _processorSelector.DrawGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextName, EditorUISize.S.MaxLabelWidth);
            _newProcessorName = GUILayout.TextField(_newProcessorName, EditorUISize.S.MinOnelineInputWidth);
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_newProcessorName));
            if (GUILayout.Button(EditorContents.CTACreateNew, Shared.EditorUISize.S.MaxButtonWidth))
            {
                _processorSelector.SetSelected(CreateNewProcessors(_newProcessorName));
                _newProcessorName = string.Empty;
                GUIUtility.ExitGUI();
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }
        
        private ScriptableObject CreateNewProcessors(string name)
        {
            if (_processorType == Processor.PreProcessor) 
            {
                return CreateBuildProcessors<PreBuildProcessors>(name);
            }
            else
            {
                return CreateBuildProcessors<PostBuildProcessors>(name);
            }
        }

        public T CreateBuildProcessors<T>(string fileName) where T : ScriptableObject
        {
            try
            {
                string fullPath = System.IO.Path.Combine("Assets", "BuildProcessors", $"{fileName}.asset");
                string folderPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "BuildProcessors");
                if (!System.IO.Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                T asset = ScriptableObject.CreateInstance<T>();
                string path = AssetDatabase.GenerateUniqueAssetPath(fullPath);
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                Selection.activeObject = asset;
                return asset;
            }
            catch (Exception ex) 
            {
                Domain.LogE(ex.ToString());
                return null;
            }
        }
    }

}
