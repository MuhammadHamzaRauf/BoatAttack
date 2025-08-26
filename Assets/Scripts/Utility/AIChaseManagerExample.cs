using UnityEngine;
using BoatAttack.AI;

namespace BoatAttack.Utility
{
    /// <summary>
    /// Example script demonstrating how to use the simplified AIChaseManager.
    /// This script shows how the AIChaseManager automatically finds boats and player
    /// from the scene without manual assignment.
    /// </summary>
    public class AIChaseManagerExample : MonoBehaviour
    {
        [Header("Chase Mode Demo")]
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool enableChaseOnStart = false;
        
        private AIChaseManager _chaseManager;
        
        void Start()
        {
            // Find or create AIChaseManager
            _chaseManager = FindObjectOfType<AIChaseManager>();
            if (_chaseManager == null)
            {
                // Create one if none exists
                var go = new GameObject("AIChaseManager");
                _chaseManager = go.AddComponent<AIChaseManager>();
                Debug.Log("AIChaseManagerExample: Created new AIChaseManager");
            }
            
            if (enableChaseOnStart)
            {
                // Wait for the manager to find boats, then start chase
                Invoke(nameof(StartChaseMode), 1f);
            }
        }
        
        void Update()
        {
            if (!showDebugInfo || _chaseManager == null) return;
            
            // Display debug info
            var boatCount = _chaseManager.GetManagedBoatCount();
            var playerTransform = _chaseManager.GetPlayerTransform();
            var playerName = playerTransform != null ? playerTransform.name : "None";
            var isReady = _chaseManager.IsReady();
            
            // Simple debug display (you could use UI text instead)
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log($"AIChaseManager Status:");
                Debug.Log($"  Ready: {isReady}");
                Debug.Log($"  Managed AI Boats: {boatCount}");
                Debug.Log($"  Player: {playerName}");
            }
        }
        
        [ContextMenu("Start Chase Mode")]
        public void StartChaseMode()
        {
            if (_chaseManager != null)
            {
                if (_chaseManager.IsReady())
                {
                    var playerTransform = _chaseManager.GetPlayerTransform();
                    _chaseManager.StartChaseAll(playerTransform);
                    Debug.Log("AIChaseManagerExample: Started chase mode");
                }
                else
                {
                    Debug.LogWarning("AIChaseManagerExample: Manager not ready yet! Wait for boats to be discovered.");
                }
            }
        }
        
        [ContextMenu("Stop Chase Mode")]
        public void StopChaseMode()
        {
            if (_chaseManager != null)
            {
                _chaseManager.StopAndReturnAll();
                Debug.Log("AIChaseManagerExample: Stopped chase mode");
            }
        }
        
        [ContextMenu("Deactivate All")]
        public void DeactivateAll()
        {
            if (_chaseManager != null)
            {
                _chaseManager.DeactivateAll();
                Debug.Log("AIChaseManagerExample: Deactivated all AI boats");
            }
        }
        
        [ContextMenu("Refresh Boat Discovery")]
        public void RefreshBoatDiscovery()
        {
            if (_chaseManager != null)
            {
                _chaseManager.RefreshBoatDiscovery();
                Debug.Log("AIChaseManagerExample: Refreshed boat discovery");
            }
        }
        
        void OnGUI()
        {
            if (!showDebugInfo || _chaseManager == null) return;
            
            // Simple on-screen debug info
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("AIChaseManager Debug Info", GUI.skin.box);
            GUILayout.Label($"Ready: {_chaseManager.IsReady()}");
            GUILayout.Label($"AI Boats: {_chaseManager.GetManagedBoatCount()}");
            GUILayout.Label($"Player: {(_chaseManager.GetPlayerTransform()?.name ?? "None")}");
            
            if (GUILayout.Button("Start Chase"))
                StartChaseMode();
            if (GUILayout.Button("Stop Chase"))
                StopChaseMode();
            if (GUILayout.Button("Deactivate All"))
                DeactivateAll();
            if (GUILayout.Button("Refresh Boats"))
                RefreshBoatDiscovery();
            
            GUILayout.EndArea();
        }
    }
}
