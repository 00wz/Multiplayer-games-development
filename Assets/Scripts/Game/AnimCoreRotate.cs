using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class AnimCoreRotate : MonoBehaviour
    {
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        // player
        private float _targetRotation = 0.0f;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private Animator _animator;

        private Vector3 _lastPosition;
        private Vector3 _currentSpeed;
        private Vector3 _smoothedSpeed;

        private void Start()
        {
            _animator = GetComponent<Animator>();

            AssignAnimationIDs();

            _lastPosition = transform.position;
        }

        private void Update()
        {
            UpdateSpeed();
            //Debug.Log("_currentSpeed: "+_currentSpeed+ "\n_controller.velocity: "+ _controller.velocity);

            GroundedCheck();
            Move();
        }

        private void UpdateSpeed()
        {
            var _currentPosition = transform.position;
            _currentSpeed = (_currentPosition - _lastPosition) / Time.deltaTime;
            _lastPosition = _currentPosition;
            _smoothedSpeed=Vector3.Lerp(_smoothedSpeed, _currentSpeed,
                    Time.deltaTime * SpeedChangeRate);
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);

            bool _currentGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
            if(Grounded&&!_currentGrounded)
            {
                _animator.SetBool(_animIDGrounded, false);
                if(_smoothedSpeed.y>0f)
                    _animator.SetBool(_animIDJump, true);
                else
                    _animator.SetBool(_animIDFreeFall, true);
            }
            else if (!Grounded && _currentGrounded)
            {
                _animator.SetBool(_animIDGrounded, true);
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }
            Grounded = _currentGrounded;
        }

        private void Move()
        {
            //float absoluteHorizontalSpeed = new Vector3(_currentSpeed.x, 0.0f, _currentSpeed.z).magnitude;
            float smoothedHorizontalSpeed = new Vector3(_smoothedSpeed.x, 0.0f, _smoothedSpeed.z).magnitude;

                _targetRotation = Mathf.Atan2(_smoothedSpeed.x, _smoothedSpeed.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0.0f, _targetRotation, 0.0f);
                /*float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
    RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);*/

                //_animator.SetFloat(_animIDSpeed, absoluteHorizontalSpeed);
                _animator.SetFloat(_animIDSpeed, smoothedHorizontalSpeed<0.01f?0f:smoothedHorizontalSpeed);
                _animator.SetFloat(_animIDMotionSpeed, 1f);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.position, FootstepAudioVolume);
            }
        }
    }
}