#if  UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT && TRI_SUPPORT
using Mine.Code.Framework.Manager.UINavigator.Runtime;
using UnityEngine;

namespace Mine.Code.Framework.Manager.UINavigator.Example.Scripts.Presenter
{
    public class BackPresenter : MonoBehaviour
    {
        async void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!await UIContainer.BackAsync())
                {
                    Debug.Log("Setting Modal 활성화");
                }
            }
        }
    }
}

#endif