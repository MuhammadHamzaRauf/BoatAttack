    using UnityEngine;
using UnityEngine.InputSystem;

namespace BoatAttack.BoatInput
{
    /// <summary>
    /// Mobile input adapter using touch controls
    /// </summary>
    public class MobileBoatInput : MonoBehaviour, IBoatInput
    {
        [Header("Touch Controls")]
        [SerializeField] private float throttleSensitivity = 1f;
        [SerializeField] private float steerSensitivity = 1f;
        [SerializeField] private float brakeThreshold = 0.3f;
        
        public float Throttle { get; private set; }
        public float Steer { get; private set; }
        public bool Brake { get; private set; }
        public bool Boost { get; private set; }

        private InputControls _controls;
        private Vector2 _touchPosition;
        private bool _isTouching;
        
        // Direct control values for button-based input
        private float _directThrottle;
        private float _directSteer;

        private void Awake()
        {
            _controls = new InputControls();
            
            // Bind to touch input
            _controls.BoatControls.Trottle.performed += context => _touchPosition.y = context.ReadValue<float>();
            _controls.BoatControls.Steering.performed += context => _touchPosition.x = context.ReadValue<float>();
            
            // Touch state
            _controls.BoatControls.Trottle.started += context => _isTouching = true;
            _controls.BoatControls.Trottle.canceled += context => _isTouching = false;
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
            // Use direct control values if set, otherwise use touch input
            if (Mathf.Abs(_directThrottle) > 0.01f || Mathf.Abs(_directSteer) > 0.01f)
            {
                Throttle = _directThrottle;
                Steer = _directSteer;
                Brake = false;
            }
            else if (_isTouching)
            {
                // Convert touch position to throttle (-1 to 1)
                // Y position controls throttle (bottom = reverse, top = forward)
                Throttle = Mathf.Clamp(_touchPosition.y * throttleSensitivity, -1f, 1f);
                
                // X position controls steering (-1 to 1)
                Steer = Mathf.Clamp(_touchPosition.x * steerSensitivity, -1f, 1f);
                
                // Brake when throttle is very low (near center)
                Brake = Mathf.Abs(Throttle) < brakeThreshold;
            }
            else
            {
                // Reset to neutral when not touching
                Throttle = 0f;
                Steer = 0f;
                Brake = false;
            }
            
            // Boost not implemented for mobile
            Boost = false;
        }

        // Public methods for direct control
        public void SetThrottle(float value)
        {
            _directThrottle = Mathf.Clamp(value, -1f, 1f);
        }
        
        public void SetSteer(float value)
        {
            _directSteer = Mathf.Clamp(value, -1f, 1f);
        }
        
        public void ResetDirectControls()
        {
            _directThrottle = 0f;
            _directSteer = 0f;
        }
        
        private void OnDestroy()
        {
            _controls?.Dispose();
        }
    }
}
