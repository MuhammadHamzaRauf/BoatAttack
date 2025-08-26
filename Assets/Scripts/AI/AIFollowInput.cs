using UnityEngine;

namespace BoatAttack.AI
{
    /// <summary>
    /// AI input that makes a boat follow a target player
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class AIFollowInput : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private float desiredSpeed = 12f;
        [SerializeField] private float minFollowDist = 10f;
        [SerializeField] private float maxFollowDist = 25f;
        
        [Header("Control Tuning")]
        [SerializeField] private float steerGain = 1.0f;
        [SerializeField] private float throttleGain = 1.0f;
        [SerializeField] private float brakeGain = 2.0f;
        [SerializeField] private float lookAheadTime = 0.75f;
        
        [Header("Debug")]
        [SerializeField] private bool showGizmos = true;
        
        // Input values that can be read by HumanController
        public float Throttle { get; private set; }
        public float Steer { get; private set; }
        public bool Brake { get; private set; }
        public bool Boost { get; private set; }

        private Rigidbody rb;
        private Vector3 _lastTargetPos;
        private Vector3 _targetVelocity;
        private Engine _engine;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            _engine = GetComponent<Engine>();
        }

        void Start()
        {
            if (target == null)
            {
                // Try to find player boat
                var playerBoat = GameObject.FindGameObjectWithTag("Player");
                if (playerBoat != null)
                {
                    target = playerBoat.transform;
                }
                else
                {
                    Debug.LogWarning("AIFollowInput: No target assigned and no player found!");
                }
            }
            
            if (target != null)
            {
                _lastTargetPos = target.position;
            }
        }

        void FixedUpdate()
        {
            if (target == null) return;

            // Calculate target velocity
            _targetVelocity = (target.position - _lastTargetPos) / Time.deltaTime;
            _lastTargetPos = target.position;

            // Calculate desired position (look ahead)
            Vector3 desiredPos = target.position + _targetVelocity * lookAheadTime;
            
            // Calculate direction to target
            Vector3 toTarget = desiredPos - transform.position;
            float distanceToTarget = toTarget.magnitude;
            
            // Steering: turn towards target
            Vector3 localToTarget = transform.InverseTransformDirection(toTarget);
            float targetSteer = Mathf.Clamp(localToTarget.x * steerGain, -1f, 1f);
            
            // Smooth steering to avoid jitter
            Steer = Mathf.Lerp(Steer, targetSteer, Time.deltaTime * 3f);
            
            // Throttle: maintain desired speed and distance
            float currentSpeed = rb.velocity.magnitude;
            float speedError = desiredSpeed - currentSpeed;
            
            // Distance-based throttle adjustment
            float distanceFactor = 1f;
            if (distanceToTarget < minFollowDist)
            {
                // Too close - slow down
                distanceFactor = Mathf.Clamp01(distanceToTarget / minFollowDist);
                speedError *= 0.5f; // Reduce speed target when close
            }
            else if (distanceToTarget > maxFollowDist)
            {
                // Too far - speed up
                distanceFactor = 1f + (distanceToTarget - maxFollowDist) / maxFollowDist;
            }
            
            float targetThrottle = Mathf.Clamp(speedError * throttleGain * distanceFactor, -1f, 1f);
            Throttle = Mathf.Lerp(Throttle, targetThrottle, Time.deltaTime * 2f);
            
            // Brake when too close or when target is slowing down
            bool shouldBrake = distanceToTarget < minFollowDist * 0.8f || 
                              (Vector3.Dot(rb.velocity.normalized, toTarget.normalized) < -0.5f);
            
            Brake = shouldBrake;
            
            // Boost not implemented for AI
            Boost = false;
            
            // Clamp outputs to prevent extreme values
            Throttle = Mathf.Clamp(Throttle, -1f, 1f);
            Steer = Mathf.Clamp(Steer, -1f, 1f);
            
            // Apply input directly to engine if available
            if (_engine != null)
            {
                _engine.Accelerate(Throttle);
                _engine.Turn(Steer);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!showGizmos || target == null) return;
            
            // Draw follow distance
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(target.position, minFollowDist);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, maxFollowDist);
            
            // Draw look ahead position
            if (Application.isPlaying)
            {
                Vector3 lookAheadPos = target.position + _targetVelocity * lookAheadTime;
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(lookAheadPos, 1f);
                Gizmos.DrawLine(target.position, lookAheadPos);
            }
            
            // Draw to target line
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.position);
        }

        // Public method to set target at runtime
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                _lastTargetPos = target.position;
            }
        }
        
        // Public methods for external control (compatible with old system)
        public void SetThrottle(float value) => Throttle = Mathf.Clamp(value, -1f, 1f);
        public void SetSteer(float value) => Steer = Mathf.Clamp(value, -1f, 1f);
        public float GetThrottle() => Throttle;
        public float GetSteer() => Steer;
    }
}
