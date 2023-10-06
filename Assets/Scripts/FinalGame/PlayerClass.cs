using Photon.Pun;
using Photon.Realtime;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerClass : MonoBehaviour,IDamageable, IInRoomCallbacks
{
    [SerializeField]
    public GameObject PlayerCameraRoot;

    [SerializeField]
    private PlayerController ControllerPrefab;

    [SerializeField]
    private int Health = 100;

    public event Action<ControllerColliderHit> OnCollision;
    private PlayerController _controller;
    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void TakeControl()
    {
        _controller=GameObject.Instantiate(ControllerPrefab, transform);
        int layerLocalPlayer = LayerMask.NameToLayer("LocalPlayer");
        SetLayerAllChildren(transform, layerLocalPlayer);
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        OnCollision?.Invoke(hit);
    }

    private void SetLayerAllChildren(Transform root, int layer)
    {
        var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = layer;
        }
    }

    public void TakeDamage(int value,Photon.Realtime.Player attacker)
    {
        Health = Health - value;
        _photonView.RPC(nameof(SynchronizeHealth), RpcTarget.All,Health, gameObject.activeSelf);
    }

    [PunRPC]
    private void SynchronizeHealth(int newHPValue,bool isEnable)
    {
        Health = newHPValue;
        gameObject.SetActive(isEnable);
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(SynchronizeHealth), newPlayer, Health, gameObject.activeSelf);
        }
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
    }

    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
    }
}
