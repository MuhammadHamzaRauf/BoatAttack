using UnityEngine;
using UnityEngine.UI;

namespace BoatAttack
{
    /// <summary>
    /// Simple controller to manage chase mode from anywhere in the scene.
    /// Can be attached to any GameObject or used as a static utility.
    /// </summary>
    public class ChaseModeController : MonoBehaviour
    {
        [Header("UI References (Optional)")]
        [SerializeField] private Button startChaseButton;
        [SerializeField] private Button stopChaseButton;
        [SerializeField] private Button deactivateButton;
        [SerializeField] private Text chaseStatusText;
        
        [Header("Auto Setup")]
        [SerializeField] private bool setupButtonsOnStart = true;
        
        private void Start()
        {
            if (setupButtonsOnStart)
            {
                SetupButtons();
            }
        }
        
        private void SetupButtons()
        {
            if (startChaseButton != null)
                startChaseButton.onClick.AddListener(StartChaseMode);
                
            if (stopChaseButton != null)
                stopChaseButton.onClick.AddListener(StopChaseMode);
                
            if (deactivateButton != null)
                deactivateButton.onClick.AddListener(DeactivateChaseMode);
        }
        
        private void Update()
        {
            if (chaseStatusText != null)
            {
                int chaseCount = RaceManager.GetChaseModeBoatCount();
                chaseStatusText.text = $"Chase Mode: {chaseCount} boats chasing";
            }
        }
        
        // Public API - can be called from anywhere
        [ContextMenu("Start Chase Mode")]
        public void StartChaseMode()
        {
            RaceManager.StartChaseMode();
        }
        
        [ContextMenu("Stop Chase Mode")]
        public void StopChaseMode()
        {
            RaceManager.StopChaseMode();
        }
        
        [ContextMenu("Deactivate Chase Mode")]
        public void DeactivateChaseMode()
        {
            RaceManager.DeactivateChaseMode();
        }
        
        // Static utility methods for easy access
        public static void StartChase() => RaceManager.StartChaseMode();
        public static void StopChase() => RaceManager.StopChaseMode();
        public static void DeactivateChase() => RaceManager.DeactivateChaseMode();
        public static int GetChaseBoatCount() => RaceManager.GetChaseModeBoatCount();
        
        private void OnDestroy()
        {
            // Clean up button listeners
            if (startChaseButton != null)
                startChaseButton.onClick.RemoveListener(StartChaseMode);
            if (stopChaseButton != null)
                stopChaseButton.onClick.RemoveListener(StopChaseMode);
            if (deactivateButton != null)
                deactivateButton.onClick.RemoveListener(DeactivateChaseMode);
        }
    }
}

