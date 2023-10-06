using Photon.Pun;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class ShootController : MonoBehaviour
{
    [SerializeField]
    private LayerMask LayerMask;

    [SerializeField]
    public Transform FiringPosition;

    [SerializeField]
    public GameObject BulletPrefab;

    [SerializeField]
    public GameObject BulletBloodPrefab;

    private const float RAYCAST_DISTANCE= 100f;
    private PhotonView _photonView;
    private float _bulletSpeed;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _bulletSpeed=BulletPrefab.GetComponent<BulletProjectile>().Speed;
    }
    public Vector3 CalculateTarget()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, RAYCAST_DISTANCE, LayerMask))
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
        GameObject bulletPrefab = BulletPrefab;
        float distance;
        Inputs.Instance.shoot = false;///////
        if(Physics.Raycast(FiringPosition.position,aimDir,out RaycastHit raycastHit, RAYCAST_DISTANCE, LayerMask))
        {
            if (raycastHit.collider.TryGetComponent<IDamageable>(out _))
            {
                bulletPrefab = BulletBloodPrefab;
                ///////
                //if (PhotonNetwork.IsMasterClient)
                    Debug.Log(info.Sender.UserId+"  "+info.Sender.ActorNumber);
            }
            distance = aimDir.magnitude;// raycastHit.distance;
        }
        else
        {
            distance = RAYCAST_DISTANCE;
        }
        var bullet=Instantiate(bulletPrefab, FiringPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        Destroy(bullet, distance / _bulletSpeed);
    }
}
