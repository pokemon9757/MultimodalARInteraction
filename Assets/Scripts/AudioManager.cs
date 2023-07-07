using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hellmade.Sound;
namespace MMI
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] float _volume = 1;
        [SerializeField, Tooltip("Say hello to the user")] AudioClip _helloClip;
        [SerializeField, Tooltip("Once an action is performed successfully")] AudioClip _actionDoneClip;
        [SerializeField, Tooltip("Once the delete action is performed successfully")] AudioClip _objectDeleteClip;
        [SerializeField, Tooltip("Once an object is selected by the gaze")] AudioClip _objectSelectClip;
        [SerializeField, Tooltip("Once an object is deselected by the gaze ")] AudioClip _objectDeselectClip;

        public Audio HelloAudio { get; private set; }
        public Audio ActionDoneAudio { get; private set; }
        public Audio SelectAudio { get; private set; }
        public Audio DeselectAudio { get; private set; }
        public Audio DeleteAudio { get; private set; }

        void Start()
        {
            EazySoundManager.IgnoreDuplicateUISounds = true;
            HelloAudio = EazySoundManager.GetSoundAudio(EazySoundManager.PrepareSound(_helloClip, _volume));
            ActionDoneAudio = EazySoundManager.GetSoundAudio(EazySoundManager.PrepareSound(_actionDoneClip, _volume));
            SelectAudio = EazySoundManager.GetSoundAudio(EazySoundManager.PrepareSound(_objectSelectClip, _volume));
            DeselectAudio = EazySoundManager.GetSoundAudio(EazySoundManager.PrepareSound(_objectDeselectClip, _volume));
            DeleteAudio = EazySoundManager.GetSoundAudio(EazySoundManager.PrepareSound(_objectDeleteClip, _volume));
        }
    }
}