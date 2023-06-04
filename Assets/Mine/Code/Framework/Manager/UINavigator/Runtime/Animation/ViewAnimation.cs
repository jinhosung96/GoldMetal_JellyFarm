#if UNITASK_SUPPORT && DOTWEEN_SUPPORT && UNITASK_DOTWEEN_SUPPORT && UNIRX_SUPPORT && TRI_SUPPORT
using Cysharp.Threading.Tasks;
using TriInspector;
using UnityEngine;

// ReSharper disable Unity.NoNullPropagation

namespace Mine.Code.Framework.Manager.UINavigator.Runtime.Animation
{
    [System.Serializable]
    [DeclareTabGroup("AnimationType")]
    public class ViewShowAnimation
    {
        #region Fields

        [SerializeReference, Group("AnimationType"), Tab("Move"), InlineProperty, HideLabel, Indent(-1)]
        MoveShowAnimation moveAnimation;

        [SerializeReference, Group("AnimationType"), Tab("Rotate"), InlineProperty, HideLabel, Indent(-1)]
        RotateShowAnimation rotateAnimation;

        [SerializeReference, Group("AnimationType"), Tab("Scale"), InlineProperty, HideLabel, Indent(-1)]
        ScaleShowAnimation scaleShowAnimation;

        [SerializeReference, Group("AnimationType"), Tab("Fade"), InlineProperty, HideLabel, Indent(-1)]
        FadeShowAnimation fadeAnimation;

        #endregion

        #region Public Methods
        
        public async UniTask AnimateAsync(Transform transform, CanvasGroup canvasGroup) => await AnimateAsync((RectTransform)transform, canvasGroup);
        public async UniTask AnimateAsync(RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            // ReSharper disable once HeapView.ObjectAllocation
            await UniTask.WhenAll(
                moveAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                rotateAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                scaleShowAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                fadeAnimation?.AnimateAsync(canvasGroup) ?? UniTask.CompletedTask
            );
        }

        #endregion
    }
    
    [System.Serializable]
    [DeclareTabGroup("AnimationType")]
    public class ViewHideAnimation
    {
        #region Fields

        [SerializeReference, Group("AnimationType"), Tab("Move"), InlineProperty, HideLabel, Indent(-1)]
        MoveHideAnimation moveAnimation;

        [SerializeReference, Group("AnimationType"), Tab("Rotate"), InlineProperty, HideLabel, Indent(-1)]
        RotateHideAnimation rotateAnimation;

        [SerializeReference, Group("AnimationType"), Tab("Scale"), InlineProperty, HideLabel, Indent(-1)]
        ScaleHideAnimation scaleShowAnimation;

        [SerializeReference, Group("AnimationType"), Tab("Fade"), InlineProperty, HideLabel, Indent(-1)]
        FadeHideAnimation fadeAnimation;

        #endregion

        #region Public Methods
        
        public async UniTask AnimateAsync(Transform transform, CanvasGroup canvasGroup) => await AnimateAsync((RectTransform)transform, canvasGroup);
        public async UniTask AnimateAsync(RectTransform rectTransform, CanvasGroup canvasGroup)
        {
            // ReSharper disable once HeapView.ObjectAllocation
            await UniTask.WhenAll(
                moveAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                rotateAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                scaleShowAnimation?.AnimateAsync(rectTransform) ?? UniTask.CompletedTask,
                fadeAnimation?.AnimateAsync(canvasGroup) ?? UniTask.CompletedTask
            );
        }

        #endregion
    }
}
#endif