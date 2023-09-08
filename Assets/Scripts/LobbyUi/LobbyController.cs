using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System;

public class LobbyController : ILobbyCallbacks,IDisposable
{
    private Dictionary<RoomInfo, RoomView> cachedRoomList = new Dictionary<RoomInfo,RoomView>();
    private LoadBalancingClient _lbc;
    private LobbyView _lobbyView;
    private RoomView _roomViewPrefab;
    private Action<string> _onJoinCullback;
    public bool enable
    {
        set
        {
            if (value)
            {
                _lobbyView.gameObject.SetActive(true);
                ClearList();
                _lbc.AddCallbackTarget(this);
            }
            else
            {
                _lobbyView.gameObject.SetActive(false);
                ClearList();
                _lbc.RemoveCallbackTarget(this);
            }
        }
    }
    public LobbyController(LoadBalancingClient lbc,Action<string> onJoin,Canvas canvas)
    {
        _lbc = lbc;
        _lobbyView=GameObject.Instantiate<LobbyView>(Resources.Load<LobbyView>("LobbyView"),canvas.transform);
        _roomViewPrefab = Resources.Load<RoomView>("Room");
        _onJoinCullback = onJoin;
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                GameObject.Destroy(cachedRoomList[info].gameObject);
                cachedRoomList.Remove(info);
            }
            else
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
        foreach(RoomView rv in cachedRoomList.Values)
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
    }

    public void OnLeftLobby()
    {
    }

    public void Dispose()
    {
        GameObject.Destroy(_lobbyView);
        _onJoinCullback = null;
    }
}
