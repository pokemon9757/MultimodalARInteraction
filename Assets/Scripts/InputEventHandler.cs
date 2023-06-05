using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MMI
{
    public class InputEventHandler : MonoBehaviour
    {
        private enum VoiceActions
        {
            Greetings = 0,
            Create = 1,
            ChangeColor = 2,
            Delete = 3,
            StartGroup = 4,
            CompleteGroup = 5,
            Select = 6,
            Deselect = 7,
            ToggleUI = 8,
            ScaleUp = 9,
            ScaleDown = 10,
        }
        [SerializeField] InteractorsManager _interactorsManager;

        [Header("Input modals")]
        [SerializeField] GestureTracking _gestureTracking;
        [SerializeField] EyeTracking _eyeTracking;
        [SerializeField] VoiceIntents _voiceItents;

        GameActionHandler _handler;

        [Header("Create action parameters")]
        [SerializeField] Vector3 _initialScale;
        [SerializeField] Material _initialMaterial;
        bool _isEditor = false;

        void Start()
        {
#if UNITY_EDITOR
            _isEditor = true;
#endif
            if (!_isEditor)
            {
                _voiceItents.Init();
                MLVoice.OnVoiceEvent += OnCommandDetected;
            }
            _eyeTracking.Init();
            _gestureTracking.Init();
            _handler = new GameActionHandler();
        }

        void Update()
        {
            _eyeTracking.ProcessAbility();
            _gestureTracking.ProcessAbility();
        }

        void OnCommandDetected(in bool wasSuccessful, in MLVoice.IntentEvent voiceEvent)
        {
            Debug.Log("--- VOICE DETECTED: " + ((VoiceActions)voiceEvent.EventID).ToString() + " ---");
            switch ((VoiceActions)voiceEvent.EventID)
            {
                case VoiceActions.Greetings:
                    break;
                case VoiceActions.Create:
                    CreateObject(GetSlotValue(voiceEvent, "Shape"), GetSlotValue(voiceEvent, "Color"));
                    break;
                case VoiceActions.ChangeColor:
                    _interactorsManager.ChangeSelectedObjectColor(GetSlotValue(voiceEvent, "Color"));
                    break;
                case VoiceActions.Delete:
                    DeleteObject();
                    break;
                case VoiceActions.StartGroup:
                    _interactorsManager.StartGrouping();
                    break;
                case VoiceActions.CompleteGroup:
                    _interactorsManager.CompleteGrouping();
                    break;
                case VoiceActions.Select:
                    _interactorsManager.SelectObjectToGroup(true);
                    break;
                case VoiceActions.Deselect:
                    _interactorsManager.SelectObjectToGroup(false);
                    break;
                case VoiceActions.ScaleUp:
                    _interactorsManager.ScaleSelectedObject(GetSlotValue(voiceEvent, "Percentage"), true);
                    break;
                case VoiceActions.ScaleDown:
                    _interactorsManager.ScaleSelectedObject(GetSlotValue(voiceEvent, "Percentage"), false);
                    break;
            }
        }

        string GetSlotValue(MLVoice.IntentEvent voiceEvent, string slotName)
        {
            foreach (MLVoice.EventSlot slot in voiceEvent.EventSlotsUsed)
            {
                if (slot.SlotName == slotName) return slot.SlotValue;
            }
            return "Could not find";
        }
        void CreateObject(string shapeName, string colorName)
        {
            Debug.Log("Creating " + shapeName + " " + colorName);
            _handler.AddGameAction(new CreateObjectAction(_eyeTracking.GazeMarkerPosition, _initialScale, _initialMaterial, colorName, shapeName));
        }

        void DeleteObject()
        {
            var objToDelete = InteractorsManager.Instance.SelectedObject;
            if (objToDelete == null) return;
            _handler.AddGameAction(new DeleteObjectAction(objToDelete.gameObject));
        }

        #region Debug UI Functions
        public void CreateCube()
        {
            CreateObject("cube", "blue");
        }

        public void DeleteSelected()
        {
            DeleteObject();
        }
        #endregion
    }
}
