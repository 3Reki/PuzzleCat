using System;
using DG.Tweening;
using PuzzleCat.LevelElements;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class MovableElementsController : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private GameObject invisibleQuad;
        [SerializeField] private LayerMask selectableLayerMask;
        [SerializeField] private LayerMask invisibleLayerMask;
        
        private Func<bool> _forwardMovementFunction = () => true;
        private Func<bool> _backwardMovementFunction = () => true;
        private MovableElement _selectedMovableElement;
        private Vector3Int _initialObjectPosition;
        private Vector3Int _currentObjectPosition;
        private Vector3Int _currentObjectDirection;
        private RaycastHit _hit;
        
        public void HandleMovement()
        {
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out _hit, 
                GameManager.Instance.MainCamera, invisibleLayerMask, 100f, true, 2)) 
                return;
            
            Vector3Int gridPoint = Utils.Utils.WorldPointAsGridPoint(_hit.normal, _hit.point);

            if (_initialObjectPosition == _currentObjectPosition)
            {
                HandleJunction(gridPoint);
            }


            if ((gridPoint - _currentObjectPosition).ApplyMask(_currentObjectDirection) >= 1)
            {
                if (_forwardMovementFunction())
                {
                    _currentObjectPosition += _currentObjectDirection;
                }
                
            }
            else if ((gridPoint - _currentObjectPosition).ApplyMask(_currentObjectDirection) <= -1)
            {
                if (_backwardMovementFunction())
                {
                    _currentObjectPosition -= _currentObjectDirection;
                }
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
                }
                else
                {
                    GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerMovement);
                }
            }
            else
            {
                SetSelectedMovableObject(null);
            }
        }
        
        private void SetSelectedMovableObject(GameObject selectedGameObject)
        {
            if (selectedGameObject == null)
            {
                _selectedMovableElement = null;
                invisibleQuad.SetActive(false);
                return;
            }

            _selectedMovableElement = selectedGameObject.GetComponent<MovableElement>();
            _initialObjectPosition = _selectedMovableElement.WorldGridPosition;
            _currentObjectPosition = _initialObjectPosition;
            invisibleQuad.SetActive(true);
            invisibleQuad.transform.position = _selectedMovableElement.WorldGridPosition;
            invisibleQuad.transform.rotation = Quaternion.LookRotation(-_selectedMovableElement.CurrentSurface.GetNormal());
            selectedGameObject.transform.DOScale(Vector3.one * 1.5f, 0.2f).onComplete =
                () => selectedGameObject.transform.DOScale(Vector3.one, 0.2f);
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
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }
        
        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }
    }
}