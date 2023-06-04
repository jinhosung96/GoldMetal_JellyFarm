#if  UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT && TRI_SUPPORT
using Cysharp.Threading.Tasks;

namespace Mine.Code.Framework.Manager.UINavigator.Runtime
{
    public interface IHasHistory
    {
        UniTask PrevAsync();
    }
}

#endif