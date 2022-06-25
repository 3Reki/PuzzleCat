using System;
using PuzzleCat.Visuals;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class MovableSelectionState : MonoBehaviour, IPlayerState
    {
        [SerializeField] private InputManager inputManager;
        private FurnitureSelectionIndicator _furnitureSelectionIndicator;
        
        private const float _durationBeforeSelection = 0.2f;
        private Vector2 _touchInitialPosition;
        private float _minimumDragDistance;
        private float _touchStartTime;
        
        public void Enter()
        {
            _touchInitialPosition = inputManager.FirstTouchPosition;
            _furnitureSelectionIndicator.Play(_touchInitialPosition, _durationBeforeSelection);
            _touchStartTime = Time.time;
        }

        public IPlayerState Handle()
        {
            if (inputManager.TouchCount > 1)
                return GameManager.Instance.CameraZoomState;
            
            switch (inputManager.FirstTouchPhase)
            {
                case TouchPhase.Moved:
                    if ((inputManager.FirstTouchPosition - _touchInitialPosition).sqrMagnitude < _minimumDragDistance)
                        goto case TouchPhase.Stationary;
                    
                    return GameManager.Instance.CameraMovementState;
                
                case TouchPhase.Stationary:
                    return Time.time - _touchStartTime > _durationBeforeSelection ? GameManager.Instance.MovableMovementState : null;

                case TouchPhase.Ended:
                    return GameManager.Instance.CatMovementState;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Exit()
        {
            _furnitureSelectionIndicator.Stop();
        }

        private void Awake()
        {
            _furnitureSelectionIndicator = FindObjectOfType<FurnitureSelectionIndicator>();
        }

        private void Start()
        {
            _minimumDragDistance = Screen.height * 0.02f;
            _minimumDragDistance *= _minimumDragDistance;
        }
    }
}
