using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    public class UIManagerToggle : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;

        [Header("Resources")]
        [SerializeField] private Image border;
        [SerializeField] private Image background;
        [SerializeField] private Image check;
        [SerializeField] private TextMeshProUGUI onLabel;
        [SerializeField] private TextMeshProUGUI offLabel;

        void Awake()
        {
            try
            {
                if (UIManagerAsset == null)
                    UIManagerAsset = Resources.Load<UIManager>("MUIP Manager");

                this.enabled = true;

                if (UIManagerAsset.enableDynamicUpdate == false)
                {
                    UpdateToggle();
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
                UpdateToggle();
        }

        void UpdateToggle()
        {
            try
            {
                border.color = UIManagerAsset.toggleBorderColor;
                background.color = UIManagerAsset.toggleBackgroundColor;
                check.color = UIManagerAsset.toggleCheckColor;
                onLabel.color = new Color(UIManagerAsset.toggleTextColor.r, UIManagerAsset.toggleTextColor.g, UIManagerAsset.toggleTextColor.b, onLabel.color.a);
                onLabel.font = UIManagerAsset.toggleFont;
                onLabel.fontSize = UIManagerAsset.toggleFontSize;
                offLabel.color = new Color(UIManagerAsset.toggleTextColor.r, UIManagerAsset.toggleTextColor.g, UIManagerAsset.toggleTextColor.b, offLabel.color.a);
                offLabel.font = UIManagerAsset.toggleFont;
                offLabel.fontSize = UIManagerAsset.toggleFontSize;
            }

            catch { }
        }
    }
}