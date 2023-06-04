#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT && TRI_SUPPORT
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = Mine.Code.Framework.Util.Debug.Debug;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Page
{
    public sealed class PageContainer : UIContainer<PageContainer>, IHasHistory
    {
        #region Properties

        [field: SerializeField] internal Mine.Code.Framework.Manager.UINavigator.Runtime.Page.Page DefaultPage { get; private set; }
        
        Dictionary<Type, Mine.Code.Framework.Manager.UINavigator.Runtime.Page.Page> Pages { get; } = new();

        /// <summary>
        /// Page UI View들의 History 목록이다. <br/>
        /// History는 각 Container에서 관리된다. <br/>
        /// </summary>
        Stack<Mine.Code.Framework.Manager.UINavigator.Runtime.Page.Page> History { get; } = new();

        public Mine.Code.Framework.Manager.UINavigator.Runtime.Page.Page CurrentView => History.TryPeek(out var currentView) ? currentView : null;
        bool IsRemainHistory => DefaultPage ? History.Count > 1 : History.Count > 0;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            // 하위 Page들을 Dictionary에 등록
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<Mine.Code.Framework.Manager.UINavigator.Runtime.Page.Page>(out var page))
                {
                    if (Pages.TryAdd(page.GetType(), page))
                    {
                        page.gameObject.SetActive(false);
                        page.UIContainer = this;
                    }
                    else Debug.LogError("Same page already exists");
                }
            }

            if (DefaultPage)
            {
                DefaultPage.ShowAsync(false).Forget();
                History.Push(DefaultPage);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 지정한 하위 Page를 활성화하고 History에 담는다. <br/>
        /// Page를 지정해주는 방법은 제네릭 타입으로 원하는 Page의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 현재 활성화된 Page가 있다면, 이전 Page를 비활성화하고 새로운 Page를 활성화하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Page가 전환 중인 상태일 때는 실행되지 않는다. <br/>
        /// </summary>
        /// <typeparam name="T"> 활성화 시킬 Page의 Type </typeparam>
        /// <returns></returns>
        public async UniTask<T> NextAsync<T>(Func<T, UniTask> onInitialize = null) where T : Mine.Code.Framework.Manager.UINavigator.Runtime.Page.Page
        {
            if (Pages.TryGetValue(typeof(T), out var page))
                return await NextAsync(page as T, onInitialize);

            Debug.LogError($"Page not found : {typeof(T)}");
            return null;
        }

        /// <summary>
        /// 지정한 하위 Page를 활성화하고 History에 담는다. <br/>
        /// Page를 지정해주는 방법은 제네릭 타입으로 원하는 Page의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 현재 활성화된 Page가 있다면, 이전 Page를 비활성화하고 새로운 Page를 활성화하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Page가 전환 중인 상태일 때는 실행되지 않는다. <br/>
        /// </summary>
        /// <param name="nextPageName"> 활성화 시킬 Page의 클래스명 </param>
        /// <returns></returns>
        public async UniTask<Mine.Code.Framework.Manager.UINavigator.Runtime.Page.Page> NextAsync(string nextPageName, Func<Mine.Code.Framework.Manager.UINavigator.Runtime.Page.Page, UniTask> onInitialize = null)
        {
            var page = Pages.Values.FirstOrDefault(x => x.GetType().Name == nextPageName);
            if (page != null) return await NextAsync(page, onInitialize);
            
            Debug.LogError($"Page not found : {nextPageName}");
            return null;
        }

        /// <summary>
        /// 특정 UI View를 종료하는 메소드이다. <br/>
        /// 해당 UI View가 종료되면 해당 View보다 Histroy상 뒤에 활성화된 View들도 모두 같이 종료된다. <br/>
        /// <br/>
        /// 해당 UI View의 History를 가지고 있는 부모 Sheet를 찾아 해당 History를 최상단부터 차근차근 비교하며 해당 View를 찾는다. <br/>
        /// 그리고 그 View들을 나중에 제거하기 위해 Queue에 담아둔다. <br/>
        /// 해당 View를 찾으면 해당 View를 제거하는데 이 때, 만약 해당 UI View가 Sheet라면, <br/>
        /// ResetOnPop 설정 여부에 따라 해당 Sheet의 부모 Container의 CurrentSheet를 null로 초기화하거나 InitialSheet를 CurrentSheet로 설정한다. <br/>
        /// <br/>
        /// 지정한 UI View가 종료되면 Queue에 담아둔 View들을 모두 종료하고 Modal은 추가로 Backdrop을 제거함과 동시에 Modal 또한 파괴한다. <br/>
        /// 이 때, PopRoutineAsync 메소드를 사용하지 않는 이유는 즉각적으로 제거해주기 위함과 더불어 Sheet의 경우 PopRoutineAsync가 정의되어있지 않기 때문이다. <br/>
        /// </summary>
        public async UniTask<Mine.Code.Framework.Manager.UINavigator.Runtime.Page.Page> NextAsync(Type nextPageType, Func<Mine.Code.Framework.Manager.UINavigator.Runtime.Page.Page, UniTask> onInitialize = null)
        {
            if (Pages.TryGetValue(nextPageType, out var page))
                return await NextAsync(page, onInitialize);

            Debug.LogError($"Page not found : {nextPageType.Name}");
            return null;
        }

        public async UniTask PrevAsync()
        {
            if (!IsRemainHistory) return;

            if (CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing) return;

            CurrentView.HideAsync().Forget();
            History.Pop();

            if (!CurrentView) return;

            await CurrentView.ShowAsync();
        }

        #endregion

        #region Private Methods

        async UniTask<T> NextAsync<T>(T nextPage, Func<T, UniTask> onInitialize) where T : Mine.Code.Framework.Manager.UINavigator.Runtime.Page.Page
        {
            if (CurrentView && CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing) return null;
            if (CurrentView && CurrentView == nextPage) return null;

            if (onInitialize != null) await onInitialize(nextPage);
            if(CurrentView) CurrentView.HideAsync().Forget();
            History.Push(nextPage);
            
            await CurrentView.ShowAsync();

            return CurrentView as T;
        }

        #endregion
    }
}
#endif