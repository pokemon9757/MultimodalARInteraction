using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
namespace MMI
{
    public class VoiceIntents : MonoBehaviour
    {
        [SerializeField, Tooltip("The text used to display status information for the example.")]
        private Text _statusText = null;

        [SerializeField, Tooltip("The text used to display input controls for the example.")]
        private Text _controlsText = null;

        [SerializeField, Tooltip("The configuration file that holds the list of intents used for this application.")]
        private MLVoiceIntentsConfiguration _voiceConfiguration;

        private MagicLeapInputs _mlInputs;
        private MagicLeapInputs.ControllerActions _controllerActions;
        private string _startupStatus = "Requesting Permission...";
        private string _lastResults = "";
        private bool _isProcessing = false;

        [SerializeField, Tooltip("Popup canvas to direct user to Voice Input settings page.")]
        private GameObject _voiceInputSettingsPopup = null;

        [SerializeField, Tooltip("Popup canvas to alert the user of error when Voice Input settings aren't enabled.")]
        private GameObject _voiceInputErrorPopup;
        private static bool _userPromptedForSetting;
        private readonly MLPermissions.Callbacks _permissionCallbacks = new MLPermissions.Callbacks();

        private void Awake()
        {
            _permissionCallbacks.OnPermissionDenied += OnPermissionDenied;
            _permissionCallbacks.OnPermissionDeniedAndDontAskAgain += OnPermissionDenied;
        }


        public void Init()
        {
            _mlInputs = new MagicLeapInputs();
            _mlInputs.Enable();
            _controllerActions = new MagicLeapInputs.ControllerActions(_mlInputs);
            Initialize();
        }

        // void OnApplicationPause(bool pauseStatus)
        // {
        //     if (!pauseStatus)
        //         Initialize();
        // }

        private void Initialize()
        {
            if (!MLPermissions.CheckPermission(MLPermission.VoiceInput).IsOk)
            {
                MLPermissions.RequestPermission(MLPermission.VoiceInput, _permissionCallbacks);
                return;
            }

            bool isEnabled = MLVoice.VoiceEnabled;
            _startupStatus = "System Supports Voice Intents: " + isEnabled.ToString();

            if (isEnabled)
            {
                _voiceInputSettingsPopup.SetActive(false);
                _voiceInputErrorPopup.SetActive(false);

                MLResult result = MLVoice.SetupVoiceIntents(_voiceConfiguration);

                if (result.IsOk)
                {
                    _controllerActions.Bumper.performed += HandleOnBumper;
                    _isProcessing = true;

                    MLVoice.OnVoiceEvent += VoiceEvent;

                    SetControlsText();
                }
                else
                {
                    _startupStatus += "\nSetup failed with result: " + result.ToString();
                    Debug.LogError("Failed to Setup Voice Intents with result: " + result);
                }
            }
            else
            {
                if (!_userPromptedForSetting)
                {
                    _userPromptedForSetting = true;
                    _voiceInputSettingsPopup.SetActive(true);
                }
                else
                {
                    Debug.LogError("Voice Commands has not been enabled. Voice intents requires this setting to enabled. It is found in system settings inside Magic Leap Inputs.");
                    _voiceInputSettingsPopup.SetActive(false);
                    _voiceInputErrorPopup.SetActive(true);
                }
            }
        }

        private void OnDestroy()
        {
#if !UNITY_EDITOR
            MLVoice.OnVoiceEvent -= VoiceEvent;
            _controllerActions.Bumper.performed -= HandleOnBumper;
            _permissionCallbacks.OnPermissionDenied -= OnPermissionDenied;
            _permissionCallbacks.OnPermissionDeniedAndDontAskAgain -= OnPermissionDenied;
#endif
        }

        void Update()
        {
            UpdateStatus();
            string micName = Microphone.devices[0];
            
        }

        private void UpdateStatus()
        {
            _statusText.text += $"\n<color=#B7B7B8><b>Voice Intents Data</b></color>\n{_startupStatus}";
            _statusText.text += "\n\nIs Processing: " + _isProcessing;
            _statusText.text += "\n\nInstructions and List of commands in Controls Tab";
            _statusText.text += _lastResults;
        }

        private void SetControlsText()
        {
            StringBuilder controlsScrollview = new StringBuilder();

            controlsScrollview.Append($"<color=#B7B7B8><b>Speak the App Specific command out loud</b></color>\n");
            controlsScrollview.Append($"Use one of these listed commands: \n");

            controlsScrollview.AppendJoin('\n', _voiceConfiguration.GetValues());

            controlsScrollview.Append($"\n\n<color=#B7B7B8><b>To Use a System Intent speak \"Hey Magic Leap\"</b></color>\nDots to indicate the device is listening should appear. Then speak one of the enabled system commands: \n");

            foreach (MLVoiceIntentsConfiguration.SystemIntentFlags flag in System.Enum.GetValues(typeof(MLVoiceIntentsConfiguration.SystemIntentFlags)))
            {
                if (_voiceConfiguration.AutoAllowAllSystemIntents || _voiceConfiguration.SystemCommands.HasFlag(flag))
                {
                    controlsScrollview.Append($"{flag.ToString()}\n");

                }
            }

            controlsScrollview.Append($"\n\n<color=#B7B7B8><b>Slots</b></color>\nA Slot is a placeholder for a list of possible values. The name of the slot is placed between brackets within the App Specific commands value and when uttering the phrase one of the slots values is used in its place.\n");
            controlsScrollview.Append($"Slots Values Used:");

            foreach (MLVoiceIntentsConfiguration.SlotData slot in _voiceConfiguration.SlotsForVoiceCommands)
            {
                controlsScrollview.Append($"\n{slot.name} : {string.Join(" - ", slot.values)}");
            }

            controlsScrollview.Append($"\n\n<color=#B7B7B8><b>Controller Bumper</b></color>\nBy Default this example scene starts processing Voice Intents. Tap the bumper to stop processing, then tap it again to begin processing again.");

            _controlsText.text = controlsScrollview.ToString();
        }

        void VoiceEvent(in bool wasSuccessful, in MLVoice.IntentEvent voiceEvent)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append($"\n\n<color=#B7B7B8><b>Last Voice Event:</b></color>\n");
            strBuilder.Append($"Was Successful: <i>{wasSuccessful}</i>\n");
            strBuilder.Append($"State: <i>{voiceEvent.State}</i>\n");
            strBuilder.Append($"No Intent Reason\n(Expected NoReason): \n<i>{voiceEvent.NoIntentReason}</i>\n");
            strBuilder.Append($"Event Unique Name:\n<i>{voiceEvent.EventName}</i>\n");
            strBuilder.Append($"Event Unique Id: <i>{voiceEvent.EventID}</i>\n");

            strBuilder.Append($"Slots Used:\n");
            strBuilder.AppendJoin("\n", voiceEvent.EventSlotsUsed.Select(v => $"Name: {v.SlotName} - Value: {v.SlotValue}"));

            _lastResults = strBuilder.ToString();
        }

        private void HandleOnBumper(InputAction.CallbackContext obj)
        {
            bool bumperDown = obj.ReadValueAsButton();

            if (bumperDown)
            {
                MLResult result;
                if (_isProcessing)
                {
                    result = MLVoice.Stop();
                    if (result.IsOk)
                    {
                        _isProcessing = false;
                    }
                    else
                    {
                        Debug.LogError("Failed to Stop Processing Voice Intents with result: " + result);
                    }
                }
                else
                {
                    result = MLVoice.SetupVoiceIntents(_voiceConfiguration);
                    if (result.IsOk)
                    {
                        _isProcessing = true;
                    }
                    else
                    {
                        Debug.LogError("Failed to Re-Setup Voice Intents with result: " + result);
                    }
                }

            }
        }

        private void OnPermissionDenied(string permission)
        {
            _startupStatus = "<color=#ff0000><b>Permission Denied!</b></color>";
        }

        public void OnVoiceInputSettingsPopupOpen()
        {
            UnityEngine.XR.MagicLeap.SettingsIntentsLauncher.LaunchSystemVoiceInputSettings();

            if (_voiceInputSettingsPopup != null)
            {
                _voiceInputSettingsPopup.SetActive(false);
            }
        }

        public void OnVoiceInputSettingsPopupCancel()
        {
            if (_voiceInputSettingsPopup != null)
            {
                _voiceInputSettingsPopup.SetActive(false);
            }
        }

    }

}


