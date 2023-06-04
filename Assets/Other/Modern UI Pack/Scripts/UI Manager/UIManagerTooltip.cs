using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    public class UIManagerTooltip : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;

        [Header("Resources")]
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI text;

        void Awake()
        {
            try
            {
                if (UIManagerAsset == null)
                    UIManagerAsset = Resources.Load<UIManager>("MUIP Manager");

                this.enabled = true;

                if (UIManagerAsset.enableDynamicUpdate == false)
                {
                    UpdateTooltip();
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
                UpdateTooltip();
        }

        void UpdateTooltip()
        {
            try
            {
                background.color = UIManagerAsset.tooltipBackgroundColor;
                text.color = UIManagerAsset.tooltipTextColor;
                text.font = UIManagerAsset.tooltipFont;
                text.fontSize = UIManagerAsset.tooltipFontSize;
            }

            catch { }
        }
    }
}