using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace BoatAttack
{
    /// <summary>
    /// This sends input controls to the boat engine if 'Human'
    /// Handles both desktop (keyboard/gamepad) and mobile (touch/buttons) input
    /// </summary>
    public class HumanController : BaseController
    {
        [Header("Input Settings")]
        [SerializeField] private bool enableMobileInput = true;
        [SerializeField] private bool enableDesktopInput = true;
        [SerializeField] private bool enableDebugLogging = false;
        
        [Header("Mobile Input")]
        [SerializeField] private float mobileThrottleSensitivity = 1f;
        [SerializeField] private float mobileSteerSensitivity = 1f;
        [SerializeField] private float mobileBrakeThreshold = 0.3f;
        [SerializeField] private bool useMobileSmoothing = true;
        [SerializeField] private float mobileSmoothing = 5f;
        
        // Input Controls for desktop
        private InputControls _controls;
        
        // Input values
        private float _throttle;
        private float _steering;
        private bool _paused;
        
        // Mobile input values
        private float _mobileThrottle;
        private float _mobileSteer;
        private float _mobileThrottleVelocity;
        private float _mobileSteerVelocity;
        
        // Mobile button references
        private Button _leftButton;
        private Button _rightButton;
        private Button _forwardButton;
        private Button _reverseButton;
        
        private void Awake()
        {
            // Setup desktop input
            if (enableDesktopInput)
            {
                SetupDesktopInput();
            }
            
            // Setup mobile input
            if (enableMobileInput)
            {
                SetupMobileInput();
            }
        }
        
        private void SetupDesktopInput()
        {
            _controls = new InputControls();
            
            _controls.BoatControls.Trottle.performed += context => {
                if (enableDesktopInput) _throttle = context.ReadValue<float>();
                if (enableDebugLogging) Debug.Log($"Desktop Throttle: {_throttle}");
            };
            _controls.BoatControls.Trottle.canceled += context => {
                if (enableDesktopInput) _throttle = 0f;
            };
            
            _controls.BoatControls.Steering.performed += context => {
                if (enableDesktopInput) _steering = context.ReadValue<float>();
                if (enableDebugLogging) Debug.Log($"Desktop Steer: {_steering}");
            };
            _controls.BoatControls.Steering.canceled += context => {
                if (enableDesktopInput) _steering = 0f;
            };

            _controls.BoatControls.Reset.performed += ResetBoat;
            _controls.BoatControls.Pause.performed += FreezeBoat;
            _controls.DebugControls.TimeOfDay.performed += SelectTime;
        }
        
        private void SetupMobileInput()
        {
            // Find mobile control buttons in the scene
            FindMobileButtons();
            
            // Setup button listeners
            SetupButtonListeners();
        }
        
        private void FindMobileButtons()
        {
            // Look for buttons with specific names or tags
            var allButtons = FindObjectsOfType<Button>();
            
            foreach (var button in allButtons)
            {
                var buttonName = button.name.ToLower();
                
                if (buttonName.Contains("left") || buttonName.Contains("turnleft"))
                    _leftButton = button;
                else if (buttonName.Contains("right") || buttonName.Contains("turnright"))
                    _rightButton = button;
                else if (buttonName.Contains("forward") || buttonName.Contains("accelerate") || buttonName.Contains("throttle"))
                    _forwardButton = button;
                else if (buttonName.Contains("reverse") || buttonName.Contains("brake") || buttonName.Contains("backward"))
                    _reverseButton = button;
            }
            
            if (enableDebugLogging)
            {
                Debug.Log($"Mobile buttons found - Left: {_leftButton != null}, Right: {_rightButton != null}, " +
                         $"Forward: {_forwardButton != null}, Reverse: {_reverseButton != null}");
            }
        }
        
        private void SetupButtonListeners()
        {
            if (_leftButton != null)
                _leftButton.onClick.AddListener(() => SetMobileSteer(-1f));
                
            if (_rightButton != null)
                _rightButton.onClick.AddListener(() => SetMobileSteer(1f));
                
            if (_forwardButton != null)
                _forwardButton.onClick.AddListener(() => SetMobileThrottle(1f));
                
            if (_reverseButton != null)
                _reverseButton.onClick.AddListener(() => SetMobileThrottle(-1f));
        }
        
        private void SetMobileThrottle(float value)
        {
            _mobileThrottle = Mathf.Clamp(value, -1f, 1f);
            if (enableDebugLogging) Debug.Log($"Mobile Throttle Set: {_mobileThrottle}");
        }
        
        private void SetMobileSteer(float value)
        {
            _mobileSteer = Mathf.Clamp(value, -1f, 1f);
            if (enableDebugLogging) Debug.Log($"Mobile Steer Set: {_mobileSteer}");
        }
        
        public void ResetMobileInput()
        {
            _mobileThrottle = 0f;
            _mobileSteer = 0f;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            
            if (enableDesktopInput && _controls != null)
            {
                _controls.BoatControls.Enable();
            }
        }

        private void OnDisable()
        {
            if (enableDesktopInput && _controls != null)
            {
                _controls.BoatControls.Disable();
            }
        }

        private void ResetBoat(InputAction.CallbackContext context)
        {
            controller.ResetPosition();
        }

        private void FreezeBoat(InputAction.CallbackContext context)
        {
            _paused = !_paused;
            if(_paused)
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }

        private void SelectTime(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();
            Debug.Log($"changing day time, input:{value}");
            DayNightController.SelectPreset(value);
        }

        void FixedUpdate()
        {
            float finalThrottle = 0f;
            float finalSteering = 0f;
            
            // Combine desktop and mobile input
            if (enableDesktopInput)
            {
                finalThrottle += _throttle;
                finalSteering += _steering;
            }
            
            if (enableMobileInput)
            {
                // Apply mobile input smoothing if enabled
                if (useMobileSmoothing)
                {
                    _mobileThrottle = Mathf.SmoothDamp(_mobileThrottle, _mobileThrottle, ref _mobileThrottleVelocity, 1f / mobileSmoothing);
                    _mobileSteer = Mathf.SmoothDamp(_mobileSteer, _mobileSteer, ref _mobileSteerVelocity, 1f / mobileSmoothing);
                }
                
                finalThrottle += _mobileThrottle;
                finalSteering += _mobileSteer;
            }
            
            // Clamp final values
            finalThrottle = Mathf.Clamp(finalThrottle, -1f, 1f);
            finalSteering = Mathf.Clamp(finalSteering, -1f, 1f);
            
            // Apply to engine
            if (engine != null)
            {
                engine.Accelerate(finalThrottle);
                engine.Turn(finalSteering);
            }
            
            if (enableDebugLogging && (Mathf.Abs(finalThrottle) > 0.1f || Mathf.Abs(finalSteering) > 0.1f))
            {
                Debug.Log($"Final Input - Throttle: {finalThrottle:F2}, Steer: {finalSteering:F2}");
            }
        }
        
        private void OnDestroy()
        {
            if (_controls != null)
            {
                _controls?.Dispose();
            }
        }
        
        // Public methods for external control
        public void SetThrottle(float value) => SetMobileThrottle(value);
        public void SetSteer(float value) => SetMobileSteer(value);
        public float GetThrottle() => _throttle + _mobileThrottle;
        public float GetSteer() => _steering + _mobileSteer;
    }
}

