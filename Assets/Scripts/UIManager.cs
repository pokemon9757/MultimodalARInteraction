using System.Collections;
using System.Collections.Generic;
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
        [SerializeField, Tooltip("The text used to display status information for the example.")]
        private Text _statusText = null;
        [SerializeField] UIToggleButton _viewlockBtn;
        [SerializeField] UnityEvent _viewLockAction;
        bool _isUILocked = true;
        [SerializeField] GameObject _userInterface;
        [SerializeField] GameObject _pocketUI;
        enum UIVoiceCommands
        {
            Greetings = 0,
            SetActive = 7,
            ToggleLock = 8,
            SetMenu = 9,
            Help = 10
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
        }
    }
}
