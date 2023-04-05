using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
namespace MMI
{
    public class VoiceIntents : MonoBehaviour, ITrackingAbility
    {
        // Voice Configuration that will be created at runtime.
        [SerializeField] private MLVoiceIntentsConfiguration _voiceConfiguration;

        // Start is called before the first frame update
        public void Init()
        {
            // Configure the voice intents based on the new configuration.
            MLResult result = MLVoice.SetupVoiceIntents(_voiceConfiguration);

            if (!result.IsOk)
            {
                Debug.LogError("Failed to Setup Voice Intents with result: " + result);
            }
        }
        public void ProcessAbility()
        {
        }

        public void SetDebugElementsActive(bool active)
        {
            throw new System.NotImplementedException();
        }
    }
}