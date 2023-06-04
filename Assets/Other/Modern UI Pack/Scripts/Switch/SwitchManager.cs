using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Michsky.UI.ModernUIPack
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Button))]
    public class SwitchManager : MonoBehaviour, IPointerEnterHandler
    {
        // Events
        public UnityEvent OnEvents;
        public UnityEvent OffEvents;

        // Saving
        public bool saveValue = true;
        public string switchTag = "Switch";

        // Settings
        public bool isOn = true;
        public bool invokeAtStart = true;
        public bool enableSwitchSounds = false;
        public bool useHoverSound = true;
        public bool useClickSound = true;

        // Resources
        public Animator switchAnimator;
        public Button switchButton;
        public AudioSource soundSource;

        // Audio
        public AudioClip hoverSound;
        public AudioClip clickSound;

        void Awake()
        {
            if (switchAnimator == null) { switchAnimator = gameObject.GetComponent<Animator>(); }
            if (switchButton == null)
            {
                switchButton = gameObject.GetComponent<Button>();
                switchButton.onClick.AddListener(AnimateSwitch);

                if (enableSwitchSounds == true && useClickSound == true)
                {
                    switchButton.onClick.AddListener(delegate
                    {
                        soundSource.PlayOneShot(clickSound);
                    });
                }
            }

            if (saveValue == true) { GetSavedData(); }
            else
            {
                StopCoroutine("DisableAnimator");
                switchAnimator.enabled = true;

                if (isOn == true) { switchAnimator.Play("On Instant"); }
                else { switchAnimator.Play("Off Instant"); }

                StartCoroutine("DisableAnimator");
            }

            if (invokeAtStart == true && isOn == true) { OnEvents.Invoke(); }
            else if (invokeAtStart == true && isOn == false) { OffEvents.Invoke(); }
        }

        void OnDisable()
        {
            StopCoroutine("DisableAnimator");
        }

        void GetSavedData()
        {
            StopCoroutine("DisableAnimator");
            switchAnimator.enabled = true;

            if (PlayerPrefs.GetString(switchTag + "Switch") == "" || PlayerPrefs.HasKey(switchTag + "Switch") == false)
            {
                if (isOn == true) { switchAnimator.Play("Switch On"); PlayerPrefs.SetString(switchTag + "Switch", "true"); }
                else { switchAnimator.Play("Switch Off"); PlayerPrefs.SetString(switchTag + "Switch", "false"); }
            }
            else if (PlayerPrefs.GetString(switchTag + "Switch") == "true") { switchAnimator.Play("Switch On"); isOn = true; }
            else if (PlayerPrefs.GetString(switchTag + "Switch") == "false") { switchAnimator.Play("Switch Off"); isOn = false; }

            StartCoroutine("DisableAnimator");
        }

        public void AnimateSwitch()
        {
            StopCoroutine("DisableAnimator");
            switchAnimator.enabled = true;

            if (isOn == true)
            {
                switchAnimator.Play("Switch Off");
                isOn = false;
                OffEvents.Invoke();

                if (saveValue == true) { PlayerPrefs.SetString(switchTag + "Switch", "false"); }
            }

            else
            {
                switchAnimator.Play("Switch On");
                isOn = true;
                OnEvents.Invoke();

                if (saveValue == true) { PlayerPrefs.SetString(switchTag + "Switch", "true"); }
            }

            StartCoroutine("DisableAnimator");
        }

        public void UpdateUI()
        {
            StopCoroutine("DisableAnimator");
            switchAnimator.enabled = true;

            if (isOn == true && switchAnimator.gameObject.activeInHierarchy == true) { switchAnimator.Play("Switch On"); }
            else if (isOn == false && switchAnimator.gameObject.activeInHierarchy == true) { switchAnimator.Play("Switch Off"); }

            StartCoroutine("DisableAnimator");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (enableSwitchSounds == true && useHoverSound == true && switchButton.interactable == true)
                soundSource.PlayOneShot(hoverSound);
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSeconds(0.5f);
            switchAnimator.enabled = false;
        }
    }
}