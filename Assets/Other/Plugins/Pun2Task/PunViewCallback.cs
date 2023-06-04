#if PUN_TO_UNITASK_SUPPORT

using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Pun2Task
{
    public sealed class PunViewCallback : MonoBehaviour, IPunOwnershipCallbacks
    {
        private readonly AsyncReactiveProperty<(PhotonView targetView, Player requestingPlayer)> _ownershipRequestAsync
            = new AsyncReactiveProperty<(PhotonView targetView, Player requestingPlayer)>(default);

        private readonly AsyncReactiveProperty<(PhotonView targetView, Player previousOwner)> _ownershipTransferedAsync
            = new AsyncReactiveProperty<(PhotonView targetView, Player previousOwner)>(default);

        private readonly AsyncReactiveProperty<(PhotonView targetView, Player senderOfFailedRequest)> _ownershipTransferFailedAsync
            = new AsyncReactiveProperty<(PhotonView targetView, Player senderOfFailedRequest)>(default);

        private readonly AsyncReactiveProperty<bool> _isMine = new AsyncReactiveProperty<bool>(false);

        public IUniTaskAsyncEnumerable<(PhotonView targetView, Player requestingPlayer)> OwnershipRequestAsyncEnumerable
            => _ownershipRequestAsync;

        public IUniTaskAsyncEnumerable<(PhotonView targetView, Player previousOwner)> OwnershipTransferedAsyncEnumerable
            => _ownershipTransferedAsync;

        public IUniTaskAsyncEnumerable<(PhotonView targetView, Player previousOwner)> OwnershipTransferFailedAsyncEnumerable
            => _ownershipTransferFailedAsync;

        public IUniTaskAsyncEnumerable<bool> IsMineAsyncEnumerable => _isMine;

        private PhotonView _myView;

        public void Setup(PhotonView view)
        {
            _myView = view;
            _isMine.Value = view.IsMine;
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void OnDestroy()
        {
            _ownershipRequestAsync.Dispose();
            _ownershipTransferedAsync.Dispose();
            _ownershipTransferFailedAsync.Dispose();
            _isMine.Dispose();
        }

        public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
        {
            _ownershipRequestAsync.Value = (targetView, requestingPlayer);
        }

        public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
        {
            _ownershipTransferedAsync.Value = (targetView, previousOwner);
            if (_isMine.Value != _myView.IsMine)
            {
                _isMine.Value = _myView.IsMine;
            }
        }

        public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
        {
            _ownershipTransferFailedAsync.Value = (targetView, senderOfFailedRequest);
        }
    }

    public static class PhotonViewExt
    {
        /// <summary>
        /// OnOwnershipRequest
        /// </summary>
        public static IUniTaskAsyncEnumerable<(PhotonView targetView, Player requestingPlayer)>
            OwnershipRequestAsyncEnumerable(this PhotonView view)
        {
            return GetOrAdd(view).OwnershipRequestAsyncEnumerable;
        }

        /// <summary>
        /// OnOwnershipTransfered
        /// </summary>
        public static IUniTaskAsyncEnumerable<(PhotonView targetView, Player previousOwner)>
            OwnershipTransferedAsyncEnumerable(this PhotonView view)
        {
            return GetOrAdd(view).OwnershipTransferedAsyncEnumerable;
        }

        /// <summary>
        /// OnOwnershipTransferFailed
        /// </summary>
        public static IUniTaskAsyncEnumerable<(PhotonView targetView, Player previousOwner)>
            OwnershipTransferFailedAsyncEnumerable(this PhotonView view)
        {
            return GetOrAdd(view).OwnershipTransferFailedAsyncEnumerable;
        }
        
        /// <summary>
        /// PhotonView.IsMine
        /// </summary>
        public static IUniTaskAsyncEnumerable<bool> IsMineAsyncEnumerable(this PhotonView view)
        {
            return GetOrAdd(view).IsMineAsyncEnumerable;
        }

        /// <summary>
        /// After call PhotonView.RequestOwnership, wait until take ownership.
        /// </summary>
        public static async UniTask RequestOwnershipAsync(this PhotonView view, CancellationToken token = default)
        {
            if (view.IsMine) return;
            view.RequestOwnership();
            await view.IsMineAsyncEnumerable().FirstOrDefaultAsync(x => x, token);
        }

        private static PunViewCallback GetOrAdd(PhotonView view)
        {
            if (view.TryGetComponent<PunViewCallback>(out var c))
            {
                return c;
            }

            var p = view.gameObject.AddComponent<PunViewCallback>();
            p.Setup(view);
            return p;
        }
    }
}

#endif