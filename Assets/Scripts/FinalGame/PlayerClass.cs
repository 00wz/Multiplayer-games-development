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
    private int Health = 5;

    [SerializeField]
    private Transform vfxBlood;

    [SerializeField]
    private AudioClip[] BodyHitAudioClips;
    [SerializeField]
    private AudioClip[] DeathAudioClips;
    [SerializeField]
    [Range(0, 1)] public float AudioVolume = 0.5f;

    public event Action<ControllerColliderHit> OnCollision;
    private PlayerController _controller;
    private PhotonView _photonView;

    public event Action OnDie;
    public event Action<int> OnHPChange;
    public Action OnKillOther;

    public int GetHealth()
    {
        return Health;
    }
    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void OnDisable()
    {
        //Debug.LogWarning("TearApart!!!!!!!!!!!");
        TearApart();
    }
    private void TearApart()
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

    public void TakeDamage(int value,Photon.Realtime.Player attacker,int attackerViewId)
    {
        PlayBodyHitSound();
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        Health = Health - value;
        if (Health <= 0)
        {
            _photonView.RPC(nameof(KillerClientOnly), attacker, _photonView.Owner,attackerViewId);
            _photonView.RPC(nameof(DeathPlayer), RpcTarget.All);
        }
        else
        {
        _photonView.RPC(nameof(SynchronizeHealth), RpcTarget.All,Health);
        }
    }


    [PunRPC]
    private void KillerClientOnly(Photon.Realtime.Player killed,int attackerViewId)
    {
        //Debug.LogWarning((OnKillOther==null)+"\n" +
        //    (OnHPChange==null));

        if(PhotonView.Find(attackerViewId).TryGetComponent<PlayerClass>(out PlayerClass playerClass))
        {
            playerClass.OnKillOther?.Invoke();
        }
        Debug.Log("you killed a player: " + killed.NickName);
    }

    [PunRPC]
    private void DeathPlayer()
    {
        PlayDeathSound();
        OnDie?.Invoke();
        if (_photonView.AmOwner)
        {

            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    private void SynchronizeHealth(int newHPValue)
    {
        Health = newHPValue;
        OnHPChange?.Invoke(Health);
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(SynchronizeHealth), newPlayer, Health);
        }
    }

    private void PlayBodyHitSound()
    {
                var index = UnityEngine.Random.Range(0, BodyHitAudioClips.Length);
                AudioSource.PlayClipAtPoint(BodyHitAudioClips[index], transform.position, AudioVolume);
    }

    private void PlayDeathSound()
    {
                var index = UnityEngine.Random.Range(0, DeathAudioClips.Length);
                AudioSource.PlayClipAtPoint(DeathAudioClips[index], transform.position, AudioVolume);
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
