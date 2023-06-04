#if PUN_TO_UNITASK_SUPPORT

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Pun2Task.Callbacks;
using UnityEngine;

namespace Pun2Task
{
    public static class Pun2TaskCallback
    {
        /// <summary>
        /// Called to signal that the raw connection got established but before the client can call operation on the server.
        /// </summary>
        /// <remarks>
        /// After the (low level transport) connection is established, the client will automatically send
        /// the Authentication operation, which needs to get a response before the client can call other operations.
        ///
        /// Your logic should wait for either: OnRegionListReceived or OnConnectedToMaster.
        ///
        /// This callback is useful to detect if the server can be reached at all (technically).
        /// Most often, it's enough to implement OnDisconnected().
        ///
        /// This is not called for transitions from the masterserver to game servers.
        /// </remarks>
        public static UniTask OnConnectedAsync()
        {
            return GetBridge().OnConnectedAsync;
        }

        /// <summary>
        /// Called when the local user/client left a room, so the game's logic can clean up it's internal state.
        /// </summary>
        /// <remarks>
        /// When leaving a room, the LoadBalancingClient will disconnect the Game Server and connect to the Master Server.
        /// This wraps up multiple internal actions.
        ///
        /// Wait for the callback OnConnectedToMaster, before you use lobbies and join or create rooms.
        /// </remarks>
        public static UniTask OnLeftRoomAsync()
        {
            return GetBridge().OnLeftRoomAsync;
        }

        /// <summary>
        /// Called after switching to a new MasterClient when the current one leaves.
        /// </summary>
        /// <remarks>
        /// This is not called when this client enters a room.
        /// The former MasterClient is still in the player list when this method get called.
        /// </remarks>
        public static UniTask<Player> OnMasterClientSwitchedAsync()
        {
            return GetBridge().OnMasterClientSwitchedAsync;
        }

        /// <summary>
        /// Called when the server couldn't create a room (OpCreateRoom failed).
        /// </summary>
        /// <remarks>
        /// The most common cause to fail creating a room, is when a title relies on fixed room-names and the room already exists.
        /// </remarks>
        public static UniTask<(short returnCode, string message)> OnCreateRoomFailedAsync()
        {
            return GetBridge().OnCreateRoomFailedAsync;
        }

        /// <summary>
        /// Called when a previous OpJoinRoom call failed on the server.
        /// </summary>
        /// <remarks>
        /// The most common causes are that a room is full or does not exist (due to someone else being faster or closing the room).
        /// </remarks>
        public static UniTask<(short returnCode, string message)> OnJoinRoomFailedAsync()
        {
            return GetBridge().OnJoinRoomFailedAsync;
        }

        /// <summary>
        /// Called when this client created a room and entered it. OnJoinedRoom() will be called as well.
        /// </summary>
        /// <remarks>
        /// This callback is only called on the client which created a room (see OpCreateRoom).
        ///
        /// As any client might close (or drop connection) anytime, there is a chance that the
        /// creator of a room does not execute OnCreatedRoom.
        ///
        /// If you need specific room properties or a "start signal", implement OnMasterClientSwitched()
        /// and make each new MasterClient check the room's state.
        /// </remarks>
        public static UniTask OnCreatedRoomAsync()
        {
            return GetBridge().OnCreatedRoomAsync;
        }

        /// <summary>
        /// Called on entering a lobby on the Master Server. The actual room-list updates will call OnRoomListUpdate.
        /// </summary>
        /// <remarks>
        /// While in the lobby, the roomlist is automatically updated in fixed intervals (which you can't modify in the public cloud).
        /// The room list gets available via OnRoomListUpdate.
        /// </remarks>
        public static UniTask OnJoinedLobbyAsync()
        {
            return GetBridge().OnJoinedLobbyAsync;
        }

        /// <summary>
        /// Called after leaving a lobby.
        /// </summary>
        /// <remarks>
        /// When you leave a lobby, [OpCreateRoom](@ref OpCreateRoom) and [OpJoinRandomRoom](@ref OpJoinRandomRoom)
        /// automatically refer to the default lobby.
        /// </remarks>
        public static UniTask OnLeftLobbyAsync()
        {
            return GetBridge().OnLeftLobbyAsync;
        }

        /// <summary>
        /// Called after disconnecting from the Photon server. It could be a failure or intentional
        /// </summary>
        /// <remarks>
        /// The reason for this disconnect is provided as DisconnectCause.
        /// </remarks>
        public static UniTask<DisconnectCause> OnDisconnectedAsync()
        {
            return GetBridge().OnDisconnectedAsync;
        }


        /// <summary>
        /// Called when the Name Server provided a list of regions for your title.
        /// </summary>
        /// <remarks>Check the RegionHandler class description, to make use of the provided values.</remarks>
        public static UniTask<RegionHandler> OnRegionListReceivedAsync()
        {
            return GetBridge().OnRegionListReceivedAsync;
        }

        /// <summary>
        /// Called for any update of the room-listing while in a lobby (InLobby) on the Master Server.
        /// </summary>
        /// <remarks>
        /// Each item is a RoomInfo which might include custom properties (provided you defined those as lobby-listed when creating a room).
        /// Not all types of lobbies provide a listing of rooms to the client. Some are silent and specialized for server-side matchmaking.
        /// </remarks>
        public static UniTask<IList<RoomInfo>> OnRoomListUpdateAsync()
        {
            return GetBridge().OnRoomListUpdateAsync;
        }

        /// <summary>
        /// Called when the LoadBalancingClient entered a room, no matter if this client created it or simply joined.
        /// </summary>
        /// <remarks>
        /// When this is called, you can access the existing players in Room.Players, their custom properties and Room.CustomProperties.
        ///
        /// In this callback, you could create player objects. For example in Unity, instantiate a prefab for the player.
        ///
        /// If you want a match to be started "actively", enable the user to signal "ready" (using OpRaiseEvent or a Custom Property).
        /// </remarks>
        public static UniTask OnJoinedRoomAsync()
        {
            return GetBridge().OnJoinedRoomAsync;
        }

        /// <summary>
        /// Called when a remote player entered the room. This Player is already added to the playerlist.
        /// </summary>
        /// <remarks>
        /// If your game starts with a certain number of players, this callback can be useful to check the
        /// Room.playerCount and find out if you can start.
        /// </remarks>
        public static UniTask<Player> OnPlayerEnteredRoomAsync()
        {
            return GetBridge().OnPlayerEnteredRoomAsync;
        }

        /// <summary>
        /// Called when a remote player entered the room. This Player is already added to the playerlist.
        /// </summary>
        /// <remarks>
        /// If your game starts with a certain number of players, this callback can be useful to check the
        /// Room.playerCount and find out if you can start.
        /// </remarks>
        public static IUniTaskAsyncEnumerable<Player> OnPlayerEnteredRoomAsyncEnumerable()
        {
            return GetBridge().OnPlayerEnteredRoomAsyncEnumerable;
        }

        /// <summary>
        /// Called when a remote player left the room or became inactive. Check otherPlayer.IsInactive.
        /// </summary>
        /// <remarks>
        /// If another player leaves the room or if the server detects a lost connection, this callback will
        /// be used to notify your game logic.
        ///
        /// Depending on the room's setup, players may become inactive, which means they may return and retake
        /// their spot in the room. In such cases, the Player stays in the Room.Players dictionary.
        ///
        /// If the player is not just inactive, it gets removed from the Room.Players dictionary, before
        /// the callback is called.
        /// </remarks>
        public static UniTask<Player> OnPlayerLeftRoomAsync()
        {
            return GetBridge().OnPlayerLeftRoomAsync;
        }
        
        /// <summary>
        /// Called when a remote player left the room or became inactive. Check otherPlayer.IsInactive.
        /// </summary>
        /// <remarks>
        /// If another player leaves the room or if the server detects a lost connection, this callback will
        /// be used to notify your game logic.
        ///
        /// Depending on the room's setup, players may become inactive, which means they may return and retake
        /// their spot in the room. In such cases, the Player stays in the Room.Players dictionary.
        ///
        /// If the player is not just inactive, it gets removed from the Room.Players dictionary, before
        /// the callback is called.
        /// </remarks>
        public static IUniTaskAsyncEnumerable<Player> OnPlayerLeftRoomAsyncEnumerable()
        {
            return GetBridge().OnPlayerLeftRoomAsyncEnumerable;
        }
        

        /// <summary>
        /// Called when a previous OpJoinRandom call failed on the server.
        /// </summary>
        /// <remarks>
        /// The most common causes are that a room is full or does not exist (due to someone else being faster or closing the room).
        ///
        /// When using multiple lobbies (via OpJoinLobby or a TypedLobby parameter), another lobby might have more/fitting rooms.<br/>
        /// </remarks>
        public static UniTask<(short returnCode, string message)> OnJoinRandomFailedAsync()
        {
            return GetBridge().OnJoinRandomFailedAsync;
        }

        /// <summary>
        /// Called when the client is connected to the Master Server and ready for matchmaking and other tasks.
        /// </summary>
        /// <remarks>
        /// The list of available rooms won't become available unless you join a lobby via LoadBalancingClient.OpJoinLobby.
        /// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.
        /// </remarks>
        public static UniTask OnConnectedToMasterAsync()
        {
            return GetBridge().OnConnectedToMasterAsync;
        }

        /// <summary>
        /// Called when a room's custom properties changed. The propertiesThatChanged contains all that was set via Room.SetCustomProperties.
        /// </summary>
        /// <remarks>
        /// Since v1.25 this method has one parameter: Hashtable propertiesThatChanged.<br/>
        /// Changing properties must be done by Room.SetCustomProperties, which causes this callback locally, too.
        /// </remarks>
        public static UniTask<Hashtable> OnRoomPropertiesUpdateAsync()
        {
            return GetBridge().OnRoomPropertiesUpdateAsync;
        }

        /// <summary>
        /// Called when custom player-properties are changed. Player and the changed properties are passed as object[].
        /// </summary>
        /// <remarks>
        /// Changing properties must be done by Player.SetCustomProperties, which causes this callback locally, too.
        /// </remarks>
        public static UniTask<(Player targetPlayer, Hashtable changedProps)> OnPlayerPropertiesUpdateAsync()
        {
            return GetBridge().OnPlayerPropertiesUpdateAsync;
        }

        /// <summary>
        /// Called when the server sent the response to a FindFriends request.
        /// </summary>
        /// <remarks>
        /// After calling OpFindFriends, the Master Server will cache the friend list and send updates to the friend
        /// list. The friends includes the name, userId, online state and the room (if any) for each requested user/friend.
        ///
        /// Use the friendList to update your UI and store it, if the UI should highlight changes.
        /// </remarks>
        public static UniTask<IList<FriendInfo>> OnFriendListUpdateAsync()
        {
            return GetBridge().OnFriendListUpdateAsync;
        }

        /// <summary>
        /// Called when your Custom Authentication service responds with additional data.
        /// </summary>
        /// <remarks>
        /// Custom Authentication services can include some custom data in their response.
        /// When present, that data is made available in this callback as Dictionary.
        /// While the keys of your data have to be strings, the values can be either string or a number (in Json).
        /// You need to make extra sure, that the value type is the one you expect. Numbers become (currently) int64.
        ///
        /// Example: void OnCustomAuthenticationResponse(Dictionary&lt;string, object&gt; data) { ... }
        /// </remarks>
        /// <see cref="https://doc.photonengine.com/en-us/realtime/current/reference/custom-authentication"/>
        public static UniTask<IReadOnlyDictionary<string, object>> OnCustomAuthenticationResponseAsync()
        {
            return GetBridge().OnCustomAuthenticationResponseAsync;
        }

        /// <summary>
        /// Called when the custom authentication failed. Followed by disconnect!
        /// </summary>
        /// <remarks>
        /// Custom Authentication can fail due to user-input, bad tokens/secrets.
        /// If authentication is successful, this method is not called. Implement OnJoinedLobby() or OnConnectedToMaster() (as usual).
        ///
        /// During development of a game, it might also fail due to wrong configuration on the server side.
        /// In those cases, logging the debugMessage is very important.
        ///
        /// Unless you setup a custom authentication service for your app (in the [Dashboard](https://dashboard.photonengine.com)),
        /// this won't be called!
        /// </remarks>
        public static UniTask<string> OnCustomAuthenticationFailedAsync()
        {
            return GetBridge().OnCustomAuthenticationFailedAsync;
        }

        //TODO: Check if this needs to be implemented
        // in: IOptionalInfoCallbacks
        public static UniTask<OperationResponse> OnWebRpcResponseAsync()
        {
            return GetBridge().OnWebRpcResponseAsync;
        }

        //TODO: Check if this needs to be implemented
        // in: IOptionalInfoCallbacks
        public static UniTask<IList<TypedLobbyInfo>> OnLobbyStatisticsUpdateAsync()
        {
            return GetBridge().OnLobbyStatisticsUpdateAsync;
        }

        /// <summary>
        /// Called when the client receives an event from the server indicating that an error happened there.
        /// </summary>
        /// <remarks>
        /// In most cases this could be either:
        /// 1. an error from webhooks plugin (if HasErrorInfo is enabled), read more here:
        /// https://doc.photonengine.com/en-us/realtime/current/gameplay/web-extensions/webhooks#options
        /// 2. an error sent from a custom server plugin via PluginHost.BroadcastErrorInfoEvent, see example here:
        /// https://doc.photonengine.com/en-us/server/current/plugins/manual#handling_http_response
        /// 3. an error sent from the server, for example, when the limit of cached events has been exceeded in the room
        /// (all clients will be disconnected and the room will be closed in this case)
        /// read more here: https://doc.photonengine.com/en-us/realtime/current/gameplay/cached-events#special_considerations
        /// </remarks>
        /// <param name="errorInfo">object containing information about the error</param>
        public static UniTask<ErrorInfo> OnErrorInfoAsync()
        {
            return GetBridge().OnErrorInfoAsync;
        }


        #region Bridge

        private static PunCallbacksBridge _instance;

        private static PunCallbacksBridge GetBridge()
        {
            if (_instance != null) return _instance;

            var gameObject = new GameObject {name = "Pun2TaskCallback"};
            Object.DontDestroyOnLoad(gameObject);
            _instance = gameObject.AddComponent<PunCallbacksBridge>();
            return _instance;
        }

        #endregion
    }
}
#endif