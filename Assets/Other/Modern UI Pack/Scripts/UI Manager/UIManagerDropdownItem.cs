using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    public class UIManagerDropdownItem : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;
        public bool overrideColors = false;
        public bool overrideFonts = false;

        [Header("Resources")]
        [SerializeField] private Image itemBackground;
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemText;

        void Awake()
        {
            try
            {
                if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("MUIP Manager"); }

                this.enabled = true;

                if (UIManagerAsset.enableDynamicUpdate == false)
                {
                    UpdateDropdown();
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
                UpdateDropdown();
        }

        void UpdateDropdown()
        {
            try
            {
                if (UIManagerAsset.buttonThemeType == UIManager.ButtonThemeType.Basic)
                {
                    if (overrideColors == false)
                    {
                        itemBackground.color = UIManagerAsset.dropdownItemColor;
                        itemIcon.color = UIManagerAsset.dropdownTextColor;
                        itemText.color = UIManagerAsset.dropdownTextColor;
                    }

                    if (overrideFonts == false)
                    {
                        itemText.font = UIManagerAsset.dropdownFont;
                        itemText.fontSize = UIManagerAsset.dropdownFontSize;
                    }
                }

                else if (UIManagerAsset.buttonThemeType == UIManager.ButtonThemeType.Custom)
                {
                    if (overrideColors == false)
                    {
                        itemBackground.color = UIManagerAsset.dropdownItemColor;
                        itemIcon.color = UIManagerAsset.dropdownItemIconColor;
                        itemText.color = UIManagerAsset.dropdownItemTextColor;
                    }

                    if (overrideFonts == false)
                    {
                        itemText.font = UIManagerAsset.dropdownItemFont;
                        itemText.fontSize = UIManagerAsset.dropdownItemFontSize;
                    }
                }
            }

            catch { }
        }
    }
}