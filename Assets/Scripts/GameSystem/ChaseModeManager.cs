using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BoatAttack.AI;

namespace BoatAttack
{
    /// <summary>
    /// Scene-level manager that provides global control over all AI boats.
    /// Automatically finds and manages BoatAIModeRouter components.
    /// </summary>
    public class ChaseModeManager : MonoBehaviour
    {
        [Header("Player Reference")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private bool autoFindPlayer = true;
        
        [Header("UI References (Optional)")]
        [SerializeField] private Button startChaseAllButton;
        [SerializeField] private Button stopChaseAllButton;
        [SerializeField] private Button deactivateAllButton;
        [SerializeField] private Button enableLegacyAllButton;
        [SerializeField] private Text statusText;
        
        [Header("Auto Setup")]
        [SerializeField] private bool setupButtonsOnStart = true;
        [SerializeField] private bool enableOnStart = false;
        
        // Runtime state
        private List<BoatAIModeRouter> _aiRouters = new List<BoatAIModeRouter>();
        private bool _initialized = false;
        
        // Public properties
        public int TotalAIBoats => _aiRouters.Count;
        public int ChasingBoats => GetChasingBoatCount();
        public int ReturningBoats => GetReturningBoatCount();
        public int DeactivatedBoats => GetDeactivatedBoatCount();
        public int LegacyBoats => GetLegacyBoatCount();
        
        private void Start()
        {
            Initialize();
            
            if (setupButtonsOnStart)
            {
                SetupButtons();
            }
            
            if (enableOnStart)
            {
                StartChaseAll();
            }
        }
        
        private void Initialize()
        {
            if (_initialized) return;
            
            // Find player if not assigned
            if (playerTransform == null && autoFindPlayer)
            {
                FindPlayerTransform();
            }
            
            // Find all AI routers in the scene
            RefreshAIBoats();
            
            _initialized = true;
        }
        
        private void FindPlayerTransform()
        {
            // Try to find player boat by looking for HumanController
            var humanControllers = FindObjectsOfType<HumanController>();
            if (humanControllers.Length > 0)
            {
                playerTransform = humanControllers[0].transform;
                Debug.Log($"ChaseModeManager: Auto-found player at {playerTransform.name}");
            }
            else
            {
                // Fallback: look for GameObject with "Player" tag
                var playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    playerTransform = playerObj.transform;
                    Debug.Log($"ChaseModeManager: Auto-found player by tag at {playerTransform.name}");
                }
                else
                {
                    Debug.LogWarning("ChaseModeManager: No player found! Assign manually or ensure player has HumanController or 'Player' tag.");
                }
            }
        }
        
        private void RefreshAIBoats()
        {
            _aiRouters.Clear();
            var routers = FindObjectsOfType<BoatAIModeRouter>();
            
            foreach (var router in routers)
            {
                // Only include routers that have both AI controllers
                if (router.GetComponent<AiController>() != null && router.GetComponent<AIChaseController>() != null)
                {
                    _aiRouters.Add(router);
                }
            }
            
            Debug.Log($"ChaseModeManager: Found {_aiRouters.Count} AI boats with routers");
        }
        
        private void SetupButtons()
        {
            if (startChaseAllButton != null)
                startChaseAllButton.onClick.AddListener(StartChaseAll);
                
            if (stopChaseAllButton != null)
                stopChaseAllButton.onClick.AddListener(StopChaseAll);
                
            if (deactivateAllButton != null)
                deactivateAllButton.onClick.AddListener(DeactivateAll);
                
            if (enableLegacyAllButton != null)
                enableLegacyAllButton.onClick.AddListener(EnableLegacyAll);
        }
        
        private void Update()
        {
            if (statusText != null)
            {
                UpdateStatusText();
            }
        }
        
        private void UpdateStatusText()
        {
            var status = $"AI Boats: {TotalAIBoats}\n" +
                        $"Chasing: {ChasingBoats}\n" +
                        $"Returning: {ReturningBoats}\n" +
                        $"Legacy: {LegacyBoats}\n" +
                        $"Deactivated: {DeactivatedBoats}";
            
            statusText.text = status;
        }
        
        // Public API Methods -------------------------------------------------
        
        /// <summary>
        /// Start chase mode for all AI boats
        /// </summary>
        [ContextMenu("Start Chase All")]
        public void StartChaseAll()
        {
            if (playerTransform == null)
            {
                Debug.LogError("ChaseModeManager: Cannot start chase - no player transform assigned!");
                return;
            }
            
            int chaseCount = 0;
            foreach (var router in _aiRouters)
            {
                if (router != null)
                {
                    router.EnableChase(playerTransform);
                    chaseCount++;
                }
            }
            
            Debug.Log($"ChaseModeManager: Started chase mode for {chaseCount} AI boats");
        }
        
        /// <summary>
        /// Stop chase mode and make all AI boats return to base
        /// </summary>
        [ContextMenu("Stop Chase All")]
        public void StopChaseAll()
        {
            int returnCount = 0;
            foreach (var router in _aiRouters)
            {
                if (router != null)
                {
                    router.StopChaseAndReturnToBase();
                    returnCount++;
                }
            }
            
            Debug.Log($"ChaseModeManager: Stopped chase mode for {returnCount} AI boats");
        }
        
        /// <summary>
        /// Deactivate all AI boats (stop all behaviors)
        /// </summary>
        [ContextMenu("Deactivate All")]
        public void DeactivateAll()
        {
            int deactivateCount = 0;
            foreach (var router in _aiRouters)
            {
                if (router != null)
                {
                    router.Deactivate();
                    deactivateCount++;
                }
            }
            
            Debug.Log($"ChaseModeManager: Deactivated {deactivateCount} AI boats");
        }
        
        /// <summary>
        /// Enable legacy racing AI for all boats
        /// </summary>
        [ContextMenu("Enable Legacy All")]
        public void EnableLegacyAll()
        {
            int legacyCount = 0;
            foreach (var router in _aiRouters)
            {
                if (router != null)
                {
                    router.EnableLegacy();
                    legacyCount++;
                }
            }
            
            Debug.Log($"ChaseModeManager: Enabled legacy mode for {legacyCount} AI boats");
        }
        
        /// <summary>
        /// Refresh the list of AI boats (useful if boats are spawned/destroyed at runtime)
        /// </summary>
        [ContextMenu("Refresh AI Boats")]
        public void RefreshAIBoatsList()
        {
            RefreshAIBoats();
        }
        
        /// <summary>
        /// Set the player transform for all AI boats
        /// </summary>
        /// <param name="newPlayer">New player transform</param>
        public void SetPlayerTransform(Transform newPlayer)
        {
            playerTransform = newPlayer;
            Debug.Log($"ChaseModeManager: Player transform set to {newPlayer?.name ?? "null"}");
        }
        
        // Utility Methods ----------------------------------------------------
        
        private int GetChasingBoatCount()
        {
            int count = 0;
            foreach (var router in _aiRouters)
            {
                if (router != null && router.CurrentMode == BoatAIModeRouter.AIMode.Chase)
                    count++;
            }
            return count;
        }
        
        private int GetReturningBoatCount()
        {
            int count = 0;
            foreach (var router in _aiRouters)
            {
                if (router != null && router.CurrentMode == BoatAIModeRouter.AIMode.Returning)
                    count++;
            }
            return count;
        }
        
        private int GetDeactivatedBoatCount()
        {
            int count = 0;
            foreach (var router in _aiRouters)
            {
                if (router != null && router.CurrentMode == BoatAIModeRouter.AIMode.Deactivated)
                    count++;
            }
            return count;
        }
        
        private int GetLegacyBoatCount()
        {
            int count = 0;
            foreach (var router in _aiRouters)
            {
                if (router != null && router.CurrentMode == BoatAIModeRouter.AIMode.Legacy)
                    count++;
            }
            return count;
        }
        
        // Cleanup ------------------------------------------------------------
        
        private void OnDestroy()
        {
            // Clean up button listeners
            if (startChaseAllButton != null)
                startChaseAllButton.onClick.RemoveListener(StartChaseAll);
            if (stopChaseAllButton != null)
                stopChaseAllButton.onClick.RemoveListener(StopChaseAll);
            if (deactivateAllButton != null)
                deactivateAllButton.onClick.RemoveListener(DeactivateAll);
            if (enableLegacyAllButton != null)
                enableLegacyAllButton.onClick.RemoveListener(EnableLegacyAll);
        }
        
        // Static utility methods for easy access
        public static void StartChase() => FindObjectOfType<ChaseModeManager>()?.StartChaseAll();
        public static void StopChase() => FindObjectOfType<ChaseModeManager>()?.StopChaseAll();
        public static void Deactivate() => FindObjectOfType<ChaseModeManager>()?.DeactivateAll();
        public static void EnableLegacy() => FindObjectOfType<ChaseModeManager>()?.EnableLegacyAll();
    }
}
