using UnityEngine;

namespace PuzzleCat.Scenes.GB_Test.GB_Scripts
{
    [System.Serializable]
    public class GB_Sound
    {
        public enum AudioTypes { sfx, music }
        public AudioTypes audioType;

        public string name;

        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume;

        [Range(0.1f, 3f)]
        public float pitch;

        public bool loop;

        [HideInInspector]
        public AudioSource source;
    }
}