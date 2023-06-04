using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    [ExecuteInEditMode]
    public class UIManagerHSelector : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UIManager UIManagerAsset;
        public bool overrideOptions = false;
        public bool overrideColors = false;
        public bool overrideFonts = false;

        [Header("Resources")]
        [SerializeField] private List<GameObject> images = new List<GameObject>();
        [SerializeField] private List<GameObject> imagesHighlighted = new List<GameObject>();
        [SerializeField] private List<GameObject> texts = new List<GameObject>();
        HorizontalSelector hSelector;

        void Awake()
        {
            try
            {
                if (hSelector == null)
                    hSelector = gameObject.GetComponent<HorizontalSelector>();

                if (UIManagerAsset == null)
                    UIManagerAsset = Resources.Load<UIManager>("MUIP Manager");

                this.enabled = true;

                if (UIManagerAsset.enableDynamicUpdate == false)
                {
                    UpdateSelector();
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
                UpdateSelector();
        }

        void UpdateSelector()
        {
            if (overrideColors == false)
            {
                for (int i = 0; i < images.Count; ++i)
                {
                    Image currentImage = images[i].GetComponent<Image>();
                    currentImage.color = new Color(UIManagerAsset.selectorColor.r, UIManagerAsset.selectorColor.g, UIManagerAsset.selectorColor.b, currentImage.color.a);
                }

                for (int i = 0; i < imagesHighlighted.Count; ++i)
                {
                    Image currentAlphaImage = imagesHighlighted[i].GetComponent<Image>();
                    currentAlphaImage.color = new Color(UIManagerAsset.selectorHighlightedColor.r, UIManagerAsset.selectorHighlightedColor.g, UIManagerAsset.selectorHighlightedColor.b, currentAlphaImage.color.a);
                }
            }

            for (int i = 0; i < texts.Count; ++i)
            {
                TextMeshProUGUI currentText = texts[i].GetComponent<TextMeshProUGUI>();

                if (overrideColors == false)
                    currentText.color = new Color(UIManagerAsset.selectorColor.r, UIManagerAsset.selectorColor.g, UIManagerAsset.selectorColor.b, currentText.color.a);

                if (overrideFonts == false)
                {
                    currentText.font = UIManagerAsset.selectorFont;
                    currentText.fontSize = UIManagerAsset.hSelectorFontSize;
                }
            }

            if (hSelector != null && overrideOptions == false)
            {
                hSelector.invertAnimation = UIManagerAsset.hSelectorInvertAnimation;
                hSelector.loopSelection = UIManagerAsset.hSelectorLoopSelection;
            }
        }
    }
}