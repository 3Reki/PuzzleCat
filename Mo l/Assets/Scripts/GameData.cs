using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PuzzleCat
{
    public class GameData : MonoBehaviour
    {
        public static GameData Instance;

        public int unlockedLevelsCount;
        public bool sfxOn;
        public bool musicOn;

        private void UpdatePlayerPrefs()
        {
            PlayerPrefs.SetInt("Unlocked Levels Count", unlockedLevelsCount);
            PlayerPrefs.SetInt("SFX On", Convert.ToInt32(sfxOn));
            PlayerPrefs.SetInt("Music On", Convert.ToInt32(musicOn));
            PlayerPrefs.Save();
        }
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                print("yes");
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(this);

#if !UNITY_EDITOR
            unlockedLevelsCount = PlayerPrefs.GetInt("Unlocked Levels Count", 1);
            sfxOn = Convert.ToBoolean(PlayerPrefs.GetInt("SFX On", 1));
            musicOn = Convert.ToBoolean(PlayerPrefs.GetInt("Music On", 1));
#else
            unlockedLevelsCount = SceneManager.sceneCountInBuildSettings - 1;
            sfxOn = true;
            musicOn = true;
#endif
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            UpdatePlayerPrefs();
        }
    }
}
