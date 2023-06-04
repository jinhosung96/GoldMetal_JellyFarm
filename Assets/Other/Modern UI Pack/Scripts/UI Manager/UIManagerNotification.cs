using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    public class UIManagerNotification : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;
        public bool overrideColors = false;
        public bool overrideFonts = false;

        [Header("Resources")]
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;

        void Awake()
        {
            try
            {
                if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("MUIP Manager"); }

                this.enabled = true;

                if (UIManagerAsset.enableDynamicUpdate == false)
                {
                    UpdateNotification();
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
                UpdateNotification();
        }

        void UpdateNotification()
        {
            try
            {
                if (overrideColors == false)
                {
                    background.color = UIManagerAsset.notificationBackgroundColor;
                    icon.color = UIManagerAsset.notificationIconColor;
                    title.color = UIManagerAsset.notificationTitleColor;
                    description.color = UIManagerAsset.notificationDescriptionColor;
                }

                if (overrideFonts == false)
                {
                    title.font = UIManagerAsset.notificationTitleFont;
                    title.fontSize = UIManagerAsset.notificationTitleFontSize;
                    description.font = UIManagerAsset.notificationDescriptionFont;
                    description.fontSize = UIManagerAsset.notificationDescriptionFontSize;
                }
            }

            catch { }
        }
    }
}