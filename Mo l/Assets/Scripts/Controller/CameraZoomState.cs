using UnityEngine;

namespace PuzzleCat.Controller
{
    public class CameraZoomState : MonoBehaviour, IPlayerState
    {
        [SerializeField] private InputManager inputManager;
        [Range(100, 200)]
        [SerializeField] private float maxZoomPercentage = 120;
        [Range(0, 100)]
        [SerializeField] private float minZoomPercentage = 50;
        [SerializeField] private float zoomSpeed = 5;
        
        private float _maxZoom;
        private float _minZoom;
        private float _previousTouchesDistance;
        
        public void Enter()
        {
            _previousTouchesDistance = (inputManager.SecondTouchPosition - inputManager.FirstTouchPosition).magnitude;
        }

        public IPlayerState Handle()
        {
            if (inputManager.TouchCount < 2)
            {
                if (inputManager.TouchCount == 0)
                    return GameManager.Instance.DefaultState;

                return null;
            }

            if (inputManager.CameraTouchesMoved)
                HandleZoom();

            return null;
        }

        public void Exit()
        {
        }
        
        private void HandleZoom()
        {
            float currentTouchesDistance =
                (inputManager.SecondTouchPosition - inputManager.FirstTouchPosition).magnitude;
            float diff = currentTouchesDistance - _previousTouchesDistance;

            GameManager.Instance.MainCamera.orthographicSize =
                Mathf.Clamp(GameManager.Instance.MainCamera.orthographicSize - diff / Screen.dpi * zoomSpeed, _minZoom, _maxZoom);
            _previousTouchesDistance = currentTouchesDistance;
        }
        
        private void Start()
        {
            var orthographicSize = GameManager.Instance.MainCamera.orthographicSize;
            _maxZoom = orthographicSize * maxZoomPercentage * 0.01f;
            _minZoom = orthographicSize * minZoomPercentage * 0.01f;
        }
    }
}
