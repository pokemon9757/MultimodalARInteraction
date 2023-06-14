using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
namespace MMI
{
    public class VoiceIntents : MonoBehaviour
    {
        public UnityEvent<bool, MLVoice.IntentEvent> OnCommandDetected;

        [SerializeField, Tooltip("The configuration file that holds the list of intents used for this application.")]
        private MLVoiceIntentsConfiguration voiceConfiguration;
        private MagicLeapInputs mlInputs;
        private MagicLeapInputs.ControllerActions controllerActions;

        private string startupStatus = "Requesting Permission...";
        private string lastResults = "";
        private bool isProcessing = false;

        [SerializeField, Tooltip("Popup canvas to direct user to Voice Input settings page.")]
        private GameObject voiceInputSettingsPopup = null;

        [SerializeField, Tooltip("Popup canvas to alert the user of error when Voice Input settings aren't enabled.")]
        private GameObject voiceInputErrorPopup;

        private static bool userPromptedForSetting;
        private readonly MLPermissions.Callbacks permissionCallbacks = new MLPermissions.Callbacks();

        void Awake()
        {
            permissionCallbacks.OnPermissionDenied += OnPermissionDenied;
            permissionCallbacks.OnPermissionDeniedAndDontAskAgain += OnPermissionDenied;
        }

        void Start()
        {
            mlInputs = new MagicLeapInputs();
            mlInputs.Enable();
            controllerActions = new MagicLeapInputs.ControllerActions(mlInputs);
            Initialize();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
                Initialize();
        }

        void Initialize()
        {
            if (!MLPermissions.CheckPermission(MLPermission.VoiceInput).IsOk)
            {
                MLPermissions.RequestPermission(MLPermission.VoiceInput, permissionCallbacks);
                return;
            }

            bool isEnabled = MLVoice.VoiceEnabled;
            startupStatus = "System Supports Voice Intents: " + isEnabled.ToString();

            if (isEnabled)
            {
                voiceInputSettingsPopup.SetActive(false);
                voiceInputErrorPopup.SetActive(false);

                MLResult result = MLVoice.SetupVoiceIntents(voiceConfiguration);

                if (result.IsOk)
                {
                    controllerActions.Bumper.performed += HandleOnBumper;
                    isProcessing = true;

                    MLVoice.OnVoiceEvent += VoiceEvent;
                }
                else
                {
                    startupStatus += "\nSetup failed with result: " + result.ToString();
                    Debug.LogError("Failed to Setup Voice Intents with result: " + result);
                }
            }
            else
            {
                if (!userPromptedForSetting)
                {
                    userPromptedForSetting = true;
                    voiceInputSettingsPopup.SetActive(true);
                }
                else
                {
                    Debug.LogError("Voice Commands has not been enabled. Voice intents requires this setting to enabled. It is found in system settings inside Magic Leap Inputs.");
                    voiceInputSettingsPopup.SetActive(false);
                    voiceInputErrorPopup.SetActive(true);
                }
            }
        }

        void OnDestroy()
        {
            MLVoice.OnVoiceEvent -= VoiceEvent;
            controllerActions.Bumper.performed -= HandleOnBumper;

            permissionCallbacks.OnPermissionDenied -= OnPermissionDenied;
            permissionCallbacks.OnPermissionDeniedAndDontAskAgain -= OnPermissionDenied;
        }

        void VoiceEvent(in bool wasSuccessful, in MLVoice.IntentEvent voiceEvent)
        {
            OnCommandDetected.Invoke(wasSuccessful, voiceEvent);
        }
        void HandleOnBumper(InputAction.CallbackContext obj)
        {
            bool bumperDown = obj.ReadValueAsButton();

            if (bumperDown)
            {
                MLResult result;
                if (isProcessing)
                {
                    result = MLVoice.Stop();
                    if (result.IsOk)
                    {
                        isProcessing = false;
                    }
                    else
                    {
                        Debug.LogError("Failed to Stop Processing Voice Intents with result: " + result);
                    }
                }
                else
                {
                    result = MLVoice.SetupVoiceIntents(voiceConfiguration);
                    if (result.IsOk)
                    {
                        isProcessing = true;
                    }
                    else
                    {
                        Debug.LogError("Failed to Re-Setup Voice Intents with result: " + result);
                    }
                }

            }
        }

        void OnPermissionDenied(string permission)
        {
            startupStatus = "<color=#ff0000><b>Permission Denied!</b></color>";
        }

        void OnVoiceInputSettingsPopupOpen()
        {
            UnityEngine.XR.MagicLeap.SettingsIntentsLauncher.LaunchSystemVoiceInputSettings();

            if (voiceInputSettingsPopup != null)
            {
                voiceInputSettingsPopup.SetActive(false);
            }
        }

        void OnVoiceInputSettingsPopupCancel()
        {
            if (voiceInputSettingsPopup != null)
            {
                voiceInputSettingsPopup.SetActive(false);
            }
        }
    }
}

