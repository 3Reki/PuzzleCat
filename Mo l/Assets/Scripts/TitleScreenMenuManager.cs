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
        [SerializeField] private Image settingsBackgroundImage;
        
        [SerializeField] private GameObject creditsCanvasGameObject;
        [SerializeField] private RectTransform creditsMenuTransform;
        
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
            settingsBackgroundImage.DOFade(0f, 0.6f);
            settingsMenuTransform.DOAnchorPosY(0, .6f).SetEase(Ease.InBack).onComplete =
                () =>
                {
                    settingsCanvasGameObject.SetActive(false);
                    _menuAlreadyClosed = false;
                };
        }

        public void OpenSettingsMenu()
        {
            settingsCanvasGameObject.SetActive(true);
            settingsBackgroundImage.DOFade(0.733f, 0.45f);
            settingsMenuTransform.DOMoveY(Screen.height * 0.88f, .6f).SetEase(Ease.OutBack);
        }
        
        public void CloseCredits()
        {
            if (_menuAlreadyClosed)
            {
                return;
            }

            _menuAlreadyClosed = true;
            creditsMenuTransform.DOAnchorPosY(0, .9f).SetEase(Ease.InBack).onComplete =
                () =>
                {
                    settingsBackgroundImage.enabled = true;
                    creditsCanvasGameObject.SetActive(false);
                    _menuAlreadyClosed = false;
                };
        }

        public void OpenCredits()
        {
            creditsCanvasGameObject.SetActive(true);
            settingsBackgroundImage.enabled = false;
            creditsMenuTransform.DOMoveY(Screen.height * 0.5f, .9f).SetEase(Ease.OutBack);
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
