#if  UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT && TRI_SUPPORT
using TriInspector;
using UnityEngine;

namespace Mine.Code.Framework.Manager.UINavigator.Example.Scripts.Presenter
{
    public class OpenViewButtonPresenter : MonoBehaviour
    {
        [SerializeField, TableList] ShowSheetButtonPresenter[] showSheetButtonPresenters;
        [SerializeField, TableList] PushPageButtonPresenter[] pushPageButtonPresenters;
        [SerializeField, TableList] PushModalButtonPresenter[] pushModalButtonPresenters;
    
        void Awake()
        {
            foreach (var showSheetButtonPresenter in showSheetButtonPresenters) showSheetButtonPresenter.Initialize();
            foreach (var pushPageButtonPresenter in pushPageButtonPresenters) pushPageButtonPresenter.Initialize();
            foreach (var pushModalButtonPresenter in pushModalButtonPresenters) pushModalButtonPresenter.Initialize();
        }
    }
}
#endif