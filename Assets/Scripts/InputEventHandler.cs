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
        [SerializeField] EyeTracking _eyeTracking;
        [SerializeField] VoiceIntents _voiceItents;
        [SerializeField] CreateObjectAction _createObjectAction;

        void Start()
        {
            _voiceItents.OnCommandDetected.AddListener(OnCommandDetected);
        }

        /// <summary>
        /// Handle voice comands & Perform actions accordingly
        /// </summary>
        /// <param name="wasSuccessful">If the voice input was recognized successfully </param>
        /// <param name="voiceEvent">The voice event data</param>
        void OnCommandDetected(bool wasSuccessful, MLVoice.IntentEvent voiceEvent)
        {
            Debug.Log("Input handler command detected " + voiceEvent.EventName);
            switch ((VoiceActions)voiceEvent.EventID)
            {
                case VoiceActions.Create:
                    // Get Shape and Color slots values 
                    string shapeName = UtilityScript.GetSlotValue(voiceEvent.EventName, "Shape");
                    string colorName = UtilityScript.GetSlotValue(voiceEvent.EventName, "Color");
                    if (string.IsNullOrEmpty(shapeName))
                    {
                        Debug.LogError("Shape not found " + shapeName);
                        return;
                    }
                    if (string.IsNullOrEmpty(colorName))
                        colorName = "white";
                    // Convert string to Color
                    Color? nullableColor = UtilityScript.StringToColor(colorName);
                    if (nullableColor == null)
                    {
                        Debug.LogWarning("The color is invalid, will not create a shape");
                        return;
                    }
                    Color color = nullableColor ?? Color.white;
                    // Get the creation position
                    float maxObjectCreationDistance = .5f;
                    Vector3 creationPos = _eyeTracking.GazeMarkerPosition;
                    float dist = Vector3.Distance(_eyeTracking.GazeOrigin, creationPos);
                    if (dist > maxObjectCreationDistance)
                    {
                        creationPos = _eyeTracking.GazeOrigin + (_eyeTracking.GazeFixationPoint.normalized - _eyeTracking.GazeOrigin) * maxObjectCreationDistance;
                    }
                    // Create new object with given shape and color

                    _createObjectAction.CreateNewObject(creationPos, color, shapeName);
                    // Play action done audio
                    AudioManager.Instance.ActionDoneAudio.Play();
                    break;

                case VoiceActions.ChangeColor:
                    // Get Color slots values 
                    colorName = UtilityScript.GetSlotValue(voiceEvent.EventName, "Color");
                    // Convert string to Color
                    nullableColor = UtilityScript.StringToColor(colorName);
                    if (nullableColor == null)
                    {
                        Debug.LogWarning("Invalid color " + colorName);
                        return;
                    }
                    color = nullableColor ?? (Color)nullableColor;
                    // Change color to new color
                    _interactorsManager.ChangeSelectedObjectColor(color);
                    // Play action done audio
                    AudioManager.Instance.ActionDoneAudio.Play();
                    break;

                case VoiceActions.Delete:
                    DeleteObject();
                    break;

                case VoiceActions.Group:
                    // Get Action slots values 
                    string action = UtilityScript.GetSlotValue(voiceEvent.EventName, "Action");
                    bool isStartAction = action == "Start" || action == "Begin";
                    // Perform either Start Group or Complete Group based on action
                    if (isStartAction) _interactorsManager.StartGrouping();
                    else _interactorsManager.CompleteGrouping();
                    break;

                case VoiceActions.Selection:
                    // Get Selection slots values 
                    string selection = UtilityScript.GetSlotValue(voiceEvent.EventName, "Selection");
                    bool isSelecting = selection == "Select";
                    // Perform either Select or Deselec based on action
                    bool result = _interactorsManager.SelectObjectToGroup(isSelecting);
                    if (result)
                        if (isSelecting)
                            AudioManager.Instance.SelectAudio.Play();
                        else
                            AudioManager.Instance.DeselectAudio.Play();
                    break;

                case VoiceActions.Scale:
                    // Get Scale and percentage slots values 
                    string upDownValue = UtilityScript.GetSlotValue(voiceEvent.EventName, "UpDown");
                    bool isScaleUp = upDownValue == "Up" || upDownValue == "Bigger";
                    string percentageString = UtilityScript.GetSlotValue(voiceEvent.EventName, "Percentage");
                    // Convert string to number
                    int percent = Int32.Parse(percentageString);
                    // Perform scaling
                    _interactorsManager.ScaleSelectedObject(percent, isScaleUp);
                    AudioManager.Instance.ActionDoneAudio.Play();
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
            AudioManager.Instance.DeleteAudio.Play();
        }
    }
}
