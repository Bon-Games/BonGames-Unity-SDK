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
    public interface IPage
    {
        public void DrawGUI();
    }

    public class BuildProfileCreator : IPage
    {
        private const uint LabelWidth = 220;

        private readonly Dictionary<string, string> _definedParams;
        private readonly Dictionary<string, string> _params2Declares;
        private readonly List<string> _ignoreParams = new List<string>()
        {
           nameof(BuildArguments.Key.BuildApp),
           nameof(BuildArguments.Key.CI),
           nameof(BuildArguments.Key.BuildAppTarget),
           nameof(BuildArguments.Key.BuildEnvironment),
           nameof(BuildArguments.Key.BuildPlatformTarget),
           nameof(BuildArguments.Key.BuildVersionString),
           nameof(BuildArguments.Key.GitRevision),
           nameof(BuildArguments.Key.GitBranch),
        };
        private string _profilePath;
        private EEnvironment _environment;
        private Dictionary<string, string> _tempParams;
        public BuildProfileCreator()
        {
            _tempParams = new();
            _definedParams = ArgumentsExpander.GetDefinedArguments().ToDictionary(it => it.Key, it => string.Empty);
            _params2Declares = ArgumentsExpander.GetDefinedArguments().ToDictionary(it => it.Value, it => it.Key);

            _profilePath = GetDefaultProfilePath();
            LoadProfile(_profilePath);
        }

        public void DrawGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Profile", GUILayout.Width(LabelWidth));
            GUI.enabled = false;
            GUILayout.TextField(_profilePath, GUILayout.MinWidth(400));
            GUI.enabled = true;
            if (GUILayout.Button("Load Profile", GUILayout.Width(LabelWidth)))
            {
                string selected = EditorUtility.OpenFilePanel("Profile", BuilderUtils.BuildProfileDirectory(), "args*");
                if (!string.IsNullOrEmpty(selected))
                {
                    _profilePath = selected;
                    LoadProfile(_profilePath);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Environment", GUILayout.Width(LabelWidth));
            _environment = (EEnvironment)EditorGUILayout.EnumPopup(_environment);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                _profilePath = GetDefaultProfilePath();
                LoadProfile(_profilePath);
            }

            EditorGUI.BeginChangeCheck();
            foreach (string key in _definedParams.Keys)
            {
                if (_ignoreParams.Contains(key)) continue;

                GUILayout.BeginHorizontal();
                GUILayout.Label(key, GUILayout.Width(LabelWidth));
                _tempParams[key] = GUILayout.TextField(_definedParams[key], GUILayout.MinWidth(400));
                GUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                foreach (string key in _tempParams.Keys)
                {
                    if (_tempParams[key] != _definedParams[key])
                    {
                        _definedParams[key] = _tempParams[key];
                    }
                }
            }

            if (GUILayout.Button("Save", GUILayout.Width(LabelWidth)))
            {
                StringBuilder content = new();
                foreach (KeyValuePair<string, string> pair in _params2Declares)
                {
                    if (_definedParams.ContainsKey(pair.Value))
                    {
                        content.AppendLine($"{pair.Key}={_definedParams[pair.Value]}");
                    }
                }
                File.WriteAllText(_profilePath, content.ToString());
            }
        }

        private string GetDefaultProfilePath()
        {
            return Path.Combine(BuilderUtils.BuildProfileDirectory(), $".args.{_environment}");
        }

        private bool LoadProfile(string profilePath)
        {
            if (!File.Exists(profilePath)) return false;

            Dictionary<string, string> loaded = ArgumentsResolver.LoadArguments(_profilePath);
            foreach (KeyValuePair<string, string> pair in loaded)
            {
                if (_params2Declares.ContainsKey(pair.Key))
                {
                    _tempParams[_params2Declares[pair.Key]] = pair.Value;
                    _definedParams[_params2Declares[pair.Key]] = pair.Value;
                }
            }

            return true;
        }
    }

    public class PlayerBuilder : IPage
    {
        private const uint LabelWidth = 220;

        private EAppTarget _appTarget = EAppTarget.Client;
        private EEnvironment _environment = EEnvironment.Development;
        private BuildTarget _buildTarget = BuildTarget.Android;
        private string _activeBuildProfile = null;
        public PlayerBuilder()
        {
            _activeBuildProfile = BuilderUtils.GetActiveBuildProfileFilePath(_environment);
        }

        public void DrawGUI()
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
            
            if (GUILayout.Button("Build", GUILayout.Width(LabelWidth)))
            {
                EasyBuilder.Build(_appTarget, _environment, _buildTarget);
            }
        }
    }

    public class EasyBuilderEditor : EditorWindow
    {
        enum Page
        {
            Builder = 0,
            BuildProfileCreator = 1,
        }
        private Page _page;
        private bool _isInited = false;
        private Dictionary<Page, IPage> _pages;



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

        private void InitializeWindow()
        {
            if (_isInited) return;

            _pages = new()
            {
                { Page.Builder, new PlayerBuilder() },
                { Page.BuildProfileCreator, new BuildProfileCreator() },
            };
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Tab: ", GUILayout.Width(220));
            _page = (Page)EditorGUILayout.EnumPopup(_page);
            GUILayout.EndHorizontal();

            if (_pages.TryGetValue(_page, out IPage p) && p != null)
            {
                p.DrawGUI();
            }
            GUILayout.EndVertical();
        }
    }
}
#endif