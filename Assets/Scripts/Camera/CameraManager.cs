using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.InputSystem;

namespace BoatAttack
{
	/// <summary>
	/// This is an overall camera manager for the demo(mainly for testing/debugging purposes) it hooks up to teh UI interface
	/// </summary>
    public class CameraManager : MonoBehaviour
    {
        public GameObject UI;
        public CameraModes _camModes;
        public PlayableDirector _cutsceneDirector;
        public List<CinemachineVirtualCamera> _cutsceneCameras = new List<CinemachineVirtualCamera>();
        public CinemachineVirtualCamera _droneCamera;
        public CinemachineVirtualCamera _raceCamera;
        public CinemachineClearShot _replayShots;
        public Text _staticCamText;
        private int _curStaticCam = 0;

        // Input System
        private InputControls _controls;
        private bool _cameraTogglePressed;
        private bool _nextCameraPressed;
        private bool _prevCameraPressed;
        private bool _toggleUIPressed;

        private void Awake()
        {
            _controls = new InputControls();
            
            // Bind to input actions
            _controls.BoatControls.Trottle.performed += context => {
                if (context.ReadValue<float>() > 0.5f) // Space key is bound to Throttle
                {
                    _cameraTogglePressed = true;
                }
            };
            _controls.BoatControls.Trottle.canceled += context => _cameraTogglePressed = false;
            
            _controls.BoatControls.Steering.performed += context => {
                float value = context.ReadValue<float>();
                if (value > 0.5f) // Right arrow
                {
                    _nextCameraPressed = true;
                }
                else if (value < -0.5f) // Left arrow
                {
                    _prevCameraPressed = true;
                }
            };
            _controls.BoatControls.Steering.canceled += context => {
                _nextCameraPressed = false;
                _prevCameraPressed = false;
            };

            // Handle H key for UI toggle
            _controls.BoatControls.Pause.performed += context => {
                _toggleUIPressed = true;
            };
        }

        private void OnEnable()
        {
            _controls?.BoatControls.Enable();
        }

        private void OnDisable()
        {
            _controls?.BoatControls.Disable();
        }

        private void OnDestroy()
        {
            _controls?.Dispose();
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            // Handle camera toggle (Space key)
            if (_cameraTogglePressed)
            {
                if (_camModes == CameraModes.Cutscene)
                    StaticCams();
                else
                    PlayCutscene();
                _cameraTogglePressed = false;
            }

            // Handle next camera (Right arrow)
            if (_nextCameraPressed)
            {
                NextStaticCam();
                _nextCameraPressed = false;
            }

            // Handle previous camera (Left arrow)
            if (_prevCameraPressed)
            {
                PrevStaticCam();
                _prevCameraPressed = false;
            }

            // Handle UI toggle (H key or double tap)
            if (_toggleUIPressed)
            {
                UI.SetActive(!UI.activeSelf);
                _toggleUIPressed = false;
            }
        }
        public void PlayCutscene()
        {
            _camModes = CameraModes.Cutscene;
            // Lower other camera priorities
            _droneCamera.Priority = 5;
            _raceCamera.Priority = 5;
            _replayShots.Priority = 5;
            // activate cutscene
            _cutsceneDirector.enabled = true;
            _cutsceneDirector.Stop();
            _cutsceneDirector.Play();
        }

        void DisableCutscene()
        {
            _cutsceneDirector.enabled = false;
            _cutsceneDirector.Stop();
        }

        public void DroneCam()
        {
            _camModes = CameraModes.Drone;
            // Lower other camera priorities
            DisableCutscene();
            _raceCamera.Priority = 5;
            _replayShots.Priority = 5;
            // activate drone
            _droneCamera.Priority = 15;
        }

        public void RaceCam()
        {
            _camModes = CameraModes.Race;
            // Lower other camera priorities
            DisableCutscene();
            _droneCamera.Priority = 5;
            _replayShots.Priority = 5;
            // activate drone
            _raceCamera.Priority = 15;
        }

        public void ReplayCam()
        {
            _camModes = CameraModes.Replay;
            // Lower other camera priorities
            DisableCutscene();
            _droneCamera.Priority = 5;
            _raceCamera.Priority = 5;
            // activate drone
            _replayShots.Priority = 15;
        }

        public void StaticCams()
        {
            _camModes = CameraModes.Static;
            // Lower other camera priorities
            DisableCutscene();
            _droneCamera.Priority = 5;
            _raceCamera.Priority = 5;
            _replayShots.Priority = 5;
            SetStaticCam(_curStaticCam);
        }

        public void NextStaticCam()
        {
            _curStaticCam++;
            if (_curStaticCam == _cutsceneCameras.Count)
                _curStaticCam = 0;
            SetStaticCam(_curStaticCam);
        }

        public void PrevStaticCam()
        {
            _curStaticCam--;
            if (_curStaticCam < 0)
                _curStaticCam = _cutsceneCameras.Count - 1;
            SetStaticCam(_curStaticCam);
        }

        void SetStaticCam(int cameraIndex)
        {
            for (var i = 0; i < _cutsceneCameras.Count; i++)
            {
                if (i != cameraIndex)
                {
                    _cutsceneCameras[i].Priority = 5;
                }
                else
                {
                    _cutsceneCameras[i].Priority = 11;
                    _staticCamText.text = _cutsceneCameras[i].gameObject.name.Substring(9);
                }
            }
        }

        public enum CameraModes
        {
            Cutscene,
            Race,
            Drone,
            Replay,
            Static
        }
    }
}
