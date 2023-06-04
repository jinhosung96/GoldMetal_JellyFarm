using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Michsky.UI.ModernUIPack
{
    [RequireComponent(typeof(Animator))]
    public class NotificationManager : MonoBehaviour
    {
        // Content
        public Sprite icon;
        public string title = "Notification Title";
        [TextArea] public string description = "Notification description";

        // Resources
        public Animator notificationAnimator;
        public Image iconObj;
        public TextMeshProUGUI titleObj;
        public TextMeshProUGUI descriptionObj;

        // Settings
        public bool enableTimer = true;
        public float timer = 3f;
        public bool useCustomContent = false;
        public bool useStacking = false;
        [HideInInspector] public bool isOn;
        public StartBehaviour startBehaviour = StartBehaviour.Disable;
        public CloseBehaviour closeBehaviour = CloseBehaviour.Disable;

        // Events
        public UnityEvent onOpen;
        public UnityEvent onClose;

        public enum StartBehaviour { None, Disable }
        public enum CloseBehaviour { None, Disable, Destroy }

        void Awake()
        {
            isOn = false;

            if (useCustomContent == false)
            {
                try { UpdateUI(); }
                catch { Debug.LogError("<b>[Notification]</b> Cannot initalize the object due to missing components.", this); }
            }

            if (useStacking == true)
            {
                try
                {
                    NotificationStacking stacking = transform.GetComponentInParent<NotificationStacking>();
                    stacking.notifications.Add(this);
                    stacking.enableUpdating = true;
                }

                catch { Debug.LogError("<b>[Notification]</b> 'Stacking' is enabled but 'Notification Stacking' cannot be found in parent.", this); }
            }

            if (notificationAnimator == null) { notificationAnimator = gameObject.GetComponent<Animator>(); }
            if (startBehaviour == StartBehaviour.Disable) { gameObject.SetActive(false); }
        }

        public void OpenNotification()
        {
            if (isOn == true)
                return;

            gameObject.SetActive(true);
            isOn = true;

            StopCoroutine("StartTimer");
            StopCoroutine("DisableNotification");

            notificationAnimator.Play("In");
            onOpen.Invoke();

            if (enableTimer == true) { StartCoroutine("StartTimer"); }
        }

        public void CloseNotification()
        {
            if (isOn == false)
                return;

            isOn = false;
            notificationAnimator.Play("Out");
            onClose.Invoke();

            StartCoroutine("DisableNotification");
        }

        public void UpdateUI()
        {
            try
            {
                iconObj.sprite = icon;
                titleObj.text = title;
                descriptionObj.text = description;
            }

            catch { Debug.LogError("<b>[Notification]</b> Cannot update the component due to missing variables.", this); }
        }

        IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(timer);

            CloseNotification();
            StartCoroutine("DisableNotification");
        }

        IEnumerator DisableNotification()
        {
            yield return new WaitForSeconds(1f);

            if (closeBehaviour == CloseBehaviour.Disable) { gameObject.SetActive(false); isOn = false; }
            else if (closeBehaviour == CloseBehaviour.Destroy) { Destroy(gameObject); }
        }
    }
}