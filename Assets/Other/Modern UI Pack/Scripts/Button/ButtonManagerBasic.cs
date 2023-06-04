using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Button))]
    public class ButtonManagerBasic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        // Content
        public string buttonText = "Button";
        public UnityEvent clickEvent;
        public UnityEvent hoverEvent;
        public AudioClip hoverSound;
        public AudioClip clickSound;
        public Button buttonVar;

        // Resources
        public TextMeshProUGUI normalText;
        public AudioSource soundSource;
        public GameObject rippleParent;

        // Settings
        public bool useCustomContent = false;
        public bool enableButtonSounds = false;
        public bool useHoverSound = true;
        public bool useClickSound = true;
        public bool useRipple = true;

        // Ripple
        public RippleUpdateMode rippleUpdateMode = RippleUpdateMode.UnscaledTime;
        public Sprite rippleShape;
        [Range(0.1f, 5)] public float speed = 1f;
        [Range(0.5f, 25)] public float maxSize = 4f;
        public Color startColor = new Color(1f, 1f, 1f, 1f);
        public Color transitionColor = new Color(1f, 1f, 1f, 0f);
        public bool renderOnTop = false;
        public bool centered = false;
        bool isPointerOn;

        public bool isPreset;

        public enum RippleUpdateMode { Normal, UnscaledTime }

        void Awake()
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                if (useCustomContent == false) { UpdateUI(); }
                return;
            }
#endif

            if (buttonVar == null) { buttonVar = gameObject.GetComponent<Button>(); }
            if (enableButtonSounds == true && useClickSound == true) { buttonVar.onClick.AddListener(() => soundSource.PlayOneShot(clickSound)); }
            if (useCustomContent == false) { UpdateUI(); }

            buttonVar.onClick.AddListener(() => clickEvent.Invoke());

            if (useRipple == true && rippleParent != null) { rippleParent.SetActive(false); }
            else if (useRipple == false && rippleParent != null) { Destroy(rippleParent); }
        }

        public void UpdateUI()
        {
            if (normalText != null) { normalText.text = buttonText; }
        }

        public void CreateRipple(Vector2 pos)
        {
            if (rippleParent != null)
            {
                GameObject rippleObj = new GameObject();
                rippleObj.AddComponent<Image>();
                rippleObj.GetComponent<Image>().sprite = rippleShape;
                rippleObj.name = "Ripple";
                rippleParent.SetActive(true);
                rippleObj.transform.SetParent(rippleParent.transform);

                if (renderOnTop == true) { rippleParent.transform.SetAsLastSibling(); }
                else { rippleParent.transform.SetAsFirstSibling(); }

                if (centered == true) { rippleObj.transform.localPosition = new Vector2(0f, 0f); }
                else { rippleObj.transform.position = pos; }

                rippleObj.AddComponent<Ripple>();
                Ripple tempRipple = rippleObj.GetComponent<Ripple>();
                tempRipple.speed = speed;
                tempRipple.maxSize = maxSize;
                tempRipple.startColor = startColor;
                tempRipple.transitionColor = transitionColor;

                if (rippleUpdateMode == RippleUpdateMode.Normal) { tempRipple.unscaledTime = false; }
                else { tempRipple.unscaledTime = true; }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (useRipple == true && isPointerOn == true)
#if ENABLE_LEGACY_INPUT_MANAGER
                CreateRipple(Input.mousePosition);
#elif ENABLE_INPUT_SYSTEM
                CreateRipple(Mouse.current.position.ReadValue());
#endif
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enableButtonSounds == true && useHoverSound == true && buttonVar.interactable == true)
                soundSource.PlayOneShot(hoverSound);

            hoverEvent.Invoke();
            isPointerOn = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerOn = false;
        }
    }
}