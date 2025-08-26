using UnityEngine;
using UnityEngine.InputSystem;

namespace BoatAttack.BoatInput
{
    /// <summary>
    /// Desktop input adapter using existing InputControls
    /// </summary>
    public class DesktopBoatInput : MonoBehaviour, IBoatInput
    {
        private InputControls _controls;
        
        public float Throttle { get; private set; }
        public float Steer { get; private set; }
        public bool Brake { get; private set; }
        public bool Boost { get; private set; }

        private void Awake()
        {
            _controls = new InputControls();
            
            // Bind to existing input actions
            _controls.BoatControls.Trottle.performed += context => Throttle = context.ReadValue<float>();
            _controls.BoatControls.Trottle.canceled += context => Throttle = 0f;
            
            _controls.BoatControls.Steering.performed += context => Steer = context.ReadValue<float>();
            _controls.BoatControls.Steering.canceled += context => Steer = 0f;
        }

        private void OnEnable()
        {
            _controls?.BoatControls.Enable();
        }

        private void OnDisable()
        {
            _controls?.BoatControls.Disable();
        }

        public void Tick()
        {
            // Values are updated via callbacks, no per-frame work needed
        }

        private void OnDestroy()
        {
            _controls?.Dispose();
        }
    }
}
