#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT && TRI_SUPPORT
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Debug = Mine.Code.Framework.Util.Debug.Debug;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Modal
{
    public class ModalComparer : IEqualityComparer<Modal>
    {
        public bool Equals(Modal x, Modal y)
        {
            //Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.GetType() == y.GetType();
        }

        public int GetHashCode(Modal obj)
        {
            return obj.GetType().GetHashCode();
        }
    }

    public sealed class ModalContainer : UIContainer<ModalContainer>, IHasHistory
    {
        #region Fields

        [SerializeField] Backdrop modalBackdrop; // 생성된 모달의 뒤에 배치될 레이어
        [SerializeField] List<Modal> modals = new(); // 해당 Container에서 생성할 수 있는 Modal들에 대한 목록

        #endregion

        #region Properties

        Dictionary<Type, Modal> Modals { get; set; }
        
        /// <summary>
        /// Page UI View들의 History 목록이다. <br/>
        /// History는 각 Container에서 관리된다. <br/>
        /// </summary>
        Stack<Modal> History { get; } = new();
        public Modal CurrentView => History.TryPeek(out var currentView) ? currentView : null;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            // modals에 등록된 모든 Modal들을 Type을 키값으로 한 Dictionary 형태로 등록
            Modals = modals.Distinct(new ModalComparer())
                .ToDictionary(modal => modal.GetType(), modal => modal);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 지정한 하위 Modal를 생성하고 History에 담는다. <br/>
        /// Modal를 지정해주는 방법은 제네릭 타입으로 원하는 Modal의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 기존에 생성된 Modal은 그대로 둔 채 새로운 Modal을 생성하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Modal이 생성 중인 상태일 때는 실행되지 않는다. <br/>
        /// </summary>
        /// <typeparam name="T"> 생성할 Modal의 Type </typeparam>
        /// <returns></returns>
        public async UniTask<T> NextAsync<T>(Func<T, UniTask> onInitialize = null) where T : Modal
        {
            if (Modals.TryGetValue(typeof(T), out var modal))
                return await NextAsync(modal as T, onInitialize);

            Debug.LogError($"Modal not found : {typeof(T)}");
            return null;
        }

        /// <summary>
        /// 지정한 하위 Modal를 생성하고 History에 담는다. <br/>
        /// Modal를 지정해주는 방법은 제네릭 타입으로 원하는 Modal의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 기존에 생성된 Modal은 그대로 둔 채 새로운 Modal을 생성하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Modal이 생성 중인 상태일 때는 실행되지 않는다. <br/>
        /// </summary>
        /// <param name="nextModalName"> 생성할 Modal의 클래스명 </param>
        /// <returns></returns>
        public async UniTask<Modal> NextAsync(string nextModalName, Func<Modal, UniTask> onInitialize = null)
        {
            var modal = Modals.Values.FirstOrDefault(x => x.GetType().Name == nextModalName);
            if (modal != null) return await NextAsync(modal, onInitialize);
            
            Debug.LogError($"Modal not found : {nextModalName}");
            return null;

        }

        public async UniTask<Modal> NextAsync(Type nextModalType, Func<Modal, UniTask> onInitialize = null)
        {
            if (Modals.TryGetValue(nextModalType, out var modal))
                return await NextAsync(modal, onInitialize);

            Debug.LogError($"Modal not found : {nextModalType.Name}");
            return null;
        }

        public async UniTask PrevAsync()
        {
            if (!CurrentView) return;
            if (CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing) return;

            if (CurrentView.BackDrop)
            {
                await UniTask.WhenAll
                (
                    CurrentView.BackDrop.DOFade(0,0.2f).ToUniTask(),
                    CurrentView.HideAsync()
                );
            }
            else await CurrentView.HideAsync();

            if (CurrentView.BackDrop) Destroy(CurrentView.BackDrop.gameObject);
            Destroy(CurrentView.gameObject);
            
            History.Pop();
        }

        #endregion

        #region Private Methods

        async UniTask<T> NextAsync<T>(T nextModal, Func<T, UniTask> onInitialize) where T : Modal
        {
            if (CurrentView != null && CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing) return null;
            
            var backdrop = await ShowBackdrop();

            nextModal.gameObject.SetActive(false);
            nextModal = Instantiate(nextModal, transform);
            
            nextModal.UIContainer = this;
            if (backdrop)
            {
                nextModal.BackDrop = backdrop;
                if (!nextModal.BackDrop.TryGetComponent<Button>(out var button)) 
                    button = nextModal.BackDrop.gameObject.AddComponent<Button>();
                
                button.OnClickAsObservable().Subscribe(_ => PrevAsync().Forget());
            }

            if (onInitialize != null) await onInitialize(nextModal);
            
            History.Push(nextModal);
            
#pragma warning disable 4014
            if (nextModal.BackDrop) CurrentView.BackDrop.DOFade(1, 0.2f);
#pragma warning restore 4014
            
            await CurrentView.ShowAsync();

            return CurrentView as T;
        }

        async UniTask<CanvasGroup> ShowBackdrop()
        {
            if (!modalBackdrop) return null;
            
            var backdrop = Instantiate(modalBackdrop.gameObject, transform, true);
            if (!backdrop.TryGetComponent<CanvasGroup>(out var canvasGroup)) 
                canvasGroup = backdrop.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            var rectTransform = (RectTransform)backdrop.transform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            await UniTask.Yield();
            rectTransform.anchoredPosition = Vector2.zero;
            return canvasGroup;
        }

        #endregion
    }
}
#endif