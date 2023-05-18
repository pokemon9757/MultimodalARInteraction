using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;

namespace MMI
{
    using GestureClassification = InputSubsystem.Extensions.MLGestureClassification;

    public class HandGrab : MonoBehaviour
    {
        [SerializeField] GestureTracking _gestureTracking;
        public bool IsLeftHand = true;
        bool _canGrab = false;
        GameObject _collidedObject;
        Vector3 _itemOffsetFromHand;
        InputDevice _handDevice;
        List<Bone> _indexFingerBones = new();
        List<Bone> _thumbFingerBones = new();

        void Start()
        {
            _handDevice = InputSubsystem.Utils.FindMagicLeapDevice(InputDeviceCharacteristics.HandTracking | (IsLeftHand ? InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right));
            GetFingerBones();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "InteractableObject" || _collidedObject != null) return;
            _canGrab = true;
            _collidedObject = other.gameObject;
        }

        void Update()
        {
            if (!_canGrab) return;

            GestureClassification.PostureType posture = IsLeftHand ? _gestureTracking.LeftPosture : _gestureTracking.RightPosture;
            if (posture != GestureClassification.PostureType.Pinch)
            {
                return;
            }
            Quaternion handRot = IsLeftHand ? _gestureTracking.LeftTransform.rotation : _gestureTracking.RightTransform.rotation;

            GetFingerBones();

            // Item is attached to either index or thumb
            Bone attachedBone = (_indexFingerBones.Count == 0) ? _thumbFingerBones[0] : _indexFingerBones[0];
            attachedBone.TryGetPosition(out Vector3 bonePosition);

            _collidedObject.transform.position = bonePosition;
            _collidedObject.transform.rotation = handRot;
        }

        void GetFingerBones()
        {
            if (!_handDevice.isValid)
            {
                _handDevice = InputSubsystem.Utils.FindMagicLeapDevice(InputDeviceCharacteristics.HandTracking | (IsLeftHand ? InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right));
            }

            // Get ALL bone pos
            if (_handDevice.TryGetFeatureValue(CommonUsages.handData, out UnityEngine.XR.Hand hand))
            {
                hand.TryGetFingerBones(UnityEngine.XR.HandFinger.Index, _indexFingerBones);
                hand.TryGetFingerBones(UnityEngine.XR.HandFinger.Index, _thumbFingerBones);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag != "InteractableObject") return;
            if (_collidedObject == null || _collidedObject != other.gameObject) return;

            _canGrab = false;
            _collidedObject = null;
            _itemOffsetFromHand = Vector3.zero;
        }
    }
}
