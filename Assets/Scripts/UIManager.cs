using System.Collections;
using System.Collections.Generic;
using System.Text;
using MagicLeap.Examples;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

namespace MMI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField, Tooltip("The systen Voice Intent")] VoiceIntents _voice;
        [SerializeField] UIButton _overviewMenuBtn;
        [SerializeField] UIButton _controlsMenuBtn;
        [SerializeField] UIButton _issuesMenuBtn;
        [SerializeField] UIButton _statusMenuBtn;
        [SerializeField, Tooltip("The text used to display recognized voice command")] Text _statusText = null;
        [SerializeField, Tooltip("Pocket UI text used to display recognized voice command")] Text _pocketUIStatusText = null;
        [SerializeField, Tooltip("The delay time in seconds to reset the recognized voice command")] float _statusResetDelay = 3f;
        [SerializeField] UIToggleButton _viewlockBtn;
        [SerializeField, Tooltip("The unity event triggered when the view lock voice command is recognized")] UnityEvent _viewLockAction;
        [SerializeField, Tooltip("The parent of the main UI")] GameObject _userInterface;
        [SerializeField, Tooltip("The pocket UI in case user wants to disable the main UI")] GameObject _pocketUI;
        bool _isUILocked = true;
        Coroutine resetStatusCoroutine;
        enum UIVoiceCommands
        {
            Greetings = 0, /**< User says hello */
            SetActive = 7, /**< User wants to disable/enable the UI */
            ToggleLock = 8,/**< User wants to lock/unlock the UI movement */
            SetMenu = 9,/**< User wants to change the UI menu */
            Help = 10 /**< Ask for assistance */
        }

        void Start()
        {
            _voice.OnCommandDetected.AddListener(OnCommandDetected);
        }

        void OnCommandDetected(bool wasSuccessful, MLVoice.IntentEvent voiceEvent)
        {
            switch ((UIVoiceCommands)voiceEvent.EventID)
            {
                case UIVoiceCommands.Greetings:
                    _statusText.text += "\nOh hello there~\n";
                    AudioManager.Instance.HelloAudio.Play();
                    break;
                case UIVoiceCommands.SetActive:
                    var onValue = UtilityScript.GetSlotValue(voiceEvent.EventName, "OnOff");
                    bool isOn = onValue == "On" || onValue == "Enable";
                    _userInterface.SetActive(isOn);
                    _pocketUI.SetActive(!isOn);
                    break;
                case UIVoiceCommands.ToggleLock:
                    onValue = UtilityScript.GetSlotValue(voiceEvent.EventName, "OnOff");
                    isOn = onValue == "On" || onValue == "Enable";
                    if (isOn == _isUILocked) return;
                    _isUILocked = isOn;
                    _viewlockBtn.Pressed();
                    _viewLockAction?.Invoke();
                    break;
                case UIVoiceCommands.SetMenu:
                    string menuType = UtilityScript.GetSlotValue(voiceEvent.EventName, "MenuName");
                    switch (menuType)
                    {
                        case "Overview":
                            _overviewMenuBtn.Pressed();
                            break;
                        case "Controls":
                            _controlsMenuBtn.Pressed();
                            break;
                        case "Issues":
                            _issuesMenuBtn.Pressed();
                            break;
                        case "Status":
                            _statusMenuBtn.Pressed();
                            break;
                    }
                    break;
                case UIVoiceCommands.Help:
                    _controlsMenuBtn.Pressed();
                    break;
            }

            // Update status text 
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append($"<color=#B7B7B8><b>Recognized voice command:</b></color> <i>{voiceEvent.EventName}</i>");
            string text = strBuilder.ToString();
            _statusText.text = text;
            _pocketUIStatusText.text = text;

            // If the coroutine is already running, stop it
            if (resetStatusCoroutine != null)
                StopCoroutine(resetStatusCoroutine);

            // Start the coroutine
            resetStatusCoroutine = StartCoroutine(ResetStatusDescription());
        }

        IEnumerator ResetStatusDescription()
        {
            yield return new WaitForSeconds(_statusResetDelay);
            string text = "Listening to your beautiful voice...\nFeel free to <b>shout</b> for help if you need assistance (not at me though)!";
            _statusText.text = text;
            _pocketUIStatusText.text = text;
        }
    }

}
