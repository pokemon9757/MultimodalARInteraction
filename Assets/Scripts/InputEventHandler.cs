using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            Scale = 9,
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

        void Start()
        {
            _eyeTracking.Init();
            _gestureTracking.Init();
            _handler = new GameActionHandler();
            _voiceItents.OnCommandDetected.AddListener(OnCommandDetected);
        }

        void Update()
        {
            _eyeTracking.ProcessAbility();
            _gestureTracking.ProcessAbility();
        }

        void OnCommandDetected(bool wasSuccessful, MLVoice.IntentEvent voiceEvent)
        {
            Debug.Log("--- VOICE DETECTED: " + ((VoiceActions)voiceEvent.EventID).ToString() + " ---");
            switch ((VoiceActions)voiceEvent.EventID)
            {
                case VoiceActions.Greetings:
                    break;
                case VoiceActions.Create:
                    string shapeName = GetSlotValue(voiceEvent.EventName, "Shape");
                    string colorName = GetSlotValue(voiceEvent.EventName, "Color");
                    // Create { Color Orange} {Shape cube}
                    if (string.IsNullOrEmpty(shapeName))
                    {
                        Debug.LogError("Shape not found");
                        return;
                    }
                    if (string.IsNullOrEmpty(colorName))
                    {
                        colorName = "white";
                    }
                    CreateObject(shapeName, colorName);
                    break;
                case VoiceActions.ChangeColor:
                    colorName = GetSlotValue(voiceEvent.EventName, "Color");
                    if (string.IsNullOrEmpty(colorName))
                    {
                        Debug.LogError("Something went terribly wrong with color change...");
                        return;
                    }
                    _interactorsManager.ChangeSelectedObjectColor(colorName);
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
                case VoiceActions.Scale:
                    bool isScaleUp = GetSlotValue(voiceEvent.EventName, "UpDown") == "Up";
                    _interactorsManager.ScaleSelectedObject(GetSlotValue(voiceEvent.EventName, "Percentage"), isScaleUp);
                    break;

            }
        }

        /// <summary>
        /// Get slot value from voiceEvent.EventName
        /// </summary>
        /// <param name="inputString">event name from ML voice event</param>
        /// <param name="keyword">Data slot</param>
        /// <returns>Slot value, null if not found</returns>
        string GetSlotValue(string inputString, string keyword)
        {
            string pattern = $@"{{\s*{keyword}\s*(.*?)}}"; // Regular expression pattern
            Match match = Regex.Match(inputString, pattern, RegexOptions.Singleline);

            if (match.Success)
            {
                // Return the captured group value
                return match.Groups[1].Value.Trim();
            }
            // Keyword not found in the string
            return null;
        }

        void CreateObject(string shapeName, string colorName)
        {
            _handler.AddGameAction(new CreateObjectAction(_eyeTracking.GazeMarkerPosition, _initialScale, _initialMaterial, colorName, shapeName));
        }

        void DeleteObject()
        {
            var objToDelete = InteractorsManager.Instance.GetSelectedObject;
            if (objToDelete == null)
            {
                Debug.LogError("No object has been selected yet");
                return;
            }
            _handler.AddGameAction(new DeleteObjectAction(objToDelete.gameObject));
        }
    }
}
