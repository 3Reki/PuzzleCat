using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PuzzleCat
{
    public class TitleScreenMenuManager : MonoBehaviour
    {
        [SerializeField] private Button[] levelButtons;
        
        public static void LoadLevel(int level)
        {
            SceneManager.LoadScene(level);
        }

        private void Awake()
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
        }

        private void Start()
        {
            for (int i = GameData.Instance.unlockedLevelsCount; i < levelButtons.Length; i++)
            {
                levelButtons[i].interactable = false;
            }
            
        }
    }
}
