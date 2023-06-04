using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    public class ContextMenuSubMenu : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public ContextMenuManager cmManager;
        public ContextMenuContent cmContent;
        public Animator subMenuAnimator;
        public Transform itemParent;
        [HideInInspector] public int subMenuIndex;

        GameObject selectedItem;
        Image setItemImage;
        TextMeshProUGUI setItemText;
        Sprite imageHelper;
        string textHelper;
        RectTransform listParent;

        void OnEnable()
        {
            if (itemParent == null) { Debug.Log("<b>[Context Menu]</b> Item Parent is missing.", this); return; }

            listParent = itemParent.parent.gameObject.GetComponent<RectTransform>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (cmManager.subMenuBehaviour == ContextMenuManager.SubMenuBehaviour.Click)
                subMenuAnimator.Play("Menu In");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            foreach (Transform child in itemParent)
                Destroy(child.gameObject);

            for (int i = 0; i < cmContent.contexItems[subMenuIndex].subMenuItems.Count; ++i)
            {
                bool nulLVariable = false;

                if (cmContent.contexItems[subMenuIndex].subMenuItems[i].contextItemType == ContextMenuContent.ContextItemType.Button && cmManager.contextButton != null)
                    selectedItem = cmManager.contextButton;
                else if (cmContent.contexItems[subMenuIndex].subMenuItems[i].contextItemType == ContextMenuContent.ContextItemType.Separator && cmManager.contextSeparator != null)
                    selectedItem = cmManager.contextSeparator;
                else
                {
                    Debug.LogError("<b>[Context Menu]</b> At least one of the item presets is missing. " +
                        "You can assign a new variable in Resources (Context Menu) tab. All default presets can be found in " +
                        "<b>Modern UI Pack > Prefabs > Context Menu</b> folder.", this);
                    nulLVariable = true;
                }

                if (nulLVariable == false)
                {
                    GameObject go = Instantiate(selectedItem, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                    go.transform.SetParent(itemParent, false);

                    if (cmContent.contexItems[subMenuIndex].subMenuItems[i].contextItemType == ContextMenuContent.ContextItemType.Button)
                    {
                        setItemText = go.GetComponentInChildren<TextMeshProUGUI>();
                        textHelper = cmContent.contexItems[subMenuIndex].subMenuItems[i].itemText;
                        setItemText.text = textHelper;

                        Transform goImage = go.gameObject.transform.Find("Icon");
                        setItemImage = goImage.GetComponent<Image>();
                        imageHelper = cmContent.contexItems[subMenuIndex].subMenuItems[i].itemIcon;
                        setItemImage.sprite = imageHelper;

                        if (imageHelper == null)
                            setItemImage.color = new Color(0, 0, 0, 0);

                        Button itemButton = go.GetComponent<Button>();
                        itemButton.onClick.AddListener(cmContent.contexItems[subMenuIndex].subMenuItems[i].onClick.Invoke);
                        itemButton.onClick.AddListener(CloseOnClick);
                        StartCoroutine(ExecuteAfterTime(0.01f));
                    }
                }
            }

            if (cmManager.autoSubMenuPosition == true)
            {
                if (cmManager.bottomLeft == true) { listParent.pivot = new Vector2(0f, listParent.pivot.y); }
                if (cmManager.bottomRight == true) { listParent.pivot = new Vector2(1f, listParent.pivot.y); }
                if (cmManager.topLeft == true) { listParent.pivot = new Vector2(listParent.pivot.x, 0f); }
                if (cmManager.topRight == true) { listParent.pivot = new Vector2(listParent.pivot.x, 1f); }
            }

            if (cmManager.subMenuBehaviour == ContextMenuManager.SubMenuBehaviour.Hover)
                subMenuAnimator.Play("Menu In");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!subMenuAnimator.GetCurrentAnimatorStateInfo(0).IsName("Start"))
                subMenuAnimator.Play("Menu Out");
        }

        IEnumerator ExecuteAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            itemParent.gameObject.SetActive(false);
            itemParent.gameObject.SetActive(true);
            StopCoroutine(ExecuteAfterTime(0.01f));
            StopCoroutine("ExecuteAfterTime");
        }

        public void CloseOnClick()
        {
            cmManager.contextAnimator.Play("Menu Out");
            cmManager.isOn = false;
        }
    }
}