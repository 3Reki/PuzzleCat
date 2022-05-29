using DG.Tweening;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private new Camera camera;
        [Range(100, 200)]
        [SerializeField] private float maxZoomPercentage = 100;
        [Range(0, 100)]
        [SerializeField] private float minZoomPercentage = 50;
        [SerializeField] private float zoomSpeed = 5;
        [SerializeField] private float movementSpeed = 5;

        private Vector2 _lastTouchPosition;
        private float _maxZoom;
        private float _minZoom;
        private float _previousTouchesDistance;
        private bool _zoomInitialized;
        private Vector3 _centerPosition;
        private float _initialSize;
        private bool _inLerp;
        
        public void HandleZoom()
        {
            if (!_zoomInitialized)
            {
                _zoomInitialized = true;
                _previousTouchesDistance = (inputManager.SecondTouchPosition - inputManager.FirstTouchPosition).magnitude;
                return;
            }

            if (inputManager.CameraTouchesFinished)
            {
                _zoomInitialized = false;
            } 
            else if (inputManager.CameraTouchesMoved)
            {
                float diff = (inputManager.SecondTouchPosition - inputManager.FirstTouchPosition).magnitude - _previousTouchesDistance;

                camera.orthographicSize =
                    Mathf.Clamp(camera.orthographicSize - diff / Screen.dpi * zoomSpeed, _minZoom, _maxZoom);
                _previousTouchesDistance =
                    (inputManager.SecondTouchPosition - inputManager.FirstTouchPosition).magnitude;
            }
        }
        
#if UNITY_EDITOR
        public void HandleZoomInEditor()
        {
            if (inputManager.IsScrolling)
            {
                camera.orthographicSize =
                    Mathf.Clamp(camera.orthographicSize - inputManager.MouseScroll * zoomSpeed * 0.5f, _minZoom, _maxZoom);
            }
        }
#endif

        public void HandleCameraMovement()
        {
            if (_inLerp)
            {
                _lastTouchPosition = inputManager.FirstTouchPosition;
                return;
            }
                
            if (inputManager.FirstTouchPosition == _lastTouchPosition) 
                return;
            
            cameraTransform.position -=
                cameraTransform.TransformDirection(inputManager.FirstTouchPosition - _lastTouchPosition) / Screen.dpi * movementSpeed;
            _lastTouchPosition = inputManager.FirstTouchPosition;

            if ((cameraTransform.position - _centerPosition).magnitude > _initialSize * 1.15f)
            {
                _inLerp = true;
                cameraTransform.DOMove(_centerPosition, _initialSize / 10).onComplete = () => _inLerp = false;
            }
        }
        
        private void OnGameStateChanged(GameManager.GameState state)
        {
            if (state == GameManager.GameState.CameraMovement)
            {
                _lastTouchPosition = inputManager.FirstTouchPosition;
            }
        }
        
        private void Awake()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;
            _initialSize = camera.orthographicSize;
            _maxZoom = _initialSize * maxZoomPercentage * 0.01f;
            _minZoom = _initialSize * minZoomPercentage * 0.01f;
            _centerPosition = cameraTransform.position;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }
    }
}
