using System;
using PuzzleCat.LevelElements;
using PuzzleCat.Utils;
using PuzzleCat.Visuals;
using Unity.VisualScripting;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class MovableMovementState : MonoBehaviour, IPlayerState
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private MovableElementDirectionIndicator movableElementDirectionIndicator;
        [SerializeField] private GameObject invisibleQuad;
        [SerializeField] private LayerMask selectableLayerMask;
        [SerializeField] private LayerMask invisibleLayerMask;
        
        private Func<bool> _forwardMovementFunction = () => true;
        private Func<bool> _backwardMovementFunction = () => true;
        private MovableElement _selectedMovableElement;
        private Vector3Int _currentObjectPosition;
        private Vector3Int _currentObjectDirection;

        public void Enter() // TODO refactor
        {
            Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out RaycastHit hit,
                GameManager.Instance.MainCamera, selectableLayerMask, 100f, true, 2);
            
            _selectedMovableElement = hit.transform.GetComponent<MovableElement>();
            _selectedMovableElement.PositionIndicator();
            _selectedMovableElement.Select();
            _currentObjectPosition = _selectedMovableElement.WorldGridPosition;
            invisibleQuad.SetActive(true);
            invisibleQuad.transform.position = _selectedMovableElement.WorldGridPosition;
            invisibleQuad.transform.rotation = Quaternion.LookRotation(-_selectedMovableElement.CurrentSurface.GetNormal());
            movableElementDirectionIndicator.SetAllIndicatorsActive(true);
        }

        public IPlayerState Handle()
        {
            if (inputManager.TouchCount > 1)
                return GameManager.Instance.CameraZoomState;

            if (inputManager.FirstTouchPhase == TouchPhase.Ended)
                return GameManager.Instance.DefaultState;
            
            if ((Time.frameCount + 1) % 6 == 0)
                UpdateDirectionIndicator();

            if (Time.frameCount % 3 == 0 && inputManager.FirstTouchPhase == TouchPhase.Moved)
                HandleMovement();

            return null;
        }

        public void Exit()
        {
            _selectedMovableElement.Deselect();
            _selectedMovableElement = null;
            movableElementDirectionIndicator.SetAllIndicatorsActive(false);
            invisibleQuad.SetActive(false);
        }

        private void UpdateDirectionIndicator()
        {
            if (_selectedMovableElement.IsUnderCat())
            {
                movableElementDirectionIndicator.SetIncorrectColor();
                return;
            }
            
            movableElementDirectionIndicator.SetDefaultColor();
        }


        private bool HandleMovement()
        {
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out RaycastHit hit, 
                GameManager.Instance.MainCamera, invisibleLayerMask, 100f, true, 2)) 
                return false;
            
            Vector3Int gridPoint = Utils.Utils.WorldPointAsGridPoint(hit.normal, hit.point);

            HandleJunction(gridPoint);


            if ((gridPoint - _currentObjectPosition).ApplyMask(_currentObjectDirection) >= 1)
            {
                if (!_forwardMovementFunction()) return false;
                
                _currentObjectPosition += _currentObjectDirection;
                _selectedMovableElement.PositionIndicator();
                return true;

            }
            
            if ((gridPoint - _currentObjectPosition).ApplyMask(_currentObjectDirection) <= -1)
            {
                if (!_backwardMovementFunction()) return false;
                
                _currentObjectPosition -= _currentObjectDirection;
                _selectedMovableElement.PositionIndicator();
                return true;
            }
            
            return false;
        }

        private void HandleJunction(Vector3Int gridPoint)
        {
            if ((gridPoint - _currentObjectPosition).ApplyMask(invisibleQuad.transform.up.ToVector3Int()) is >= 1 or <= -1)
            {
                _currentObjectDirection = invisibleQuad.transform.up.ToVector3Int();
                _forwardMovementFunction = () => _selectedMovableElement.MoveForward();
                _backwardMovementFunction = () => _selectedMovableElement.MoveBackward();
            }
            else if ((gridPoint - _currentObjectPosition).ApplyMask(invisibleQuad.transform.right.ToVector3Int()) is >= 1 or <= -1)
            {
                _currentObjectDirection = invisibleQuad.transform.right.ToVector3Int();
                _forwardMovementFunction = () => _selectedMovableElement.MoveRight();
                _backwardMovementFunction = () => _selectedMovableElement.MoveLeft();
            }
        }

        private void Awake()
        {
            MovableElement.DirectionIndicator = movableElementDirectionIndicator;
        }
    }
}
