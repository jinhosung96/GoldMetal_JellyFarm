using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    public class UIManagerProgressBar : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;
        public bool overrideColors = false;
        public bool overrideFonts = false;

        [Header("Resources")]
        [SerializeField] private Image bar;
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI label;

        bool dynamicUpdateEnabled;

        void Awake()
        {
            try
            {
                if (UIManagerAsset == null)
                    UIManagerAsset = Resources.Load<UIManager>("MUIP Manager");

                this.enabled = true;

                if (UIManagerAsset.enableDynamicUpdate == false)
                {
                    UpdateProgressBar();
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
                UpdateProgressBar();
        }

        void UpdateProgressBar()
        {
            try
            {
                if (overrideColors == false)
                {
                    bar.color = UIManagerAsset.progressBarColor;
                    background.color = UIManagerAsset.progressBarBackgroundColor;
                    label.color = UIManagerAsset.progressBarLabelColor;
                }

                if (overrideFonts == false)
                {
                    label.font = UIManagerAsset.progressBarLabelFont;
                    label.fontSize = UIManagerAsset.progressBarLabelFontSize;
                }
            }

            catch { }
        }
    }
}