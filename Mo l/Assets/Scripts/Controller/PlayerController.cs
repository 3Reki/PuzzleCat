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
        
        private Vector2 _touchInitialPosition;
        private float _touchStartTime;
        

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
        }

        private void HandleTouchStationary()
        {
            if (GameManager.Instance.State is GameManager.GameState.PortalMode 
                or GameManager.GameState.FurnitureMovement or GameManager.GameState.CameraMovement) return;
            if (!(Time.time - _touchStartTime > holdTouchThreshold)) return;
            
            GameManager.Instance.UpdateGameState(GameManager.GameState.FurnitureMovement);
        }

        private void HandleTouchMoved()
        {
            if ((inputManager.FirstTouchPosition - _touchInitialPosition).magnitude < dragDistance) return;

            if (GameManager.Instance.State == GameManager.GameState.FurnitureMovement)
            {
                movableElementsController.HandleMovement();
            }
            else
            {
                if (GameManager.Instance.State != GameManager.GameState.CameraMovement)
                {
                    GameManager.Instance.UpdateGameState(GameManager.GameState.CameraMovement);
                }
                cameraController.HandleCameraMovement();
            }
        }

        private void HandleTouchEnd()
        {
            switch (GameManager.Instance.State)
            {
                case GameManager.GameState.PlayerMovement:
                    catController.HandlePlayerMovement();
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

#if UNITY_EDITOR
            if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop)
            {
                cameraController.HandleZoomInEditor();
            }
#endif
        }
    }
}
