using System.Collections;
using System.Collections.Generic;
using MagicLeap.Examples;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
        [SerializeField] UIToggleButton _viewlockBtn;
        [SerializeField] UnityEvent _viewLockAction;
        [SerializeField] GameObject _userInterface;
        enum UIVoiceCommands
        {
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
                case UIVoiceCommands.SetActive:
                    bool isOn = UtilityScript.GetSlotValue(voiceEvent.EventName, "OnOff") == "On";
                    _userInterface.SetActive(isOn);
                    break;
                case UIVoiceCommands.ToggleLock:
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
