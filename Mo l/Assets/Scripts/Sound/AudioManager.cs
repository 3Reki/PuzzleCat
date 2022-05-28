using System;
using UnityEngine;
using UnityEngine.Audio;

namespace PuzzleCat.Sound
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        public AudioMixerGroup musicMixerGroup;
        public AudioMixerGroup sfxMixerGroup;
        public Sound[] sounds;


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;

                switch (s.audioType)
                {
                    case Sound.AudioTypes.Sfx:
                        s.source.outputAudioMixerGroup = sfxMixerGroup;
                        break;
                    case Sound.AudioTypes.Music:
                        s.source.outputAudioMixerGroup = musicMixerGroup;
                        break;
                }
            }
        }

        public void Play(string soundName)
        {
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + soundName + " not found!");
            }
            s.source.Play();
        }
        
        public void StopPlaying(string soundName)
        {
            Sound s = Array.Find(sounds, item => item.name == soundName);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }

            s.source.Stop();
        }
        
        public void SfxMixerVolumeOn()
        {
            sfxMixerGroup.audioMixer.SetFloat("Exposed Sfx Volume", 0f);
        }
        
        public void SfxMixerVolumeOff()
        {
            sfxMixerGroup.audioMixer.SetFloat("Exposed Sfx Volume", -80f);
        }
        
        public void MusicMixerVolumeOn()
        {
            musicMixerGroup.audioMixer.SetFloat("Exposed Music Volume", 0f);
        }
        
        public void MusicMixerVolumeOff()
        {
            musicMixerGroup.audioMixer.SetFloat("Exposed Music Volume", -80f);
        }
    }
}