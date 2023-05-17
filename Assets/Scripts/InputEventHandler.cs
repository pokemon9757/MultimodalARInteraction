using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MMI
{
    using GestureClassification = InputSubsystem.Extensions.MLGestureClassification;
    public class InputEventHandler : MonoBehaviour
    {
        private enum VoiceActions
        {
            Greetings = 0,
            Create = 1,
            ChangeColor = 2
        }
        [Header("Input modals")]
        [SerializeField] GestureTracking _gestureTracking;
        [SerializeField] EyeTracking _eyeTracking;
        [SerializeField] VoiceIntents _voiceItents;
        GameActionHandler _handler;

        [Header("Create action parameters")]
        [SerializeField] Vector3 _initialScale;
        [SerializeField] Material _initialMaterial;

        void Start()
        {
#if !UNITY_EDITOR
            _voiceItents.Init();
            MLVoice.OnVoiceEvent += OnCommandDetected;
#endif
            _eyeTracking.Init();
            _gestureTracking.Init();
            _gestureTracking.OnGestureDetected += OnGestureDetected;
            _handler = new GameActionHandler();
        }

        void Update()
        {
            _eyeTracking.ProcessAbility();
            _gestureTracking.ProcessAbility();
        }

        void OnGestureDetected(GestureClassification.KeyPoseType keyPoseType, GestureClassification.PostureType postureType)
        {
            Debug.Log("Detected pose " + keyPoseType.ToString() + " and posture " + postureType.ToString());
        }

        void OnCommandDetected(in bool wasSuccessful, in MLVoice.IntentEvent voiceEvent)
        {
            switch ((VoiceActions)voiceEvent.EventID)
            {
                case VoiceActions.Greetings:
                    break;
                case VoiceActions.Create:
                    string shapeName = "";
                    string colorName = "grey";
                    foreach (MLVoice.EventSlot slot in voiceEvent.EventSlotsUsed)
                    {
                        if (slot.SlotName == "Shape")
                            shapeName = slot.SlotValue;
                        else if (slot.SlotName == "Color")
                            colorName = slot.SlotValue;
                    }
                    CreateObject(shapeName, colorName);
                    break;
                case VoiceActions.ChangeColor:
                    break;
            }
        }

        void CreateObject(string shapeName, string colorName)
        {
            _handler.AddGameAction(new CreateObjectAction(_eyeTracking.EyesFixationPoint, _initialScale, _initialMaterial, colorName, shapeName));
        }

        #region Debug UI Functions
        public void CreateCube()
        {
            CreateObject("cube", "blue");
        }
        #endregion
    }
}
