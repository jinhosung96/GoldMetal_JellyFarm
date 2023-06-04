#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.ModernUIPack
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ButtonManager))]
    public class ButtonManagerEditor : Editor
    {
        private GUISkin customSkin;
        private ButtonManager bTarget;
        private UIManagerButton tempUIM;
        private int currentTab;

        private void OnEnable()
        {
            bTarget = (ButtonManager)target;

            try { tempUIM = bTarget.GetComponent<UIManagerButton>(); }
            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = (GUISkin)Resources.Load("Editor\\MUI Skin Dark"); }
            else { customSkin = (GUISkin)Resources.Load("Editor\\MUI Skin Light"); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "Button Top Header");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            currentTab = MUIPEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
                currentTab = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 2;

            GUILayout.EndHorizontal();

            var buttonText = serializedObject.FindProperty("buttonText");
            var hoverSound = serializedObject.FindProperty("hoverSound");
            var clickSound = serializedObject.FindProperty("clickSound");
            var clickEvent = serializedObject.FindProperty("clickEvent");
            var hoverEvent = serializedObject.FindProperty("hoverEvent");
            var normalText = serializedObject.FindProperty("normalText");
            var highlightedText = serializedObject.FindProperty("highlightedText");
            var soundSource = serializedObject.FindProperty("soundSource");
            var useCustomContent = serializedObject.FindProperty("useCustomContent");
            var enableButtonSounds = serializedObject.FindProperty("enableButtonSounds");
            var useHoverSound = serializedObject.FindProperty("useHoverSound");
            var useClickSound = serializedObject.FindProperty("useClickSound");
            var rippleParent = serializedObject.FindProperty("rippleParent");
            var useRipple = serializedObject.FindProperty("useRipple");
            var renderOnTop = serializedObject.FindProperty("renderOnTop");
            var centered = serializedObject.FindProperty("centered");
            var rippleShape = serializedObject.FindProperty("rippleShape");
            var speed = serializedObject.FindProperty("speed");
            var maxSize = serializedObject.FindProperty("maxSize");
            var startColor = serializedObject.FindProperty("startColor");
            var transitionColor = serializedObject.FindProperty("transitionColor");
            var animationSolution = serializedObject.FindProperty("animationSolution");
            var fadingMultiplier = serializedObject.FindProperty("fadingMultiplier");
            var rippleUpdateMode = serializedObject.FindProperty("rippleUpdateMode");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (useCustomContent.boolValue == false)
                    {
                        MUIPEditorHandler.DrawProperty(buttonText, customSkin, "Button Text");

                        if (bTarget.normalText != null) { bTarget.normalText.text = buttonText.stringValue; }
                        else if (bTarget.normalText == null) { EditorGUILayout.HelpBox("'Text Object' is missing.", MessageType.Error); }
                    }

                    else { EditorGUILayout.HelpBox("'Use Custom Content' is enabled.", MessageType.Info); }

                    if (enableButtonSounds.boolValue == true && useHoverSound.boolValue == true)
                        MUIPEditorHandler.DrawProperty(hoverSound, customSkin, "Hover Sound");
                    if (enableButtonSounds.boolValue == true && useClickSound.boolValue == true)
                        MUIPEditorHandler.DrawProperty(clickSound, customSkin, "Click Sound");

                    MUIPEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(clickEvent, new GUIContent("On Click Event"), true);
                    EditorGUILayout.PropertyField(hoverEvent, new GUIContent("On Hover Event"), true);
                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(normalText, customSkin, "Normal Text");
                    MUIPEditorHandler.DrawProperty(highlightedText, customSkin, "Highlighted Text");
                    if (enableButtonSounds.boolValue == true) { MUIPEditorHandler.DrawProperty(soundSource, customSkin, "Sound Source"); }
                    if (useRipple.boolValue == true) { MUIPEditorHandler.DrawProperty(rippleParent, customSkin, "Ripple Parent"); }
                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Customization Header", 6);
                    MUIPEditorHandler.DrawProperty(animationSolution, customSkin, "Animation Solution");
                    MUIPEditorHandler.DrawProperty(fadingMultiplier, customSkin, "Fading Multiplier");
                    useCustomContent.boolValue = MUIPEditorHandler.DrawToggle(useCustomContent.boolValue, customSkin, "Use Custom Content");
                   
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                   
                    enableButtonSounds.boolValue = MUIPEditorHandler.DrawTogglePlain(enableButtonSounds.boolValue, customSkin, "Enable Button Sounds");
                   
                    GUILayout.Space(3);

                    if (enableButtonSounds.boolValue == true)
                    {
                        useHoverSound.boolValue = MUIPEditorHandler.DrawToggle(useHoverSound.boolValue, customSkin, "Enable Hover Sound");
                        useClickSound.boolValue = MUIPEditorHandler.DrawToggle(useClickSound.boolValue, customSkin, "Enable Click Sound");

                        if (bTarget.soundSource == null)
                        {
                            EditorGUILayout.HelpBox("'Sound Source' is not assigned. Go to Resources tab or click the button to create a new audio source.", MessageType.Info);

                            if (GUILayout.Button("Create a new one", customSkin.button))
                            {
                                bTarget.soundSource = bTarget.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
                                currentTab = 2;
                            }
                        }
                    }

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-2);

                    useRipple.boolValue = MUIPEditorHandler.DrawTogglePlain(useRipple.boolValue, customSkin, "Use Ripple");

                    GUILayout.Space(4);

                    if (useRipple.boolValue == true)
                    {
                        renderOnTop.boolValue = MUIPEditorHandler.DrawToggle(renderOnTop.boolValue, customSkin, "Render On Top");
                        centered.boolValue = MUIPEditorHandler.DrawToggle(centered.boolValue, customSkin, "Centered");
                        MUIPEditorHandler.DrawProperty(rippleUpdateMode, customSkin, "Update Mode");
                        MUIPEditorHandler.DrawProperty(rippleShape, customSkin, "Shape");
                        MUIPEditorHandler.DrawProperty(speed, customSkin, "Speed");
                        MUIPEditorHandler.DrawProperty(maxSize, customSkin, "Max Size");
                        MUIPEditorHandler.DrawProperty(startColor, customSkin, "Start Color");
                        MUIPEditorHandler.DrawProperty(transitionColor, customSkin, "Transition Color");
                    }

                    GUILayout.EndVertical();
                    MUIPEditorHandler.DrawHeader(customSkin, "UIM Header", 10);

                    if (tempUIM != null)
                    {
                        MUIPEditorHandler.DrawUIManagerConnectedHeader();
                        tempUIM.overrideColors = MUIPEditorHandler.DrawToggle(tempUIM.overrideColors, customSkin, "Override Colors");
                        tempUIM.overrideFonts = MUIPEditorHandler.DrawToggle(tempUIM.overrideFonts, customSkin, "Override Fonts");

                        if (GUILayout.Button("Open UI Manager", customSkin.button))
                            EditorApplication.ExecuteMenuItem("Tools/Modern UI Pack/Show UI Manager");

                        if (GUILayout.Button("Disable UI Manager Connection", customSkin.button))
                        {
                            if (EditorUtility.DisplayDialog("Modern UI Pack", "Are you sure you want to disable UI Manager connection with the object? " +
                                "This operation cannot be undone.", "Yes", "Cancel"))
                            {
                                try { DestroyImmediate(tempUIM); }
                                catch { Debug.LogError("<b>[Horizontal Selector]</b> Failed to delete UI Manager connection.", this); }
                            }
                        }
                    }

                    else if (tempUIM == null)
                    {
                        if (bTarget.isPreset == true) { MUIPEditorHandler.DrawUIManagerPresetHeader(); }
                        else
                        {
                            MUIPEditorHandler.DrawUIManagerDisconnectedHeader();

                            if (GUILayout.Button("Restore UI Manager", customSkin.button))
                            {
                                UIManagerButton uimb = bTarget.gameObject.AddComponent<UIManagerButton>();

                                try
                                {
                                    uimb.buttonType = UIManagerButton.ButtonType.BasicOutline;
                                    uimb.basicOutlineBorder = bTarget.transform.Find("Normal/Background").GetComponent<UnityEngine.UI.Image>();
                                    uimb.basicOutlineFilled = bTarget.transform.Find("Highlighted/Background").GetComponent<UnityEngine.UI.Image>();
                                    uimb.basicOutlineText = bTarget.transform.Find("Normal/Text").GetComponent<TMPro.TextMeshProUGUI>();
                                    uimb.basicOutlineTextHighligted = bTarget.transform.Find("Highlighted/Text").GetComponent<TMPro.TextMeshProUGUI>();
                                }

                                catch { DestroyImmediate(uimb); Debug.LogError("<b>[Modern UI Pack]</b> Cannot restore the UI Manager connection."); }
                            }
                        }
                    }

                    break;            
            }

            this.Repaint();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif