using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    public class UIManagerScrollbar : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;

        [Header("Resources")]
        [SerializeField] private Image background;
        [SerializeField] private Image bar;

        void Awake()
        {
            try
            {
                if (UIManagerAsset == null)
                    UIManagerAsset = Resources.Load<UIManager>("MUIP Manager");

                this.enabled = true;

                if (UIManagerAsset.enableDynamicUpdate == false)
                {
                    UpdateScrollbar();
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
                UpdateScrollbar();
        }

        void UpdateScrollbar()
        {
            try
            {
                background.color = UIManagerAsset.scrollbarBackgroundColor;
                bar.color = UIManagerAsset.scrollbarColor;
            }

            catch { }
        }
    }
}