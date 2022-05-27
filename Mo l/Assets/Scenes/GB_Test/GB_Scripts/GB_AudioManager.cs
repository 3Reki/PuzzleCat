using System;
using UnityEngine;

namespace PuzzleCat.Scenes.GB_Test.GB_Scripts
{
    public class GB_AudioManager : MonoBehaviour
    {
        //using PuzzleCat.Scenes.GB_Test.GB_Scripts;
        //GB_AudioManager.instance.Play("stringName");

        public GB_Sound[] sounds;

        public static float volumeSlider;

        public static GB_AudioManager instance;

        void Awake()
        {
            if (instance == null)
                instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            foreach (GB_Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }

        private void Update()
        {
            //à réactiver si on a un volumeSlider
            /*foreach (GB_Sound s in sounds)
            s.source.volume = s.volume * volumeSlider;*/

        }
        public void Play(string name)
        {
            GB_Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
            }
            s.source.Play();
        }
        public void StopPlaying(string sound)
        {
            GB_Sound s = Array.Find(sounds, item => item.name == sound);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }

            s.source.Stop();
        }
    }
}

