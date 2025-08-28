using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

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
        [SerializeField] private float desiredSpeed = 8f;
        [Tooltip("Minimum distance to the player before slowing/holding.")]
        [SerializeField] private float chaseMinDistance = 20f;
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

        // NavMesh navigation (mirrors AiController)
        private NavMeshPath _navPath; // navigation path (reused)
        private Vector3[] _pathCorners = System.Array.Empty<Vector3>();
        private int _currentCornerIndex;
        private bool _hasValidPath;
        private float _recalcTimer;
        
        // Smooth movement state (like AiController)
        private float _targetSide; // side of destination, positive on right side, negative on left side

        [Header("NavMesh Settings")]
        [SerializeField] private float pathRecalcInterval = 0.5f;
        [SerializeField] private float cornerArrivalDistance = 6f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _engine = GetComponentInChildren<Engine>();
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

            // Initialize nav path once to avoid allocations each recalc
            _navPath = new NavMeshPath();
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

            // Enforce standoff distance
            var toPredicted = predicted - transform.position;
            float dist = toPredicted.magnitude;
            if (dist < chaseMinDistance)
            {
                onInsideMinDistance?.Invoke();
                onInsideMinDistanceEvent?.Invoke();
                TargetZeroInputs(2.0f, 2.0f);
                return;
            }

            // Recalculate path at interval
            _recalcTimer += Time.deltaTime;
            if (_recalcTimer >= pathRecalcInterval || !_hasValidPath)
            {
                _recalcTimer = 0f;
                CalculatePath(predicted, out _hasValidPath);
            }

            // Advance along path
            AdvanceCornerIfReached();

            // Steer and throttle toward current corner (like AiController)
            var corner = GetCurrentCornerOr(predicted);
            var toCorner = corner - transform.position;
            
            // Get angle to destination and side (mirrors AiController logic)
            var normDir = toCorner.normalized;
            var dot = Vector3.Dot(normDir, transform.forward);
            _targetSide = Vector3.Cross(transform.forward, normDir).y; // positive on right side, negative on left side

            // Smooth steering like AiController
            float desiredSteer = Mathf.Clamp(_targetSide, -maxSteer, maxSteer);
            
            // Smooth throttle like AiController
            float desiredThrottle = dot > 0 ? 1f : 0.25f; // Full throttle when facing target, reduced when turning
            
            // Apply smoothing if enabled
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

            // Recalculate path at interval
            _recalcTimer += Time.deltaTime;
            if (_recalcTimer >= pathRecalcInterval || !_hasValidPath)
            {
                _recalcTimer = 0f;
                CalculatePath(target, out _hasValidPath);
            }

            // Advance along path
            AdvanceCornerIfReached();

            // Steer and throttle toward current corner (like AiController)
            var corner = GetCurrentCornerOr(target);
            var toCorner = corner - transform.position;
            
            // Get angle to destination and side (mirrors AiController logic)
            var normDir = toCorner.normalized;
            var dot = Vector3.Dot(normDir, transform.forward);
            _targetSide = Vector3.Cross(transform.forward, normDir).y; // positive on right side, negative on left side

            // Smooth steering like AiController
            float desiredSteer = Mathf.Clamp(_targetSide, -maxSteer, maxSteer);
            
            // Smooth throttle like AiController
            float desiredThrottle = dot > 0 ? 0.8f : 0.2f; // Reduced throttle for return journey

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

        // NavMesh helpers ----------------------------------------------------
        private void CalculatePath(Vector3 destination, out bool hasPath)
        {
            hasPath = false;
            if (_navPath == null)
            {
                _navPath = new NavMeshPath();
            }
            NavMesh.CalculatePath(transform.position, destination, 255, _navPath);
            if (_navPath.status == NavMeshPathStatus.PathComplete)
            {
                _pathCorners = _navPath.corners;
                _currentCornerIndex = _pathCorners.Length > 1 ? 1 : 0; // skip origin
                hasPath = true;
            }
            else
            {
                _pathCorners = System.Array.Empty<Vector3>();
                _currentCornerIndex = 0;
                hasPath = false;
            }
        }

        private void AdvanceCornerIfReached()
        {
            if (_pathCorners == null || _pathCorners.Length == 0) return;
            if (_currentCornerIndex >= _pathCorners.Length) return;

            var corner = _pathCorners[_currentCornerIndex];
            if (Vector3.Distance(transform.position, corner) < cornerArrivalDistance)
            {
                _currentCornerIndex++;
                if (_currentCornerIndex >= _pathCorners.Length)
                {
                    _currentCornerIndex = _pathCorners.Length - 1;
                }
            }
        }
        
        // Add debug gizmos like AiController
        private void OnDrawGizmos()
        {
            if (!showGizmos || _pathCorners == null || _pathCorners.Length < 2) return;
            
            var c = Color.yellow;
            c.a = 0.5f;
            Gizmos.color = c;

            for (var i = 0; i < _pathCorners.Length - 1; i++)
            {
                if (i == _pathCorners.Length - 1)
                    Gizmos.DrawLine(_pathCorners[_pathCorners.Length - 1], _pathCorners[i]);
                else
                    Gizmos.DrawLine(_pathCorners[i], _pathCorners[i + 1]);
            }
        }

        private Vector3 GetCurrentCornerOr(Vector3 fallback)
        {
            if (_pathCorners != null && _pathCorners.Length > 0 && _currentCornerIndex < _pathCorners.Length)
            {
                return _pathCorners[_currentCornerIndex];
            }
            return fallback;
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



