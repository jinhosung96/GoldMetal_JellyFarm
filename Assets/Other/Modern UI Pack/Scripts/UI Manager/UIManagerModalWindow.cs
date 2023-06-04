using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    public class UIManagerModalWindow : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;

        [Header("Resources")]
        [SerializeField] private Image background;
        [SerializeField] private Image contentBackground;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;

        void Awake()
        {
            try
            {
                if (UIManagerAsset == null)
                    UIManagerAsset = Resources.Load<UIManager>("MUIP Manager");

                this.enabled = true;

                if (UIManagerAsset.enableDynamicUpdate == false)
                {
                    UpdateModalWindow();
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
                UpdateModalWindow();
        }

        void UpdateModalWindow()
        {
            try
            {
                background.color = UIManagerAsset.modalWindowBackgroundColor;
                contentBackground.color = UIManagerAsset.modalWindowContentPanelColor;
                icon.color = UIManagerAsset.modalWindowIconColor;
                title.color = UIManagerAsset.modalWindowTitleColor;
                description.color = UIManagerAsset.modalWindowDescriptionColor;
                title.font = UIManagerAsset.modalWindowTitleFont;
                description.font = UIManagerAsset.modalWindowContentFont;
            }

            catch { }
        }
    }
}