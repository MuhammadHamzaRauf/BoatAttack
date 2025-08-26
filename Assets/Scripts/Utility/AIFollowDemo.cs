using UnityEngine;
using BoatAttack.AI;

namespace BoatAttack.Utility
{
    /// <summary>
    /// Utility script to easily set up AI follow mode on boats
    /// </summary>
    public class AIFollowDemo : MonoBehaviour
    {
        [Header("AI Follow Setup")]
        [SerializeField] private GameObject aiBoat;
        [SerializeField] private GameObject playerBoat;
        [SerializeField] private bool enableOnStart = true;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = true;
        
        private AIFollowInput _aiFollowInput;
        
        void Start()
        {
            if (enableOnStart)
            {
                SetupAIFollow();
            }
        }
        
        [ContextMenu("Setup AI Follow")]
        public void SetupAIFollow()
        {
            if (aiBoat == null)
            {
                Debug.LogError("AIFollowDemo: No AI boat assigned!");
                return;
            }
            
            if (playerBoat == null)
            {
                // Try to find player boat
                playerBoat = GameObject.FindGameObjectWithTag("Player");
                if (playerBoat == null)
                {
                    Debug.LogError("AIFollowDemo: No player boat found! Make sure your player boat has the 'Player' tag.");
                    return;
                }
            }
            
            // Add AI follow input component if it doesn't exist
            _aiFollowInput = aiBoat.GetComponent<AIFollowInput>();
            if (_aiFollowInput == null)
            {
                _aiFollowInput = aiBoat.AddComponent<AIFollowInput>();
                Debug.Log($"AIFollowDemo: Added AIFollowInput to {aiBoat.name}");
            }
            
            // Set the target
            _aiFollowInput.SetTarget(playerBoat.transform);
            Debug.Log($"AIFollowDemo: {aiBoat.name} will now follow {playerBoat.name}");
        }
        
        [ContextMenu("Remove AI Follow")]
        public void RemoveAIFollow()
        {
            if (_aiFollowInput != null)
            {
                DestroyImmediate(_aiFollowInput);
                _aiFollowInput = null;
                Debug.Log($"AIFollowDemo: Removed AIFollowInput from {aiBoat.name}");
            }
        }
        
        void OnGUI()
        {
            if (!showDebugInfo || _aiFollowInput == null) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label("AI Follow Demo", GUI.skin.box);
            
            if (GUILayout.Button("Setup AI Follow"))
            {
                SetupAIFollow();
            }
            
            if (GUILayout.Button("Remove AI Follow"))
            {
                RemoveAIFollow();
            }
            
            if (_aiFollowInput != null)
            {
                GUILayout.Label($"AI Boat: {aiBoat?.name ?? "None"}");
                GUILayout.Label($"Target: {playerBoat?.name ?? "None"}");
                GUILayout.Label($"Throttle: {_aiFollowInput.Throttle:F2}");
                GUILayout.Label($"Steer: {_aiFollowInput.Steer:F2}");
                GUILayout.Label($"Brake: {_aiFollowInput.Brake}");
            }
            
            GUILayout.EndArea();
        }
    }
}
