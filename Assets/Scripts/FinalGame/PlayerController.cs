using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;


        // player
        private float _speed;
        private float _targetAzimuth = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;

        private CharacterController _controller;
        private GameObject _mainCamera;//?????????????????????
        private GroundedCheck _groundedObserver;
        private ShootController _shootController;
        private bool Grounded
        {
            get
            {
                //if (_groundedObserver == null) _groundedObserver = GetComponent<AnimCoreRotate>();
                return _groundedObserver.Grounded;
            }
        }

        [SerializeField]
        private bool DirectionalControl = false;

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            Inputs.Instance.shoot = false;////
            Inputs.Instance.jump = false;////
        }

        private void Start()
        {
            _controller = GetComponentInParent<CharacterController>();
            if (_controller == null) throw new Exception("CharacterController not found");
            _groundedObserver = GetComponentInParent<GroundedCheck>();
            if(_groundedObserver==null) throw new Exception("GroundedCheck script not found");
            _shootController = GetComponentInParent<ShootController>();
            if(_shootController==null) throw new Exception("ShootController script not found");

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
        }

        private void Update()
        {
            JumpAndGravity();

            Move();

            if (Inputs.Instance.shoot)
            {
                _shootController.Shoot();
                Inputs.Instance.shoot = false;///////
            }
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = Inputs.Instance.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (Inputs.Instance.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = Inputs.Instance.analogMovement ? Inputs.Instance.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }


            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (Inputs.Instance.move != Vector2.zero)
            {
                // normalise input direction
                Vector3 inputDirection = new Vector3(Inputs.Instance.move.x, 0.0f, Inputs.Instance.move.y).normalized;

                _targetAzimuth = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                if (!DirectionalControl)
                {
                    float rotation = Mathf.SmoothDampAngle(_controller.transform.eulerAngles.y,
                        _targetAzimuth,  ref _rotationVelocity, RotationSmoothTime);

                    // rotate to face input direction relative to camera position
                    _controller.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }

            }

            if (DirectionalControl)
            {
                float rotation = Mathf.SmoothDampAngle(_controller.transform.eulerAngles.y,
                    _mainCamera.transform.eulerAngles.y, ref _rotationVelocity, RotationSmoothTime);

                // rotate to face input direction relative to camera position
                _controller.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetAzimuth, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (Inputs.Instance.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // if we are not grounded, do not jump
                Inputs.Instance.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
    }
}
