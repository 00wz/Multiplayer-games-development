using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour {

    [SerializeField] private Transform vfxHit;
    [SerializeField] public float Speed = 100;

    [SerializeField]
    private bool IsEnableHitSound = false;
    [SerializeField]
    private AudioClip[] HitAudioClips;
    [SerializeField]
    [Range(0, 1)] public float AudioVolume = 1f;

    private Vector3 _destination;
    public void Init(Vector3 destination)
    {
        _destination = destination;
    }
    private void Update()
    {
        if (transform.position == _destination) Destroy(gameObject);
        transform.position = Vector3.MoveTowards(transform.position, _destination, Speed * Time.deltaTime);
    }
    private void OnDestroy()
    {
        if (IsEnableHitSound)
        {
            PlayHitSound();
        }
        Instantiate(vfxHit, transform.position, Quaternion.identity);
    }

    private void PlayHitSound()
    {
        var index = UnityEngine.Random.Range(0, HitAudioClips.Length);
        AudioSource.PlayClipAtPoint(HitAudioClips[index], transform.position, AudioVolume);
    }
}