using UnityEngine;
using System.Collections.Generic;

namespace BoatAttack.AI
{
    /// <summary>
    /// Simple scene-level manager to toggle Chase Mode globally.
    /// Automatically finds boats and player from RaceManager at Start.
    /// </summary>
    public class AIChaseManager : MonoBehaviour
    {
        [SerializeField] private bool enableOnStart = false;
        
        // Auto-discovered fields
        private Transform _player;
        private List<AIChaseController> _aiBoats = new List<AIChaseController>();

        private void Start()
        {
            // Wait a frame for everything to initialize, then find boats
            Invoke(nameof(FindBoatsAndPlayer), 0.1f);
        }

        /// <summary>
        /// Find all AI boats and the player boat from the scene
        /// </summary>
        private void FindBoatsAndPlayer()
        {
            _aiBoats.Clear();
            _player = null;
            
            // Find all boats in the scene
            var allBoats = FindObjectsOfType<Boat>();
            
            foreach (var boat in allBoats)
            {
                // Check if this is a player boat (has HumanController)
                var humanController = boat.GetComponent<HumanController>();
                if (humanController != null)
                {
                    _player = boat.transform;
                    Debug.Log($"AIChaseManager: Found player boat: {boat.name}");
                    continue;
                }
                
                // Check if this is an AI boat (has AiController)
                var aiController = boat.GetComponent<AiController>();
                if (aiController != null)
                {
                    // Add AIChaseController if not already present
                    var chaseController = boat.GetComponent<AIChaseController>();
                    if (chaseController == null)
                    {
                        chaseController = boat.gameObject.AddComponent<AIChaseController>();
                        Debug.Log($"AIChaseManager: Added AIChaseController to {boat.name}");
                    }
                    _aiBoats.Add(chaseController);
                }
            }
            
            Debug.Log($"AIChaseManager: Found {_aiBoats.Count} AI boats and player: {(_player != null ? _player.name : "None")}");
            
            // // Start chase mode if enabled
            // if (enableOnStart && _player != null && _aiBoats.Count > 0)
            // {
            //     StartChaseAll(_player);
            // }
        }

        public void StartChaseAll(Transform target)
        {
            if (target == null)
            {
                Debug.LogWarning("AIChaseManager: No target provided for chase mode!");
                return;
            }
            
            if (_aiBoats.Count == 0)
            {
                Debug.LogWarning("AIChaseManager: No AI boats found!");
                return;
            }
            
            foreach (var ai in _aiBoats)
            {
                if (ai != null)
                {
                    ai.StartChasing(target);
                }
            }
            
            Debug.Log($"AIChaseManager: Started chase mode for {_aiBoats.Count} AI boats");
        }

        public void StopAndReturnAll()
        {
            if (_aiBoats.Count == 0) return;
            
            foreach (var ai in _aiBoats)
            {
                if (ai != null)
                {
                    ai.StopChasingAndReturnToBase();
                }
            }
            
            Debug.Log($"AIChaseManager: Stopped chase mode for {_aiBoats.Count} AI boats");
        }
        
        /// <summary>
        /// Manually refresh the boat discovery
        /// Useful if boats are added/removed dynamically
        /// </summary>
        [ContextMenu("Refresh Boat Discovery")]
        public void RefreshBoatDiscovery()
        {
            FindBoatsAndPlayer();
        }
        
        /// <summary>
        /// Get the current number of AI boats managed by this manager
        /// </summary>
        public int GetManagedBoatCount()
        {
            return _aiBoats.Count;
        }
        
        /// <summary>
        /// Get the current player transform
        /// </summary>
        public Transform GetPlayerTransform()
        {
            return _player;
        }
        
        /// <summary>
        /// Check if the manager has found boats and is ready
        /// </summary>
        public bool IsReady()
        {
            return _player != null && _aiBoats.Count > 0;
        }
    }
}



