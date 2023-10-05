using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    [SerializeField]
    Vector3 Speed;

    private CharacterController _controller; 
    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }
    void Update()
    {
        _controller.Move(Speed * Time.deltaTime);
    }
}
