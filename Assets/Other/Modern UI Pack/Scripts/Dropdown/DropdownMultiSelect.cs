using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    public class DropdownMultiSelect : MonoBehaviour, IPointerExitHandler
    {
        // Resources
        public GameObject triggerObject;
        public Transform itemParent;
        public GameObject itemObject;
        public GameObject scrollbar;
        private VerticalLayoutGroup itemList;
        private Transform currentListParent;
        public Transform listParent;
        private Animator dropdownAnimator;
        public TextMeshProUGUI setItemText;

        // Settings
        public bool initAtStart = true;
        public bool enableIcon = true;
        public bool enableTrigger = true;
        public bool enableScrollbar = true;
        public bool setHighPriorty = true;
        public bool outOnPointerExit = false;
        public bool isListItem = false;
        [Range(1, 50)] public int itemPaddingTop = 8;
        [Range(1, 50)] public int itemPaddingBottom = 8;
        [Range(1, 50)] public int itemPaddingLeft = 8;
        [Range(1, 50)] public int itemPaddingRight = 25;
        [Range(1, 50)] public int itemSpacing = 8;

        // Animation
        public AnimationType animationType;
        [Range(1, 25)] public float transitionSmoothness = 10;
        [Range(1, 25)] public float sizeSmoothness = 15;
        public float panelSize = 200;
        public RectTransform listRect;
        public CanvasGroup listCG;
        bool isInTransition = false;
        float closeOn;

        // Saving
        public bool invokeAtStart = false;
        public string toggleTag = "Multi Dropdown";

        // Items
        [SerializeField]
        public List<Item> dropdownItems = new List<Item>();

        // Other variables
        int currentIndex;
        Toggle currentToggle;
        string textHelper;
        bool isOn;
        public int siblingIndex = 0;
        EventTrigger triggerEvent;

        [System.Serializable]
        public class ToggleEvent : UnityEvent<bool> { }

        public enum AnimationType { Modular, Stylish }

        [System.Serializable]
        public class Item
        {
            public string itemName = "Dropdown Item";
            public bool isOn;
            [HideInInspector] public int itemIndex;
            [SerializeField] public ToggleEvent onValueChanged = new ToggleEvent();
        }

        void OnEnable()
        {
            if (animationType == AnimationType.Stylish) { return; }
            else if (animationType == AnimationType.Modular && dropdownAnimator != null) { Destroy(dropdownAnimator); }

            if (listCG == null) { listCG = gameObject.GetComponentInChildren<CanvasGroup>(); }
            listCG.alpha = 0;
            listCG.interactable = false;
            listCG.blocksRaycasts = false;

            if (listRect == null) { listRect = listCG.GetComponent<RectTransform>(); }
            closeOn = gameObject.GetComponent<RectTransform>().sizeDelta.y;
            listRect.sizeDelta = new Vector2(listRect.sizeDelta.x, closeOn);
        }

        void Awake()
        {
            try
            {
                if (initAtStart == true) { SetupDropdown(); }

                currentListParent = transform.parent;

                if (enableTrigger == true && triggerObject != null)
                {
                    // triggerButton = gameObject.GetComponent<Button>();
                    triggerEvent = triggerObject.AddComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerClick;
                    entry.callback.AddListener((eventData) => { Animate(); });
                    triggerEvent.GetComponent<EventTrigger>().triggers.Add(entry);
                }
            }

            catch { Debug.LogError("<b>[Dropdown]</b> Cannot initalize the object due to missing resources.", this); }
        }

        void Update()
        {
            if (isInTransition == false)
                return;

            ProcessModularAnimation();
        }

        void ProcessModularAnimation()
        {
            if (isOn == true)
            {
                listCG.alpha += Time.unscaledDeltaTime * transitionSmoothness;
                listRect.sizeDelta = Vector2.Lerp(listRect.sizeDelta, new Vector2(listRect.sizeDelta.x, panelSize), Time.unscaledDeltaTime * sizeSmoothness);

                if (listRect.sizeDelta.y >= panelSize - 0.1f && listCG.alpha >= 1) { isInTransition = false; }
            }

            else
            {
                listCG.alpha -= Time.unscaledDeltaTime * transitionSmoothness;
                listRect.sizeDelta = Vector2.Lerp(listRect.sizeDelta, new Vector2(listRect.sizeDelta.x, closeOn), Time.unscaledDeltaTime * sizeSmoothness);

                if (listRect.sizeDelta.y <= closeOn + 0.1f && listCG.alpha <= 0) { isInTransition = false; this.enabled = false; }
            }
        }

        public void SetupDropdown()
        {
            if (dropdownAnimator == null) { dropdownAnimator = gameObject.GetComponent<Animator>(); }
            if (enableScrollbar == false && scrollbar != null) { Destroy(scrollbar); }
            if (setHighPriorty == true) { transform.SetAsLastSibling(); }
            if (itemList == null) { itemList = itemParent.GetComponent<VerticalLayoutGroup>(); }

            UpdateItemLayout();

            foreach (Transform child in itemParent)
                Destroy(child.gameObject);

            for (int i = 0; i < dropdownItems.Count; ++i)
            {
                GameObject go = Instantiate(itemObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                go.transform.SetParent(itemParent, false);

                setItemText = go.GetComponentInChildren<TextMeshProUGUI>();
                textHelper = dropdownItems[i].itemName;
                setItemText.text = textHelper;

                dropdownItems[i].itemIndex = i;
                DropdownMultiSelect.Item mainItem = dropdownItems[i];

                Toggle itemToggle = go.GetComponent<Toggle>();
                itemToggle.onValueChanged.AddListener(delegate { UpdateToggleData(mainItem.itemIndex); });
                itemToggle.onValueChanged.AddListener(UpdateToggle);
                itemToggle.onValueChanged.AddListener(dropdownItems[i].onValueChanged.Invoke);

                if (dropdownItems[i].isOn == true) { itemToggle.isOn = true; }
                else { itemToggle.isOn = false; }

                if (invokeAtStart == true)
                {
                    if (dropdownItems[i].isOn == true) { dropdownItems[i].onValueChanged.Invoke(true); }
                    else { dropdownItems[i].onValueChanged.Invoke(false); }
                }
            }

            currentListParent = transform.parent;
        }

        void UpdateToggle(bool value)
        {
            if (value == true) { currentToggle.isOn = true; dropdownItems[currentIndex].isOn = true; }
            else { currentToggle.isOn = false; dropdownItems[currentIndex].isOn = false; }
        }

        void UpdateToggleData(int itemIndex)
        {
            currentIndex = itemIndex;
            currentToggle = itemParent.GetChild(currentIndex).GetComponent<Toggle>();
        }

        public void Animate()
        {
            if (isOn == false && animationType == AnimationType.Modular)
            {
                isOn = true;
                isInTransition = true;
                this.enabled = true;
                listCG.blocksRaycasts = true;
                listCG.interactable = true;

                if (isListItem == true)
                {
                    siblingIndex = transform.GetSiblingIndex();
                    gameObject.transform.SetParent(listParent, true);
                }
            }

            else if (isOn == true && animationType == AnimationType.Modular)
            {
                isOn = false;
                isInTransition = true;
                this.enabled = true;
                listCG.blocksRaycasts = false;
                listCG.interactable = false;

                if (isListItem == true)
                {
                    gameObject.transform.SetParent(currentListParent, true);
                    gameObject.transform.SetSiblingIndex(siblingIndex);
                }
            }

            else if (isOn == false && animationType == AnimationType.Stylish)
            {
                dropdownAnimator.Play("Stylish In");
                isOn = true;

                if (isListItem == true)
                {
                    siblingIndex = transform.GetSiblingIndex();
                    gameObject.transform.SetParent(listParent, true);
                }
            }

            else if (isOn == true && animationType == AnimationType.Stylish)
            {
                dropdownAnimator.Play("Stylish Out");
                isOn = false;

                if (isListItem == true)
                {
                    gameObject.transform.SetParent(currentListParent, true);
                    gameObject.transform.SetSiblingIndex(siblingIndex);
                }
            }

            if (enableTrigger == true && isOn == false) { triggerObject.SetActive(false); }
            else if (enableTrigger == true && isOn == true) { triggerObject.SetActive(true); }

            if (enableTrigger == true && outOnPointerExit == true) { triggerObject.SetActive(false); }
            if (setHighPriorty == true) { transform.SetAsLastSibling(); }
        }

        public void CreateNewItem(string title, bool value)
        {
            Item item = new Item();
            item.itemName = title;
            item.isOn = value;
            dropdownItems.Add(item);
            SetupDropdown();
        }

        public void CreateNewItemFast(string title, bool value)
        {
            Item item = new Item();
            item.itemName = title;
            item.isOn = value;
            dropdownItems.Add(item);
        }

        public void RemoveItem(string itemTitle)
        {
            var item = dropdownItems.Find(x => x.itemName == itemTitle);
            dropdownItems.Remove(item);
            SetupDropdown();
        }

        public void AddNewItem()
        {
            Item item = new Item();
            dropdownItems.Add(item);
        }

        public void UpdateItemLayout()
        {
            if (itemList != null)
            {
                itemList.spacing = itemSpacing;
                itemList.padding.top = itemPaddingTop;
                itemList.padding.bottom = itemPaddingBottom;
                itemList.padding.left = itemPaddingLeft;
                itemList.padding.right = itemPaddingRight;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (outOnPointerExit == true && isOn == true)
            {
                Animate();
                isOn = false;

                if (isListItem == true) { gameObject.transform.SetParent(currentListParent, true); }
            }
        }
    }
}