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

    private const float RAYCAST_DISTANCE= 100f;
    private PhotonView _photonView;
    private float _bulletSpeed;
    /*
    enum GunState
    {
        Ready,
        Waiting
    }
    */
    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _bulletSpeed=BulletPrefab.GetComponent<BulletProjectile>().Speed;
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

    public void Shoot()
    {
        Vector3 aimDir = (CalculateTarget() - FiringPosition.position);
        _photonView.RPC("ShootRPC", RpcTarget.All,aimDir);
    }

    [PunRPC]
    private void ShootRPC(Vector3 aimDir, PhotonMessageInfo info)
    {
        PlayHitSound();
        BulletProjectile bulletPrefab = BulletPrefab;
        RaycastHit raycastHit;
        if(Physics.Raycast(FiringPosition.position,aimDir,out raycastHit, RAYCAST_DISTANCE, LayerMaskBulletTarget))
        {
            if (raycastHit.collider.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                bulletPrefab = BulletBloodPrefab;
                
                //if (PhotonNetwork.IsMasterClient)
                    damageable.TakeDamage(1, info.Sender,_photonView.ViewID);
            }
        }

        var bullet=Instantiate<BulletProjectile>(bulletPrefab, FiringPosition.position, 
            Quaternion.LookRotation(aimDir, Vector3.up));
        bullet.Init(raycastHit.point);
    }

    private void PlayHitSound()
    {
        var index = UnityEngine.Random.Range(0, ShootAudioClips.Length);
        AudioSource.PlayClipAtPoint(ShootAudioClips[index], transform.position, AudioVolume);
    }
}
