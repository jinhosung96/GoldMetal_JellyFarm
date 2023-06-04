using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Michsky.UI.ModernUIPack
{
    [RequireComponent(typeof(TMP_InputField))]
    [RequireComponent(typeof(Animator))]
    public class CustomInputField : MonoBehaviour
    {
        [Header("Resources")]
        public TMP_InputField inputText;
        public Animator inputFieldAnimator;

        [Header("Settings")]
        public bool processSubmit = false;

        [Header("Events")]
        public UnityEvent onSubmit;

        // Hidden variables
        private string inAnim = "In";
        private string outAnim = "Out";
        private string instaInAnim = "Instant In";
        private string instaOutAnim = "Instant Out";

        void Awake()
        {
            if (inputText == null) { inputText = gameObject.GetComponent<TMP_InputField>(); }
            if (inputFieldAnimator == null) { inputFieldAnimator = gameObject.GetComponent<Animator>(); }

            inputText.onSelect.AddListener(delegate { AnimateIn(); });
            inputText.onEndEdit.AddListener(delegate { AnimateOut(); });
            UpdateStateInstant();
        }

        void OnEnable()
        {
            if (inputText == null)
                return;

            inputText.ForceLabelUpdate();
            UpdateStateInstant();

            if (gameObject.activeInHierarchy == true) { StartCoroutine("DisableAnimator"); }
        }

        void Update()
        {
            if (processSubmit == false ||
                string.IsNullOrEmpty(inputText.text) == true ||
                EventSystem.current.currentSelectedGameObject != inputText.gameObject)
            { return; }

#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.Return)) { onSubmit.Invoke(); inputText.text = ""; }
#elif ENABLE_INPUT_SYSTEM
            if (Keyboard.current.enterKey.wasPressedThisFrame) { onSubmit.Invoke(); }
#endif
        }

        public void AnimateIn() 
        {
            StopCoroutine("DisableAnimator");
         
            if (inputFieldAnimator.gameObject.activeInHierarchy == true) 
            {
                inputFieldAnimator.enabled = true;
                inputFieldAnimator.Play(inAnim);
                StartCoroutine("DisableAnimator");
            }
        }

        public void AnimateOut()
        {
            if (inputFieldAnimator.gameObject.activeInHierarchy == true)
            {
                inputFieldAnimator.enabled = true;
                if (inputText.text.Length == 0) { inputFieldAnimator.Play(outAnim); }
                StartCoroutine("DisableAnimator");
            }
        }

        public void UpdateState()
        {
            if (inputText.text.Length == 0) { AnimateOut(); }
            else { AnimateIn(); }
        }

        public void UpdateStateInstant()
        {
            if (inputText.text.Length == 0) { inputFieldAnimator.Play(instaOutAnim); }
            else { inputFieldAnimator.Play(instaInAnim); }
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSeconds(1);
            inputFieldAnimator.enabled = false;
        }
    }
}