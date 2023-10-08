using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PhotonLauncher2 : MonoBehaviourPunCallbacks,IDisposable
{
    [SerializeField]
    private ServerSettings _serverSettings;

    [SerializeField]
    private TMP_Text _stateUiText;

    [SerializeField]
    private GameObject MenuRoot;

    [SerializeField]
    private string SceneNameLoad = "FirstScene";

    [SerializeField]
    private GameObject LoadWindow;

    private LobbyController2 _lobbyController;
    private NewRoomFieldController _newRoomFieldController;

    //private LoadBalancingClient _lbc;

    private TypedLobby _defaultLobby = new TypedLobby("defaultLobby", LobbyType.Default);

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        PhotonNetwork.NetworkingClient.StateChanged += OnStateChanged;
        
        Connect();

        _lobbyController = new LobbyController2(PhotonNetwork.NetworkingClient, JoinRoom, MenuRoot);
        _newRoomFieldController = new NewRoomFieldController(CreateRoom, MenuRoot);
        _newRoomFieldController.enable = true;
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            //LoadWindow.SetActive(false);
            return;
        }
        LoadWindow.SetActive(true);

        PhotonNetwork.ConnectUsingSettings(_serverSettings.AppSettings);
        PhotonNetwork.GameVersion = Application.version;

    }

    private void OnDestroy()
    {
        Dispose();
    }

    private void OnStateChanged(ClientState arg1, ClientState arg2)
    {
        //var state = _lbc.State.ToString();
        _stateUiText.text = arg2.ToString();
    }


    public override void OnConnectedToMaster()
    {
        LoadWindow.SetActive(false);
        JoinLobby(_defaultLobby);
    }

    private void JoinLobby(TypedLobby typedLobby)
    {
        if (PhotonNetwork.InLobby && typedLobby == PhotonNetwork.CurrentLobby)
        {
            return;
        }

        PhotonNetwork.NetworkingClient.OpJoinLobby(typedLobby);
    }

    private void JoinRoom(string roomName)
    {
        LoadWindow.SetActive(true);
        PhotonNetwork.NetworkingClient.OpJoinRoom(new EnterRoomParams() { RoomName = roomName });
    }

    private void CreateRoom(string roomName)
    {
        LoadWindow.SetActive(true);
        var enterRoomParams = new EnterRoomParams()
        {
            RoomName = roomName,
            Lobby = _defaultLobby
        };
        PhotonNetwork.NetworkingClient.OpCreateRoom(enterRoomParams);
    }

    public override void OnCreatedRoom()
    {
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        LoadWindow.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        LoadWindow.SetActive(true);
        Connect();
    }

    public override void OnJoinedLobby()
    {
    }


    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            // #Critical
            // Load the Room Level. 
            PhotonNetwork.LoadLevel(SceneNameLoad);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        LoadWindow.SetActive(false);
    }

    public override void OnLeftLobby()
    {
    }

    public override void OnLeftRoom()
    {
        //JoinLobby(_defaultLobby);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
    }

    public void Dispose()
    {
        _lobbyController.Dispose();
        PhotonNetwork.NetworkingClient.StateChanged -= OnStateChanged;
        _newRoomFieldController.Dispose();
    }
}