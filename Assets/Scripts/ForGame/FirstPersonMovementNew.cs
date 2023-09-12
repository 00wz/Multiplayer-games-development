using UnityEngine;

public class FirstPersonMovementNew : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody _rigidbody;
    private GameObject _camera;

    void Awake()
    {
        // Get the rigidbody on this.
        /*_rigidbody = GetComponent<Rigidbody>();
        _rigidbody ??= gameObject.AddComponent<Rigidbody>();*/
        _rigidbody = gameObject.AddComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
    }

    void Update()
    {
        // Get targetVelocity from input.
        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * speed,
            Input.GetAxis("Vertical") * speed);//по диагонали - быстрее

        // Apply movement.
        _rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, _rigidbody.velocity.y, targetVelocity.y);
    }
}