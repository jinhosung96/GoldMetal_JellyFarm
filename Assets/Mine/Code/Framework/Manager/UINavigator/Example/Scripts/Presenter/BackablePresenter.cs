#if  UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT && TRI_SUPPORT

using Cysharp.Threading.Tasks;
using Mine.Code.Framework.Manager.UINavigator.Runtime;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Mine.Code.Framework.Manager.UINavigator.Example.Scripts.Presenter
{
    public class BackablePresenter : MonoBehaviour
    {
        #region Fields

        [SerializeField] Button backButton;

        #endregion

        #region Unity Lifecycle

        void Awake() => backButton.OnClickAsObservable().Subscribe(_ =>
        {
            if (UIContainer.Of(transform) is IHasHistory container) container.PrevAsync().Forget();
        }).AddTo(gameObject);

        #endregion
    }
}

#endif