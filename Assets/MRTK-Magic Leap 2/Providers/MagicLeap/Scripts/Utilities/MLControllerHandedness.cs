// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.MRTK.DeviceManagement.Input
{
    public class MLControllerHandedness
    {
        //Timestamp to test stability.
        private static float _stateTimeStamp;

        //Timestamp to avoid redundant queries.
        private static float _checkTimeStamp;

        //Cached handedness that is returned by GetControllerHandedness().
        private static Handedness _controllerHandedness = Handedness.None;

        //The amount of time the controller state needs to return Left or Right hand to be considered stable.
        private const float TrackDelayTime = 1;

        //The amount of time the controller state needs to return None hand to be considered stable.
        private const float LoseTrackingDelayTime = 3;

        //The handedness that was detected in the previous check;
        private static Handedness _lastControllerHandedness = Handedness.None;

        //When true, filtering will be applied to the controller state query.
        private static bool _useDelay = true;

        //Input Devices used to query the distance between the controller and hands.
        private static InputDevice controllerDevice;
        private static InputDevice leftHandDevice;
        private static InputDevice rightHandDevice;

        public static Handedness GetControllerHandedness()
        {
            if (MLDevice.IsReady() && Time.time - _checkTimeStamp > .01f)
            {

                _checkTimeStamp = Time.time;

                InputSubsystem.Extensions.Controller.State
                    controllerState = InputSubsystem.Extensions.Controller.GetState();

                Handedness detectHandedness = StateToHandedness(controllerState);
                if (_useDelay)
                {
                    //Get Controller Handedness
                    bool isHandednessStable = _lastControllerHandedness != Handedness.None
                        ? Time.time - _stateTimeStamp > TrackDelayTime
                        : Time.time - _stateTimeStamp > LoseTrackingDelayTime;

                    if (detectHandedness == _lastControllerHandedness && isHandednessStable)
                    {
                        _controllerHandedness = _lastControllerHandedness;
                    }
                    else if (detectHandedness != _lastControllerHandedness)
                    {
                        _lastControllerHandedness = detectHandedness;
                        _stateTimeStamp = Time.time;
                    }
                }
                else
                {
                    _controllerHandedness = detectHandedness;
                }
            }
            return _controllerHandedness;
        }

        private static void FindDevices()
        {
            if(!controllerDevice.isValid)
                controllerDevice = InputSubsystem.Utils.FindMagicLeapDevice(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand);

            if (!leftHandDevice.isValid || !rightHandDevice.isValid)
            {
                List<InputDevice> devices = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.Left, devices);
                foreach (var device in devices)
                {
                    if (device.isValid && device.name.Contains("MagicLeap"))
                    {
                        leftHandDevice = device;
                        break;
                    }
                }

                devices.Clear();
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HandTracking | InputDeviceCharacteristics.Right, devices);
                foreach (var device in devices)
                {
                    if (device.isValid && device.name.Contains("MagicLeap"))
                    {
                        rightHandDevice = device;
                        break;
                    }
                }
            }
       
        }

        private static Handedness StateToHandedness(InputSubsystem.Extensions.Controller.State state)
        {
            switch (state.Hand)
            {
                case InputSubsystem.Extensions.Controller.MLInputControllerHand.None:
                    FindDevices();

                  
                    bool isControllerPositionAvailable = controllerDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out Vector3 controllerPosition);
                    leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out Vector3 leftHandPosition);
                    rightHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out Vector3 rightHandPosition);

                    leftHandDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.Hand.Confidence, out float leftHandConfidence);
                    bool isLeftHandPositionAvailable = leftHandConfidence > 0;

                    rightHandDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.Hand.Confidence, out float rightHandConfidence);
                    bool isRightHandPositionAvailable = rightHandConfidence > 0;

                    if (isControllerPositionAvailable && controllerDevice.isValid)
                    {
                        if (!isLeftHandPositionAvailable && !isRightHandPositionAvailable)
                        {
                            return _lastControllerHandedness == Handedness.None? Handedness.Left: _lastControllerHandedness;
                        }

                        float leftHandDistance = isLeftHandPositionAvailable
                            ? (leftHandPosition -controllerPosition).sqrMagnitude : float.MaxValue;
                        float rightHandDistance = isLeftHandPositionAvailable
                            ?(rightHandPosition- controllerPosition).sqrMagnitude : float.MaxValue;

                        bool closestToLeftHand = leftHandDistance < rightHandDistance;
                        //0.0441 = 0.21m / 8inch  squared 
                        if (closestToLeftHand && leftHandDistance < 0.0441f)
                        {
                            return Handedness.Left;
                        }
                        else if (rightHandDistance < 0.0441f)
                        {
                            return Handedness.Right;
                        }
                    }

                    return Handedness.None;
                case InputSubsystem.Extensions.Controller.MLInputControllerHand.Left:
                    return Handedness.Left;
                case InputSubsystem.Extensions.Controller.MLInputControllerHand.Right:
                    return Handedness.Right;
                default:
                    return Handedness.None;
            }
        }
    }
}