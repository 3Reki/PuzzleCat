using PuzzleCat.TutorialAnimations;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class TutorialPlayerController : PlayerController
    {
        [SerializeField] private Tutorial tutorial;
        //
        // private bool _triedSelectingFurniture;
        //
        // protected override void HandleTouchStationary()
        // {
        //     if (_triedSelectingFurniture)
        //         return;
        //     if (GameManager.Instance.State is GameManager.PlayerState.PortalMode 
        //         or GameManager.PlayerState.FurnitureMovement or GameManager.PlayerState.CameraMovement) 
        //         return;
        //     // if (!(Time.time - TouchStartTime > holdTouchThreshold)) 
        //     //     return;
        //
        //     _triedSelectingFurniture = true;
        //     
        //     if (!tutorial.CanSelectElement()) 
        //         return;
        //     
        //     GameManager.Instance.UpdateGameState(GameManager.PlayerState.FurnitureMovement);
        //     tutorial.OnElementSelection();
        // }
        //
        // protected override void HandleTouchMoved()
        // {
        //     //if (!TouchMoved && (inputManager.FirstTouchPosition - TouchInitialPosition).magnitude < dragDistance) return;
        //     TouchMoved = true;
        //
        //     if (GameManager.Instance.State != GameManager.PlayerState.FurnitureMovement ||
        //         !tutorial.CanMoveElement()) 
        //         return;
        //     
        //     if (movableElementsController.HandleMovement())
        //     {
        //         tutorial.OnElementMovement();
        //     }
        // }
        //
        // protected override void HandleTouchEnd()
        // {
        //     _triedSelectingFurniture = false;
        //     switch (GameManager.Instance.State)
        //     {
        //         case GameManager.PlayerState.PlayerMovement:
        //             if (!tutorial.CanMovePlayer())
        //             {
        //                 break;
        //             }
        //             
        //             //if (catController.HandlePlayerMovement())
        //             {
        //                 tutorial.OnPlayerMovement();
        //             }
        //             
        //             break;
        //         case GameManager.PlayerState.PortalMode:
        //             if (!tutorial.CanPlacePortal())
        //             {
        //                 break;
        //             }
        //
        //             if (portalPlacementController.HandlePortalPlacement())
        //             {
        //                 tutorial.OnPortalPlaced();
        //             }
        //             
        //             break;
        //         case GameManager.PlayerState.FurnitureMovement:
        //             tutorial.OnElementDeselection();
        //             GameManager.Instance.UpdateGameState(GameManager.PlayerState.PlayerMovement);
        //             break;
        //     }
        // }
        //
        // private void Update()
        // {
        //     if (GameManager.Instance.State is GameManager.PlayerState.Menu or GameManager.PlayerState.End)
        //         return;
        //
        //     if (!inputManager.TwoTouchesDone && inputManager.TouchCount == 1)
        //     {
        //         HandleSingleTouch();
        //     }
        // }
    }
}
