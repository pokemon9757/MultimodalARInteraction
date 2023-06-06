using System.Collections;
using System.Collections.Generic;
using MagicLeap.Examples;
using UnityEngine;
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
        enum UIVoiceCommands
        {
            SetActive = 7,
            SetLock = 8,
            SetMenu = 9
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
                    break;
                case UIVoiceCommands.SetLock:
                    bool isLock = UtilityScript.GetSlotValue(voiceEvent.EventName, "LockState") == "Lock";

                    break;
                case UIVoiceCommands.SetMenu:
                    string menuType = UtilityScript.GetSlotValue(voiceEvent.EventName, "MenuType");
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
            }
        }
    }
}
