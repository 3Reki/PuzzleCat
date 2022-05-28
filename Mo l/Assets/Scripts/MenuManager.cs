using System;
using DG.Tweening;
using PuzzleCat.Controller;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PuzzleCat
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private PortalPlacementController portalPlacementController;
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

        private GameManager.GameState _unpausedGameState;
        private float _menuInitialPositionY;
        private float _backgroundInitialAlpha;
        private bool _menuAlreadyClosed;
        
        public void ResetLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        public void SwitchPortalMode()
        {
            if (GameManager.Instance.State == GameManager.GameState.PortalMode)
            {
                GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerMovement);
                portalPlacementController.ResetSelectedGroup();
                portalSelectionToggleGroup.SetAllTogglesOff();
                portalMenuImage.rectTransform.DOAnchorPosX(0, .6f).onComplete = () =>
                {
                    portalMenuImage.enabled = false;
                    portalButtonImage.sprite = portalBookClosed;
                };
                
                return;
            }

            GameManager.Instance.UpdateGameState(GameManager.GameState.PortalMode);
            portalButtonImage.sprite = portalBookOpen;
            portalMenuImage.enabled = true;
            portalMenuImage.rectTransform.DOAnchorPosX(-portalMenuImage.rectTransform.rect.width + 23 +
                                                       ((RectTransform) portalToggles[0].transform).rect.width *
                                                       (4 - portalToggles.Length), .4f);
        }

        public void UpdateSelectedPortalGroup(int index)
        {
            portalPlacementController.UpdateSelectedPortalGroup(index);
        }

        public void SwitchPauseMenuState()
        {
            if (GameManager.Instance.State != GameManager.GameState.Menu)
            {
                _unpausedGameState = GameManager.Instance.State;
                GameManager.Instance.UpdateGameState(GameManager.GameState.Menu);
                pauseCanvasGameObject.SetActive(true);
                pauseMenuTransform.DOMoveY(Screen.height * 0.6f, .6f).SetEase(Ease.OutBack);
            }
            else
            {
                if (_menuAlreadyClosed)
                {
                    return;
                }

                _menuAlreadyClosed = true;
                pauseMenuTransform.DOMoveY(Screen.height + _menuInitialPositionY, .6f).SetEase(Ease.InBack).onComplete =
                    () =>
                    {
                        GameManager.Instance.UpdateGameState(_unpausedGameState);
                        pauseCanvasGameObject.SetActive(false);
                        _menuAlreadyClosed = false;
                    };
            }
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        public void LoadNextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        
        public void SwitchSFX(bool state)
        {
            GameData.Instance.sfxOn = state;
        }
        
        public void SwitchMusic(bool state)
        {
            GameData.Instance.musicOn = state;
        }
        
        private void ActivateLevelEndMenu()
        {
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
                        selectedPortalCheckmark.rectTransform.anchoredPosition = new Vector2(
                            toggleTransform.anchoredPosition.x - toggleTransform.sizeDelta.x * 0.5f, 0);
                        selectedPortalCheckmark.DOFade(1, .3f);
                    }
                    else
                    {
                        selectedPortalCheckmark.DOFade(0, .3f);
                    }
                });
            }
        }

        private void OnGameStateChanged(GameManager.GameState state)
        {
            if (state == GameManager.GameState.End)
            {
                ActivateLevelEndMenu();
            }
        }

        private void Awake()
        {
            _menuInitialPositionY = pauseMenuTransform.anchoredPosition.y;

            SetupPortalToggles();

            if (SceneManager.GetActiveScene().buildIndex + 1 == SceneManager.sceneCountInBuildSettings)
            {
                nextLevelButton.interactable = false;
            }

            _backgroundInitialAlpha = endLevelBackground.color.a;
            endLevelBackground.color = new Color(endLevelBackground.color.r, endLevelBackground.color.g, endLevelBackground.color.b, 0);
            endLevelFrame.localScale = Vector3.zero;

            GameManager.OnGameStateChanged += OnGameStateChanged;
        }

        private void Start()
        {
            sfxToggle.isOn = GameData.Instance.sfxOn;
            musicToggle.isOn = GameData.Instance.musicOn;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }
    }
}
