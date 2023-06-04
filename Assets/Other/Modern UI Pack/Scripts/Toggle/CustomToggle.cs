using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.ModernUIPack
{
    [RequireComponent(typeof(Toggle))]
    [RequireComponent(typeof(Animator))]
    public class CustomToggle : MonoBehaviour
    {
        [HideInInspector] public Toggle toggleObject;
        [HideInInspector] public Animator toggleAnimator;

        [Header("Settings")]
        public bool invokeOnAwake;

        void Awake()
        {
            if (toggleObject == null) { toggleObject = gameObject.GetComponent<Toggle>(); }
            if (toggleAnimator == null) { toggleAnimator = toggleObject.GetComponent<Animator>(); }

            toggleObject.onValueChanged.AddListener(UpdateStateDynamic);
            UpdateState();

            if (invokeOnAwake == true) { toggleObject.onValueChanged.Invoke(toggleObject.isOn); }
        }

        public void UpdateState()
        {
            StopCoroutine("DisableAnimator");
            toggleAnimator.enabled = true;

            if (toggleObject.isOn) { toggleAnimator.Play("On Instant"); }
            else { toggleAnimator.Play("Off Instant"); }

            StartCoroutine("DisableAnimator");
        }

        public void UpdateStateDynamic(bool value)
        {
            StopCoroutine("DisableAnimator");
            toggleAnimator.enabled = true;

            if (toggleObject.isOn) { toggleAnimator.Play("Toggle On"); }
            else { toggleAnimator.Play("Toggle Off"); }

            StartCoroutine("DisableAnimator");
        }

        IEnumerator DisableAnimator()
        {
            yield return new WaitForSeconds(0.5f);
            toggleAnimator.enabled = false;
        }
    }
}