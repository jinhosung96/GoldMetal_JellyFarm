#if PUN_TO_UNITASK_SUPPORT

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

namespace Pun2Task
{
    public static class Pun2TaskNetwork
    {
        #region ServerConnection

        /// <summary>Connect to Photon as configured in the PhotonServerSettings file.</summary>
        /// <remarks>
        /// Implement IConnectionCallbacks, to make your game logic aware of state changes.
        /// Especially, IConnectionCallbacks.ConnectedToMasterServer is useful to react when
        /// the client can do matchmaking.
        ///
        /// This method will disable OfflineMode (which won't destroy any instantiated GOs) and it
        /// will set IsMessageQueueRunning to true.
        ///
        /// Your Photon configuration is created by the PUN Wizard and contains the AppId,
        /// region for Photon Cloud games, the server address among other things.
        ///
        /// To ignore the settings file, set the relevant values and connect by calling
        /// ConnectToMaster, ConnectToRegion.
        ///
        /// To connect to the Photon Cloud, a valid AppId must be in the settings file (shown in the Photon Cloud Dashboard).
        /// https://dashboard.photonengine.com
        ///
        /// Connecting to the Photon Cloud might fail due to:
        /// - Invalid AppId
        /// - Network issues
        /// - Invalid region
        /// - Subscription CCU limit reached
        /// - etc.
        ///
        /// In general check out the <see cref="DisconnectCause"/> from the <see cref="IConnectionCallbacks.OnDisconnected"/> callback.
        ///  </remarks>
        public static async UniTask ConnectUsingSettingsAsync(CancellationToken token = default)
        {
            if (PhotonNetwork.IsConnected) return;

            var task = UniTask.WhenAny(
                Pun2TaskCallback.OnConnectedToMasterAsync().AsAsyncUnitUniTask(),
                Pun2TaskCallback.OnDisconnectedAsync());

            PhotonNetwork.ConnectUsingSettings();

            var (winIndex, _, disconnectCause) = await task.AttachExternalCancellation(token);

            if (winIndex == 0) return;
            throw new ConnectionFailedException(disconnectCause);
        }

        public static async UniTask ConnectUsingSettingsAsync(
            AppSettings appSettings,
            bool startInOfflineMode = false,
            CancellationToken token = default)
        {
            var task = UniTask.WhenAny(
                Pun2TaskCallback.OnConnectedToMasterAsync().AsAsyncUnitUniTask(),
                Pun2TaskCallback.OnDisconnectedAsync());

            PhotonNetwork.ConnectUsingSettings(appSettings, startInOfflineMode);

            var (winIndex, _, disconnectCause) = await task.AttachExternalCancellation(token);

            if (winIndex == 0) return;
            throw new ConnectionFailedException(disconnectCause);
        }

        public static async UniTask ConnectToMasterAsync(
            string masterServerAddress,
            int port,
            string appID,
            CancellationToken token = default)
        {
            if (PhotonNetwork.IsConnected) return;

            var task = UniTask.WhenAny(
                Pun2TaskCallback.OnConnectedToMasterAsync().AsAsyncUnitUniTask(),
                Pun2TaskCallback.OnDisconnectedAsync());

            PhotonNetwork.ConnectToMaster(masterServerAddress, port, appID);

            var (winIndex, _, disconnectCause) = await task.AttachExternalCancellation(token);

            if (winIndex == 0) return;
            throw new ConnectionFailedException(disconnectCause);
        }

        /// <summary>
        /// Connect to the Photon Cloud region with the lowest ping (on platforms that support Unity's Ping).
        /// </summary>
        /// <remarks>
        /// Will save the result of pinging all cloud servers in PlayerPrefs. Calling this the first time can take +-2 seconds.
        /// The ping result can be overridden via PhotonNetwork.OverrideBestCloudServer(..)
        /// This call can take up to 2 seconds if it is the first time you are using this, all cloud servers will be pinged to check for the best region.
        ///
        /// The PUN Setup Wizard stores your appID in a settings file and applies a server address/port.
        /// To connect to the Photon Cloud, a valid AppId must be in the settings file (shown in the Photon Cloud Dashboard).
        /// https://dashboard.photonengine.com
        ///
        /// Connecting to the Photon Cloud might fail due to:
        /// - Invalid AppId
        /// - Network issues
        /// - Invalid region
        /// - Subscription CCU limit reached
        /// - etc.
        ///
        /// In general check out the <see cref="DisconnectCause"/> from the <see cref="IConnectionCallbacks.OnDisconnected"/> callback.
        /// </remarks>
        /// <returns>If this client is going to connect to cloud server based on ping. Even if true, this does not guarantee a connection but the attempt is being made.</returns>
        public static async UniTask ConnectToBestCloudServerAsync(CancellationToken token = default)
        {
            if (PhotonNetwork.IsConnected) return;

            var task = UniTask.WhenAny(
                Pun2TaskCallback.OnConnectedToMasterAsync().AsAsyncUnitUniTask(),
                Pun2TaskCallback.OnDisconnectedAsync());

            PhotonNetwork.ConnectToBestCloudServer();

            var (winIndex, _, disconnectCause) = await task.AttachExternalCancellation(token);

            if (winIndex == 0) return;
            throw new ConnectionFailedException(disconnectCause);
        }

        /// <summary>
        /// Connects to the Photon Cloud region of choice.
        /// </summary>
        /// <remarks>
        /// It's typically enough to define the region code ("eu", "us", etc).
        /// Connecting to a specific cluster may be necessary, when regions get sharded and you support friends / invites.
        ///
        /// In all other cases, you should not define a cluster as this allows the Name Server to distribute
        /// clients as needed. A random, load balanced cluster will be selected.
        ///
        /// The Name Server has the final say to assign a cluster as available.
        /// If the requested cluster is not available another will be assigned.
        ///
        /// Once connected, check the value of CurrentCluster.
        /// </remarks>
        public static async UniTask ConnectToRegionAsync(string region, CancellationToken token = default)
        {
            if (PhotonNetwork.IsConnected) return;

            var task = UniTask.WhenAny(
                Pun2TaskCallback.OnConnectedToMasterAsync().AsAsyncUnitUniTask(),
                Pun2TaskCallback.OnDisconnectedAsync());

            PhotonNetwork.ConnectToRegion(region);

            var (winIndex, _, disconnectCause) = await task.AttachExternalCancellation(token);

            if (winIndex == 0) return;
            throw new ConnectionFailedException(disconnectCause);
        }

        /// <summary>
        /// Makes this client disconnect from the photon server, a process that leaves any room and calls OnDisconnected on completion.
        /// </summary>
        /// <remarks>
        /// When you disconnect, the client will send a "disconnecting" message to the server. This speeds up leave/disconnect
        /// messages for players in the same room as you (otherwise the server would timeout this client's connection).
        /// When used in OfflineMode, the state-change and event-call OnDisconnected are immediate.
        /// Offline mode is set to false as well.
        /// Once disconnected, the client can connect again. Use ConnectUsingSettings.
        /// </remarks>
        public static async UniTask DisconnectAsync()
        {
            if (PhotonNetwork.NetworkClientState == ClientState.Disconnected) return;
            PhotonNetwork.Disconnect();
            await Pun2TaskCallback.OnDisconnectedAsync();
        }

        /// <summary>Can be used to reconnect to the master server after a disconnect.</summary>
        /// <remarks>
        /// After losing connection, you can use this to connect a client to the region Master Server again.
        /// Cache the room name you're in and use RejoinRoom(roomname) to return to a game.
        /// Common use case: Press the Lock Button on a iOS device and you get disconnected immediately.
        /// </remarks>
        public static async UniTask ReconnectAsync(CancellationToken token = default)
        {
            if (PhotonNetwork.IsConnected) return;

            var task = UniTask.WhenAny(
                Pun2TaskCallback.OnConnectedToMasterAsync().AsAsyncUnitUniTask(),
                Pun2TaskCallback.OnDisconnectedAsync());

            PhotonNetwork.Reconnect();

            var (winIndex, _, disconnectCause) = await task.AttachExternalCancellation(token);

            if (winIndex == 0) return;
            throw new ConnectionFailedException(disconnectCause);
        }

        #endregion

        #region RoomConnection

        /// <summary>
        /// Creates a new room. Will callback: OnCreatedRoom and OnJoinedRoom or OnCreateRoomFailed.
        /// </summary>
        /// <remarks>
        /// When successful, this calls the callbacks OnCreatedRoom and OnJoinedRoom (the latter, cause you join as first player).
        /// In all error cases, OnCreateRoomFailed gets called.
        ///
        /// Creating a room will fail if the room name is already in use or when the RoomOptions clashing
        /// with one another. Check the EnterRoomParams reference for the various room creation options.
        ///
        /// If you don't want to create a unique room-name, pass null or "" as name and the server will assign a roomName (a GUID as string).
        ///
        /// This method can only be called while the client is connected to a Master Server so you should
        /// implement the callback OnConnectedToMaster.
        /// Check the return value to make sure the operation will be called on the server.
        /// Note: There will be no callbacks if this method returned false.
        ///
        /// More about PUN matchmaking:
        /// https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        /// <param name="roomName">Unique name of the room to create. Pass null or "" to make the server generate a name.</param>
        /// <param name="roomOptions">Common options for the room like MaxPlayers, initial custom room properties and similar. See RoomOptions type..</param>
        /// <param name="typedLobby">If null, the room is automatically created in the currently used lobby (which is "default" when you didn't join one explicitly).</param>
        /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
        public static async UniTask CreateRoomAsync(
            string roomName,
            RoomOptions roomOptions = null,
            TypedLobby typedLobby = null,
            string[] expectedUsers = null,
            CancellationToken token = default)
        {
            var task = UniTask.WhenAny(
                Pun2TaskCallback.OnJoinedRoomAsync().AsAsyncUnitUniTask(),
                Pun2TaskCallback.OnCreateRoomFailedAsync());

            var valid = PhotonNetwork.CreateRoom(roomName, roomOptions, typedLobby, expectedUsers);
            if (!valid) throw new InvalidRoomOperationException("It is not ready to create a room.");

            var (winIndex, _, (returnCode, message)) = await task.AttachExternalCancellation(token);
            if (winIndex == 0) return;
            throw new FailedToCreateRoomException(returnCode, message);
        }


        /// <summary>
        /// Joins a specific room by name and creates it on demand. Will callback: OnJoinedRoom or OnJoinRoomFailed.
        /// </summary>
        /// <remarks>
        /// Useful when players make up a room name to meet in:
        /// All involved clients call the same method and whoever is first, also creates the room.
        ///
        /// When successful, the client will enter the specified room.
        /// The client which creates the room, will callback both OnCreatedRoom and OnJoinedRoom.
        /// Clients that join an existing room will only callback OnJoinedRoom.
        /// In all error cases, OnJoinRoomFailed gets called.
        ///
        /// Joining a room will fail, if the room is full, closed or when the user
        /// already is present in the room (checked by userId).
        ///
        /// To return to a room, use OpRejoinRoom.
        ///
        /// This method can only be called while the client is connected to a Master Server so you should
        /// implement the callback OnConnectedToMaster.
        /// Check the return value to make sure the operation will be called on the server.
        /// Note: There will be no callbacks if this method returned false.
        ///
        ///
        /// If you set room properties in roomOptions, they get ignored when the room is existing already.
        /// This avoids changing the room properties by late joining players.
        ///
        /// You can define an array of expectedUsers, to block player slots in the room for these users.
        /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
        ///
        ///
        /// More about PUN matchmaking:
        /// https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        /// <param name="roomName">Name of the room to join. Must be non null.</param>
        /// <param name="roomOptions">Options for the room, in case it does not exist yet. Else these values are ignored.</param>
        /// <param name="typedLobby">Lobby you want a new room to be listed in. Ignored if the room was existing and got joined.</param>
        /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
        /// <returns>True will be returned when you are the first user.</returns>>
        public static async UniTask<bool> JoinOrCreateRoomAsync(
            string roomName,
            RoomOptions roomOptions,
            TypedLobby typedLobby,
            string[] expectedUsers = null,
            CancellationToken token = default)
        {
            var createdRoomTask = Pun2TaskCallback.OnCreatedRoomAsync().GetAwaiter();
            var task = UniTask.WhenAny(
                Pun2TaskCallback.OnJoinedRoomAsync().AsAsyncUnitUniTask(),
                Pun2TaskCallback.OnCreateRoomFailedAsync(),
                Pun2TaskCallback.OnJoinRoomFailedAsync());

            var valid = PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby, expectedUsers);
            if (!valid) throw new InvalidRoomOperationException("It is not ready to join a room.");

            var (winIndex,
                _,
                (createFailedCode, createFailedMessage),
                (joinFailedCode, joinFailedMessage)) = await task.AttachExternalCancellation(token);
            if (winIndex == 0) return createdRoomTask.IsCompleted;

            if (winIndex == 1)
            {
                throw new FailedToCreateRoomException(createFailedCode, createFailedMessage);
            }
            else
            {
                throw new FailedToJoinRoomException(createFailedCode, createFailedMessage);
            }
        }

        /// <summary>
        /// Joins a room by name. Will callback: OnJoinedRoom or OnJoinRoomFailed.
        /// </summary>
        /// <remarks>
        /// Useful when using lobbies or when players follow friends or invite each other.
        ///
        /// When successful, the client will enter the specified room and callback via OnJoinedRoom.
        /// In all error cases, OnJoinRoomFailed gets called.
        ///
        /// Joining a room will fail if the room is full, closed, not existing or when the user
        /// already is present in the room (checked by userId).
        ///
        /// To return to a room, use OpRejoinRoom.
        /// When players invite each other and it's unclear who's first to respond, use OpJoinOrCreateRoom instead.
        ///
        /// This method can only be called while the client is connected to a Master Server so you should
        /// implement the callback OnConnectedToMaster.
        /// Check the return value to make sure the operation will be called on the server.
        /// Note: There will be no callbacks if this method returned false.
        ///
        ///
        /// More about PUN matchmaking:
        /// https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        /// <see cref="OnJoinRoomFailed"/>
        /// <see cref="OnJoinedRoom"/>
        /// <param name="roomName">Unique name of the room to join.</param>
        /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
        public static async UniTask JoinRoomAsync(string roomName, string[] expectedUsers = null,
            CancellationToken token = default)
        {
            var task = UniTask.WhenAny(
                Pun2TaskCallback.OnJoinedRoomAsync().AsAsyncUnitUniTask(),
                Pun2TaskCallback.OnJoinRoomFailedAsync());

            var valid = PhotonNetwork.JoinRoom(roomName, expectedUsers);
            if (!valid) throw new InvalidRoomOperationException("It is not ready to join a room.");

            var (winIndex, _, (returnCode, message)) = await task.AttachExternalCancellation(token);
            if (winIndex == 0) return;
            throw new FailedToJoinRoomException(returnCode, message);
        }

        /// <summary>
        /// Joins a random room that matches the filter. Will callback: OnJoinedRoom or OnJoinRandomFailed.
        /// </summary>
        /// <remarks>
        /// Used for random matchmaking. You can join any room or one with specific properties defined in opJoinRandomRoomParams.
        /// 
        /// This operation fails if no rooms are fitting or available (all full, closed, in another lobby or not visible).
        /// It may also fail when actually joining the room which was found. Rooms may close, become full or empty anytime.
        /// 
        /// This method can only be called while the client is connected to a Master Server so you should
        /// implement the callback OnConnectedToMaster.
        /// Check the return value to make sure the operation will be called on the server.
        /// Note: There will be no callbacks if this method returned false.
        /// 
        /// More about PUN matchmaking:
        /// https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        /// <param name="expectedCustomRoomProperties">Filters for rooms that match these custom properties (string keys and values). To ignore, pass null.</param>
        /// <param name="expectedMaxPlayers">Filters for a particular maxplayer setting. Use 0 to accept any maxPlayer value.</param>
        /// <param name="matchingType">Selects one of the available matchmaking algorithms. See MatchmakingMode enum for options.</param>
        /// <param name="typedLobby">The lobby in which you want to lookup a room. Pass null, to use the default lobby. This does not join that lobby and neither sets the lobby property.</param>
        /// <param name="sqlLobbyFilter">A filter-string for SQL-typed lobbies.</param>
        /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
        /// <returns>If the operation got queued and will be sent.</returns>
        public static async UniTask JoinRandomRoomAsync(
            Hashtable expectedCustomRoomProperties,
            byte expectedMaxPlayers,
            MatchmakingMode matchingType,
            TypedLobby typedLobby,
            string sqlLobbyFilter,
            string[] expectedUsers = null,
            CancellationToken token = default)
        {
            var task = UniTask.WhenAny(
                Pun2TaskCallback.OnJoinedRoomAsync().AsAsyncUnitUniTask(),
                Pun2TaskCallback.OnJoinRandomFailedAsync());

            var valid = PhotonNetwork.JoinRandomRoom(
                expectedCustomRoomProperties,
                expectedMaxPlayers,
                matchingType,
                typedLobby,
                sqlLobbyFilter,
                expectedUsers);
            if (!valid) throw new InvalidRoomOperationException("It is not ready to join a room.");

            var (winIndex, _, (returnCode, message)) = await task.AttachExternalCancellation(token);
            if (winIndex == 0) return;
            throw new FailedToJoinRoomException(returnCode, message);
        }

        /// <summary>
        /// Rejoins a room by roomName (using the userID internally to return).  Will callback: OnJoinedRoom or OnJoinRoomFailed.
        /// </summary>
        /// <remarks>
        /// After losing connection, you might be able to return to a room and continue playing,
        /// if the client is reconnecting fast enough. Use Reconnect() and this method.
        /// Cache the room name you're in and use RejoinRoom(roomname) to return to a game.
        ///
        /// Note: To be able to Rejoin any room, you need to use UserIDs!
        /// You also need to set RoomOptions.PlayerTtl.
        ///
        /// <b>Important: Instantiate() and use of RPCs is not yet supported.</b>
        /// The ownership rules of PhotonViews prevent a seamless return to a game, if you use PhotonViews.
        /// Use Custom Properties and RaiseEvent with event caching instead.
        ///
        /// Common use case: Press the Lock Button on a iOS device and you get disconnected immediately.
        ///
        /// Rejoining room will not send any player properties. Instead client will receive up-to-date ones from server.
        /// If you want to set new player properties, do it once rejoined.
        public static async UniTask RejoinRoomAsync(string roomName, CancellationToken token = default)
        {
            var task = UniTask.WhenAny(
                Pun2TaskCallback.OnJoinedRoomAsync().AsAsyncUnitUniTask(),
                Pun2TaskCallback.OnJoinRoomFailedAsync());

            var valid = PhotonNetwork.RejoinRoom(roomName);
            if (!valid) throw new InvalidRoomOperationException("It is not ready to join a room.");

            var (winIndex, _, (returnCode, message)) = await task.AttachExternalCancellation(token);
            if (winIndex == 0) return;
            throw new FailedToJoinRoomException(returnCode, message);
        }

        /// <summary>When the client lost connection during gameplay, this method attempts to reconnect and rejoin the room.</summary>
        /// <remarks>
        /// This method re-connects directly to the game server which was hosting the room PUN was in before.
        /// If the room was shut down in the meantime, PUN will call OnJoinRoomFailed and return this client to the Master Server.
        ///
        /// Check the return value, if this client will attempt a reconnect and rejoin (if the conditions are met).
        /// If ReconnectAndRejoin returns false, you can still attempt a Reconnect and Rejoin.
        ///
        /// Similar to PhotonNetwork.RejoinRoom, this requires you to use unique IDs per player (the UserID).
        ///
        /// Rejoining room will not send any player properties. Instead client will receive up-to-date ones from server.
        /// If you want to set new player properties, do it once rejoined.
        /// </remarks>
        /// <returns>False, if there is no known room or game server to return to. Then, this client does not attempt the ReconnectAndRejoin.</returns>
        public static async UniTask ReconnectAndRejoinAsync(CancellationToken token)
        {
            var task = UniTask.WhenAny(
                Pun2TaskCallback.OnJoinedRoomAsync().AsAsyncUnitUniTask(),
                Pun2TaskCallback.OnJoinRoomFailedAsync());

            var valid = PhotonNetwork.ReconnectAndRejoin();
            if (!valid) throw new InvalidRoomOperationException("It is not ready to join a room.");

            var (winIndex, _, (returnCode, message)) = await task.AttachExternalCancellation(token);
            if (winIndex == 0) return;
            throw new FailedToJoinRoomException(returnCode, message);
        }

        /// <summary>Leave the current room and return to the Master Server where you can join or create rooms (see remarks).</summary>
        /// <remarks>
        /// This will clean up all (network) GameObjects with a PhotonView, unless you changed autoCleanUp to false.
        /// Returns to the Master Server.
        ///
        /// In OfflineMode, the local "fake" room gets cleaned up and OnLeftRoom gets called immediately.
        ///
        /// In a room with playerTTL &lt; 0, LeaveRoom just turns a client inactive. The player stays in the room's player list
        /// and can return later on. Setting becomeInactive to false deliberately, means to "abandon" the room, despite the
        /// playerTTL allowing you to come back.
        ///
        /// In a room with playerTTL == 0, become inactive has no effect (clients are removed from the room right away).
        /// </remarks>
        /// <param name="becomeInactive">If this client becomes inactive in a room with playerTTL &lt; 0. Defaults to true.</param>
        public static async UniTask LeaveRoomAsync(bool becomeInactive = true)
        {
            if (!PhotonNetwork.InRoom) return;
            PhotonNetwork.LeaveRoom(becomeInactive);
            await Pun2TaskCallback.OnLeftRoomAsync();
        }

        /// <summary>On MasterServer this joins the default lobby which list rooms currently in use.</summary>
        /// <remarks>
        /// The room list is sent and refreshed by the server using <see cref="ILobbyCallbacks.OnRoomListUpdate"/>.
        ///
        /// Per room you should check if it's full or not before joining. Photon also lists rooms that are
        /// full, unless you close and hide them (room.open = false and room.visible = false).
        ///
        /// In best case, you make your clients join random games, as described here:
        /// https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        ///
        ///
        /// You can show your current players and room count without joining a lobby (but you must
        /// be on the master server). Use: CountOfPlayers, CountOfPlayersOnMaster, CountOfPlayersInRooms and
        /// CountOfRooms.
        ///
        /// You can use more than one lobby to keep the room lists shorter. See JoinLobby(TypedLobby lobby).
        /// When creating new rooms, they will be "attached" to the currently used lobby or the default lobby.
        ///
        /// You can use JoinRandomRoom without being in a lobby!
        /// </remarks>
        public static async UniTask JoinLobbyAsync(TypedLobby typedLobby = null, CancellationToken token = default)
        {
            PhotonNetwork.JoinLobby(typedLobby);
            await Pun2TaskCallback.OnJoinedLobbyAsync().AttachExternalCancellation(token);
        }

        /// <summary>Leave a lobby to stop getting updates about available rooms.</summary>
        /// <remarks>
        /// This does not reset PhotonNetwork.lobby! This allows you to join this particular lobby later
        /// easily.
        ///
        /// The values CountOfPlayers, CountOfPlayersOnMaster, CountOfPlayersInRooms and CountOfRooms
        /// are received even without being in a lobby.
        ///
        /// You can use JoinRandomRoom without being in a lobby.
        /// </remarks>
        public static async UniTask LeaveLobbyAsync(CancellationToken token = default)
        {
            if (PhotonNetwork.LeaveLobby())
            {
                await Pun2TaskCallback.OnLeftLobbyAsync().AttachExternalCancellation(token);
            }
        }

        #endregion

        /// <summary>Fetches a custom list of games from the server, matching a SQL-like "where" clause, then triggers OnRoomListUpdate callback.</summary>
        /// <remarks>
        /// Operation is only available for lobbies of type SqlLobby.
        /// This is an async request.
        ///
        /// Note: You don't have to join a lobby to query it. Rooms need to be "attached" to a lobby, which can be done
        /// via the typedLobby parameter in CreateRoom, JoinOrCreateRoom, etc..
        ///
        /// When done, OnRoomListUpdate gets called.
        /// </remarks>
        /// <see cref="https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby/#sql_lobby_type"/>
        /// <param name="typedLobby">The lobby to query. Has to be of type SqlLobby.</param>
        /// <param name="sqlLobbyFilter">The sql query statement.</param>
        public static async UniTask<IList<RoomInfo>> GetCustomRoomListAsync(TypedLobby typedLobby,
            string sqlLobbyFilter,
            CancellationToken token = default)
        {
            PhotonNetwork.GetCustomRoomList(typedLobby, sqlLobbyFilter);
            return await Pun2TaskCallback.OnRoomListUpdateAsync().AttachExternalCancellation(token);
        }

        #region Exceptions

        public class ConnectionFailedException : Pun2TaskException
        {
            public DisconnectCause DisconnectCause { get; }

            public ConnectionFailedException(DisconnectCause disconnectCause)
            {
                DisconnectCause = disconnectCause;
            }
        }

        public class InvalidRoomOperationException : Pun2TaskException
        {
            public InvalidRoomOperationException(string message) : base(message)
            {
            }
        }

        public class FailedToJoinRoomException : Pun2TaskException
        {
            public short ReturnCode { get; }

            public FailedToJoinRoomException(short returnCode, string message) : base(message)
            {
                ReturnCode = returnCode;
            }
        }

        public class FailedToCreateRoomException : Pun2TaskException
        {
            public short ReturnCode { get; }

            public FailedToCreateRoomException(short returnCode, string message) : base(message)
            {
                ReturnCode = returnCode;
            }
        }

        public class Pun2TaskException : Exception
        {
            public Pun2TaskException()
            {
            }

            public Pun2TaskException(string message) : base(message)
            {
            }
        }

        #endregion
    }
}
#endif