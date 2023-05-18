using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;

namespace MMI
{
    using HandGestures = InputSubsystem.Extensions.DeviceFeatureUsages.HandGesture;
    using GestureClassification = InputSubsystem.Extensions.MLGestureClassification;

    public class GestureTracking : MonoBehaviour
    {
        public Transform LeftTransform;
        public Transform RightTransform;
        public Transform LeftInteractionPoint;
        public Transform RightInteractionPoint;

        [SerializeField, Tooltip("The text used to display status information for the example.")]
        private Text statusText = null;
        private string _statusStringBuilder = "Starting Tracking...";
        private InputDevice _leftHandDevice;
        private InputDevice _rightHandDevice;
        private GestureClassification.PostureType _leftPosture;

        private GestureClassification.PostureType _rightPosture;

        private GestureClassification.KeyPoseType _leftKeyPose;

        private GestureClassification.KeyPoseType _rightKeyPose;

        public GestureClassification.PostureType LeftPosture
        {
            get
            {
                return _leftPosture;
            }
        }
        public GestureClassification.PostureType RightPosture
        {
            get
            {
                return _rightPosture;
            }
        }
        public GestureClassification.KeyPoseType LeftKeyPose
        {
            get
            {
                return _leftKeyPose;
            }
        }
        public GestureClassification.KeyPoseType RightKeyPose
        {
            get
            {
                return _rightKeyPose;
            }
        }


        public void Init()
        {
            if (!MLPermissions.CheckPermission(MLPermission.HandTracking).IsOk)
            {
                Debug.LogError($"You must include the {MLPermission.HandTracking} permission in the AndroidManifest.xml to run this example.");
                return;
            }

            if (LeftTransform && RightTransform && LeftInteractionPoint && RightInteractionPoint && statusText)
            {
                GestureClassification.StartTracking();
            }
            else
            {
                Debug.LogError($"One or More required references are missing from the GestureTrackingExample script. Example is disabled until fixed.");
                return;
            }
        }

        public void ProcessAbility()
        {
            if (!_leftHandDevice.isValid || !_rightHandDevice.isValid)
            {
                List<InputDevice> foundDevices = new List<InputDevice>();
                InputDevices.GetDevices(foundDevices);

                foreach (InputDevice device in foundDevices)
                {
                    if (device.name == GestureClassification.LeftGestureInputDeviceName)
                    {
                        _leftHandDevice = device;
                        continue;
                    }

                    if (device.name == GestureClassification.RightGestureInputDeviceName)
                    {
                        _rightHandDevice = device;
                        continue;
                    }

                    if (_leftHandDevice.isValid && _rightHandDevice.isValid)
                    {
                        break;
                    }
                }
                return;
            }

            // Check Enabled Status - Confirms a valid Handle, so only need to check one hand
            _leftHandDevice.TryGetFeatureValue(HandGestures.GesturesEnabled, out bool leftEnableCheck);

            _statusStringBuilder = "Gesture Tracking Enabled: " + leftEnableCheck.ToString();

            if (leftEnableCheck)
            {
                // Hand Transforms
                _leftHandDevice.TryGetFeatureValue(HandGestures.GestureTransformPosition, out Vector3 leftPos);
                _leftHandDevice.TryGetFeatureValue(HandGestures.GestureTransformRotation, out Quaternion leftRot);

                LeftTransform.localPosition = leftPos;
                LeftTransform.localRotation = leftRot;

                _rightHandDevice.TryGetFeatureValue(HandGestures.GestureTransformPosition, out Vector3 rightPos);
                _rightHandDevice.TryGetFeatureValue(HandGestures.GestureTransformRotation, out Quaternion rightRot);

                RightTransform.localPosition = rightPos;
                RightTransform.localRotation = rightRot;

                // Interaction Points
                _leftHandDevice.TryGetFeatureValue(HandGestures.GestureInteractionPosition, out Vector3 leftIntPos);
                _leftHandDevice.TryGetFeatureValue(HandGestures.GestureInteractionRotation, out Quaternion leftIntRot);

                LeftInteractionPoint.localPosition = leftIntPos;
                LeftInteractionPoint.localRotation = leftIntRot;

                _rightHandDevice.TryGetFeatureValue(HandGestures.GestureInteractionPosition, out Vector3 rightIntPos);
                _rightHandDevice.TryGetFeatureValue(HandGestures.GestureInteractionRotation, out Quaternion rightIntRot);

                RightInteractionPoint.localPosition = rightIntPos;
                RightInteractionPoint.localRotation = rightIntRot;

                // Posture
                GestureClassification.TryGetHandPosture(_leftHandDevice, out _leftPosture);
                GestureClassification.TryGetHandPosture(_rightHandDevice, out _rightPosture);

                _statusStringBuilder += "\n\n<color=#B7B7B8><b>Left Posture</b></color>: " + _leftPosture.ToString();
                _statusStringBuilder += "\n<color=#B7B7B8><b>Right Posture</b></color>: " + _rightPosture.ToString();

                // KeyPose
                GestureClassification.TryGetHandKeyPose(_leftHandDevice, out _leftKeyPose);
                GestureClassification.TryGetHandKeyPose(_rightHandDevice, out _rightKeyPose);

                _statusStringBuilder += "\n\n<color=#B7B7B8><b>Left KeyPose</b></color>: " + _leftKeyPose.ToString();
                _statusStringBuilder += "\n<color=#B7B7B8><b>Right KeyPose</b></color>: " + _rightKeyPose.ToString();
            }
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            statusText.text = $"<color=#B7B7B8><b>Gesture Tracking Data</b></color>\n{_statusStringBuilder}";
        }
    }
}
