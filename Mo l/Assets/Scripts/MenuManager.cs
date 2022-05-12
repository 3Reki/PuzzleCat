using System;
using DG.Tweening;
using PuzzleCat.Controller;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PuzzleCat
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private PortalPlacementController portalPlacementController;
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

        public void SwitchPortalMode(int index)
        {
            portalPlacementController.SwitchPortalMode(index);
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
        }
    }
}
