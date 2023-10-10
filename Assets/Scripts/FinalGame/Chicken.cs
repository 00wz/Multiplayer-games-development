using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(CharacterController))] 
public class Chicken : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float RotateSpeed=30f;
    [SerializeField]
    private float Speed=2f;
    [SerializeField]
    private LayerMask ReversalLayers;
    [SerializeField]
    private AudioClip[] BodyHitAudioClips;
    [SerializeField]
    [Range(0, 1)] public float AudioVolume = 0.5f;

    private Vector3 _currentDirection=Vector3.forward;
    private CharacterController _characterController;
    private PhotonView _photonView;
    private const float PLANE_OFFSET=0.05f;

    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _characterController = GetComponent<CharacterController>();
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        _currentDirection = CreateUnitVector();
        transform.rotation = Quaternion.LookRotation(_currentDirection, Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        var sign = Random.Range(0, 2) * 2 - 1;
        Quaternion delataRot = Quaternion.AngleAxis(sign*RotateSpeed * Time.deltaTime, Vector3.up);
        _currentDirection = delataRot*_currentDirection;
        //_currentDirection = new Vector3(_currentDirection.x, 0f, _currentDirection.z);
        transform.rotation = Quaternion.LookRotation(_currentDirection, Vector3.up);
        _characterController.Move((_currentDirection * Speed  +
                             new Vector3(0.0f, -2f, 0.0f)) * Time.deltaTime);
    }

    private Vector3 CreateUnitVector()
    {
        Vector3 vertical = Random.insideUnitCircle.normalized;
        Vector3 horizontal = new Vector3(vertical.x, vertical.z, vertical.y);
        return horizontal;
    }

    public void TakeDamage(int value, Player attacker, int attakerViewId)
    {
        PlayBodyHitSound();
        _photonView.RPC(nameof(TakeDamageRPC), RpcTarget.MasterClient);
    }

    [PunRPC]
    private void TakeDamageRPC()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        _currentDirection = CreateUnitVector();
        transform.rotation = Quaternion.LookRotation(_currentDirection, Vector3.up);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (hit.point.y < transform.position.y + PLANE_OFFSET)
            return;
        if ((ReversalLayers & (1 << hit.gameObject.layer)) == 0)
            return;
        var reflect=Vector3.Reflect(_currentDirection, hit.normal);
        _currentDirection = new Vector3(reflect.x, 0f, reflect.z).normalized;

    }

    private void PlayBodyHitSound()
    {
        var index = UnityEngine.Random.Range(0, BodyHitAudioClips.Length);
        AudioSource.PlayClipAtPoint(BodyHitAudioClips[index], transform.position, AudioVolume);
    }
}
