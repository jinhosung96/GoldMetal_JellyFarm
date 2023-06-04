using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Michsky.UI.ModernUIPack
{
    [RequireComponent(typeof(Animator))]
    public class ContextMenuManager : MonoBehaviour
    {
        // Resources
        public Canvas mainCanvas;
        public Camera targetCamera;
        public GameObject contextContent;
        public Animator contextAnimator;
        public GameObject contextButton;
        public GameObject contextSeparator;
        public GameObject contextSubMenu;

        // Settings
        public bool autoSubMenuPosition = true;
        public SubMenuBehaviour subMenuBehaviour;
        public CameraSource cameraSource = CameraSource.Main;

        // Bounds
        [Range(-50, 50)] public int vBorderTop = -10;
        [Range(-50, 50)] public int vBorderBottom = 10;
        [Range(-50, 50)] public int hBorderLeft = 15;
        [Range(-50, 50)] public int hBorderRight = -15;

        Vector2 uiPos;
        Vector3 cursorPos;
        Vector3 contentPos = new Vector3(0, 0, 0);
        Vector3 contextVelocity = Vector3.zero;

        RectTransform contextRect;
        RectTransform contentRect;

        [HideInInspector] public bool isOn;
        [HideInInspector] public bool bottomLeft;
        [HideInInspector] public bool bottomRight;
        [HideInInspector] public bool topLeft;
        [HideInInspector] public bool topRight;

        public enum CameraSource { Main, Custom }

        public enum SubMenuBehaviour { Hover, Click }

        void Awake()
        {
            if (mainCanvas == null) { mainCanvas = gameObject.GetComponentInParent<Canvas>(); }
            if (contextAnimator == null) { contextAnimator = gameObject.GetComponent<Animator>(); }
            if (cameraSource == CameraSource.Main) { targetCamera = Camera.main; }

            contextRect = gameObject.GetComponent<RectTransform>();
            contentRect = contextContent.GetComponent<RectTransform>();
            contentPos = new Vector3(vBorderTop, hBorderLeft, 0);
            gameObject.transform.SetAsLastSibling();
        }

        public void CheckForBounds()
        {
            if (uiPos.x <= -100) { contentPos = new Vector3(hBorderLeft, contentPos.y, 0); contentRect.pivot = new Vector2(0f, contentRect.pivot.y); bottomLeft = true; }
            else { bottomLeft = false; }

            if (uiPos.x >= 100) { contentPos = new Vector3(hBorderRight, contentPos.y, 0); contentRect.pivot = new Vector2(1f, contentRect.pivot.y); bottomRight = true; }
            else { bottomRight = false; }

            if (uiPos.y <= -75) { contentPos = new Vector3(contentPos.x, vBorderBottom, 0); contentRect.pivot = new Vector2(contentRect.pivot.x, 0f); topLeft = true; }
            else { topLeft = false; }

            if (uiPos.y >= 75) { contentPos = new Vector3(contentPos.x, vBorderTop, 0); contentRect.pivot = new Vector2(contentRect.pivot.x, 1f); topRight = true; }
            else { topRight = false; }
        }

        public void SetContextMenuPosition()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            cursorPos = Input.mousePosition;
#elif ENABLE_INPUT_SYSTEM
            cursorPos = Mouse.current.position.ReadValue();
#endif
            uiPos = contextRect.anchoredPosition;
            CheckForBounds();

            if (mainCanvas.renderMode == RenderMode.ScreenSpaceCamera || mainCanvas.renderMode == RenderMode.WorldSpace)
            {
                contextRect.position = targetCamera.ScreenToWorldPoint(cursorPos);
                contextRect.localPosition = new Vector3(contextRect.localPosition.x, contextRect.localPosition.y, 0);
                contextContent.transform.localPosition = Vector3.SmoothDamp(contextContent.transform.localPosition, contentPos, ref contextVelocity, 0);
            }

            else if (mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                contextRect.position = cursorPos;
                contextContent.transform.position = new Vector3(cursorPos.x + contentPos.x, cursorPos.y + contentPos.y, 0);
            }
        }

        public void Open() { contextAnimator.Play("Menu Out"); isOn = false; }
        public void Close() { contextAnimator.Play("Menu Out"); isOn = false; }
      
        // Obsolote
        public void OpenContextMenu() { contextAnimator.Play("Menu In"); isOn = true; }
        public void CloseOnClick() { contextAnimator.Play("Menu Out"); isOn = false; }

    }
}