using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Michsky.UI.ModernUIPack
{
    [AddComponentMenu("Modern UI Pack/Context Menu/Context Menu Content")]
    public class ContextMenuContent : MonoBehaviour, IPointerClickHandler
    {
        // Resources
        public ContextMenuManager contextManager;
        public Transform itemParent;
      
        // Settings
        public bool useIn3D = false;

        // Items
        public List<ContextItem> contexItems = new List<ContextItem>();

        Animator contextAnimator;
        GameObject selectedItem;
        Image setItemImage;
        TextMeshProUGUI setItemText;
        Sprite imageHelper;
        string textHelper;

        [System.Serializable]
        public class ContextItem
        {
            [Header("Information")]
            [Space(-5)]
            public string itemText = "Item Text";
            public Sprite itemIcon;
            public ContextItemType contextItemType;

            [Header("Sub Menu")]
            public List<SubMenuItem> subMenuItems = new List<SubMenuItem>();

            [Header("Events")]
            public UnityEvent onClick;
        }

        [System.Serializable]
        public class SubMenuItem
        {
            public string itemText = "Item Text";
            public Sprite itemIcon;
            public ContextItemType contextItemType;
            public UnityEvent onClick;
        }

        public enum ContextItemType { Button, Separator }

        void Awake()
        {
            if (contextManager == null)
            {
                try
                {
                    contextManager = (ContextMenuManager)GameObject.FindObjectsOfType(typeof(ContextMenuManager))[0];
                    itemParent = contextManager.transform.Find("Content/Item List").transform;
                }

                catch { Debug.LogError("<b>[Context Menu]</b> Context Manager is missing.", this); return; }
            }

            contextAnimator = contextManager.contextAnimator;

            foreach (Transform child in itemParent)
                Destroy(child.gameObject);
        }

        void ProcessClick()
        {
            foreach (Transform child in itemParent)
                Destroy(child.gameObject);

            for (int i = 0; i < contexItems.Count; ++i)
            {
                bool nulLVariable = false;

                if (contexItems[i].contextItemType == ContextItemType.Button && contextManager.contextButton != null)
                    selectedItem = contextManager.contextButton;
                else if (contexItems[i].contextItemType == ContextItemType.Separator && contextManager.contextSeparator != null)
                    selectedItem = contextManager.contextSeparator;
                else
                {
                    Debug.LogError("<b>[Context Menu]</b> At least one of the item presets is missing. " +
                        "You can assign a new variable in Resources (Context Menu) tab. All default presets can be found in " +
                        "<b>Modern UI Pack > Prefabs > Context Menu</b> folder.", this);
                    nulLVariable = true;
                }

                if (nulLVariable == false)
                {
                    if (contexItems[i].subMenuItems.Count == 0)
                    {
                        GameObject go = Instantiate(selectedItem, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                        go.transform.SetParent(itemParent, false);

                        if (contexItems[i].contextItemType == ContextItemType.Button)
                        {
                            setItemText = go.GetComponentInChildren<TextMeshProUGUI>();
                            textHelper = contexItems[i].itemText;
                            setItemText.text = textHelper;

                            Transform goImage = go.gameObject.transform.Find("Icon");
                            setItemImage = goImage.GetComponent<Image>();
                            imageHelper = contexItems[i].itemIcon;
                            setItemImage.sprite = imageHelper;

                            if (imageHelper == null)
                                setItemImage.color = new Color(0, 0, 0, 0);

                            Button itemButton = go.GetComponent<Button>();
                            itemButton.onClick.AddListener(contexItems[i].onClick.Invoke);
                            itemButton.onClick.AddListener(CloseOnClick);
                        }
                    }

                    else if (contextManager.contextSubMenu != null && contexItems[i].subMenuItems.Count != 0)
                    {
                        GameObject go = Instantiate(contextManager.contextSubMenu, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                        go.transform.SetParent(itemParent, false);

                        ContextMenuSubMenu subMenu = go.GetComponent<ContextMenuSubMenu>();
                        subMenu.cmManager = contextManager;
                        subMenu.cmContent = this;
                        subMenu.subMenuIndex = i;

                        setItemText = go.GetComponentInChildren<TextMeshProUGUI>();
                        textHelper = contexItems[i].itemText;
                        setItemText.text = textHelper;

                        Transform goImage;
                        goImage = go.gameObject.transform.Find("Icon");
                        setItemImage = goImage.GetComponent<Image>();
                        imageHelper = contexItems[i].itemIcon;
                        setItemImage.sprite = imageHelper;
                    }

                    StopCoroutine("ExecuteAfterTime");
                    StartCoroutine("ExecuteAfterTime", 0.01f);
                }
            }

            contextManager.SetContextMenuPosition();
            contextAnimator.Play("Menu In");
            contextManager.isOn = true;
            contextManager.SetContextMenuPosition();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (contextManager.isOn == true) { contextAnimator.Play("Menu Out"); contextManager.isOn = false; }
            else if (eventData.button == PointerEventData.InputButton.Right && contextManager.isOn == false) { ProcessClick(); }
        }

        IEnumerator ExecuteAfterTime(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            itemParent.gameObject.SetActive(false);
            itemParent.gameObject.SetActive(true);
        }

        public void OnMouseOver() 
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            if (useIn3D == true && Input.GetMouseButtonDown(1))
#elif ENABLE_INPUT_SYSTEM
            if (useIn3D == true && Mouse.current.rightButton.wasPressedThisFrame)
#endif
            {
                ProcessClick();
            }
        }

        public void CloseOnClick()
        {
            contextAnimator.Play("Menu Out");
            contextManager.isOn = false;
        }

        public void AddNewItem()
        {
            ContextItem item = new ContextItem();
            contexItems.Add(item);
        }
    }
}