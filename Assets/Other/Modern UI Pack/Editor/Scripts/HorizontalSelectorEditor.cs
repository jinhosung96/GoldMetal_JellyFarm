#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.UI.ModernUIPack
{
    [CustomEditor(typeof(HorizontalSelector))]
    public class HorizontalSelectorEditor : Editor
    {
        private GUISkin customSkin;
        private HorizontalSelector hsTarget;
        private UIManagerHSelector tempUIM;
        private int currentTab;

        private void OnEnable()
        {
            hsTarget = (HorizontalSelector)target;

            try { tempUIM = hsTarget.GetComponent<UIManagerHSelector>(); }
            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = (GUISkin)Resources.Load("Editor\\MUI Skin Dark"); }
            else { customSkin = (GUISkin)Resources.Load("Editor\\MUI Skin Light"); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "HS Top Header");

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

            var itemList = serializedObject.FindProperty("itemList");
            var onValueChanged = serializedObject.FindProperty("onValueChanged");
            var label = serializedObject.FindProperty("label");
            var selectorAnimator = serializedObject.FindProperty("selectorAnimator");
            var labelHelper = serializedObject.FindProperty("labelHelper");
            var labelIcon = serializedObject.FindProperty("labelIcon");
            var labelIconHelper = serializedObject.FindProperty("labelIconHelper");
            var indicatorParent = serializedObject.FindProperty("indicatorParent");
            var indicatorObject = serializedObject.FindProperty("indicatorObject");
            var enableIcon = serializedObject.FindProperty("enableIcon");
            var saveValue = serializedObject.FindProperty("saveValue");
            var selectorTag = serializedObject.FindProperty("selectorTag");
            var enableIndicators = serializedObject.FindProperty("enableIndicators");
            var invokeAtStart = serializedObject.FindProperty("invokeAtStart");
            var invertAnimation = serializedObject.FindProperty("invertAnimation");
            var loopSelection = serializedObject.FindProperty("loopSelection");
            var defaultIndex = serializedObject.FindProperty("defaultIndex");
            var iconScale = serializedObject.FindProperty("iconScale");
            var contentSpacing = serializedObject.FindProperty("contentSpacing");
            var contentLayout = serializedObject.FindProperty("contentLayout");
            var contentLayoutHelper = serializedObject.FindProperty("contentLayoutHelper");
            var enableUIManager = serializedObject.FindProperty("enableUIManager");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (hsTarget.itemList.Count != 0)
                    {
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);

                        EditorGUILayout.LabelField(new GUIContent("Default Item Index"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                        defaultIndex.intValue = EditorGUILayout.IntSlider(defaultIndex.intValue, 0, hsTarget.itemList.Count - 1);

                        GUILayout.Space(2);
                        GUILayout.EndHorizontal();
                    }

                    else
                        EditorGUILayout.HelpBox("There is no item in the list.", MessageType.Warning);

                    if (Application.isPlaying == true)
                    {
                        GUI.enabled = false;
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);

                        EditorGUILayout.LabelField(new GUIContent("Current Index"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                        EditorGUILayout.IntSlider(hsTarget.index, 0, hsTarget.itemList.Count - 1);

                        GUILayout.Space(2);
                        GUILayout.EndHorizontal();
                        GUI.enabled = true;
                    }

#if UNITY_2020_1_OR_NEWER
                    GUILayout.BeginVertical();
#else
                    GUILayout.BeginVertical(EditorStyles.helpBox);
#endif
                    EditorGUI.indentLevel = 1;

                    EditorGUILayout.PropertyField(itemList, new GUIContent("Selector Items"), true);
                    itemList.isExpanded = true;

                    EditorGUI.indentLevel = 1;
                    if (GUILayout.Button("+  Add a new item", customSkin.button))
                        hsTarget.AddNewItem();
                   
                    GUILayout.EndVertical();

                    MUIPEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onValueChanged, new GUIContent("On Value Changed"), true);
                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(selectorAnimator, customSkin, "Animator");
                    MUIPEditorHandler.DrawProperty(label, customSkin, "Label");
                    MUIPEditorHandler.DrawProperty(labelHelper, customSkin, "Label Helper");
                    MUIPEditorHandler.DrawProperty(labelIcon, customSkin, "Label Icon");
                    MUIPEditorHandler.DrawProperty(labelIconHelper, customSkin, "Label Icon Helper");
                    MUIPEditorHandler.DrawProperty(indicatorParent, customSkin, "Indicator Parent");
                    MUIPEditorHandler.DrawProperty(indicatorObject, customSkin, "Indicator Object");
                    MUIPEditorHandler.DrawProperty(contentLayout, customSkin, "Content Layout");
                    MUIPEditorHandler.DrawProperty(contentLayoutHelper, customSkin, "Content Layout Helper");
                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Customization Header", 6);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);       
                    enableIcon.boolValue = MUIPEditorHandler.DrawTogglePlain(enableIcon.boolValue, customSkin, "Enable Icon");            
                    GUILayout.Space(3);

                    if (enableIcon.boolValue == true && hsTarget.labelIcon == null)
                        EditorGUILayout.HelpBox("'Enable Icon' is enabled but 'Label Icon' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                    else if (enableIcon.boolValue == true && hsTarget.labelIcon != null)
                        hsTarget.labelIcon.gameObject.SetActive(true);
                    else if (enableIcon.boolValue == false && hsTarget.labelIcon != null)
                        hsTarget.labelIcon.gameObject.SetActive(false);

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    enableIndicators.boolValue = MUIPEditorHandler.DrawTogglePlain(enableIndicators.boolValue, customSkin, "Enable Indicators");
                    GUILayout.Space(3);
                    GUILayout.BeginHorizontal();

                    if (enableIndicators.boolValue == true)
                    {
                        if (hsTarget.indicatorObject == null)
                            EditorGUILayout.HelpBox("'Enable Indicators' is enabled but 'Indicator Object' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);

                        if (hsTarget.indicatorParent == null)
                            EditorGUILayout.HelpBox("'Enable Indicators' is enabled but 'Indicator Parent' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                        else
                            hsTarget.indicatorParent.gameObject.SetActive(true);
                    }

                    else if (enableIndicators.boolValue == false && hsTarget.indicatorParent != null)
                        hsTarget.indicatorParent.gameObject.SetActive(false);

                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    MUIPEditorHandler.DrawProperty(iconScale, customSkin, "Icon Scale");
                    MUIPEditorHandler.DrawProperty(contentSpacing, customSkin, "Content Spacing");
                    hsTarget.UpdateContentLayout();

                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 10);
                    invokeAtStart.boolValue = MUIPEditorHandler.DrawToggle(invokeAtStart.boolValue, customSkin, "Invoke At Start");

                    if (tempUIM != null && tempUIM.overrideOptions == false)
                        GUI.enabled = false;

                    invertAnimation.boolValue = MUIPEditorHandler.DrawToggle(invertAnimation.boolValue, customSkin, "Invert Animation");
                    loopSelection.boolValue = MUIPEditorHandler.DrawToggle(loopSelection.boolValue, customSkin, "Loop Selection");
                    GUI.enabled = true;
                    
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    saveValue.boolValue = MUIPEditorHandler.DrawTogglePlain(saveValue.boolValue, customSkin, "Save Selection");
                    GUILayout.Space(3);

                    if (saveValue.boolValue == true)
                    {
                        MUIPEditorHandler.DrawPropertyCW(selectorTag, customSkin, "Selector Tag:", 90);
                        EditorGUILayout.HelpBox("Each selector should has its own unique tag.", MessageType.Info);
                    }

                    GUILayout.EndVertical();
                    MUIPEditorHandler.DrawHeader(customSkin, "UIM Header", 10);

                    if (tempUIM != null)
                    {
                        MUIPEditorHandler.DrawUIManagerConnectedHeader();
                        tempUIM.overrideColors = MUIPEditorHandler.DrawToggle(tempUIM.overrideColors, customSkin, "Override Colors");
                        tempUIM.overrideFonts = MUIPEditorHandler.DrawToggle(tempUIM.overrideFonts, customSkin, "Override Fonts");
                        tempUIM.overrideOptions = MUIPEditorHandler.DrawToggle(tempUIM.overrideOptions, customSkin, "Override Options");

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

                    else if (tempUIM == null) { MUIPEditorHandler.DrawUIManagerDisconnectedHeader(); }

                    break;
            }

            this.Repaint();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif