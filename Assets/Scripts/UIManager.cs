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
        [SerializeField] VoiceIntents _voice;
        [SerializeField] UIButton _overviewMenuBtn;
        [SerializeField] UIButton _controlsMenuBtn;
        [SerializeField] UIButton _issuesMenuBtn;
        [SerializeField] UIButton _statusMenuBtn;
        [SerializeField] private Text _statusText = null;
        [SerializeField] private Text _pocketUIStatusText = null;
        [SerializeField] float _statusResetDelay = 3f;
        [SerializeField] UIToggleButton _viewlockBtn;
        [SerializeField] UnityEvent _viewLockAction;
        [SerializeField] GameObject _userInterface;
        [SerializeField] GameObject _pocketUI;
        bool _isUILocked = true;
        enum UIVoiceCommands
        {
            Greetings = 0,
            SetActive = 7,
            ToggleLock = 8,
            SetMenu = 9,
            Help = 10
        }
        private Coroutine resetStatusCoroutine;

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
