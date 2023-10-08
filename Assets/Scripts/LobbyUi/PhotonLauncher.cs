using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private ServerSettings _serverSettings;

    [SerializeField]
    private TMP_Text _stateUiText;

    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private string SceneNameLoad = "FirstScene";

    private LobbyController _lobbyController;
    private NewRoomFieldController _newRoomFieldController;

    //private LoadBalancingClient _lbc;

    private TypedLobby _defaultLobby = new TypedLobby("defaultLobby", LobbyType.Default);

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Start()
    {
        //_lbc = new LoadBalancingClient();
        //_lbc.AddCallbackTarget(this);
        //_lbc.StateChanged += OnStateChanged;
        PhotonNetwork.NetworkingClient.StateChanged += OnStateChanged;

        //_lbc.ConnectUsingSettings(_serverSettings.AppSettings);
        Connect();

        //_lobbyController = new LobbyController(_lbc, JoinRoom,_canvas);
        _lobbyController = new LobbyController(PhotonNetwork.NetworkingClient, JoinRoom,_canvas);
        ///_newRoomFieldController = new NewRoomFieldController(CreateRoom,_canvas);
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
            return;

        PhotonNetwork.ConnectUsingSettings(_serverSettings.AppSettings);
        PhotonNetwork.GameVersion = Application.version;

    }

    private void OnDestroy()
    {
        ///_lbc.RemoveCallbackTarget(this);
        PhotonNetwork.NetworkingClient.StateChanged -= OnStateChanged;
    }
    /*
    private void Update()
    {
        if (_lbc == null)
            return;

        _lbc.Service();
    }
    */
    private void OnStateChanged(ClientState arg1, ClientState arg2)
    {
        //var state = _lbc.State.ToString();
        _stateUiText.text = arg2.ToString();
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        //_lbc.OpJoinLobby(_defaultLobby);
        JoinLobby(_defaultLobby);
        _newRoomFieldController.enable = true;
    }

    private void JoinLobby(TypedLobby typedLobby)
    {
        if (PhotonNetwork.InLobby && typedLobby == PhotonNetwork.CurrentLobby)
        {
            _lobbyController.enable = true;
            return;
        }

        PhotonNetwork.NetworkingClient.OpJoinLobby(typedLobby);
    }

    private void JoinRoom(string roomName)
    {
        //_lbc.OpJoinRoom(new EnterRoomParams() { RoomName = roomName });
        PhotonNetwork.NetworkingClient.OpJoinRoom(new EnterRoomParams() { RoomName = roomName });
    }

    private void CreateRoom(string roomName)
    {
        var enterRoomParams = new EnterRoomParams()
        {
            RoomName = roomName,
            Lobby = _defaultLobby
        };
        //_lbc.OpCreateRoom(enterRoomParams);
        PhotonNetwork.NetworkingClient.OpCreateRoom(enterRoomParams);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"OnCreateRoomFailed\n{message}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Connect();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby");
        _lobbyController.enable = true;
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        _lobbyController.enable = false;
        _newRoomFieldController.enable = false;

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1' ");

            // #Critical
            // Load the Room Level. 
            PhotonNetwork.LoadLevel(SceneNameLoad);

        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"OnJoinRoomFailed\n{message}");
    }

    public override void OnLeftLobby()
    {
        _lobbyController.enable = false;
    }

    public override void OnLeftRoom()
    {
        _newRoomFieldController.enable = true;
        JoinLobby(_defaultLobby);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");
    }
}

