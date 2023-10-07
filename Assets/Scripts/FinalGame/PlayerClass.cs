using Photon.Pun;
using Photon.Realtime;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerClass : MonoBehaviour,IDamageable, IInRoomCallbacks, IOnPhotonViewPreNetDestroy
{
    [SerializeField]
    public GameObject PlayerCameraRoot;

    [SerializeField]
    private PlayerController ControllerPrefab;

    [SerializeField]
    private int Health = 5;

    [SerializeField]
    private Transform vfxBlood;

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

    public void OnPreNetDestroy(PhotonView rootView)
    {
        OnDeath();
    }
    private void OnDeath()
    {
        var children = transform.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            Instantiate(vfxBlood, child.position, Quaternion.identity);
        }
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
        if (Health <= 0)
        {
            _photonView.RPC(nameof(KillOther), attacker, _photonView.Owner);
            _photonView.RPC(nameof(DeathPlayer), _photonView.Owner);
        }
        else
        {
        _photonView.RPC(nameof(SynchronizeHealth), RpcTarget.All,Health);
        }
    }

    [PunRPC]
    private void KillOther(Photon.Realtime.Player killed)
    {
        Debug.Log("you killed a player: " + killed.NickName);
    }

    [PunRPC]
    private void DeathPlayer()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    private void SynchronizeHealth(int newHPValue)
    {
        Health = newHPValue;
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(SynchronizeHealth), newPlayer, Health);
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
