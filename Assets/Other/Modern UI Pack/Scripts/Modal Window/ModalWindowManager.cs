using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Michsky.UI.ModernUIPack
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ModalWindowManager : MonoBehaviour
    {
        // Resources
        public Image windowIcon;
        public TextMeshProUGUI windowTitle;
        public TextMeshProUGUI windowDescription;
        public Button confirmButton;
        public Button cancelButton;
        public Animator mwAnimator;

        // Content
        public Sprite icon;
        public string titleText = "Title";
        [TextArea] public string descriptionText = "Description here";

        // Events
        public UnityEvent onConfirm;
        public UnityEvent onCancel;

        // Settings
        public bool sharpAnimations = false;
        public bool useCustomValues = false;
        public bool isOn = false;
        public StartBehaviour startBehaviour = StartBehaviour.Disable;
        public CloseBehaviour closeBehaviour = CloseBehaviour.Disable;

        public enum StartBehaviour { None, Disable }
        public enum CloseBehaviour { None, Disable, Destroy }

        void Awake()
        {
            isOn = false;

            if (mwAnimator == null) { mwAnimator = gameObject.GetComponent<Animator>(); }
            if (confirmButton != null) { confirmButton.onClick.AddListener(onConfirm.Invoke); }
            if (cancelButton != null) { cancelButton.onClick.AddListener(onCancel.Invoke); }
            if (useCustomValues == false) { UpdateUI(); }
            if (startBehaviour == StartBehaviour.Disable) { gameObject.SetActive(false); }
        }

        public void UpdateUI()
        {
            try
            {
                windowIcon.sprite = icon;
                windowTitle.text = titleText;
                windowDescription.text = descriptionText;
            }

            catch { Debug.LogWarning("<b>[Modal Window]</b> Cannot update the content due to missing variables.", this); }
        }

        public void OpenWindow()
        {
            if (isOn == false)
            {
                StopCoroutine("DisableObject");
                gameObject.SetActive(true);
                isOn = true;

                if (sharpAnimations == false) { mwAnimator.CrossFade("Fade-in", 0.1f); }
                else { mwAnimator.Play("Fade-in"); }
            }
        }

        public void CloseWindow()
        {
            if (isOn == true)
            {
                StartCoroutine("DisableObject");
                isOn = false;

                if (sharpAnimations == false) { mwAnimator.CrossFade("Fade-out", 0.1f); }
                else { mwAnimator.Play("Fade-out"); }
            }
        }

        public void AnimateWindow()
        {
            if (isOn == false)
            {
                StopCoroutine("DisableObject");
                gameObject.SetActive(true);
                isOn = true;

                if (sharpAnimations == false) { mwAnimator.CrossFade("Fade-in", 0.1f); }
                else { mwAnimator.Play("Fade-in"); }
            }

            else
            {
                StartCoroutine("DisableObject");
                isOn = false;

                if (sharpAnimations == false) { mwAnimator.CrossFade("Fade-out", 0.1f); }
                else { mwAnimator.Play("Fade-out"); }
            }
        }

        IEnumerator DisableObject()
        {
            yield return new WaitForSeconds(1);

            if (closeBehaviour == CloseBehaviour.Disable) { gameObject.SetActive(false); }
            else if (closeBehaviour == CloseBehaviour.Destroy) { Destroy(gameObject); }
        }
    }
}