using UnityEngine;
using BoatAttack.BoatInput;

namespace BoatAttack.Utility
{
    /// <summary>
    /// Utility script to test mobile input functionality
    /// </summary>
    public class MobileInputTest : MonoBehaviour
    {
        [Header("Input Testing")]
        [SerializeField] private MobileBoatInput mobileInput;
        [SerializeField] private bool showDebugInfo = true;
        
        void Start()
        {
            if (mobileInput == null)
            {
                mobileInput = FindObjectOfType<MobileBoatInput>();
            }
        }
        
        void OnGUI()
        {
            if (!showDebugInfo || mobileInput == null) return;
            
            GUILayout.BeginArea(new Rect(10, 250, 300, 200));
            GUILayout.Label("Mobile Input Test", GUI.skin.box);
            
            if (mobileInput != null)
            {
                GUILayout.Label($"Throttle: {mobileInput.Throttle:F2}");
                GUILayout.Label($"Steer: {mobileInput.Steer:F2}");
                GUILayout.Label($"Brake: {mobileInput.Brake}");
                GUILayout.Label($"Boost: {mobileInput.Boost}");
            }
            else
            {
                GUILayout.Label("No MobileBoatInput found!");
            }
            
            GUILayout.EndArea();
        }
        
        [ContextMenu("Find Mobile Input")]
        public void FindMobileInput()
        {
            mobileInput = FindObjectOfType<MobileBoatInput>();
            if (mobileInput != null)
            {
                Debug.Log($"Found MobileBoatInput: {mobileInput.name}");
            }
            else
            {
                Debug.LogWarning("No MobileBoatInput found in scene!");
            }
        }
    }
}
