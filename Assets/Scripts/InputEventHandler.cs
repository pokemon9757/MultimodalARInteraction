using System;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace MMI
{
    public class InputEventHandler : MonoBehaviour
    {
        /** 
        *   The registered voice actions. NOTE: The enum values correspond to the ID of actions in the MLVoiceConfig file
        */
        private enum VoiceActions
        {
            Create = 1, /**< Create a new object */
            ChangeColor = 2, /**< Change the selected object color */
            Delete = 3, /**< Disable selected object */
            Group = 4, /**< Start/Complete Group mode  */
            Selection = 5, /**< Only usable when Group mode is active, used to Select/Deselect objects to group */
            Scale = 6, /**< Scale selected object */
        }
        [SerializeField] InteractorsManager _interactorsManager;
        [SerializeField] GestureTracking _gestureTracking;
        [SerializeField] EyeTracking _eyeTracking;
        [SerializeField] VoiceIntents _voiceItents;
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
                case VoiceActions.Create:
                    // Get Shape and Color slots values 
                    string shapeName = UtilityScript.GetSlotValue(voiceEvent.EventName, "Shape");
                    string colorName = UtilityScript.GetSlotValue(voiceEvent.EventName, "Color");
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
                    nullableColor = UtilityScript.StringToColor(colorName);
                    if (nullableColor == null)
                    {
                        Debug.LogWarning("Invalid color " + colorName);
                        return;
                    }
                    color = nullableColor ?? (Color)nullableColor;
                    _interactorsManager.ChangeSelectedObjectColor(color);
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
                    string percentageString = UtilityScript.GetSlotValue(voiceEvent.EventName, "Percentage");
                    int percent = Int32.Parse(percentageString);

                    _interactorsManager.ScaleSelectedObject(percent, isScaleUp);
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
