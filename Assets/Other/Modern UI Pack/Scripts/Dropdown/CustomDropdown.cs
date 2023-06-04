using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    public class CustomDropdown : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler
    {
        // Resources
        public Animator dropdownAnimator;
        public GameObject triggerObject;
        public TextMeshProUGUI selectedText;
        public Image selectedImage;
        public Transform itemParent;
        public GameObject itemObject;
        public GameObject scrollbar;
        public VerticalLayoutGroup itemList;
        public Transform listParent;
        public AudioSource soundSource;
        [HideInInspector] public Transform currentListParent;

        // Settings
        public bool enableIcon = true;
        public bool enableTrigger = true;
        public bool enableScrollbar = true;
        public bool setHighPriorty = true;
        public bool outOnPointerExit = false;
        public bool isListItem = false;
        public bool invokeAtStart = false;
        public bool initAtStart = true;
        public bool enableDropdownSounds = false;
        public bool useHoverSound = true;
        public bool useClickSound = true;
        [Range(1, 50)] public int itemPaddingTop = 8;
        [Range(1, 50)] public int itemPaddingBottom = 8;
        [Range(1, 50)] public int itemPaddingLeft = 8;
        [Range(1, 50)] public int itemPaddingRight = 25;
        [Range(1, 50)] public int itemSpacing = 8;
        public int selectedItemIndex = 0;

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
        public bool saveSelected = false;
        public string dropdownTag = "Dropdown";

        // Item list
        [SerializeField]
        public List<Item> dropdownItems = new List<Item>();
        [System.Serializable]
        public class DropdownEvent : UnityEvent<int> { }
        [Space(8)] public DropdownEvent dropdownEvent;

        // Audio
        public AudioClip hoverSound;
        public AudioClip clickSound;

        // Other variables
        [HideInInspector] public bool isOn;
        [HideInInspector] public int index = 0;
        [HideInInspector] public int siblingIndex = 0;
        [HideInInspector] public TextMeshProUGUI setItemText;
        [HideInInspector] public Image setItemImage;
        EventTrigger triggerEvent;
        Sprite imageHelper;
        string textHelper;

        public enum AnimationType { Modular, Stylish }

        [System.Serializable]
        public class Item
        {
            public string itemName = "Dropdown Item";
            public Sprite itemIcon;
            [HideInInspector] public int itemIndex;
            public UnityEvent OnItemSelection = new UnityEvent();
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

            index = 0;

            for (int i = 0; i < dropdownItems.Count; ++i)
            {
                GameObject go = Instantiate(itemObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                go.transform.SetParent(itemParent, false);

                setItemText = go.GetComponentInChildren<TextMeshProUGUI>();
                textHelper = dropdownItems[i].itemName;
                setItemText.text = textHelper;

                Transform goImage = go.gameObject.transform.Find("Icon");
                setItemImage = goImage.GetComponent<Image>();
                imageHelper = dropdownItems[i].itemIcon;
                setItemImage.sprite = imageHelper;

                dropdownItems[i].itemIndex = i;
                CustomDropdown.Item mainItem = dropdownItems[i];

                Button itemButton = go.GetComponent<Button>();
                itemButton.onClick.AddListener(Animate);
                itemButton.onClick.AddListener(delegate
                {
                    ChangeDropdownInfo(index = mainItem.itemIndex);
                    dropdownEvent.Invoke(index = mainItem.itemIndex);

                    if (saveSelected == true) { PlayerPrefs.SetInt("Dropdown" + dropdownTag, mainItem.itemIndex); }
                });

                itemButton.onClick.AddListener(dropdownItems[i].OnItemSelection.Invoke);
                if (invokeAtStart == true) { dropdownItems[i].OnItemSelection.Invoke(); }
            }

            if (selectedImage != null && enableIcon == false)
                selectedImage.gameObject.SetActive(false);

            try
            {
                selectedText.text = dropdownItems[selectedItemIndex].itemName;
                selectedImage.sprite = dropdownItems[selectedItemIndex].itemIcon;
                currentListParent = transform.parent;
            }

            catch
            {
                selectedText.text = dropdownTag;
                currentListParent = transform.parent;
                Debug.LogWarning("<b>[Dropdown]</b> There is no dropdown items in the list.", this);
                return;
            }

            if (saveSelected == true)
            {
                if (invokeAtStart == true) { dropdownItems[PlayerPrefs.GetInt("Dropdown" + dropdownTag)].OnItemSelection.Invoke(); }
                else { ChangeDropdownInfo(PlayerPrefs.GetInt("Dropdown" + dropdownTag)); }
            }
        }

        public void ChangeDropdownInfo(int itemIndex)
        {
            if (selectedImage != null && enableIcon == true) { selectedImage.sprite = dropdownItems[itemIndex].itemIcon; }
            if (selectedText != null) { selectedText.text = dropdownItems[itemIndex].itemName; }
            if (enableDropdownSounds == true && useClickSound == true) { soundSource.PlayOneShot(clickSound); }

            selectedItemIndex = itemIndex;
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

        public void CreateNewItem(string title, Sprite icon)
        {
            Item item = new Item();
            item.itemName = title;
            item.itemIcon = icon;
            dropdownItems.Add(item);
            SetupDropdown();
        }

        public void CreateNewItemFast(string title, Sprite icon)
        {
            Item item = new Item();
            item.itemName = title;
            item.itemIcon = icon;
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

        public void OnPointerClick(PointerEventData eventData)
        {
            if (enableDropdownSounds == true && useClickSound == true)
                soundSource.PlayOneShot(clickSound);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enableDropdownSounds == true && useHoverSound == true)
                soundSource.PlayOneShot(hoverSound);
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