using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PuzzleCat
{
    public class TitleScreenMenuManager : MonoBehaviour
    {
        public static void LoadLevel(int level)
        {
            SceneManager.LoadScene(level);
        }

        private void Awake()
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
        }
    }
}
