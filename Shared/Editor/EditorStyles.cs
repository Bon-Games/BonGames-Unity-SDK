#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace BonGames.Shared
{
    public static class EditorCustomStyles
    {
        public static readonly GUIStyle CriticalIcon = new GUIStyle(GUI.skin.button)
        {
            normal = new GUIStyleState() { textColor = Color.red },
            hover = new GUIStyleState() { textColor = Color.red }
        };

        public static readonly GUIStyle WarningIcon = new GUIStyle(GUI.skin.button)
        {
            normal = new GUIStyleState() { textColor = Color.yellow },
            hover = new GUIStyleState() { textColor = Color.yellow },
        };
    }

    public static class EditorUISize
    {
        public static readonly GUILayoutOption ExpandWidth  = GUILayout.ExpandWidth(true);
        public static readonly GUILayoutOption ExpandHeight = GUILayout.ExpandHeight(true);

        public static class S
        {
            public static readonly GUILayoutOption MaxButtonWidth               = GUILayout.MaxWidth(180);
            public static readonly GUILayoutOption MaxLabelWidth                = GUILayout.MaxWidth(180);
            public static readonly GUILayoutOption MinOnelineInputWidth         = GUILayout.MaxWidth(400);
            public static readonly GUILayoutOption[] Icon                       = new GUILayoutOption[] { GUILayout.Height(20), GUILayout.Width(20) };
        }

        public static class M
        {
            public static readonly GUILayoutOption[] Icon = new GUILayoutOption[] { GUILayout.Height(32), GUILayout.Width(32) };
        }

        public static class L
        {
            
        }

        public static class XL
        {

        }
    }

    public static class EditorContents
    {
        public static readonly GUIContent CTAClear              = new GUIContent("Clear", string.Empty);
        public static readonly GUIContent CTACopyToClipboard    = new GUIContent("Copy to Clipboard", string.Empty);
        public static readonly GUIContent CTAComplete           = new GUIContent("Complete", string.Empty);
        public static readonly GUIContent CTABuild              = new GUIContent("Build", "Start Build");
        public static readonly GUIContent CTASave               = new GUIContent("Save", "Start Build");
        public static readonly GUIContent CTAEditorSetup        = new GUIContent("Editor Setup", "Start Editor Setup");
        public static readonly GUIContent CTACreateNew          = new GUIContent("Create New", string.Empty);
        public static readonly GUIContent CTARefresh            = new GUIContent("Refresh", string.Empty);
        public static readonly GUIContent CTARemove             = new GUIContent("Remove", string.Empty);
        public static readonly GUIContent CTADuplicate          = new GUIContent("Duplicate", string.Empty);
        public static readonly GUIContent CTAFormat             = new GUIContent("Format", string.Empty);
        public static readonly GUIContent CTAEdit               = new GUIContent("Edit", string.Empty);

        public static readonly GUIContent TextWindow            = new GUIContent("Window", string.Empty);
        public static readonly GUIContent TextBuildProfile      = new GUIContent("Build Profile", string.Empty);
        public static readonly GUIContent TextDlcProfile        = new GUIContent("Dlc Profile", string.Empty);
        public static readonly GUIContent TextBuildEnvironment  = new GUIContent("Environment", "Build Environment");
        public static readonly GUIContent TextBuildTarget       = new GUIContent("Build Target", "The build target platform");
        public static readonly GUIContent TextAppTarget         = new GUIContent("App Target", "Either Client or Server");
        public static readonly GUIContent TextSelected          = new GUIContent("Selected", string.Empty);
        public static readonly GUIContent TextCollection        = new GUIContent("Collection", string.Empty);
        public static readonly GUIContent TextName              = new GUIContent("Name", string.Empty);
        public static readonly GUIContent TextType              = new GUIContent("Type", string.Empty);
        public static readonly GUIContent TextOutput            = new GUIContent("Output", string.Empty);

        public static readonly GUIContent IconDoesntExistWarning    = new GUIContent("\u26A0", "Doesn't exist");
        public static readonly GUIContent IconAdd                   = new GUIContent("\u271A", "Add");
        public static readonly GUIContent IconRemove                = new GUIContent("\u2501", "Remove");
        public static readonly GUIContent IconCopy                  = new GUIContent("[C]", "Copy to Clipboard");
        public static readonly GUIContent IconDelete                = new GUIContent("\u2715", "Delete");
        public static readonly GUIContent IconEdit                  = new GUIContent("\u270E", "Edit");
        public static readonly GUIContent IconDuplicate             = new GUIContent("\u271A\u271A", "Duplicate");
        public static readonly GUIContent IconFormat                = new GUIContent("\u2261", "Format");
        public static readonly GUIContent IconRefresh               = new GUIContent("\u21BB", "Refresh");
        public static readonly GUIContent IconClear                 = new GUIContent("\u2327", "Clear");

    }
}
#endif