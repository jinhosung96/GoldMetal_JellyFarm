using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Michsky.UI.ModernUIPack
{
    public class TooltipManager : MonoBehaviour
    {
        // Resources
        public Canvas mainCanvas;
        public GameObject tooltipObject;
        public GameObject tooltipContent;
        public Camera targetCamera;

        // Settings
        [Range(0.01f, 0.5f)] public float tooltipSmoothness = 0.1f;
        [Range(5, 10)] public float dampSpeed = 10;
        public float preferredWidth = 375;
        public bool allowUpdating = false;
        public CameraSource cameraSource = CameraSource.Main;

        // Content Bounds
        [Range(-50, 50)] public int vBorderTop = -15;
        [Range(-50, 50)] public int vBorderBottom = 10;
        [Range(-50, 50)] public int hBorderLeft = 20;
        [Range(-50, 50)] public int hBorderRight = -15;

        // Border Bounds
        [SerializeField] private int xLeft = -400;
        [SerializeField] private int xRight = 400;
        [SerializeField] private int yTop = -325;
        [SerializeField] private int yBottom = 325;

        Vector2 uiPos;
        Vector3 cursorPos;
        Vector3 contentPos = new Vector3(0, 0, 0);
        Vector3 tooltipVelocity = Vector3.zero;

        RectTransform contentRect;
        RectTransform tooltipRect;
        
        [HideInInspector] public LayoutElement contentLE;

        public enum CameraSource { Main, Custom }

        void Awake()
        {
            RectTransform sourceRect = gameObject.GetComponent<RectTransform>();

            if (sourceRect == null)
            {
                Debug.LogError("<b>[Tooltip]</b> Rect Transform is missing from the object.", this);
                return;
            }

            sourceRect.anchorMin = new Vector2(0, 0);
            sourceRect.anchorMax = new Vector2(1, 1);
            sourceRect.offsetMin = new Vector2(0, 0);
            sourceRect.offsetMax = new Vector2(0, 0);

            tooltipContent.GetComponent<RectTransform>().pivot = new Vector2(0f, tooltipContent.GetComponent<RectTransform>().pivot.y);
            tooltipContent.GetComponent<RectTransform>().pivot = new Vector2(tooltipContent.GetComponent<RectTransform>().pivot.x, 0f);

            if (mainCanvas == null) { mainCanvas = gameObject.GetComponentInParent<Canvas>(); }
            if (cameraSource == CameraSource.Main) { targetCamera = Camera.main; }

            contentRect = tooltipContent.GetComponentInParent<RectTransform>();
            tooltipRect = tooltipObject.GetComponent<RectTransform>();

            contentPos = new Vector3(vBorderTop, hBorderLeft, 0);
            gameObject.transform.SetAsLastSibling();
        }

        void Update()
        {
            if (allowUpdating == false)
                return;

            CheckForPosition();
        }

        void CheckForPosition()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            cursorPos = Input.mousePosition;
#elif ENABLE_INPUT_SYSTEM
            cursorPos = Mouse.current.position.ReadValue();
#endif
            uiPos = tooltipRect.anchoredPosition;
            CheckForBounds();

            if (mainCanvas.renderMode == RenderMode.ScreenSpaceCamera || mainCanvas.renderMode == RenderMode.WorldSpace)
            {
                tooltipRect.position = targetCamera.ScreenToWorldPoint(cursorPos);
                tooltipRect.localPosition = new Vector3(tooltipRect.localPosition.x, tooltipRect.localPosition.y, 0);
                tooltipContent.transform.localPosition = Vector3.SmoothDamp(tooltipContent.transform.localPosition, contentPos, ref tooltipVelocity, tooltipSmoothness, dampSpeed * 1000, Time.unscaledDeltaTime);
            }

            else if (mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                tooltipRect.position = cursorPos;
                tooltipContent.transform.position = Vector3.SmoothDamp(tooltipContent.transform.position, cursorPos + contentPos, ref tooltipVelocity, tooltipSmoothness, dampSpeed * 1000, Time.unscaledDeltaTime);
            }
        }

        void CheckForBounds()
        {
            if (uiPos.x <= xLeft)
            {
                contentPos = new Vector3(hBorderLeft, contentPos.y, 0);
                contentRect.pivot = new Vector2(0f, contentRect.pivot.y);
            }

            else if (uiPos.x >= xRight)
            {
                contentPos = new Vector3(hBorderRight, contentPos.y, 0);
                contentRect.pivot = new Vector2(1f, contentRect.pivot.y);
            }

            if (uiPos.y <= yTop)
            {
                contentPos = new Vector3(contentPos.x, vBorderBottom, 0);
                contentRect.pivot = new Vector2(contentRect.pivot.x, 0f);
            }

            else if (uiPos.y >= yBottom)
            {
                contentPos = new Vector3(contentPos.x, vBorderTop, 0);
                contentRect.pivot = new Vector2(contentRect.pivot.x, 1f);
            }
        }
    }
}