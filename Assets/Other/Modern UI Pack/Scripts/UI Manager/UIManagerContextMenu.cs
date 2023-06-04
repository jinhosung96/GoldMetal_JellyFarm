using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    public class UIManagerContextMenu : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;

        [Header("Resources")]
        [SerializeField] private Image backgroundImage;

        void Awake()
        {
            try
            {
                if (UIManagerAsset == null)
                    UIManagerAsset = Resources.Load<UIManager>("MUIP Manager");

                this.enabled = true;

                if (UIManagerAsset.enableDynamicUpdate == false)
                {
                    UpdateContextMenu();
                    this.enabled = false;
                }
            }

            catch { Debug.Log("<b>[Modern UI Pack]</b> No UI Manager found, assign it manually.", this); }
        }

        void LateUpdate()
        {
            if (UIManagerAsset == null)
                return;

            if (UIManagerAsset.enableDynamicUpdate == true)
                UpdateContextMenu();
        }

        void UpdateContextMenu()
        {
            backgroundImage.color = UIManagerAsset.contextBackgroundColor;
        }
    }
}