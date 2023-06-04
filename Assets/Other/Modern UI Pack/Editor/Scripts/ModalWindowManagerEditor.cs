#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.UI.ModernUIPack
{
    [CustomEditor(typeof(ModalWindowManager))]
    public class ModalWindowManagerEditor : Editor
    {
        private GUISkin customSkin;
        private ModalWindowManager mwTarget;
        private UIManagerModalWindow tempUIM;
        private int currentTab;

        private void OnEnable()
        {
            mwTarget = (ModalWindowManager)target;

            try { tempUIM = mwTarget.GetComponent<UIManagerModalWindow>(); }
            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = (GUISkin)Resources.Load("Editor\\MUI Skin Dark"); }
            else { customSkin = (GUISkin)Resources.Load("Editor\\MUI Skin Light"); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "MW Top Header");

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

            var icon = serializedObject.FindProperty("icon");
            var titleText = serializedObject.FindProperty("titleText");
            var descriptionText = serializedObject.FindProperty("descriptionText");
            var onConfirm = serializedObject.FindProperty("onConfirm");
            var onCancel = serializedObject.FindProperty("onCancel");
            var windowIcon = serializedObject.FindProperty("windowIcon");
            var windowTitle = serializedObject.FindProperty("windowTitle");
            var windowDescription = serializedObject.FindProperty("windowDescription");
            var confirmButton = serializedObject.FindProperty("confirmButton");
            var cancelButton = serializedObject.FindProperty("cancelButton");
            var mwAnimator = serializedObject.FindProperty("mwAnimator");
            var sharpAnimations = serializedObject.FindProperty("sharpAnimations");
            var useCustomValues = serializedObject.FindProperty("useCustomValues");
            var closeBehaviour = serializedObject.FindProperty("closeBehaviour");
            var startBehaviour = serializedObject.FindProperty("startBehaviour");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (useCustomValues.boolValue == false)
                    {
                        MUIPEditorHandler.DrawProperty(icon, customSkin, "Icon");

                        if (mwTarget.windowIcon != null) { mwTarget.windowIcon.sprite = mwTarget.icon; }
                        else if (mwTarget.windowIcon == null)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("'Icon Object' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                            GUILayout.EndHorizontal();
                        }

                        MUIPEditorHandler.DrawProperty(titleText, customSkin, "Title");

                        if (mwTarget.windowTitle != null) { mwTarget.windowTitle.text = titleText.stringValue; }
                        else if (mwTarget.windowTitle == null)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("'Title Object' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.BeginHorizontal(EditorStyles.helpBox);
                        EditorGUILayout.LabelField(new GUIContent("Description"), customSkin.FindStyle("Text"), GUILayout.Width(-3));
                        EditorGUILayout.PropertyField(descriptionText, new GUIContent(""), GUILayout.Height(80));
                        GUILayout.EndHorizontal();

                        if (mwTarget.windowDescription != null) { mwTarget.windowDescription.text = descriptionText.stringValue; }
                        else if (mwTarget.windowDescription == null)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("'Description Object' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                            GUILayout.EndHorizontal();
                        }
                    }

                    else { EditorGUILayout.HelpBox("'Use Custom Content' is enabled.", MessageType.Info); }

                    if (mwTarget.GetComponent<CanvasGroup>().alpha == 0)
                    {
                        if (GUILayout.Button("Make It Visible", customSkin.button))
                        {
                            mwTarget.GetComponent<CanvasGroup>().alpha = 1;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }

                    else
                    {
                        if (GUILayout.Button("Make It Invisible", customSkin.button))
                        {
                            mwTarget.GetComponent<CanvasGroup>().alpha = 0;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }

                    MUIPEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onConfirm, new GUIContent("On Confirm"), true);
                    EditorGUILayout.PropertyField(onCancel, new GUIContent("On Cancel"), true);
                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(windowIcon, customSkin, "Icon Object");
                    MUIPEditorHandler.DrawProperty(windowTitle, customSkin, "Title Object");
                    MUIPEditorHandler.DrawProperty(windowDescription, customSkin, "Description Object");
                    MUIPEditorHandler.DrawProperty(confirmButton, customSkin, "Confirm Button");
                    MUIPEditorHandler.DrawProperty(cancelButton, customSkin, "Cancel Button");
                    MUIPEditorHandler.DrawProperty(mwAnimator, customSkin, "Animator");
                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    MUIPEditorHandler.DrawProperty(startBehaviour, customSkin, "Start Behaviour");
                    MUIPEditorHandler.DrawProperty(closeBehaviour, customSkin, "Close Behaviour");
                    sharpAnimations.boolValue = MUIPEditorHandler.DrawToggle(sharpAnimations.boolValue, customSkin, "Sharp Animations");
                    useCustomValues.boolValue = MUIPEditorHandler.DrawToggle(useCustomValues.boolValue, customSkin, "Use Custom Content");

                    MUIPEditorHandler.DrawHeader(customSkin, "UIM Header", 10);

                    if (tempUIM != null)
                    {
                        MUIPEditorHandler.DrawUIManagerConnectedHeader();

                        if (GUILayout.Button("Open UI Manager", customSkin.button))
                            EditorApplication.ExecuteMenuItem("Tools/Modern UI Pack/Show UI Manager");

                        if (GUILayout.Button("Disable UI Manager Connection", customSkin.button))
                        {
                            if (EditorUtility.DisplayDialog("Modern UI Pack", "Are you sure you want to disable UI Manager connection with the object? " +
                                "This operation cannot be undone.", "Yes", "Cancel"))
                            {
                                try { DestroyImmediate(tempUIM); }
                                catch { Debug.LogError("<b>[Modal Window]</b> Failed to delete UI Manager connection.", this); }
                            }
                        }
                    }

                    else if (tempUIM == null) { MUIPEditorHandler.DrawUIManagerDisconnectedHeader(); }

                    break;
            }

            this.Repaint();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif