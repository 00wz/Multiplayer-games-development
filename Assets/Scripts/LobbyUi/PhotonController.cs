using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhotonController : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks
{
    [SerializeField]
    private ServerSettings _serverSettings;

    [SerializeField]
    private TMP_Text _stateUiText;

    [SerializeField]
    private Canvas _canvas;

    private LobbyController _lobbyController;
    private NewRoomFieldController _newRoomFieldController;

    private LoadBalancingClient _lbc;

    private TypedLobby _defaultLobby = new TypedLobby("defaultLobby", LobbyType.Default);

    private void Start()
    {
        _lbc = new LoadBalancingClient();
        _lbc.AddCallbackTarget(this);

        _lbc.ConnectUsingSettings(_serverSettings.AppSettings);

        _lobbyController = new LobbyController(_lbc, JoinRoom,_canvas);
        _newRoomFieldController = new NewRoomFieldController(CreateRoom,_canvas);
    }

    private void OnDestroy()
    {
        _lbc.RemoveCallbackTarget(this);
    }

    private void Update()
    {
        if (_lbc == null)
            return;

        _lbc.Service();

        var state = _lbc.State.ToString();
        _stateUiText.text = state;
    }

    public void OnConnected()
    {
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        _lbc.OpJoinLobby(_defaultLobby);
        _newRoomFieldController.enable = true;
    }

    private void JoinRoom(string roomName)
    {
        _lbc.OpJoinRoom(new EnterRoomParams() { RoomName = roomName });
    }

    private void CreateRoom(string roomName)
    {
        var enterRoomParams = new EnterRoomParams()
        {
            RoomName = roomName,
            Lobby = _defaultLobby
        };
        _lbc.OpCreateRoom(enterRoomParams);
    }

    public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"OnCreateRoomFailed\n{message}");
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnDisconnected(DisconnectCause cause)
    {
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
    }

    public void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        _lobbyController.enable = true;
    }


    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        _lobbyController.enable = false;
        _newRoomFieldController.enable = false;
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"OnJoinRoomFailed\n{message}");
    }

    public void OnLeftLobby()
    {
        _lobbyController.enable = false;
    }

    public void OnLeftRoom()
    {
        _newRoomFieldController.enable = true;
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");
    }
}

