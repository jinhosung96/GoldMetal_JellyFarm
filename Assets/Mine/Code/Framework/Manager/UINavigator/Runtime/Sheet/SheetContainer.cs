#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT && TRI_SUPPORT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Debug = Mine.Code.Framework.Util.Debug.Debug;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet
{
    public sealed class SheetContainer : UIContainer<SheetContainer>
    {
        #region Fields

        [SerializeField] Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet.Sheet initialSheet; // 시작하자마자 활성화 되는 시트

        #endregion

        #region Properties

        Dictionary<Type, Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet.Sheet> Sheets { get; } = new();
        
        /// <summary>
        /// 현재 Container의 현재 Sheet
        /// </summary>
        public Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet.Sheet CurrentView { get; private set; }

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            // 하위 Sheet들을 Dictionary에 등록
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet.Sheet>(out var sheet))
                {
                    if (Sheets.TryAdd(sheet.GetType(), sheet))
                    {
                        sheet.gameObject.SetActive(false);
                        sheet.UIContainer = this;
                    }
                    else Debug.LogError("Same sheet already exists");
                }
            }
        }

        void OnEnable()
        {
            // 초기 시트를 활성화
            if (initialSheet != null)
            {
                CurrentView = initialSheet;
                CurrentView.ShowAsync(false).Forget();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 지정한 하위 Sheet를 활성화한다. <br/>
        /// Sheet를 지정해주는 방법은 제네릭 타입으로 원하는 Sheet의 타입을 넘기는 것으로 이루어진다. <br/>
        /// <br/>
        /// 현재 활성화된 Sheet가 있다면, 이전 Sheet를 비활성화하고 새로운 Sheet를 활성화하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Sheet가 전환 중인 상태일 때는 실행되지 않는다. <br/>
        /// <br/>
        /// resetOnChangeSheet가 true일 경우, 이전 Sheet의 History를 초기화한다. <br/>
        /// </summary>
        /// <typeparam name="T"> 활성화 시킬 Sheet의 Type </typeparam>
        /// <returns></returns>
        public async UniTask<T> NextAsync<T>() where T : Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet.Sheet => await NextAsync(typeof(T)) as T;
        
        /// <summary>
        /// 지정한 하위 Sheet를 활성화한다. <br/>
        /// <br/>
        /// 현재 활성화된 Sheet가 있다면, 이전 Sheet를 비활성화하고 새로운 Sheet를 활성화하는 방식이며 FocusView를 갱신해준다. <br/>
        /// 이 때, 기존 Sheet가 전환 중인 상태일 때는 실행되지 않는다. <br/>
        /// <br/>
        /// resetOnChangeSheet가 true일 경우, 이전 Sheet의 History를 초기화한다. <br/>
        /// </summary>
        /// <param name="sheetName"> 활성화 시킬 Sheet의 클래스명 </param>
        /// <returns></returns>
        public async UniTask<Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet.Sheet> NextAsync(string sheetName)
        {
            var sheet = Sheets.Values.FirstOrDefault(x => x.GetType().Name == sheetName);
            if (sheet != null) return await NextAsync(sheet);
            
            Debug.LogError($"Sheet not found : {sheetName}");
            return null;

        }

        public async UniTask<Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet.Sheet> NextAsync(Type targetSheet)
        {
            if (Sheets.TryGetValue(targetSheet, out var nextSheet)) 
                return await NextAsync(nextSheet);
            
            Debug.LogError("Sheet not found");
            return null;
        }

        #endregion

        #region Private Methods

        async Task<Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet.Sheet> NextAsync(Mine.Code.Framework.Manager.UINavigator.Runtime.Sheet.Sheet nextSheet)
        {
            if (CurrentView != null && CurrentView.VisibleState is VisibleState.Appearing or VisibleState.Disappearing) return null;
            if (CurrentView != null && CurrentView == nextSheet) return null;

            var prevSheet = CurrentView;
            CurrentView = nextSheet;

            if (prevSheet != null) prevSheet.HideAsync().Forget();
            await CurrentView.ShowAsync();

            return CurrentView;
        }

        #endregion
    }
}
#endif