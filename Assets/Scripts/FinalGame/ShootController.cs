using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootController : MonoBehaviour
{
    [SerializeField]
    private LayerMask LayerMask;

    [SerializeField]
    public Transform FiringPosition;

    private const float RAYCAST_DISTANCE= 999f;
    public Vector3? CalculateTarget()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if(Physics.Raycast(ray, out RaycastHit raycastHit, RAYCAST_DISTANCE, LayerMask))
        {
            return raycastHit.point;
        }
        return ray.origin + ray.direction * RAYCAST_DISTANCE;
    }

    private void Update()
    {
        if (Inputs.Instance.shoot)
        {

        }
    }
}
