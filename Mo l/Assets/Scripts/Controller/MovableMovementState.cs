using System;
using PuzzleCat.LevelElements;
using PuzzleCat.Utils;
using PuzzleCat.Visuals;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class MovableMovementState : MonoBehaviour, IPlayerState
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private LayerMask selectableLayerMask;
        [SerializeField] private LayerMask invisibleLayerMask;
        
        private MovableElementDirectionIndicator _movableElementDirectionIndicator;
        private GameObject _invisibleQuad;
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
            _invisibleQuad.SetActive(true);
            _invisibleQuad.transform.position = _selectedMovableElement.WorldGridPosition;
            _invisibleQuad.transform.rotation = Quaternion.LookRotation(-_selectedMovableElement.CurrentSurface.GetNormal());
            _movableElementDirectionIndicator.SetAllIndicatorsActive(true);
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
            _movableElementDirectionIndicator.SetAllIndicatorsActive(false);
            _invisibleQuad.SetActive(false);
        }

        private void UpdateDirectionIndicator()
        {
            if (_selectedMovableElement.IsUnderCat())
            {
                _movableElementDirectionIndicator.SetIncorrectColor();
                return;
            }
            
            _movableElementDirectionIndicator.SetDefaultColor();
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
            if ((gridPoint - _currentObjectPosition).ApplyMask(_invisibleQuad.transform.up.ToVector3Int()) is >= 1 or <= -1)
            {
                _currentObjectDirection = _invisibleQuad.transform.up.ToVector3Int();
                _forwardMovementFunction = () => _selectedMovableElement.MoveForward();
                _backwardMovementFunction = () => _selectedMovableElement.MoveBackward();
            }
            else if ((gridPoint - _currentObjectPosition).ApplyMask(_invisibleQuad.transform.right.ToVector3Int()) is >= 1 or <= -1)
            {
                _currentObjectDirection = _invisibleQuad.transform.right.ToVector3Int();
                _forwardMovementFunction = () => _selectedMovableElement.MoveRight();
                _backwardMovementFunction = () => _selectedMovableElement.MoveLeft();
            }
        }

        private void Awake()
        {
            _movableElementDirectionIndicator = FindObjectOfType<MovableElementDirectionIndicator>();
            MovableElement.DirectionIndicator = _movableElementDirectionIndicator;
            _invisibleQuad = Utils.Utils.FindGameObjectWithLayer(LayerMask.NameToLayer("Invisible"));
            print(_invisibleQuad);
        }
    }
}
