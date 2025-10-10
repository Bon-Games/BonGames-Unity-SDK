#if UNITY_EDITOR
using UnityEngine;

namespace BonGames.Shared
{
    public static class EditorCustomStyles
    {
        
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

        public static readonly GUIContent TextTab               = new GUIContent("Tab", string.Empty);
        public static readonly GUIContent TextBuildProfile      = new GUIContent("Build Profile", string.Empty);
        public static readonly GUIContent TextDlcProfile        = new GUIContent("Dlc Profile", string.Empty);
        public static readonly GUIContent TextBuildEnvironment  = new GUIContent("Environment", "Build Environment");
        public static readonly GUIContent TextBuildTarget       = new GUIContent("Build Target", "The build target platform");
        public static readonly GUIContent TextAppTarget         = new GUIContent("App Target", "Either Client or Server");
    }
}
#endif