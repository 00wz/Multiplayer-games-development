using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour {

    [SerializeField] private Transform vfxHit;
    [SerializeField] public float Speed = 100;
    //public static float BULLET_SPEED = 100f;

    private Rigidbody bulletRigidbody;

    private void Awake() {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start() {
        bulletRigidbody.velocity = transform.forward * Speed;
    }
    /*
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<BulletTarget>() != null) {
            // Hit target
            Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
        } else {
            // Hit something else
            Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    */
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent<PlayerClass>(out _))
        {
            // Hit target
            Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
        }
        else
        {
            // Hit something else
            Instantiate(vfxHitRed, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    */
    private void OnDestroy()
    {
        Instantiate(vfxHit, transform.position, Quaternion.identity);
    }
}