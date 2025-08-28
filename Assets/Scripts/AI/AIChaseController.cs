using System;
using UnityEngine;
using UnityEngine.Events;

namespace BoatAttack.AI
{
    /// <summary>
    /// Lightweight, additive AI brain that makes an AI boat chase the player,
    /// maintain a standoff distance, and return to base or deactivate on command.
    ///
    /// Decisions are computed in Update and cached. Physics and forces are applied in FixedUpdate.
    /// Avoids per-frame allocations and does not depend on input systems.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class AIChaseController : MonoBehaviour
    {
        public enum ChaseState
        {
            Idle = 0,
            Chasing = 1,
            Returning = 2,
            Deactivated = 3
        }

        [Header("Targets")]
        [Tooltip("Player or target to chase. Can be set at runtime via StartChasing().")]
        [SerializeField] private Transform player;
        [Tooltip("Optional base Transform. If null, spawns capture current transform as base on Start().")]
        [SerializeField] private Transform basePoint;

        [Header("Chase Tuning")]
        [Tooltip("Desired forward speed while chasing, in m/s (approx).")]
        [SerializeField] private float desiredSpeed = 12f;
        [Tooltip("Minimum distance to the player before slowing/holding.")]
        [SerializeField] private float chaseMinDistance = 12f;
        [Tooltip("Steering proportional gain.")]
        [SerializeField] private float steerGain = 1.0f;
        [Tooltip("Throttle proportional gain.")]
        [SerializeField] private float throttleGain = 1.0f;
        [Tooltip("Optional throttle smoothing (0 = off). Higher is smoother.")]
        [SerializeField] private float throttleSlewRate = 5.0f;
        [Tooltip("Optional steering smoothing (0 = off). Higher is smoother.")]
        [SerializeField] private float steerSlewRate = 6.0f;
        [Tooltip("Cap the throttle magnitude to avoid oscillations.")]
        [SerializeField] private float maxThrottle = 1.0f;
        [Tooltip("Cap the steering magnitude to avoid oscillations.")]
        [SerializeField] private float maxSteer = 1.0f;

        [Header("Return/Idle Tuning")]
        [Tooltip("Distance to base considered 'arrived'.")]
        [SerializeField] private float returnTolerance = 3.0f;
        [Tooltip("Speed while returning to base.")]
        [SerializeField] private float returnSpeed = 8.0f;

        [Header("Events (Optional)")]
        public Action onInsideMinDistance;
        public Action onArrivedAtBase;
        
        // UnityEvent versions for inspector assignment
        [SerializeField] private UnityEvent onInsideMinDistanceEvent;
        [SerializeField] private UnityEvent onArrivedAtBaseEvent;

        [Header("Debug")]
        [SerializeField] private bool showGizmos = true;

        // Public API surface
        public ChaseState State { get; private set; } = ChaseState.Idle;
        public Transform Player => player;
        public Transform BasePoint => basePoint;

        // Cached components and state
        private Rigidbody _rb;
        private Engine _engine; // existing boat propulsion component
        private Vector3 _basePosition;
        private Vector3 _lastPlayerPos;
        private Vector3 _playerVelocity;

        // Cached outputs for physics
        private float _steer;
        private float _throttle;

        private static readonly Vector3 Vector3Zero = Vector3.zero;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _engine = GetComponent<Engine>();
            if (_engine == null)
            {
                Debug.LogWarning("AIChaseController: No Engine found on boat. Component will not drive movement.");
            }
        }

        private void Start()
        {
            if (basePoint != null)
            {
                _basePosition = basePoint.position;
            }
            else
            {
                _basePosition = transform.position;
            }

            if (player != null)
            {
                _lastPlayerPos = player.position;
            }
        }

        // Public API ---------------------------------------------------------
        public void StartChasing(Transform playerTarget)
        {
            if (playerTarget != null)
            {
                player = playerTarget;
                _lastPlayerPos = player.position;
            }
            State = ChaseState.Chasing;
        }

        public void StopChasingAndReturnToBase()
        {
            State = ChaseState.Returning;
        }

        public void Deactivate()
        {
            State = ChaseState.Deactivated;
            _throttle = 0f;
            _steer = 0f;
        }

        public void SetBase(Transform newBase)
        {
            basePoint = newBase;
            _basePosition = newBase != null ? newBase.position : transform.position;
        }

        // Decision step (no physics) ----------------------------------------
        private void Update()
        {
            switch (State)
            {
                case ChaseState.Chasing:
                    UpdateChase();
                    break;
                case ChaseState.Returning:
                    UpdateReturn();
                    break;
                case ChaseState.Idle:
                    // gentle stabilization
                    TargetZeroInputs(2.0f, 2.0f);
                    break;
                case ChaseState.Deactivated:
                    // hard zero
                    _throttle = 0f;
                    _steer = 0f;
                    break;
            }
        }

        private void UpdateChase()
        {
            if (player == null)
            {
                TargetZeroInputs(2.0f, 2.0f);
                return;
            }

            // Estimate player velocity (simple finite difference)
            var currentPlayerPos = player.position;
            _playerVelocity = (currentPlayerPos - _lastPlayerPos) * (1f / Mathf.Max(Time.deltaTime, 0.0001f));
            _lastPlayerPos = currentPlayerPos;

            // Predict a short look-ahead target to reduce cut-corners
            float lookAheadTime = 0.5f;
            var predicted = currentPlayerPos + _playerVelocity * lookAheadTime;

            // Vector from us to predicted target
            var toTarget = predicted - transform.position;
            float dist = toTarget.magnitude;

            // Local-space steering (x is left/right)
            var local = transform.InverseTransformDirection(toTarget);
            float desiredSteer = Mathf.Clamp(local.x * steerGain, -maxSteer, maxSteer);

            // Throttle control with standoff enforcement
            float desiredSpeedLocal = desiredSpeed;
            if (dist < chaseMinDistance)
            {
                desiredSpeedLocal = 0f; // hold position
                onInsideMinDistance?.Invoke();
                onInsideMinDistanceEvent?.Invoke();
            }

            float forwardSpeed = Vector3.Dot(_rb.velocity, transform.forward);
            float speedError = desiredSpeedLocal - forwardSpeed;
            float desiredThrottle = Mathf.Clamp(speedError * throttleGain, -maxThrottle, maxThrottle);

            // Optional smoothing to avoid oscillations
            if (throttleSlewRate > 0f)
                _throttle = Mathf.MoveTowards(_throttle, desiredThrottle, throttleSlewRate * Time.deltaTime);
            else
                _throttle = desiredThrottle;

            if (steerSlewRate > 0f)
                _steer = Mathf.MoveTowards(_steer, desiredSteer, steerSlewRate * Time.deltaTime);
            else
                _steer = desiredSteer;
        }

        private void UpdateReturn()
        {
            var target = _basePosition;
            var toBase = target - transform.position;
            float dist = toBase.magnitude;
            if (dist <= returnTolerance)
            {
                State = ChaseState.Idle;
                _throttle = 0f;
                _steer = 0f;
                onArrivedAtBase?.Invoke();
                onArrivedAtBaseEvent?.Invoke();
                return;
            }

            // Local-space steering towards base
            var local = transform.InverseTransformDirection(toBase);
            float desiredSteer = Mathf.Clamp(local.x * steerGain, -maxSteer, maxSteer);

            // Aim for return speed
            float forwardSpeed = Vector3.Dot(_rb.velocity, transform.forward);
            float speedError = returnSpeed - forwardSpeed;
            float desiredThrottle = Mathf.Clamp(speedError * throttleGain, -maxThrottle, maxThrottle);

            if (throttleSlewRate > 0f)
                _throttle = Mathf.MoveTowards(_throttle, desiredThrottle, throttleSlewRate * Time.deltaTime);
            else
                _throttle = desiredThrottle;

            if (steerSlewRate > 0f)
                _steer = Mathf.MoveTowards(_steer, desiredSteer, steerSlewRate * Time.deltaTime);
            else
                _steer = desiredSteer;
        }

        private void TargetZeroInputs(float throttleRate, float steerRate)
        {
            _throttle = Mathf.MoveTowards(_throttle, 0f, throttleRate * Time.deltaTime);
            _steer = Mathf.MoveTowards(_steer, 0f, steerRate * Time.deltaTime);
        }

        // Physics application ------------------------------------------------
        private void FixedUpdate()
        {
            if (_engine == null) return;
            if (State == ChaseState.Deactivated)
            {
                _engine.Accelerate(0f);
                _engine.Turn(0f);
                return;
            }

            _engine.Accelerate(_throttle);
            _engine.Turn(_steer);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            // Base point
            var basePos = basePoint != null ? basePoint.position : (_basePosition != Vector3Zero ? _basePosition : transform.position);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(basePos, 1.0f);
            Gizmos.DrawLine(transform.position, basePos);

            // Min distance ring for chase
            Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
            Gizmos.DrawWireSphere(player != null ? player.position : transform.position, chaseMinDistance);
        }
#endif
    }
}



