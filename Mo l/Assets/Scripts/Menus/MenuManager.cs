using DG.Tweening;
using PuzzleCat.Controller;
using PuzzleCat.Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PuzzleCat.Menus
{
    public class MenuManager : MonoBehaviour
    {
        
        [SerializeField] private Image portalButtonImage;
        [SerializeField] private Image portalMenuImage;
        [SerializeField] private Image selectedPortalCheckmark;
        [SerializeField] private ToggleGroup portalSelectionToggleGroup;
        public Toggle[] portalToggles;
        [SerializeField] private Sprite portalBookOpen;
        [SerializeField] private Sprite portalBookClosed;

        [SerializeField] private GameObject pauseCanvasGameObject;
        [SerializeField] private RectTransform pauseMenuTransform;

        [SerializeField] private GameObject endLevelCanvasGameObject;
        [SerializeField] private Image endLevelBackground;
        [SerializeField] private RectTransform endLevelFrame;
        [SerializeField] private Button nextLevelButton;

        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Toggle musicToggle;

        private InputManager _inputManager;
        private float _menuInitialPositionY;
        private float _backgroundInitialAlpha;
        private bool _menuAlreadyClosed;

        public void OpenPortalBook()
        {
            portalMenuImage.rectTransform.DOComplete();
            portalButtonImage.sprite = portalBookOpen;
            portalMenuImage.enabled = true;
            AudioManager.Instance.Play("PortalBookIn");
            portalMenuImage.rectTransform.DOAnchorPosX(-portalMenuImage.rectTransform.rect.width +
                                                       ((RectTransform) portalToggles[0].transform).rect.width *
                                                       (4 - portalToggles.Length), .4f);
        }

        public void ClosePortalBook()
        {
            AudioManager.Instance.Play("PortalBookOut");
            portalSelectionToggleGroup.SetAllTogglesOff();
            portalMenuImage.rectTransform.DOComplete();
            portalMenuImage.rectTransform.DOAnchorPosX(0, .6f).onComplete = () =>
            {
                portalMenuImage.enabled = false;
                portalButtonImage.sprite = portalBookClosed;
            };
        }
        
        public void ResetLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            AudioManager.Instance.Play("Ok");
        }

        public void SwitchPortalMode(bool state)
        {
            _inputManager.PortalMode = state;
        }

        public void UpdateSelectedPortalGroup(int index)
        {
            GameManager.Instance.portalState.UpdateSelectedPortalGroup(index);
            //inputManager.SelectedPortalIndex = index;
            AudioManager.Instance.Play("Ok");
        }

        public void SwitchPauseMenuState()
        {
            if (!GameManager.Instance.MenuOpened)
            {
                GameManager.Instance.MenuOpened = true;
                pauseCanvasGameObject.SetActive(true);
                pauseMenuTransform.DOMoveY(Screen.height * 0.6f, .6f).SetEase(Ease.OutBack);

                AudioManager.Instance.Play("Pause");
            }
            else
            {
                if (_menuAlreadyClosed)
                {
                    return;
                }

                _menuAlreadyClosed = true;
                AudioManager.Instance.Play("Pause");
                pauseMenuTransform.DOMoveY(Screen.height + _menuInitialPositionY, .6f).SetEase(Ease.InBack).onComplete =
                    () =>
                    {
                        GameManager.Instance.MenuOpened = false;
                        pauseCanvasGameObject.SetActive(false);
                        _menuAlreadyClosed = false;
                    };
            }
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene(0);
            AudioManager.Instance.Play("Ok");
        }

        public void LoadNextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            AudioManager.Instance.Play("Ok");
        }

        public void SwitchSFX(bool state)
        {
            GameData.Instance.sfxOn = state;
            AudioManager.Instance.Play("Ok");

            if (sfxToggle.isOn)
            {
                AudioManager.Instance.SetSfxVolumeOn();
            }
            else
            {
                AudioManager.Instance.SetSfxVolumeOff();
            }
        }

        public void SwitchMusic(bool state)
        {
            GameData.Instance.musicOn = state;
            AudioManager.Instance.Play("Ok");

            if (musicToggle.isOn)
            {
                AudioManager.Instance.SetMusicVolumeOn();
            }
            else
            {
                AudioManager.Instance.SetMusicVolumeOff();
            }
        }

        public void OpenAdd()
        {
            Application.OpenURL("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
        }

        private void ActivateLevelEndMenu()
        {
            GameManager.Instance.MenuOpened = true;
            endLevelCanvasGameObject.SetActive(true);
            endLevelBackground.DOFade(_backgroundInitialAlpha, .4f);
            endLevelFrame.DOScale(1, .4f).SetEase(Ease.OutBack);
        }

        private void SetupPortalToggles()
        {
            foreach (Toggle portalToggle in portalToggles)
            {
                portalToggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                    {
                        var toggleTransform = (RectTransform) portalToggle.transform;
                        toggleTransform.DOScale(1.2f, 0.2f).onComplete =
                            () => portalToggle.transform.DOScale(1f, 0.2f);
                        selectedPortalCheckmark.rectTransform.position = toggleTransform.position;
                        selectedPortalCheckmark.DOFade(1, .3f);
                    }
                    else
                    {
                        selectedPortalCheckmark.DOFade(0, .3f);
                    }
                });
            }
        }

        private void Awake()
        {
            _inputManager = FindObjectOfType<InputManager>();
            _menuInitialPositionY = pauseMenuTransform.anchoredPosition.y;

            SetupPortalToggles();

            if (SceneManager.GetActiveScene().buildIndex + 1 == SceneManager.sceneCountInBuildSettings)
            {
                nextLevelButton.onClick.RemoveAllListeners();
                nextLevelButton.onClick.AddListener(() =>
                {
                    GameData.Instance.gameFinished = true;
                    SceneManager.LoadScene(0);
                    AudioManager.Instance.Play("Ok");
                });
            }

            _backgroundInitialAlpha = endLevelBackground.color.a;
            endLevelBackground.color = new Color(endLevelBackground.color.r, endLevelBackground.color.g, endLevelBackground.color.b, 0);
            endLevelFrame.localScale = Vector3.zero;

            GameManager.OnLevelEnd += ActivateLevelEndMenu;
        }

        private void Start()
        {
            sfxToggle.isOn = GameData.Instance.sfxOn;
            musicToggle.isOn = GameData.Instance.musicOn;
        }

        private void OnDestroy()
        {
            GameManager.OnLevelEnd -= ActivateLevelEndMenu;
        }
    }
}
