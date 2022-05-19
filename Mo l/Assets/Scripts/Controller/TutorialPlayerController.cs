using PuzzleCat.TutorialAnimations;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class TutorialPlayerController : PlayerController
    {
        [SerializeField] private TutorialOne firstTutorial;

        private RaycastHit _hit;

        protected override void HandleTouchMoved()
        {
            
        }

        protected override void HandleTouchEnd()
        {
            switch (GameManager.Instance.State)
            {
                case GameManager.GameState.PlayerMovement:
                    if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out _hit, GameManager.Instance.MainCamera, -5, 100f, true, 2)) 
                        break;

                    if (!firstTutorial.IsValidTouch(Utils.Utils.WorldPointAsGridPoint(_hit.normal, _hit.point)))
                        break;
                    
                    if (catController.HandlePlayerMovement())
                    {
                        if (firstTutorial.HasNextPosition())
                        {
                            firstTutorial.NextPosition();
                            firstTutorial.PlayAnimation();
                            break;
                        }
                        
                        firstTutorial.StopAnimation();
                    }
                    
                    break;
                case GameManager.GameState.PortalMode:
                    portalPlacementController.HandlePortalPlacement();
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
