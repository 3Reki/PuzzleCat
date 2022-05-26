using UnityEngine;

namespace PuzzleCat.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] protected InputManager inputManager;
        [SerializeField] protected CameraController cameraController;
        [SerializeField] protected MovableElementsController movableElementsController;
        [SerializeField] protected CatController catController;
        [SerializeField] protected PortalPlacementController portalPlacementController;
        [SerializeField] protected FurnitureSelectionIndicator furnitureSelectionIndicator;
        [SerializeField] protected float holdTouchThreshold = 0.3f;
        [SerializeField] protected float dragDistance = 3;

        protected Vector2 TouchInitialPosition;
        protected float TouchStartTime;
        protected bool TouchMoved;
        

        protected void HandleSingleTouch()
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
            TouchInitialPosition = inputManager.FirstTouchPosition;
            TouchStartTime = Time.time;
            TouchMoved = false;
            
            if (movableElementsController.CanEnterFurnitureMode())
            {
                furnitureSelectionIndicator.Play(TouchInitialPosition + new Vector2(-0.12f, 0.12f) * Screen.width, holdTouchThreshold);
            }
        }

        protected virtual void HandleTouchStationary()
        {
            if (GameManager.Instance.State is GameManager.GameState.PortalMode 
                or GameManager.GameState.FurnitureMovement or GameManager.GameState.CameraMovement) return;
            if (!(Time.time - TouchStartTime > holdTouchThreshold)) return;
            
            GameManager.Instance.UpdateGameState(GameManager.GameState.FurnitureMovement);
        }

        protected virtual void HandleTouchMoved()
        {
            if (!TouchMoved)
            {
                if ((inputManager.FirstTouchPosition - TouchInitialPosition).magnitude < dragDistance) return;
                TouchMoved = true;
                furnitureSelectionIndicator.Stop();
            }

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

        protected virtual void HandleTouchEnd()
        {
            switch (GameManager.Instance.State)
            {
                case GameManager.GameState.PlayerMovement:
                    catController.HandlePlayerMovement();
                    furnitureSelectionIndicator.Stop();
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
            if (GameManager.Instance.State is GameManager.GameState.Menu or GameManager.GameState.End) 
                return;

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
