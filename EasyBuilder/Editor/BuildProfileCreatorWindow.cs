using BonGames.CommandLine;
using BonGames.EasyBuilder.Argument;
using BonGames.EasyBuilder.Enum;
using BonGames.Shared;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BonGames.EasyBuilder
{
    public class BuildProfileCreatorWindow : BaseEditorWindow
    {
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
        private bool _doesProfileExist;

        public BuildProfileCreatorWindow()
        {
            _definedParams = ArgumentsExpander.GetDefinedArguments().ToDictionary(it => it.Key, it => string.Empty);
            _params2Declares = ArgumentsExpander.GetDefinedArguments().ToDictionary(it => it.Value, it => it.Key);
            _environment = EEnvironment.Development;
            ReloadBuildProfile();
        }

        public override void DrawGUI(IEasyBuilderEditor parent)
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextBuildProfile, EditorUISize.S.MaxLabelWidth);
            EditorGUI.BeginDisabledGroup(true);
            GUILayout.TextField(_profilePath, EditorUISize.S.MinOnelineInputWidth, EditorUISize.ExpandWidth);
            EditorGUI.EndDisabledGroup();
            if (!_doesProfileExist)
            {
                GUILayout.Label(EditorContents.IconDoesntExistWarning, EditorCustomStyles.WarningIcon, Shared.EditorUISize.S.Icon);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label(EditorContents.TextBuildEnvironment , EditorUISize.S.MaxButtonWidth);
            _environment = (EEnvironment)EditorGUILayout.EnumPopup(_environment, EditorUISize.S.MaxButtonWidth);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                ReloadBuildProfile();
            }

            EditorGUI.BeginChangeCheck();
            foreach (string key in _params2Declares.Values)
            {
                if (_ignoreParams.Contains(key)) continue;

                GUILayout.BeginHorizontal();
                GUILayout.Label(key, EditorUISize.S.MaxLabelWidth);
                if (!_definedParams.ContainsKey(key))
                {
                    _definedParams.Add(key, string.Empty);
                }
                _definedParams[key] = GUILayout.TextField(_definedParams[key], EditorUISize.S.MinOnelineInputWidth, Shared.EditorUISize.ExpandWidth);
                GUILayout.EndHorizontal();
            }

            if (GUILayout.Button(EditorContents.CTASave, EditorUISize.S.MaxButtonWidth))
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
                ReloadBuildProfile();
            }
        }

        private string GetDefaultProfilePath()
        {
            return Path.Combine(BuilderUtils.BuildProfileDirectory(), $".args.{_environment}".ToLower());
        }

        private bool LoadProfile(string profilePath)
        {
            _definedParams.Clear();

            if (!File.Exists(profilePath)) return false;

            Dictionary<string, string> loaded = ArgumentsResolver.LoadArguments(_profilePath);
            foreach (KeyValuePair<string, string> pair in loaded)
            {
                if (_params2Declares.ContainsKey(pair.Key))
                {
                    _definedParams[_params2Declares[pair.Key]] = pair.Value;
                }
            }

            return true;
        }

        private void ReloadBuildProfile()
        {
            _profilePath = GetDefaultProfilePath();
            _doesProfileExist = System.IO.File.Exists(_profilePath);
            LoadProfile(_profilePath);
        }
    }
}
