using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    public class UIManagerSwitch : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;
        public bool overrideColors = false;

        [Header("Resources")]
        [SerializeField] private Image border;
        [SerializeField] private Image background;
        [SerializeField] private Image handleOn;
        [SerializeField] private Image handleOff;

        void Awake()
        {
            try
            {
                if (UIManagerAsset == null) { UIManagerAsset = Resources.Load<UIManager>("MUIP Manager"); }

                this.enabled = true;

                if (UIManagerAsset.enableDynamicUpdate == false)
                {
                    UpdateSwitch();
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
                UpdateSwitch();
        }

        void UpdateSwitch()
        {
            if (overrideColors == false)
            {
                try
                {
                    border.color = new Color(UIManagerAsset.switchBorderColor.r, UIManagerAsset.switchBorderColor.g, UIManagerAsset.switchBorderColor.b, border.color.a);
                    background.color = new Color(UIManagerAsset.switchBackgroundColor.r, UIManagerAsset.switchBackgroundColor.g, UIManagerAsset.switchBackgroundColor.b, background.color.a);
                    handleOn.color = new Color(UIManagerAsset.switchHandleOnColor.r, UIManagerAsset.switchHandleOnColor.g, UIManagerAsset.switchHandleOnColor.b, handleOn.color.a);
                    handleOff.color = new Color(UIManagerAsset.switchHandleOffColor.r, UIManagerAsset.switchHandleOffColor.g, UIManagerAsset.switchHandleOffColor.b, handleOff.color.a);
                }

                catch { }
            }
        }
    }
}