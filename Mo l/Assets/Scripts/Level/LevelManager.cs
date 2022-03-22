using System;
using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private LayerMask selectableLayerMask;
        [SerializeField] private Cat cat;
        [SerializeField] private float dragDistance = 15;

        private SingleMovable _selectedMovableObject;
        private bool _playerSelected;
        private Vector3 _initialTouchPosition;
        private Vector3 _lastTouchPosition;
        private bool _doRaycast;
        private Vector3 _position;

        private void SetSelectedMovableObject(GameObject selectedGameObject)
        {
            if (selectedGameObject == null)
            {
                _selectedMovableObject = null;
                _playerSelected = false;
                return;
            }

            if (_playerSelected || _selectedMovableObject != null)
            {
                return;
            }

            if (cat.IsCat(selectedGameObject))
            {
                _playerSelected = true;
                return;
            }

            _selectedMovableObject = selectedGameObject.GetComponent<SingleMovable>();
        }

        private void SingleTouch()
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                
                case TouchPhase.Began:
                    _initialTouchPosition = touch.position;
                    _lastTouchPosition = touch.position;
                    break;
                
                
                case TouchPhase.Ended:
                    _lastTouchPosition = touch.position;

                    if (Mathf.Abs(_lastTouchPosition.x - _initialTouchPosition.x) > dragDistance ||
                        Mathf.Abs(_lastTouchPosition.y - _initialTouchPosition.y) > dragDistance)
                    {
                        if (_selectedMovableObject != null)
                        {
                            HandleSwipe();
                        }
                        
                    }
                    else
                    {
                        _position = _lastTouchPosition;
                        _doRaycast = true;
                        Debug.Log("Tap");
                    }

                    break;
                
            }
        }

        private void SingleTouchRaycast()
        {
            bool raycastResult = UtilsClass.ScreenPointRaycast(_position, out RaycastHit hit, camera);

            if (raycastResult)
            {
                if (_playerSelected && hit.normal == cat.transform.up)
                {
                    cat.TryMovingTo(hit.point);
                    return;
                }
                
                GameObject hitGameObject = hit.transform.gameObject;
                if (!UtilsClass.IsInLayerMask(hitGameObject, selectableLayerMask)) return;
                
                SetSelectedMovableObject(hitGameObject);
            }
            else
            {
                SetSelectedMovableObject(null);
            }
        }

        private void HandleSwipe()
        {
            if (Mathf.Abs(_lastTouchPosition.x - _initialTouchPosition.x) > Mathf.Abs(_lastTouchPosition.y - _initialTouchPosition.y))
            {
                if (_lastTouchPosition.x > _initialTouchPosition.x)
                {   //Right swipe
                    _selectedMovableObject.MoveRight();
                    Debug.Log("Right Swipe");
                }
                else
                {   //Left swipe
                    _selectedMovableObject.MoveLeft();
                    Debug.Log("Left Swipe");
                }
            }
            else
            {
                if (_lastTouchPosition.y > _initialTouchPosition.y)
                {   //Up swipe
                    _selectedMovableObject.MoveForward();
                    Debug.Log("Up Swipe");
                }
                else
                {   //Down swipe
                    _selectedMovableObject.MoveBackward();
                    Debug.Log("Down Swipe");
                }
            }
        }

#if UNITY_EDITOR
        private void DebugInputs()
        {
            if (_selectedMovableObject != null)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    _selectedMovableObject.MoveLeft();
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    _selectedMovableObject.MoveRight();
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    _selectedMovableObject.MoveForward();
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _selectedMovableObject.MoveBackward();
                }
            }

            if (UnityEngine.Device.SystemInfo.deviceType != DeviceType.Desktop)
            {
                // game view is in simulator mode (touch is already handled)
                return;
            }
                
            
            if (Input.GetMouseButtonUp(0))
            {
                _position = Input.mousePosition;
                _doRaycast = true;
            }
        }
#endif

        private void Start()
        {
            dragDistance = Screen.height * dragDistance * 0.01f;
        }

        private void Update()
        {
            if (Input.touchCount == 1)
            {
                SingleTouch();
            }
            
#if UNITY_EDITOR
            DebugInputs();
#endif

            if (_doRaycast)
            {
                SingleTouchRaycast();
                _doRaycast = false;
            }
        }
    }
}