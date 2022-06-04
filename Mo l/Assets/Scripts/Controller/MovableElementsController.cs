using System;
using PuzzleCat.LevelElements;
using PuzzleCat.Utils;
using PuzzleCat.Visuals;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class MovableElementsController : MonoBehaviour
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
        private RaycastHit _hit;
        
        public bool CanEnterFurnitureMode()
        {
            return Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out _hit,
                GameManager.Instance.MainCamera, selectableLayerMask, 100f, true, 2)
                && !_hit.transform.GetComponent<MovableElement>().InPortal;
        }
        
        public bool HandleMovement()
        {
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out _hit, 
                GameManager.Instance.MainCamera, invisibleLayerMask, 100f, true, 2)) 
                return false;
            
            Vector3Int gridPoint = Utils.Utils.WorldPointAsGridPoint(_hit.normal, _hit.point);

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

        private void OnGameStateChanged(GameManager.GameState state)
        {
            if (state == GameManager.GameState.FurnitureMovement)
            {
                if (Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out _hit, 
                    GameManager.Instance.MainCamera, selectableLayerMask, 100f, true, 2))
                {
                    SetSelectedMovableObject(_hit.transform.gameObject);
                    _selectedMovableElement.Select();
                }
                else
                {
                    GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerMovement);
                }
            }
            else
            {
                if (_selectedMovableElement != null)
                {
                    _selectedMovableElement.Deselect();
                }
                SetSelectedMovableObject(null);
            }
        }
        
        private void SetSelectedMovableObject(GameObject selectedGameObject)
        {
            if (selectedGameObject == null)
            {
                if (_selectedMovableElement == null) return;
                
                movableElementDirectionIndicator.SetAllIndicatorsActive(false);
                _selectedMovableElement = null;
                invisibleQuad.SetActive(false);

                return;
            }

            _selectedMovableElement = selectedGameObject.GetComponent<MovableElement>();
            _currentObjectPosition = _selectedMovableElement.WorldGridPosition;
            invisibleQuad.SetActive(true);
            invisibleQuad.transform.position = _selectedMovableElement.WorldGridPosition;
            invisibleQuad.transform.rotation = Quaternion.LookRotation(-_selectedMovableElement.CurrentSurface.GetNormal());
            _selectedMovableElement.PositionIndicator();
            movableElementDirectionIndicator.SetAllIndicatorsActive(true);
        }

        private void Awake()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;
            MovableElement.DirectionIndicator = movableElementDirectionIndicator;
        }

        private void Update()
        {
            if (Time.frameCount % 8 != 0)
                return;

            if (_selectedMovableElement == null)
                return;
            
            if (_selectedMovableElement.IsUnderCat())
            {
                movableElementDirectionIndicator.SetIncorrectColor();
                return;
            }
            
            movableElementDirectionIndicator.SetDefaultColor();
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }
    }
}
