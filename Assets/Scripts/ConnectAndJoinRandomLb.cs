using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConnectAndJoinRandomLb : MonoBehaviour,IConnectionCallbacks, IMatchmakingCallbacks,ILobbyCallbacks
{
    [SerializeField]
    private ServerSettings _serverSettings;

    [SerializeField]
    private TMP_Text _stateUiText;

    private LoadBalancingClient _lbc;

    private void Start()
    {
        _lbc = new LoadBalancingClient();
        _lbc.AddCallbackTarget(this);

        _lbc.ConnectUsingSettings(_serverSettings.AppSettings);
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
        _lbc.OpJoinRandomRoom();
    }

    public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
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
    }

    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");
        _lbc.OpCreateRoom(new EnterRoomParams());
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
    }

    public void OnLeftLobby()
    {
    }

    public void OnLeftRoom()
    {
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
    }
}
