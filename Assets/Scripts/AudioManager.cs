using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hellmade.Sound;
namespace MMI
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] float _volume = 1;
        [SerializeField] AudioClip _helloClip;
        [SerializeField] AudioClip _actionDoneClip;
        [SerializeField] AudioClip _objectDeleteClip;
        [SerializeField] AudioClip _objectSelectClip;
        [SerializeField] AudioClip _objectDeselectClip;

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