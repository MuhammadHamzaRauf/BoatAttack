using System;
using UnityEngine;
using UnityEngine.Events;

namespace BoatAttack.AI
{
    /// <summary>
    /// Single-authority router that ensures only ONE AI controller drives the boat per frame.
    /// Prevents overlap between legacy AI (AiController) and chase AI (AIChaseController).
    /// </summary>
    [DisallowMultipleComponent]
    public class BoatAIModeRouter : MonoBehaviour
    {
        public enum AIMode
        {
            Legacy = 0,      // Traditional waypoint racing AI
            Chase = 1,       // Chasing player with standoff distance
            Returning = 2,   // Returning to base/home position
            Deactivated = 3  // No AI control, boat idles
        }

        [Header("AI Controller References")]
        [SerializeField] private AiController legacyAI;
        [SerializeField] private AIChaseController chaseAI;
        
        [Header("Base/Home Position")]
        [SerializeField] private Transform baseHome;
        [SerializeField] private bool useSpawnPositionAsBase = true;
        
        [Header("Events")]
        public UnityEvent<AIMode> onModeChanged;
        public UnityEvent onArrivedAtBase;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        // Public API
        public AIMode CurrentMode { get; private set; } = AIMode.Legacy;
        public Transform BaseHome => baseHome;
        
        // Private state
        private Engine _engine;
        private Vector3 _spawnPosition;
        private bool _initialized = false;
        
        private void Awake()
        {
            // Find Engine component (could be on this GameObject or in children)
            _engine = GetComponent<Engine>();
            if (_engine == null)
            {
                _engine = GetComponentInChildren<Engine>();
            }
            
            if (_engine == null)
            {
                Debug.LogError("BoatAIModeRouter: No Engine component found on this GameObject or in children!");
                enabled = false;
                return;
            }
            
            // Cache spawn position for base reference
            _spawnPosition = transform.position;
        }
        
        private void Start()
        {
            // Auto-find controllers if not assigned
            if (legacyAI == null)
                legacyAI = GetComponent<AiController>();
            if (chaseAI == null)
                chaseAI = GetComponent<AIChaseController>();
            
            // Setup base position
            if (baseHome == null && useSpawnPositionAsBase)
            {
                // Create an empty GameObject as base reference
                var baseObj = new GameObject($"{gameObject.name}_Base");
                baseObj.transform.position = _spawnPosition;
                baseObj.transform.SetParent(transform.parent);
                baseHome = baseObj.transform;
            }
            
            // Subscribe to chase AI events
            if (chaseAI != null)
            {
                chaseAI.onArrivedAtBase += OnChaseAIArrivedAtBase;
            }
            
            // Initialize in Legacy mode (preserves existing behavior)
            SetMode(AIMode.Legacy, false);
            _initialized = true;
        }
        
        private void OnDestroy()
        {
            if (chaseAI != null)
            {
                chaseAI.onArrivedAtBase -= OnChaseAIArrivedAtBase;
            }
        }
        
        // Public API Methods -------------------------------------------------
        
        /// <summary>
        /// Enable legacy waypoint racing AI
        /// </summary>
        public void EnableLegacy()
        {
            SetMode(AIMode.Legacy);
        }
        
        /// <summary>
        /// Start chasing the specified player transform
        /// </summary>
        /// <param name="player">Player transform to chase</param>
        public void EnableChase(Transform player)
        {
            if (player == null)
            {
                Debug.LogWarning("BoatAIModeRouter: Cannot enable chase with null player!");
                return;
            }
            
            if (chaseAI == null)
            {
                Debug.LogError("BoatAIModeRouter: No AIChaseController found! Add one to enable chase mode.");
                return;
            }
            
            // Set the player target and start chasing
            chaseAI.StartChasing(player);
            SetMode(AIMode.Chase);
        }
        
        /// <summary>
        /// Stop chasing and return to base/home position
        /// </summary>
        public void StopChaseAndReturnToBase()
        {
            if (chaseAI == null) return;
            
            chaseAI.StopChasingAndReturnToBase();
            SetMode(AIMode.Returning);
        }
        
        /// <summary>
        /// Deactivate all AI control (boat will idle)
        /// </summary>
        public void Deactivate()
        {
            SetMode(AIMode.Deactivated);
        }
        
        /// <summary>
        /// Set the base/home position for return navigation
        /// </summary>
        /// <param name="baseTransform">Transform representing the base/home position</param>
        public void SetBase(Transform baseTransform)
        {
            baseHome = baseTransform;
            
            // Update chase AI base if it exists
            if (chaseAI != null)
            {
                chaseAI.SetBase(baseTransform);
            }
        }
        
        // Private Methods ----------------------------------------------------
        
        private void SetMode(AIMode newMode, bool notifyChange = true)
        {
            if (CurrentMode == newMode) return;
            
            var oldMode = CurrentMode;
            CurrentMode = newMode;
            
            // Update controller states
            UpdateControllerStates();
            
            // Notify listeners
            if (notifyChange)
            {
                onModeChanged?.Invoke(newMode);
                
                if (showDebugInfo)
                {
                    Debug.Log($"BoatAIModeRouter [{gameObject.name}]: Mode changed from {oldMode} to {newMode}");
                }
            }
        }
        
        private void UpdateControllerStates()
        {
            // Disable both controllers first
            if (legacyAI != null)
                legacyAI.enabled = false;
            if (chaseAI != null)
                chaseAI.enabled = false;
            
            // Enable only the active one
            switch (CurrentMode)
            {
                case AIMode.Legacy:
                    if (legacyAI != null)
                        legacyAI.enabled = true;
                    break;
                    
                case AIMode.Chase:
                case AIMode.Returning:
                    if (chaseAI != null)
                        chaseAI.enabled = true;
                    break;
                    
                case AIMode.Deactivated:
                    // Both disabled - boat will idle
                    break;
            }
        }
        
        private void OnChaseAIArrivedAtBase()
        {
            // Chase AI has returned to base, switch back to Legacy mode
            SetMode(AIMode.Legacy);
            onArrivedAtBase?.Invoke();
        }
        
        // Debug and Validation -----------------------------------------------
        
        private void OnValidate()
        {
            // Ensure we have at least one AI controller
            if (legacyAI == null && chaseAI == null)
            {
                Debug.LogWarning("BoatAIModeRouter: No AI controllers assigned! Add either AiController or AIChaseController.");
            }
            
            // Check if Engine component exists (on this GameObject or in children)
            var engine = GetComponent<Engine>();
            if (engine == null)
            {
                engine = GetComponentInChildren<Engine>();
            }
            
            if (engine == null)
            {
                Debug.LogWarning("BoatAIModeRouter: No Engine component found! Make sure Engine is on this GameObject or in children.");
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!showDebugInfo) return;
            
            // Draw base position
            if (baseHome != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(baseHome.position, 2f);
                Gizmos.DrawLine(transform.position, baseHome.position);
            }
            else if (useSpawnPositionAsBase)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_spawnPosition, 2f);
                Gizmos.DrawLine(transform.position, _spawnPosition);
            }
            
            // Draw mode indicator
            var modeColor = CurrentMode switch
            {
                AIMode.Legacy => Color.green,
                AIMode.Chase => Color.red,
                AIMode.Returning => Color.blue,
                AIMode.Deactivated => Color.gray,
                _ => Color.white
            };
            
            Gizmos.color = modeColor;
            Gizmos.DrawWireCube(transform.position + Vector3.up * 3f, Vector3.one);
        }
        
        // Runtime validation
        private void Update()
        {
            if (!_initialized) return;
            
            // Ensure only one controller is active
            var activeControllers = 0;
            if (legacyAI != null && legacyAI.enabled) activeControllers++;
            if (chaseAI != null && chaseAI.enabled) activeControllers++;
            
            if (activeControllers > 1)
            {
                Debug.LogError($"BoatAIModeRouter [{gameObject.name}]: Multiple AI controllers active! This should not happen.");
                UpdateControllerStates(); // Force fix
            }
        }
    }
}
