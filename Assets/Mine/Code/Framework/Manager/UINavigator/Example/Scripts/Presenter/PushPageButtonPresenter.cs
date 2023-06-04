#if  UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT && TRI_SUPPORT
using System;
using Cysharp.Threading.Tasks;
using Mine.Code.Framework.Manager.UINavigator.Runtime.Page;
using TriInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mine.Code.Framework.Manager.UINavigator.Example.Scripts.Presenter
{
    [Serializable]
    public class PushPageButtonPresenter
    {
        #region Fields

        [SerializeField, SceneObjectsOnly] Button targetButton;
        [SerializeField, SceneObjectsOnly] PageContainer targetContainer;
        [SerializeField, SceneObjectsOnly] Page targetView;

        #endregion

        #region Public Methods

        public void Initialize() => targetButton.OnClickAsObservable().Subscribe(_ => targetContainer.NextAsync(targetView.GetType().Name).Forget());

        #endregion
    }
}
#endif