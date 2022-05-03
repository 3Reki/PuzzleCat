using UnityEngine;

namespace PuzzleCat.Controller
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private new Camera camera;
        [Range(100, 200)]
        [SerializeField] private float maxZoomPercentage = 100;
        [Range(0, 100)]
        [SerializeField] private float minZoomPercentage = 50;
        [SerializeField] private float zoomSpeed = 5;
        
        private float _maxZoom;
        private float _minZoom;
        private float _previousTouchesDistance;
        private bool _zoomInitialized;
        
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
                    Mathf.Clamp(camera.orthographicSize - diff * zoomSpeed * 0.0001f, _minZoom, _maxZoom);
                _previousTouchesDistance =
                    (inputManager.SecondTouchPosition - inputManager.FirstTouchPosition).magnitude;
            }
        }

        private void Start()
        {
            var orthographicSize = camera.orthographicSize;
            _maxZoom = orthographicSize * maxZoomPercentage * 0.01f;
            _minZoom = orthographicSize * minZoomPercentage * 0.01f;
        }
    }
}
