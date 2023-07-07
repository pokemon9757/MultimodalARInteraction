using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;

namespace MMI
{
    /// <summary>
    /// Behaviour that moves object to fixation point position.
    /// </summary>
    public class EyeTracking : MonoBehaviour
    {
        [SerializeField, Tooltip("Fixation Point marker")] Transform _eyesMarker;
        [SerializeField] RaycastObjectInteractor _rayInteractor;
        public Vector3 GazeFixationPoint { get { return _eyesActions.Data.ReadValue<UnityEngine.InputSystem.XR.Eyes>().fixationPoint; } }
        public Vector3 GazeMarkerPosition { get { return _eyesMarker.position; } }
        public Vector3 GazeOrigin { get { return CoreServices.InputSystem.EyeGazeProvider.GazeOrigin; } }
        public Vector3 GazeDirection { get { return CoreServices.InputSystem.EyeGazeProvider.GazeDirection; } }
        // Used to get ml inputs.
        private MagicLeapInputs _mlInputs;
        // Used to get eyes action data.
        private MagicLeapInputs.EyesActions _eyesActions;
        // Used to get other eye data
        private InputDevice _eyesDevice;
        // Was EyeTracking permission granted by user
        private bool _permissionGranted = false;
        private readonly MLPermissions.Callbacks _permissionCallbacks = new MLPermissions.Callbacks();


        private void Awake()
        {
            _permissionCallbacks.OnPermissionGranted += OnPermissionGranted;
        }

        void Update()
        {
            ProcessAbility();
        }

        void Start()
        {
            _mlInputs = new MagicLeapInputs();
            _mlInputs.Enable();

            MLPermissions.RequestPermission(MLPermission.EyeTracking, _permissionCallbacks);
        }

        void ProcessAbility()
        {
            if (!_permissionGranted)
            {
                Debug.LogError("Permission for eye tracking isn't granted");
                return;
            }

            if (!_eyesDevice.isValid)
            {
                this._eyesDevice = InputSubsystem.Utils.FindMagicLeapDevice(InputDeviceCharacteristics.EyeTracking | InputDeviceCharacteristics.TrackedDevice);
                return;
            }

            // Manually set fixation point marker so we can apply rotation, since UnityXREyes
            // does not provide it
            Vector3 markerPosition = GazeFixationPoint;
            _eyesMarker.position = markerPosition;
            _eyesMarker.rotation = Quaternion.LookRotation(GazeFixationPoint - Camera.main.transform.position);
            _rayInteractor.PerformRaycast(GazeOrigin, GazeDirection, Mathf.Infinity);
        }

        private void OnDestroy()
        {
            _permissionCallbacks.OnPermissionGranted -= OnPermissionGranted;
            _mlInputs.Disable();
            _mlInputs.Dispose();
            InputSubsystem.Extensions.MLEyes.StopTracking();
        }

        private void OnPermissionGranted(string permission)
        {
            InputSubsystem.Extensions.MLEyes.StartTracking();
            _eyesActions = new MagicLeapInputs.EyesActions(_mlInputs);
            _permissionGranted = true;
        }
    }
}

