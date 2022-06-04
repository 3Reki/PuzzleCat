using UnityEngine;

namespace PuzzleCat.Sound
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        public AudioSource musicSource;
        public AudioSource[] sfxSources;
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
        }

        public void Play(string soundName)
        {
            foreach (Sound sound in sounds)
            {
                if (soundName != sound.name) continue;

                AudioSource source = sound.audioType == Sound.AudioTypes.Sfx ? FindAvailableAudioSource() : musicSource;
                source.clip = sound.clip;
                source.volume = sound.volume;
                source.pitch = sound.pitch;
                source.loop = sound.loop;
                source.Play();
                return;
            }
            
            Debug.LogWarning("Sound: " + soundName + " not found!");
        }
        
        public void StopPlaying(string soundName)
        {
            foreach (Sound sound in sounds)
            {
                if (soundName != sound.name) continue;

                switch (sound.audioType)
                {
                    case Sound.AudioTypes.Sfx:
                        StopSfxSource(sound.clip);
                        break;
                    case Sound.AudioTypes.Music:
                        musicSource.Stop();
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException();
                }
                return;
            }
            
            Debug.LogWarning("Sound: " + soundName + " not found!");
        }
        
        public void SetSfxVolumeOn()
        {
            foreach (AudioSource source in sfxSources)
            {
                source.mute = false;
            }
        }
        
        public void SetSfxVolumeOff()
        {
            foreach (AudioSource source in sfxSources)
            {
                source.mute = true;
            }
        }
        
        public void SetMusicVolumeOn()
        {
            musicSource.mute = false;
        }
        
        public void SetMusicVolumeOff()
        {
            musicSource.mute = true;
        }

        private AudioSource FindAvailableAudioSource()
        {
            foreach (AudioSource source in sfxSources)
            {
                if (!source.isPlaying) 
                    return source;
            }
            
            sfxSources[0].Stop();
            return sfxSources[0];
        }
        
        private void StopSfxSource(AudioClip audioClip)
        {
            foreach (AudioSource source in sfxSources)
            {
                if (!source.isPlaying || source.clip != audioClip) continue;
                
                source.Stop();
                return;
            }
        }
    }
}