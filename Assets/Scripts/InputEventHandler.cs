using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MMI
{
    using GestureClassification = InputSubsystem.Extensions.MLGestureClassification;
    public class InputEventHandler : MonoBehaviour
    {
        public enum VoiceActions
        {
            Greetings = 0,
            Create = 1,
            ChangeColor = 2
        }
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
            switch ((VoiceActions)voiceEvent.EventID)
            {
                case VoiceActions.Greetings:


                    break;
                case VoiceActions.Create:
                    GameObject test = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    test.transform.position = _eyeTracking.EyesFixationPoint;
                    test.transform.localScale = Vector3.one;
                    break;
                case VoiceActions.ChangeColor:
                    break;
            }

        }
    }
}
