using UnityEngine;
using UnityEngine.UI;

namespace BoatAttack.UI
{
    /// <summary>
    /// Manages mobile on-screen controls visibility and setup
    /// </summary>
    public class MobileControlsManager : MonoBehaviour
    {
        [Header("Mobile Controls")]
        [SerializeField] private Canvas mobileCanvas;
        [SerializeField] private bool forceEnable = false;
        
        [Header("Control Elements")]
        [SerializeField] private SimpleMobileControls simpleControls;
        
        private bool _isMobilePlatform;
        
        void Awake()
        {
            // Check if we're on a mobile platform
            _isMobilePlatform = Application.isMobilePlatform;
            
            // Auto-enable on mobile platforms or when forced
            if (_isMobilePlatform || forceEnable)
            {
                EnableMobileControls();
            }
            else
            {
                DisableMobileControls();
            }
        }
        
        void Start()
        {
            // Set up simple controls if available
            if (simpleControls != null)
            {
                Debug.Log("Simple mobile controls configured");
            }
        }
        
        private void EnableMobileControls()
        {
            if (mobileCanvas != null)
            {
                mobileCanvas.gameObject.SetActive(true);
                Debug.Log("Mobile controls enabled");
            }
        }
        
        private void DisableMobileControls()
        {
            if (mobileCanvas != null)
            {
                mobileCanvas.gameObject.SetActive(false);
                Debug.Log("Mobile controls disabled");
            }
        }
        
        // Control event handlers - now handled by SimpleMobileControls
        
        // Public method to toggle controls for testing
        public void ToggleMobileControls()
        {
            if (mobileCanvas != null)
            {
                mobileCanvas.gameObject.SetActive(!mobileCanvas.gameObject.activeSelf);
            }
        }
        
        // Public method to check if mobile controls are active
        public bool AreMobileControlsActive()
        {
            return mobileCanvas != null && mobileCanvas.gameObject.activeSelf;
        }
        
        void OnDestroy()
        {
            // Event listeners are now handled by SimpleMobileControls
        }
    }
}
