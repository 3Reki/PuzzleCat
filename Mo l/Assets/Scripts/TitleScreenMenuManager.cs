using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PuzzleCat
{
    public class TitleScreenMenuManager : MonoBehaviour
    {
        [SerializeField] private Button[] levelButtons;
        [SerializeField] private GameObject settingsCanvasGameObject;
        [SerializeField] private RectTransform settingsMenuTransform;
        
        private bool _settingsOpened;
        private bool _menuAlreadyClosed;
        
        public static void LoadLevel(int level)
        {
            SceneManager.LoadScene(level);
        }

        public void UnlockAllLevels()
        {
            foreach (Button button in levelButtons)
            {
                button.interactable = true;
            }

            GameData.Instance.unlockedLevelsCount = levelButtons.Length;
        }
        
        public void ResetSave()
        {
            for (var i = 1; i < levelButtons.Length; i++)
            {
                levelButtons[i].interactable = false;
            }

            GameData.Instance.unlockedLevelsCount = 1;
        }

        public void CloseSettingsMenu()
        {
            if (_menuAlreadyClosed)
            {
                return;
            }

            _menuAlreadyClosed = true;
            settingsMenuTransform.DOAnchorPosY(0, .6f).SetEase(Ease.InBack).onComplete =
                () =>
                {
                    _settingsOpened = false;
                    settingsCanvasGameObject.SetActive(false);
                    _menuAlreadyClosed = false;
                };
        }

        public void OpenSettingsMenu()
        {
            _settingsOpened = true;
            settingsCanvasGameObject.SetActive(true);
            settingsMenuTransform.DOMoveY(Screen.height * 0.88f, .6f).SetEase(Ease.OutBack);
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
