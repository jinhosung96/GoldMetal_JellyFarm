using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    public class UIManagerSlider : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;
        public bool hasLabel;
        public bool hasPopupLabel;
        public bool overrideColors = false;
        public bool overrideFonts = false;

        [Header("Resources")]
        [SerializeField] private Image background;
        [SerializeField] private Image bar;
        [SerializeField] private Image handle;
        [HideInInspector] public TextMeshProUGUI label;
        [HideInInspector] public TextMeshProUGUI popupLabel;

        void Awake()
        {
            try
            {
                if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("MUIP Manager"); }

                this.enabled = true;

                if (UIManagerAsset.enableDynamicUpdate == false)
                {
                    UpdateSlider();
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
                UpdateSlider();
        }

        void UpdateSlider()
        {
            try
            {
                if (UIManagerAsset.sliderThemeType == UIManager.SliderThemeType.Basic)
                {
                    if (overrideColors == false)
                    {
                        background.color = UIManagerAsset.sliderBackgroundColor;
                        bar.color = UIManagerAsset.sliderColor;
                        handle.color = UIManagerAsset.sliderColor;
                    }

                    if (hasLabel == true)
                    {
                        if (overrideColors == false)
                            label.color = new Color(UIManagerAsset.sliderColor.r, UIManagerAsset.sliderColor.g, UIManagerAsset.sliderColor.b, label.color.a);

                        if (overrideFonts == false)
                        {
                            label.font = UIManagerAsset.sliderLabelFont;
                            label.fontSize = UIManagerAsset.sliderLabelFontSize;
                        }
                    }

                    if (hasPopupLabel == true)
                    {
                        if (overrideColors == false)
                            popupLabel.color = new Color(UIManagerAsset.sliderPopupLabelColor.r, UIManagerAsset.sliderPopupLabelColor.g, UIManagerAsset.sliderPopupLabelColor.b, popupLabel.color.a);
                       
                        if (overrideFonts == false)
                            popupLabel.font = UIManagerAsset.sliderLabelFont;
                    }
                }

                else if (UIManagerAsset.sliderThemeType == UIManager.SliderThemeType.Custom)
                {
                    if (overrideColors == false)
                    {
                        background.color = UIManagerAsset.sliderBackgroundColor;
                        bar.color = UIManagerAsset.sliderColor;
                        handle.color = UIManagerAsset.sliderHandleColor;
                    }

                    if (hasLabel == true)
                    {
                        if (overrideColors == false)
                            label.color = new Color(UIManagerAsset.sliderLabelColor.r, UIManagerAsset.sliderLabelColor.g, UIManagerAsset.sliderLabelColor.b, label.color.a);

                        if (overrideFonts == false)
                        {
                            label.font = UIManagerAsset.sliderLabelFont;
                            label.font = UIManagerAsset.sliderLabelFont;
                        }
                    }

                    if (hasPopupLabel == true)
                    {
                        if (overrideColors == false)
                            popupLabel.color = new Color(UIManagerAsset.sliderPopupLabelColor.r, UIManagerAsset.sliderPopupLabelColor.g, UIManagerAsset.sliderPopupLabelColor.b, popupLabel.color.a);
                        
                        if (overrideFonts == false)
                            popupLabel.font = UIManagerAsset.sliderLabelFont;
                    }
                }
            }

            catch { }
        }
    }
}