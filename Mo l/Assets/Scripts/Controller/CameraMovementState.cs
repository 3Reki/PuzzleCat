using DG.Tweening;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class CameraMovementState : MonoBehaviour, IPlayerState
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private float movementSpeed = 4;
        
        private Transform _cameraTransform;
        private Vector3 _centerPosition;
        private float _initialSize;
        private bool _inLerp;
        
        public void Enter()
        {
        }

        public IPlayerState Handle()
        {
            if (inputManager.TouchCount > 1)
                return GameManager.Instance.CameraZoomState;

            if (inputManager.FirstTouchPhase == TouchPhase.Ended)
                return GameManager.Instance.DefaultState;
            
            if (inputManager.FirstTouchPhase == TouchPhase.Moved)
                HandleCameraMovement();
            
            return null;
        }

        public void Exit()
        {
        }
        
        private void HandleCameraMovement()
        {
            if (_inLerp)
            {
                return;
            }
                
            if (inputManager.FirstTouchDeltaPosition == Vector2.zero) 
                return;

            _cameraTransform.position -=
                _cameraTransform.TransformDirection(inputManager.FirstTouchDeltaPosition) / Screen.dpi * 
                (GameManager.Instance.MainCamera.orthographicSize * .1f * movementSpeed);

            if ((_cameraTransform.position - _centerPosition).magnitude > _initialSize * 1.15f)
            {
                _inLerp = true;
                _cameraTransform.DOMove(_centerPosition, _initialSize / 10).onComplete = () => _inLerp = false;
            }
        }
        
        private void Start()
        {
            _cameraTransform = GameManager.Instance.MainCamera.transform;
            _initialSize = GameManager.Instance.MainCamera.orthographicSize;
            _centerPosition = _cameraTransform.position;
        }
    }
}
