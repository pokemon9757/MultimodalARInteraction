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
            Group = 4,
            Selection = 5,
            Scale = 6,
        }
        [SerializeField] InteractorsManager _interactorsManager;

        [Header("Input modals")]
        [SerializeField] GestureTracking _gestureTracking;
        [SerializeField] EyeTracking _eyeTracking;
        [SerializeField] VoiceIntents _voiceItents;

        [Header("Create action parameters")]
        [SerializeField] Vector3 _initialScale;
        [SerializeField] Material _initialMaterial;
        [SerializeField] CreateObjectAction _createObjectAction;

        void Start()
        {
            _eyeTracking.Init();
            _gestureTracking.Init();
            _voiceItents.OnCommandDetected.AddListener(OnCommandDetected);
        }

        void Update()
        {
            _eyeTracking.ProcessAbility();
            _gestureTracking.ProcessAbility();
        }

        /// <summary>
        /// Handle voice comands & Perform actions accordingly
        /// </summary>
        /// <param name="wasSuccessful">If the voice input was recognized successfully </param>
        /// <param name="voiceEvent">The voice event data</param>
        void OnCommandDetected(bool wasSuccessful, MLVoice.IntentEvent voiceEvent)
        {
            switch ((VoiceActions)voiceEvent.EventID)
            {
                case VoiceActions.Greetings:
                    break;
                case VoiceActions.Create:
                    string shapeName = UtilityScript.GetSlotValue(voiceEvent.EventName, "Shape");
                    string colorName = UtilityScript.GetSlotValue(voiceEvent.EventName, "Color");
                    // Create { Color Orange} {Shape cube}
                    if (string.IsNullOrEmpty(shapeName))
                    {
                        Debug.LogError("Shape not found");
                        return;
                    }
                    if (string.IsNullOrEmpty(colorName))
                        colorName = "white";
                    Color? nullableColor = UtilityScript.StringToColor(colorName);
                    if (nullableColor == null)
                    {
                        Debug.LogWarning("The color is invalid, will not create a shape");
                        return;
                    }
                    // Perform explicit conversion from nullable Color to non-nullable Color
                    Color color = nullableColor ?? Color.white;
                    _createObjectAction.CreateNewObject(_eyeTracking.GazeMarkerPosition, color, shapeName);

                    break;
                case VoiceActions.ChangeColor:
                    colorName = UtilityScript.GetSlotValue(voiceEvent.EventName, "Color");
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
                case VoiceActions.Group:
                    string action = UtilityScript.GetSlotValue(voiceEvent.EventName, "Action");
                    bool isStartAction = action == "Start" || action == "Begin";
                    if (isStartAction) _interactorsManager.StartGrouping();
                    else _interactorsManager.CompleteGrouping();
                    break;
                case VoiceActions.Selection:
                    string selection = UtilityScript.GetSlotValue(voiceEvent.EventName, "Selection");
                    _interactorsManager.SelectObjectToGroup(selection == "Select");
                    break;
                case VoiceActions.Scale:
                    string upDownValue = UtilityScript.GetSlotValue(voiceEvent.EventName, "UpDown");
                    bool isScaleUp = upDownValue == "Up" || upDownValue == "Bigger";
                    _interactorsManager.ScaleSelectedObject(UtilityScript.GetSlotValue(voiceEvent.EventName, "Percentage"), isScaleUp);
                    break;
            }
        }

        /// <summary>
        ///  Perform object deletion action
        /// </summary>
        void DeleteObject()
        {
            var objToDelete = InteractorsManager.Instance.GetSelectedObject;
            if (objToDelete == null)
            {
                Debug.LogError("No object has been selected yet");
                return;
            }
            objToDelete.gameObject.SetActive(false);
        }
    }
}
