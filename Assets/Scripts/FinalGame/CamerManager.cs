using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerManager : MonoBehaviour
{
	[Tooltip("Main Virtual Camera")]
	[SerializeField]
	private CinemachineVirtualCamera VirtualCamera;

	[Tooltip("Aim Virtual Camera")]
	[SerializeField]
	private CinemachineVirtualCamera VirtualCameraAim;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    [Tooltip("Crosshair")]
    public GameObject Crosshair;

    [Tooltip("Sensitivity")]
    public float Sensitivity = 1f;

    [Tooltip("AimSensitivity")]
    public float AimSensitivity = 1f;
    
    [SerializeField]
	private bool _aimingIsEnabled;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;
    private float _currentSensitivity;
    private CameraState _cameraState;
    private enum CameraState
    {
        Default,
        Aim
    }
    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return Inputs.Instance.playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }
	public void AimingIsEnabled(bool value)
    {
        _aimingIsEnabled = value;
    }
    
    
    private void Awake()
    {
        SetState(CameraState.Default);
    }
    
    public void SetTarget(Transform target)
    {
		VirtualCamera.m_Follow = target;
		VirtualCameraAim.m_Follow = target;
        _cinemachineTargetYaw = target.rotation.eulerAngles.y;
    }

    private void Update()
    {
        if (_aimingIsEnabled&& Inputs.Instance.aim)
        {
            if(_cameraState==CameraState.Default)
                SetState(CameraState.Aim);
        }
        else
        {
            if (_cameraState == CameraState.Aim)
                SetState(CameraState.Default);
        }
    }

    private void SetState(CameraState state)
    {
        if (state==CameraState.Aim)
        {
            VirtualCameraAim.gameObject.SetActive(true);
            Crosshair.SetActive(true);
            _currentSensitivity = AimSensitivity;
            //Inputs.Instance.shoot = false;
        }
        else
        {
            VirtualCameraAim.gameObject.SetActive(false);
            Crosshair.SetActive(false);
            _currentSensitivity = Sensitivity;
        }
        _cameraState = state;
    }

    private void LateUpdate()
    {
        if(VirtualCamera.m_Follow != null)
            CameraRotation();
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (Inputs.Instance.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += Inputs.Instance.look.x * deltaTimeMultiplier*_currentSensitivity;
            _cinemachineTargetPitch += Inputs.Instance.look.y * deltaTimeMultiplier*_currentSensitivity;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        VirtualCamera.m_Follow.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
