using PuzzleCat.TutorialAnimations;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class TutorialPlayerController : PlayerController
    {
        [SerializeField] private Tutorial tutorial;

        private RaycastHit _hit;

        protected override void HandleTouchMoved()
        {
            
        }

        protected override void HandleTouchEnd()
        {
            switch (GameManager.Instance.State)
            {
                case GameManager.GameState.PlayerMovement:
                    if (!tutorial.CanMovePlayer())
                    {
                        break;
                    }
                    
                    if (catController.HandlePlayerMovement())
                    {
                        tutorial.OnPlayerMovement();
                    }
                    
                    break;
                case GameManager.GameState.PortalMode:
                    if (tutorial.CanPlacePortal())
                    {
                        portalPlacementController.HandlePortalPlacement();
                    }
                    break;
                case GameManager.GameState.CameraMovement:
                case GameManager.GameState.FurnitureMovement:
                    GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerMovement);
                    break;
            }
        }

        private void Update()
        {
            if (GameManager.Instance.State is GameManager.GameState.Menu or GameManager.GameState.End)
                return;

            if (!inputManager.TwoTouchesDone && inputManager.TouchCount == 1)
            {
                HandleSingleTouch();
            }
        }
    }
}
