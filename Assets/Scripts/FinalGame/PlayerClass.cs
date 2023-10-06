using Photon.Pun;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerClass : MonoBehaviour,IDamageable
{
    [SerializeField]
    public GameObject PlayerCameraRoot;

    [SerializeField]
    private PlayerController ControllerPrefab;

    public event Action<ControllerColliderHit> OnCollision;
    private PlayerController _controller;
    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
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
        _photonView.RPC("TakeDamageRPC", RpcTarget.All,value, attacker);
    }

    [PunRPC]
    private void TakeDamageRPC(int value, Photon.Realtime.Player attacker)
    {
        
    }
}
