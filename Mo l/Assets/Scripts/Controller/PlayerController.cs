using UnityEngine;

namespace PuzzleCat.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private MovableElementsController movableElementsController;
        [SerializeField] private CatController catController;
        [SerializeField] private PortalPlacementController portalPlacementController;
        [SerializeField] private float holdTouchThreshold = 0.3f;
        [SerializeField] private float dragDistance = 3;
        
        private Vector3 _touchInitialPosition;
        private float _touchStartTime;
        private bool _touchMoved;
        private bool _holdingTouch;
        

        private void HandleSingleTouch()
        {
            switch (inputManager.FirstTouchPhase)
            {
                case TouchPhase.Began:
                    HandleTouchStart();
                    break;
                
                case TouchPhase.Stationary:
                    HandleTouchStationary();
                    break;
                
                case TouchPhase.Moved:
                    HandleTouchMoved();
                    break;

                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    HandleTouchEnd();
                    break;
            }
        }

        private void HandleTouchStart()
        {
            _touchInitialPosition = inputManager.FirstTouchPosition;
            _touchStartTime = Time.time;
            _touchMoved = false;
        }

        private void HandleTouchStationary()
        {
            if (_touchMoved || _holdingTouch || !(Time.time - _touchStartTime > holdTouchThreshold)) return;
            
            GameManager.Instance.UpdateGameState(GameManager.GameState.FurnitureMovement);
            _holdingTouch = true;
        }

        private void HandleTouchMoved()
        {
            if ((inputManager.FirstTouchPosition - _touchInitialPosition).magnitude < dragDistance) return;
            
            _touchMoved = true;
            if (GameManager.Instance.State == GameManager.GameState.FurnitureMovement)
            {
                movableElementsController.HandleMovement();
            }
        }

        private void HandleTouchEnd()
        {
            _holdingTouch = false;

            switch (GameManager.Instance.State)
            {
                case GameManager.GameState.PlayerMovement:
                    catController.HandlePlayerMovement();
                    break;
                case GameManager.GameState.PortalMode:
                    portalPlacementController.HandlePortalPlacement();
                    break;
                case GameManager.GameState.FurnitureMovement:
                    GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerMovement);
                    break;
            }
        }

        private void Start()
        {
            dragDistance = Screen.height * dragDistance * 0.01f;
        }

        private void Update()
        {
            if (!inputManager.TwoTouchesDone && inputManager.TouchCount == 1)
            {
                HandleSingleTouch();
            }
            else if (inputManager.TouchCount == 2)
            {
                cameraController.HandleZoom();
            }
        }
    }
}
