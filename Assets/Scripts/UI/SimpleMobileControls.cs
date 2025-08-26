using UnityEngine;
using UnityEngine.UI;

namespace BoatAttack.UI
{
    /// <summary>
    /// Simple mobile controls with basic buttons for boat movement
    /// Works with the updated HumanController system
    /// </summary>
    public class SimpleMobileControls : MonoBehaviour
    {
        [Header("Control Buttons")]
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button forwardButton;
        [SerializeField] private Button reverseButton;
        
        [Header("Settings")]
        [SerializeField] private float steerValue = 0.5f;
        [SerializeField] private float throttleValue = 0.5f;
        
        private HumanController _humanController;
        
        void Start()
        {
            // Find the HumanController component
            _humanController = FindObjectOfType<HumanController>();
            if (_humanController == null)
            {
                Debug.LogWarning("SimpleMobileControls: No HumanController found in scene!");
                return;
            }
            
            SetupButtonListeners();
        }
        
        private void SetupButtonListeners()
        {
            if (leftButton != null)
                leftButton.onClick.AddListener(OnLeftPressed);
                
            if (rightButton != null)
                rightButton.onClick.AddListener(OnRightPressed);
                
            if (forwardButton != null)
                forwardButton.onClick.AddListener(OnForwardPressed);
                
            if (reverseButton != null)
                reverseButton.onClick.AddListener(OnReversePressed);
        }
        
        public void OnLeftPressed()
        {
            if (_humanController != null)
            {
                _humanController.SetSteer(-steerValue);
            }
        }
        
        public void OnRightPressed()
        {
            if (_humanController != null)
            {
                _humanController.SetSteer(steerValue);
            }
        }
        
        public void OnForwardPressed()
        {
            if (_humanController != null)
            {
                _humanController.SetThrottle(throttleValue);
            }
        }
        
        public void OnReversePressed()
        {
            if (_humanController != null)
            {
                _humanController.SetThrottle(-throttleValue);
            }
        }
        
        // Public methods for external control
        public void SetSteerValue(float value) => steerValue = Mathf.Abs(value);
        public void SetThrottleValue(float value) => throttleValue = Mathf.Abs(value);
        
        void OnDestroy()
        {
            // Clean up button listeners
            if (leftButton != null)
                leftButton.onClick.RemoveListener(OnLeftPressed);
            if (rightButton != null)
                rightButton.onClick.RemoveListener(OnRightPressed);
            if (forwardButton != null)
                forwardButton.onClick.RemoveListener(OnForwardPressed);
            if (reverseButton != null)
                reverseButton.onClick.RemoveListener(OnReversePressed);
        }
    }
}
