using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MMI
{
    using GestureClassification = InputSubsystem.Extensions.MLGestureClassification;
    public class InputEventHandler : MonoBehaviour
    {
        [SerializeField] GestureTracking _gestureTracking;
        [SerializeField] EyeTracking _eyeTracking;
        [SerializeField] VoiceIntents _voiceItents;

        void Start()
        {
            _eyeTracking.Init();
            _voiceItents.Init();
            MLVoice.OnVoiceEvent += OnCommandDetected;
            _gestureTracking.Init();
            _gestureTracking.OnGestureDetected += OnGestureDetected;
        }

        void Update()
        {
            _eyeTracking.ProcessAbility();
            _voiceItents.ProcessAbility();
            _gestureTracking.ProcessAbility();
        }

        void OnGestureDetected(GestureClassification.KeyPoseType keyPoseType, GestureClassification.PostureType postureType)
        {
            Debug.Log("Detected pose " + keyPoseType.ToString() + " and posture " + postureType.ToString());
        }

        void OnCommandDetected(in bool wasSuccessful, in MLVoice.IntentEvent voiceEvent)
        {
            Debug.Log("Detected Voice Command : " + voiceEvent.EventName);
            switch (voiceEvent.EventName)
            {
                case "Create":
                    GameObject test = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    test.transform.position = _eyeTracking.EyesFixationPoint;
                    test.transform.localScale = Vector3.one;

                    break;
            }

        }
    }
}
