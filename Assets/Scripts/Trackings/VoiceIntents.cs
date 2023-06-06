using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.MagicLeap;
namespace MMI
{
    public class VoiceIntents : MonoBehaviour
    {

        // Voice Configuration that will be created at runtime.
        [SerializeField] private MLVoiceIntentsConfiguration _voiceConfiguration;

        public UnityEvent<bool, MLVoice.IntentEvent> OnCommandDetected;

        // Start is called before the first frame update
        void Start()
        {
            // Configure the voice intents based on the new configuration.
            MLResult result = MLVoice.SetupVoiceIntents(_voiceConfiguration);

            if (result.IsOk)
            {
                MLVoice.OnVoiceEvent += OnVoiceEvent;
            }
            else
            {
                Debug.LogError("Failed to Setup Voice Intents with result: " + result);
            }
        }

        private void OnVoiceEvent(in bool wasSuccessful, in MLVoice.IntentEvent voiceEvent)
        {
            OnCommandDetected.Invoke(wasSuccessful, voiceEvent);
        }
    }
}


