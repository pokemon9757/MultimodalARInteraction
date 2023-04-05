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

    public class GestureTracking : MonoBehaviour, ITrackingAbility
    {
        public Transform LeftTransform;
        public Transform RightTransform;
        public Transform LeftInteractionPoint;
        public Transform RightInteractionPoint;

        [SerializeField, Tooltip("The text used to display status information for the example.")]
        private Text statusText = null;
        private string statusStringBuilder = "Starting Tracking...";
        private InputDevice leftHandDevice;
        private InputDevice rightHandDevice;

        [SerializeField] GestureClassification.KeyPoseType[] _trackedKeyPose;
        [SerializeField] GestureClassification.PostureType[] _trackedPostureType;

        Dictionary<GestureClassification.KeyPoseType, bool> _trackedKeyPoseDict = new();
        Dictionary<GestureClassification.PostureType, bool> _trackedPostureTypeDict = new();

        public event UnityAction<GestureClassification.KeyPoseType, GestureClassification.PostureType> OnGestureDetected;

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
            foreach (var pose in _trackedKeyPose)
            {
                _trackedKeyPoseDict[pose] = true;
            }
            foreach (var posture in _trackedPostureType)
            {
                _trackedPostureTypeDict[posture] = true;
            }
        }
        public void ProcessAbility()
        {
            if (!leftHandDevice.isValid || !rightHandDevice.isValid)
            {
                List<InputDevice> foundDevices = new List<InputDevice>();
                InputDevices.GetDevices(foundDevices);

                foreach (InputDevice device in foundDevices)
                {
                    if (device.name == GestureClassification.LeftGestureInputDeviceName)
                    {
                        leftHandDevice = device;
                        continue;
                    }

                    if (device.name == GestureClassification.RightGestureInputDeviceName)
                    {
                        rightHandDevice = device;
                        continue;
                    }

                    if (leftHandDevice.isValid && rightHandDevice.isValid)
                    {
                        break;
                    }
                }
                return;
            }

            // Check Enabled Status - Confirms a valid Handle, so only need to check one hand
            leftHandDevice.TryGetFeatureValue(HandGestures.GesturesEnabled, out bool leftEnableCheck);

            statusStringBuilder = "Gesture Tracking Enabled: " + leftEnableCheck.ToString();

            if (leftEnableCheck)
            {
                // Hand Transforms
                leftHandDevice.TryGetFeatureValue(HandGestures.GestureTransformPosition, out Vector3 leftPos);
                leftHandDevice.TryGetFeatureValue(HandGestures.GestureTransformRotation, out Quaternion leftRot);

                LeftTransform.localPosition = leftPos;
                LeftTransform.localRotation = leftRot;

                rightHandDevice.TryGetFeatureValue(HandGestures.GestureTransformPosition, out Vector3 rightPos);
                rightHandDevice.TryGetFeatureValue(HandGestures.GestureTransformRotation, out Quaternion rightRot);

                RightTransform.localPosition = rightPos;
                RightTransform.localRotation = rightRot;

                // Interaction Points
                leftHandDevice.TryGetFeatureValue(HandGestures.GestureInteractionPosition, out Vector3 leftIntPos);
                leftHandDevice.TryGetFeatureValue(HandGestures.GestureInteractionRotation, out Quaternion leftIntRot);

                LeftInteractionPoint.localPosition = leftIntPos;
                LeftInteractionPoint.localRotation = leftIntRot;

                rightHandDevice.TryGetFeatureValue(HandGestures.GestureInteractionPosition, out Vector3 rightIntPos);
                rightHandDevice.TryGetFeatureValue(HandGestures.GestureInteractionRotation, out Quaternion rightIntRot);

                RightInteractionPoint.localPosition = rightIntPos;
                RightInteractionPoint.localRotation = rightIntRot;

                // Posture
                GestureClassification.TryGetHandPosture(leftHandDevice, out GestureClassification.PostureType leftPosture);
                GestureClassification.TryGetHandPosture(rightHandDevice, out GestureClassification.PostureType rightPosture);

                statusStringBuilder += "\n\n<color=#B7B7B8><b>Left Posture</b></color>: " + leftPosture.ToString();
                statusStringBuilder += "\n<color=#B7B7B8><b>Right Posture</b></color>: " + rightPosture.ToString();

                // KeyPose
                GestureClassification.TryGetHandKeyPose(leftHandDevice, out GestureClassification.KeyPoseType leftKeyPose);
                GestureClassification.TryGetHandKeyPose(rightHandDevice, out GestureClassification.KeyPoseType rightKeyPose);

                statusStringBuilder += "\n\n<color=#B7B7B8><b>Left KeyPose</b></color>: " + leftKeyPose.ToString();
                statusStringBuilder += "\n<color=#B7B7B8><b>Right KeyPose</b></color>: " + rightKeyPose.ToString();

                if (_trackedKeyPoseDict.ContainsKey(rightKeyPose) || _trackedPostureTypeDict.ContainsKey(rightPosture))
                {
                    OnGestureDetected.Invoke(rightKeyPose, rightPosture);
                }
            }
            UpdateStatus();
        }
        public void SetDebugElementsActive(bool active)
        {
            throw new System.NotImplementedException();
        }

        private void UpdateStatus()
        {
            statusText.text = $"<color=#B7B7B8><b>Gesture Tracking Data</b></color>\n{statusStringBuilder}";
        }

    }
}
