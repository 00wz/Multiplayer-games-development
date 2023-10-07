using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System;
using Photon.Pun;

public class LobbyController2 : ILobbyCallbacks, IDisposable
{
    private Dictionary<RoomInfo, RoomView> cachedRoomList = new Dictionary<RoomInfo, RoomView>();
    private LoadBalancingClient _lbc;
    private LobbyView2 _lobbyView;
    private RoomView _roomViewPrefab;
    private Action<string> _onJoinCullback;
    
    public LobbyController2(LoadBalancingClient lbc, Action<string> onJoin, Canvas canvas)
    {
        _lbc = lbc;
        _lobbyView = GameObject.Instantiate<LobbyView2>(Resources.Load<LobbyView2>("LobbyView2"), canvas.transform);
        _roomViewPrefab = Resources.Load<RoomView>("Room");
        _onJoinCullback = onJoin;
        _lbc.AddCallbackTarget(this);
        _lobbyView.IsLoading = !PhotonNetwork.InLobby;
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        ClearList();
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (!info.RemovedFromList)
            {
                var room = GameObject.Instantiate(_roomViewPrefab, _lobbyView.RoomList.transform);
                room.JoinButton.onClick.AddListener(() => _onJoinCullback(info.Name));
                room.RoomName.text = info.Name;
                cachedRoomList[info] = room;
            }
        }
    }

    private void ClearList()
    {
        foreach (RoomView rv in cachedRoomList.Values)
        {
            GameObject.Destroy(rv.gameObject);
        }
        cachedRoomList.Clear();

        /*var rooms=RoomList.GetComponentsInChildren<RoomView>();
        int length = rooms.Length;
        for (int i = 0; i < length; i++)
            GameObject.Destroy(rooms[i].gameObject);*/
    }


    void ILobbyCallbacks.OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }


    public void OnJoinedLobby()
    {
        _lobbyView.IsLoading = false;
    }

    public void OnLeftLobby()
    {
        _lobbyView.IsLoading = true;
    }

    public void Dispose()
    {
        if (_lobbyView != null) 
        {
            GameObject.Destroy(_lobbyView.gameObject); 
        }
        _lbc.RemoveCallbackTarget(this);
    }
}
