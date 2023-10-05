using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
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

        //[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        //public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        // animation IDs
        private int _animIDForwardSpeed;
        private int _animIDRightSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private Animator _animator;

        private Vector3 _lastPosition;
        private Vector3 _currentSpeed;
        private Vector3 _smoothedSpeed;
        private GroundedCheck _groundedCheck;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _groundedCheck = GetComponent<GroundedCheck>();

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
            _animIDForwardSpeed = Animator.StringToHash("ForwardSpeed");
            _animIDRightSpeed = Animator.StringToHash("RightSpeed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            bool _currentGrounded = _groundedCheck.Grounded;
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
            Vector3 smoothedHorizontalSpeed =transform.InverseTransformVector(_smoothedSpeed);

                _animator.SetFloat(_animIDForwardSpeed, Mathf.Abs(smoothedHorizontalSpeed.z)<0.01f?0f:smoothedHorizontalSpeed.z);
                _animator.SetFloat(_animIDRightSpeed, Mathf.Abs(smoothedHorizontalSpeed.x)<0.01f?0f:smoothedHorizontalSpeed.x);
                //_animator.SetFloat(_animIDMotionSpeed, 1f);
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