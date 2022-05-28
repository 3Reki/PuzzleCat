using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PuzzleCat.Scenes.GB_Test.GB_Scripts;

namespace PuzzleCat
{
    public class TitleScreenMenuManager : MonoBehaviour
    {
        [SerializeField] private RectTransform[] levelSelectionTransforms;
        [SerializeField] private Button leftSelectionArrow;
        [SerializeField] private Button rightSelectionArrow;
        [SerializeField] private Button[] levelButtons;
        
        [SerializeField] private GameObject settingsCanvasGameObject;
        [SerializeField] private RectTransform settingsMenuTransform;
        [SerializeField] private Image settingsBackgroundImage;
        
        [SerializeField] private GameObject creditsCanvasGameObject;
        [SerializeField] private RectTransform creditsMenuTransform;

        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Toggle musicToggle;
        
        private bool _menuAlreadyClosed;
        private int _currentLevelSelectionIndex;
        
        public static void LoadLevel(int level)
        {
            SceneManager.LoadScene(level);
            GB_AudioManager.instance.Play("Ok");
        }

        public void UnlockAllLevels()
        {
            foreach (Button button in levelButtons)
            {
                button.interactable = true;
            }

            GameData.Instance.unlockedLevelsCount = levelButtons.Length;
            GB_AudioManager.instance.Play("Ok");
        }

        public void ResetSave()
        {
            for (var i = 1; i < levelButtons.Length; i++)
            {
                levelButtons[i].interactable = false;
            }

            GameData.Instance.unlockedLevelsCount = 1;
            GB_AudioManager.instance.Play("Back");
        }

        public void CloseSettingsMenu()
        {
            if (_menuAlreadyClosed)
            {
                return;
            }

            _menuAlreadyClosed = true;
            GB_AudioManager.instance.Play("Back");
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
            GB_AudioManager.instance.Play("Back");
        }

        public void CloseCredits()
        {
            if (_menuAlreadyClosed)
            {
                return;
            }

            _menuAlreadyClosed = true;
            GB_AudioManager.instance.Play("Ok");
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
            GB_AudioManager.instance.Play("Ok");
        }

        public void SwitchSFX(bool state)
        {
            GameData.Instance.sfxOn = state;
            GB_AudioManager.instance.Play("Ok");

            if (sfxToggle.isOn)
            {
                GB_AudioManager.instance.SfxMixerVolumeOn();
            }   
            else
            {
                GB_AudioManager.instance.SfxMixerVolumeOff();
            }
        }
        public void SwitchMusic(bool state)
        {
            GameData.Instance.musicOn = state;
            GB_AudioManager.instance.Play("Ok");

            if (musicToggle.isOn)
            {
                GB_AudioManager.instance.MusicMixerVolumeOn();
            }
            else
            {
                GB_AudioManager.instance.MusicMixerVolumeOff();
            }
        }

        public void ChangeLevelSelectionPanel(int direction)
        {
            foreach (RectTransform selectionTransform in levelSelectionTransforms)
            {
                selectionTransform.DOAnchorPosX(selectionTransform.anchoredPosition.x - Screen.width * direction, 0.5f);
                GB_AudioManager.instance.Play("Ok");
            }

            _currentLevelSelectionIndex += direction;

            rightSelectionArrow.interactable = _currentLevelSelectionIndex != levelSelectionTransforms.Length - 1;
            leftSelectionArrow.interactable = _currentLevelSelectionIndex != 0;
        }

        private void Awake()
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;

            GB_AudioManager.instance.StopPlaying("LevelMusic");
            GB_AudioManager.instance.StopPlaying("LevelWin");
            GB_AudioManager.instance.Play("MenuMusic");
        }

        private void Start()
        {
            for (int i = GameData.Instance.unlockedLevelsCount; i < levelButtons.Length; i++)
            {
                levelButtons[i].interactable = false;
            }

            sfxToggle.isOn = GameData.Instance.sfxOn;
            musicToggle.isOn = GameData.Instance.musicOn;

            leftSelectionArrow.interactable = false;
        }
    }
}