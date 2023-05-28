using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;
using InputDevice = UnityEngine.XR.InputDevice;

namespace MMI
{
    /// <summary>
    /// Behaviour that moves object to fixation point position.
    /// </summary>
    public class EyeTracking : MonoBehaviour
    {
        [SerializeField, Tooltip("Left Eye Statistic Panel")]
        private Text leftEyeTextStatic;
        [SerializeField, Tooltip("Right Eye Statistic Panel")]
        private Text rightEyeTextStatic;
        [SerializeField, Tooltip("Both Eyes Statistic Panel")]
        private Text bothEyesTextStatic;
        [SerializeField, Tooltip("Fixation Point marker")]
        private Transform eyesFixationPoint;

        // Used to get ml inputs.
        private MagicLeapInputs _mlInputs;

        // Used to get eyes action data.
        private MagicLeapInputs.EyesActions _eyesActions;
        public Vector3 EyesFixationPoint
        {
            get
            {
                return _eyesActions.Data.ReadValue<UnityEngine.InputSystem.XR.Eyes>().fixationPoint;
            }
        }

        // Used to get other eye data
        private InputDevice _eyesDevice;

        // Was EyeTracking permission granted by user
        private bool _permissionGranted = false;
        private readonly MLPermissions.Callbacks _permissionCallbacks = new MLPermissions.Callbacks();
        [SerializeField] bool isDebugActive = true;
        [SerializeField] GameObject debugGameObjects;
        private void Awake()
        {
            _permissionCallbacks.OnPermissionGranted += OnPermissionGranted;
            _permissionCallbacks.OnPermissionDenied += OnPermissionDenied;
            _permissionCallbacks.OnPermissionDeniedAndDontAskAgain += OnPermissionDenied;
        }

        public void Init()
        {
            _mlInputs = new MagicLeapInputs();
            _mlInputs.Enable();

            MLPermissions.RequestPermission(MLPermission.EyeTracking, _permissionCallbacks);
            SetDebugElementsActive();
        }

        public void ProcessAbility()
        {
            if (!_permissionGranted)
            {
                return;
            }

            if (!_eyesDevice.isValid)
            {
                this._eyesDevice = InputSubsystem.Utils.FindMagicLeapDevice(InputDeviceCharacteristics.EyeTracking | InputDeviceCharacteristics.TrackedDevice);
                return;
            }

            // Eye data provided by the engine for all XR devices.
            // Used here only to update the status text. The 
            // left/right eye centers are moved to their respective positions &
            // orientations using InputSystem's TrackedPoseDriver component.
            var eyes = _eyesActions.Data.ReadValue<UnityEngine.InputSystem.XR.Eyes>();

            // Manually set fixation point marker so we can apply rotation, since UnityXREyes
            // does not provide it
            eyesFixationPoint.position = eyes.fixationPoint;
            eyesFixationPoint.rotation = Quaternion.LookRotation(eyes.fixationPoint - Camera.main.transform.position);

            // Eye data specific to Magic Leap
            InputSubsystem.Extensions.TryGetEyeTrackingState(_eyesDevice, out var trackingState);

            var leftEyeForwardGaze = eyes.leftEyeRotation * Vector3.forward;

            string leftEyeText =
                $"Center:\n({eyes.leftEyePosition.x:F2}, {eyes.leftEyePosition.y:F2}, {eyes.leftEyePosition.z:F2})\n" +
                $"Gaze:\n({leftEyeForwardGaze.x:F2}, {leftEyeForwardGaze.y:F2}, {leftEyeForwardGaze.z:F2})\n" +
                $"Confidence:\n{trackingState.LeftCenterConfidence:F2}\n" +
                $"Pupil Size:\n{eyes.leftEyeOpenAmount:F2}";

            leftEyeTextStatic.text = leftEyeText;

            var rightEyeForwardGaze = eyes.rightEyeRotation * Vector3.forward;

            string rightEyeText =
                $"Center:\n({eyes.rightEyePosition.x:F2}, {eyes.rightEyePosition.y:F2}, {eyes.rightEyePosition.z:F2})\n" +
                $"Gaze:\n({rightEyeForwardGaze.x:F2}, {rightEyeForwardGaze.y:F2}, {rightEyeForwardGaze.z:F2})\n" +
                $"Confidence:\n{trackingState.RightCenterConfidence:F2}\n" +
                $"Pupil Size:\n{eyes.rightEyeOpenAmount:F2}";

            rightEyeTextStatic.text = rightEyeText;

            string bothEyesText =
                $"Fixation Point:\n({eyes.fixationPoint.x:F2}, {eyes.fixationPoint.y:F2}, {eyes.fixationPoint.z:F2})\n" +
                $"Confidence:\n{trackingState.FixationConfidence:F2}";

            bothEyesTextStatic.text = $"{bothEyesText}";
        }

        private void OnDestroy()
        {
            _permissionCallbacks.OnPermissionGranted -= OnPermissionGranted;
            _permissionCallbacks.OnPermissionDenied -= OnPermissionDenied;
            _permissionCallbacks.OnPermissionDeniedAndDontAskAgain -= OnPermissionDenied;

            _mlInputs.Disable();
            _mlInputs.Dispose();

            InputSubsystem.Extensions.MLEyes.StopTracking();
        }

        private void OnPermissionDenied(string permission)
        {
            MLPluginLog.Error($"{permission} denied, example won't function.");
        }

        private void OnPermissionGranted(string permission)
        {
            InputSubsystem.Extensions.MLEyes.StartTracking();
            _eyesActions = new MagicLeapInputs.EyesActions(_mlInputs);
            _permissionGranted = true;
        }

        public void ToggleDebugElements()
        {
            isDebugActive = !isDebugActive;
            SetDebugElementsActive();
        }

        void SetDebugElementsActive()
        {
            debugGameObjects.SetActive(isDebugActive);
        }
    }
}

