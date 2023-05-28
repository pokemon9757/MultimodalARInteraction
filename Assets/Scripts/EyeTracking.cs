using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace MMI
{
    /// <summary>
    /// Behaviour that moves object to fixation point position.
    /// </summary>
    public class EyeTracking : MonoBehaviour
    {
        [Tooltip("Display the game object along the eye gaze ray at a default distance (in meters).")]
        [SerializeField] float _defaultDistanceInMeters = 2f;
        [SerializeField, Tooltip("Fixation Point marker")] Transform _eyesMarker;
        [SerializeField] bool _isDebugActive = true;
        IMixedRealityEyeGazeProvider _eyeGazeProvider;
        public Vector3 GazeMarkerPosition { get { return _eyesMarker.position; } }
        public Vector3 GazeOrigin { get { return _eyeGazeProvider.GazeOrigin; } }
        public Vector3 GazeDirection { get { return _eyeGazeProvider.GazeDirection; } }

        public void Init()
        {
            _eyeGazeProvider = CoreServices.InputSystem?.EyeGazeProvider;
            SetDebugElementsActive();
        }

        public void ProcessAbility()
        {
            if (_eyeGazeProvider == null)
            {
                Init();
            }

            _eyesMarker.position = _eyeGazeProvider.GazeOrigin + _eyeGazeProvider.GazeDirection.normalized * _defaultDistanceInMeters;

            EyeTrackingTarget lookedAtEyeTarget = EyeTrackingTarget.LookedAtEyeTarget;

            // Update GameObject to the current eye gaze position at a given distance
            if (lookedAtEyeTarget != null)
            {
                // Show the object at the center of the currently looked at target.
                if (lookedAtEyeTarget.EyeCursorSnapToTargetCenter)
                {
                    Ray rayToCenter = new Ray(CameraCache.Main.transform.position, lookedAtEyeTarget.transform.position - CameraCache.Main.transform.position);
                    RaycastHit hitInfo;
                    UnityEngine.Physics.Raycast(rayToCenter, out hitInfo);
                    _eyesMarker.position = hitInfo.point;
                }
                else
                {
                    // Show the object at the hit position of the user's eye gaze ray with the target.
                    _eyesMarker.position = _eyeGazeProvider.GazeOrigin + _eyeGazeProvider.GazeDirection.normalized * _defaultDistanceInMeters;
                }
            }
            else
            {
                // If no target is hit, show the object at a default distance along the gaze ray.
                _eyesMarker.position = _eyeGazeProvider.GazeOrigin + _eyeGazeProvider.GazeDirection.normalized * _defaultDistanceInMeters;
            }
        }

        public void ToggleDebugElements()
        {
            _isDebugActive = !_isDebugActive;
            SetDebugElementsActive();
        }

        void SetDebugElementsActive()
        {
            _eyesMarker.gameObject.SetActive(_isDebugActive);
        }
    }
}

