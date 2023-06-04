using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    public class UIManagerButton : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;
        public ButtonType buttonType;
        public bool overrideColors = false;
        public bool overrideFonts = false;

        // Basic Resources
        [HideInInspector] public Image basicFilled;
        [HideInInspector] public TextMeshProUGUI basicText;

        // Basic Only Icon Resources
        [HideInInspector] public Image basicOnlyIconFilled;
        [HideInInspector] public Image basicOnlyIconIcon;

        // Basic With Icon Resources
        [HideInInspector] public Image basicWithIconFilled;
        [HideInInspector] public Image basicWithIconIcon;
        [HideInInspector] public TextMeshProUGUI basicWithIconText;

        // Basic Outline Resources
        [HideInInspector] public Image basicOutlineBorder;
        [HideInInspector] public Image basicOutlineFilled;
        [HideInInspector] public TextMeshProUGUI basicOutlineText;
        [HideInInspector] public TextMeshProUGUI basicOutlineTextHighligted;

        // Basic Outline Only Icon Resources
        [HideInInspector] public Image basicOutlineOOBorder;
        [HideInInspector] public Image basicOutlineOOFilled;
        [HideInInspector] public Image basicOutlineOOIcon;
        [HideInInspector] public Image basicOutlineOOIconHighlighted;

        // Basic Outline With Icon Resources
        [HideInInspector] public Image basicOutlineWOBorder;
        [HideInInspector] public Image basicOutlineWOFilled;
        [HideInInspector] public Image basicOutlineWOIcon;
        [HideInInspector] public Image basicOutlineWOIconHighlighted;
        [HideInInspector] public TextMeshProUGUI basicOutlineWOText;
        [HideInInspector] public TextMeshProUGUI basicOutlineWOTextHighligted;

        // Radial Only Icon Resources
        [HideInInspector] public Image radialOOBackground;
        [HideInInspector] public Image radialOOIcon;

        // Radial Outline Only Icon Resources
        [HideInInspector] public Image radialOutlineOOBorder;
        [HideInInspector] public Image radialOutlineOOFilled;
        [HideInInspector] public Image radialOutlineOOIcon;
        [HideInInspector] public Image radialOutlineOOIconHighlighted;

        // Rounded Resources
        [HideInInspector] public Image roundedBackground;
        [HideInInspector] public TextMeshProUGUI roundedText;

        // Rounded Outline Resources
        [HideInInspector] public Image roundedOutlineBorder;
        [HideInInspector] public Image roundedOutlineFilled;
        [HideInInspector] public TextMeshProUGUI roundedOutlineText;
        [HideInInspector] public TextMeshProUGUI roundedOutlineTextHighligted;

        public enum ButtonType
        {
            Basic,
            BasicOnlyIcon,
            BasicWithIcon,
            BasicOutline,
            BasicOutlineOnlyIcon,
            BasicOutlineWithIcon,
            RadialOnlyIcon,
            RadialOutlineOnlyIcon,
            Rounded,
            RoundedOutline,
        }

        void Awake()
        {
            try
            {
                if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("MUIP Manager"); }

                this.enabled = true;

                if (UIManagerAsset.enableDynamicUpdate == false)
                {
                    UpdateButton();
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
                UpdateButton();
        }

        void UpdateButton()
        {
            try
            {
                if (UIManagerAsset.buttonThemeType == UIManager.ButtonThemeType.Basic)
                {
                    if (buttonType == ButtonType.Basic)
                    {
                        if (overrideColors == false)
                        {
                            basicFilled.color = UIManagerAsset.buttonBorderColor;
                            basicText.color = UIManagerAsset.buttonFilledColor;
                        }

                        if (overrideFonts == false)
                        {
                            basicText.font = UIManagerAsset.buttonFont;
                            basicText.fontSize = UIManagerAsset.buttonFontSize;
                        }
                    }

                    else if (buttonType == ButtonType.BasicOnlyIcon && overrideColors == false)
                    {
                        basicOnlyIconFilled.color = UIManagerAsset.buttonBorderColor;
                        basicOnlyIconIcon.color = UIManagerAsset.buttonFilledColor;
                    }

                    else if (buttonType == ButtonType.BasicWithIcon)
                    {
                        if (overrideColors == false)
                        {
                            basicWithIconFilled.color = UIManagerAsset.buttonBorderColor;
                            basicWithIconIcon.color = UIManagerAsset.buttonFilledColor;
                            basicWithIconText.color = UIManagerAsset.buttonFilledColor;
                        }

                        if (overrideFonts == false)
                        {
                            basicWithIconText.font = UIManagerAsset.buttonFont;
                            basicWithIconText.fontSize = UIManagerAsset.buttonFontSize;
                        }
                    }

                    else if (buttonType == ButtonType.BasicOutline)
                    {
                        if (overrideColors == false)
                        {
                            basicOutlineBorder.color = UIManagerAsset.buttonBorderColor;
                            basicOutlineFilled.color = UIManagerAsset.buttonBorderColor;
                            basicOutlineText.color = UIManagerAsset.buttonBorderColor;
                            basicOutlineTextHighligted.color = UIManagerAsset.buttonFilledColor;
                        }

                        if (overrideFonts == false)
                        {
                            basicOutlineText.font = UIManagerAsset.buttonFont;
                            basicOutlineTextHighligted.font = UIManagerAsset.buttonFont;
                            basicOutlineText.fontSize = UIManagerAsset.buttonFontSize;
                            basicOutlineTextHighligted.fontSize = UIManagerAsset.buttonFontSize;
                        }
                    }

                    else if (buttonType == ButtonType.BasicOutlineOnlyIcon && overrideColors == false)
                    {
                        basicOutlineOOBorder.color = UIManagerAsset.buttonBorderColor;
                        basicOutlineOOFilled.color = UIManagerAsset.buttonBorderColor;
                        basicOutlineOOIcon.color = UIManagerAsset.buttonBorderColor;
                        basicOutlineOOIconHighlighted.color = UIManagerAsset.buttonFilledColor;
                    }

                    else if (buttonType == ButtonType.BasicOutlineWithIcon)
                    {
                        if (overrideColors == false)
                        {
                            basicOutlineWOBorder.color = UIManagerAsset.buttonBorderColor;
                            basicOutlineWOFilled.color = UIManagerAsset.buttonBorderColor;
                            basicOutlineWOIcon.color = UIManagerAsset.buttonBorderColor;
                            basicOutlineWOIconHighlighted.color = UIManagerAsset.buttonFilledColor;
                            basicOutlineWOText.color = UIManagerAsset.buttonBorderColor;
                            basicOutlineWOTextHighligted.color = UIManagerAsset.buttonFilledColor;
                        }

                        if (overrideFonts == false)
                        {
                            basicOutlineWOText.font = UIManagerAsset.buttonFont;
                            basicOutlineWOTextHighligted.font = UIManagerAsset.buttonFont;
                            basicOutlineWOText.fontSize = UIManagerAsset.buttonFontSize;
                            basicOutlineWOTextHighligted.fontSize = UIManagerAsset.buttonFontSize;
                        }

                    }

                    else if (buttonType == ButtonType.RadialOnlyIcon && overrideColors == false)
                    {
                        radialOOBackground.color = UIManagerAsset.buttonBorderColor;
                        radialOOIcon.color = UIManagerAsset.buttonFilledColor;
                    }

                    else if (buttonType == ButtonType.RadialOutlineOnlyIcon && overrideColors == false)
                    {
                        radialOutlineOOBorder.color = UIManagerAsset.buttonBorderColor;
                        radialOutlineOOFilled.color = UIManagerAsset.buttonBorderColor;
                        radialOutlineOOIcon.color = UIManagerAsset.buttonIconColor;
                        radialOutlineOOIconHighlighted.color = UIManagerAsset.buttonFilledColor;
                    }

                    else if (buttonType == ButtonType.Rounded)
                    {
                        if (overrideColors == false)
                        {
                            roundedBackground.color = UIManagerAsset.buttonBorderColor;
                            roundedText.color = UIManagerAsset.buttonFilledColor;
                        }

                        if (overrideFonts == false)
                        {
                            roundedText.font = UIManagerAsset.buttonFont;
                            roundedText.fontSize = UIManagerAsset.buttonFontSize;
                        }
                    }

                    else if (buttonType == ButtonType.RoundedOutline)
                    {
                        if (overrideColors == false)
                        {
                            roundedOutlineBorder.color = UIManagerAsset.buttonBorderColor;
                            roundedOutlineFilled.color = UIManagerAsset.buttonBorderColor;
                            roundedOutlineText.color = UIManagerAsset.buttonBorderColor;
                            roundedOutlineTextHighligted.color = UIManagerAsset.buttonFilledColor;
                        }

                        if (overrideFonts == false)
                        {
                            roundedOutlineText.font = UIManagerAsset.buttonFont;
                            roundedOutlineTextHighligted.font = UIManagerAsset.buttonFont;
                            roundedOutlineText.fontSize = UIManagerAsset.buttonFontSize;
                            roundedOutlineTextHighligted.fontSize = UIManagerAsset.buttonFontSize;
                        }
                    }
                }

                else if (UIManagerAsset.buttonThemeType == UIManager.ButtonThemeType.Custom)
                {
                    if (buttonType == ButtonType.Basic)
                    {
                        if (overrideColors == false)
                        {
                            basicFilled.color = UIManagerAsset.buttonFilledColor;
                            basicText.color = UIManagerAsset.buttonTextBasicColor;
                        }

                        if (overrideFonts == false)
                        {
                            basicText.font = UIManagerAsset.buttonFont;
                            basicText.fontSize = UIManagerAsset.buttonFontSize;
                        }
                    }

                    else if (buttonType == ButtonType.BasicOnlyIcon && overrideColors == false)
                    {
                        basicOnlyIconFilled.color = UIManagerAsset.buttonFilledColor;
                        basicOnlyIconIcon.color = UIManagerAsset.buttonIconBasicColor;
                    }

                    else if (buttonType == ButtonType.BasicWithIcon)
                    {
                        if (overrideColors == false)
                        {
                            basicWithIconFilled.color = UIManagerAsset.buttonFilledColor;
                            basicWithIconIcon.color = UIManagerAsset.buttonIconBasicColor;
                            basicWithIconText.color = UIManagerAsset.buttonTextBasicColor;
                        }

                        if (overrideFonts == false)
                        {
                            basicWithIconText.font = UIManagerAsset.buttonFont;
                            basicWithIconText.fontSize = UIManagerAsset.buttonFontSize;
                        }
                    }

                    else if (buttonType == ButtonType.BasicOutline)
                    {
                        if (overrideColors == false)
                        {
                            basicOutlineBorder.color = UIManagerAsset.buttonBorderColor;
                            basicOutlineFilled.color = UIManagerAsset.buttonFilledColor;
                            basicOutlineText.color = UIManagerAsset.buttonTextColor;
                            basicOutlineTextHighligted.color = UIManagerAsset.buttonTextHighlightedColor;
                        }

                        if (overrideFonts == false)
                        {
                            basicOutlineText.font = UIManagerAsset.buttonFont;
                            basicOutlineTextHighligted.font = UIManagerAsset.buttonFont;
                            basicOutlineText.fontSize = UIManagerAsset.buttonFontSize;
                            basicOutlineTextHighligted.fontSize = UIManagerAsset.buttonFontSize;
                        }
                    }

                    else if (buttonType == ButtonType.BasicOutlineOnlyIcon && overrideFonts == false)
                    {
                        basicOutlineOOBorder.color = UIManagerAsset.buttonBorderColor;
                        basicOutlineOOFilled.color = UIManagerAsset.buttonFilledColor;
                        basicOutlineOOIcon.color = UIManagerAsset.buttonBorderColor;
                        basicOutlineOOIconHighlighted.color = UIManagerAsset.buttonFilledColor;
                    }

                    else if (buttonType == ButtonType.BasicOutlineWithIcon)
                    {
                        if (overrideColors == false)
                        {
                            basicOutlineWOBorder.color = UIManagerAsset.buttonBorderColor;
                            basicOutlineWOFilled.color = UIManagerAsset.buttonFilledColor;
                            basicOutlineWOIcon.color = UIManagerAsset.buttonIconColor;
                            basicOutlineWOIconHighlighted.color = UIManagerAsset.buttonIconHighlightedColor;
                            basicOutlineWOText.color = UIManagerAsset.buttonTextColor;
                            basicOutlineWOTextHighligted.color = UIManagerAsset.buttonTextHighlightedColor;
                        }

                        if (overrideFonts == false)
                        {
                            basicOutlineWOText.font = UIManagerAsset.buttonFont;
                            basicOutlineWOTextHighligted.font = UIManagerAsset.buttonFont;
                            basicOutlineWOText.fontSize = UIManagerAsset.buttonFontSize;
                            basicOutlineWOTextHighligted.fontSize = UIManagerAsset.buttonFontSize;
                        }
                    }

                    else if (buttonType == ButtonType.RadialOnlyIcon && overrideColors == false)
                    {
                        radialOOBackground.color = UIManagerAsset.buttonFilledColor;
                        radialOOIcon.color = UIManagerAsset.buttonIconBasicColor;
                    }

                    else if (buttonType == ButtonType.RadialOutlineOnlyIcon && overrideColors == false)
                    {
                        radialOutlineOOBorder.color = UIManagerAsset.buttonBorderColor;
                        radialOutlineOOFilled.color = UIManagerAsset.buttonFilledColor;
                        radialOutlineOOIcon.color = UIManagerAsset.buttonIconColor;
                        radialOutlineOOIconHighlighted.color = UIManagerAsset.buttonIconHighlightedColor;
                    }

                    else if (buttonType == ButtonType.Rounded)
                    {
                        if (overrideColors == false)
                        {
                            roundedBackground.color = UIManagerAsset.buttonFilledColor;
                            roundedText.color = UIManagerAsset.buttonTextBasicColor;
                        }

                        if (overrideFonts == false)
                        {
                            roundedText.font = UIManagerAsset.buttonFont;
                            roundedText.fontSize = UIManagerAsset.buttonFontSize;
                        }
                    }

                    else if (buttonType == ButtonType.RoundedOutline)
                    {
                        if (overrideColors == false)
                        {
                            roundedOutlineBorder.color = UIManagerAsset.buttonBorderColor;
                            roundedOutlineFilled.color = UIManagerAsset.buttonFilledColor;
                            roundedOutlineText.color = UIManagerAsset.buttonTextColor;
                            roundedOutlineTextHighligted.color = UIManagerAsset.buttonTextHighlightedColor;
                        }

                        if (overrideFonts == false)
                        {
                            roundedOutlineText.font = UIManagerAsset.buttonFont;
                            roundedOutlineTextHighligted.font = UIManagerAsset.buttonFont;
                            roundedOutlineText.fontSize = UIManagerAsset.buttonFontSize;
                            roundedOutlineTextHighligted.fontSize = UIManagerAsset.buttonFontSize;
                        }
                    }
                }
            }

            catch { }
        }
    }
}