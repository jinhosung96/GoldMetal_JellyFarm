#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT && TRI_SUPPORT
using UnityEngine;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Modal
{
    public abstract class Modal : UIScope
    {
        #region Properties

        public CanvasGroup BackDrop { get; internal set; }

        #endregion
    }
}
#endif