using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour {

    [SerializeField] private Transform vfxHit;
    [SerializeField] public float Speed = 100;

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
        Instantiate(vfxHit, transform.position, Quaternion.identity);
    }
}