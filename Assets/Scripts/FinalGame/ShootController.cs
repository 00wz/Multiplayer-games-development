using Photon.Pun;
using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ShootController : MonoBehaviour
{
    [SerializeField]
    private LayerMask LayerMaskScreenRaycast;

    [SerializeField]
    private LayerMask LayerMaskBulletTarget;

    [SerializeField]
    public Transform FiringPosition;

    [SerializeField]
    public BulletProjectile BulletPrefab;

    [SerializeField]
    public BulletProjectile BulletBloodPrefab;

    [SerializeField]
    private AudioClip[] ShootAudioClips;
    [SerializeField]
    [Range(0, 1)] public float AudioVolume = 1f;

    [SerializeField]
    private float AnimationChangeTime = 0.2f;

    private const float RAYCAST_DISTANCE= 100f;
    private PhotonView _photonView;
    private float _bulletSpeed;
    private Animator _animator;
    private GunState _gunState;
    private float _animAimWaight;
    private float _animationChangeSpeed;

    enum GunState
    {
        Default,
        Aiming
    }

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _bulletSpeed=BulletPrefab.GetComponent<BulletProjectile>().Speed;////
        _animator = GetComponent<Animator>();
        _animationChangeSpeed = 1 / AnimationChangeTime;
        SetState(GunState.Default);
    }
    public Vector3 CalculateTarget()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, RAYCAST_DISTANCE, LayerMaskScreenRaycast))
        {
            return raycastHit.point;
        }
        return ray.origin + ray.direction * RAYCAST_DISTANCE;
    }

    private void Update()
    {
        if(_photonView.AmOwner)
        {
            if (Inputs.Instance.aim)
            {
                if (_gunState == GunState.Default)
                    SetState(GunState.Aiming);
            }
            else
            {
                if (_gunState == GunState.Aiming)
                    SetState(GunState.Default);
            }
        }
        _animator.SetLayerWeight(1,Mathf.MoveTowards(_animator.GetLayerWeight(1),_animAimWaight,_animationChangeSpeed*Time.deltaTime) );
    }

    private void SetState(GunState state)
    {
        _photonView.RPC(nameof(SetStateRPC), RpcTarget.All, state);
    }

    [PunRPC]
    private void SetStateRPC(GunState state)
    {
        _gunState = state;
        if (state == GunState.Default)
        {
            _animAimWaight = 0;
        }
        else
        {
            _animAimWaight = 1;
        }
    }

    public void Shoot()
    {
        //Vector3 aimDir = (CalculateTarget() - FiringPosition.position);
        _photonView.RPC("ShootRPC", RpcTarget.All, CalculateTarget());
    }

    [PunRPC]
    private void ShootRPC(Vector3 target, PhotonMessageInfo info)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        Vector3 aimDir = target - FiringPosition.position;

        PlayHitSound();
        BulletProjectile bulletPrefab = BulletPrefab;
        Vector3 shootPoint;
        if(Physics.Raycast(FiringPosition.position,aimDir,out RaycastHit raycastHit, RAYCAST_DISTANCE, LayerMaskBulletTarget))
        {
            if (raycastHit.collider.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                bulletPrefab = BulletBloodPrefab;
                
                //if (PhotonNetwork.IsMasterClient)
                    damageable.TakeDamage(1, info.Sender,_photonView.ViewID);
            }
            shootPoint = raycastHit.point;
        }
        else
        {
            shootPoint = FiringPosition.position + aimDir;
        }

        var bullet=Instantiate<BulletProjectile>(bulletPrefab, FiringPosition.position, 
            Quaternion.LookRotation(aimDir, Vector3.up));
        bullet.Init(shootPoint);
    }

    private void PlayHitSound()
    {
        var index = UnityEngine.Random.Range(0, ShootAudioClips.Length);
        AudioSource.PlayClipAtPoint(ShootAudioClips[index], transform.position, AudioVolume);
    }
}
