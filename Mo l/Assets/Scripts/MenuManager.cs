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
        [SerializeField] private Toggle[] portalToggles;
        [SerializeField] private Sprite portalBookOpen;
        [SerializeField] private Sprite portalBookClosed;
        [SerializeField] private GameObject pauseCanvasGameObject;
        [SerializeField] private RectTransform pauseMenuTransform;

        private GameManager.GameState _unpausedGameState;
        private float _menuInitialPositionY;
        private bool _gamePaused;
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
            portalMenuImage.rectTransform.DOAnchorPosX(-0.422f * Screen.width, .4f);
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

        public void LoadMenu()
        {
            SceneManager.LoadScene(0);
        }

        private void Awake()
        {
            _menuInitialPositionY = pauseMenuTransform.anchoredPosition.y;

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
    }
}
