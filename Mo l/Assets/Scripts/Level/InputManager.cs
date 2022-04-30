using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.Level
{
    public class InputManager : MonoBehaviour
    {
        public static NavMeshSurface[] Surfaces;
        
        [SerializeField] private new Camera camera;
        [SerializeField] private LayerMask selectableLayerMask;
        [SerializeField] private Cat cat;
        [SerializeField] private float dragDistance = 15;
        [SerializeField] private Transform[] portalsParentTransform;

        
        private Dictionary<int, List<Portal>> _portals;
        private SingleMovable _selectedMovableObject;
        [CanBeNull] private Tuple<int, int> _portalIndex;
        private Vector3 _initialTouchPosition;
        private Vector3 _lastTouchPosition;
        private Vector3 _position;
        private bool _playerSelected;
        private bool _doRaycast;
        private bool _portalMode;

#if UNITY_EDITOR
        public void Init(Camera cam, LayerMask layerMask, Cat sceneCat, float defaultDragDistance, Transform[] furniturePortals)
        {
            camera = cam;
            selectableLayerMask = layerMask;
            cat = sceneCat;
            dragDistance = defaultDragDistance;
            portalsParentTransform = furniturePortals;
        }
#endif

        public void SwitchPortalMode(int id)
        {
            if (!_portalMode)
            {
                _portalMode = true;
                _portalIndex = FindCurrentPortalIndex(id);

                if (_portalIndex == null)
                {
                    _portalMode = false;
                }
            }
            else
            {
                _portalMode = false;
            }
           
            cat.SetIdle(_portalMode);
        }
        
        private Tuple<int, int> FindCurrentPortalIndex(int portalId)
        {
            for (var i = 0; i < _portals[portalId].Count; i++)
            {
                if (!_portals[portalId][i].Placed)
                {
                    return new Tuple<int, int>(portalId, i);
                }
            }

            return null;
        }

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

            if (Cat.IsCat(selectedGameObject))
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
                    }

                    break;
                
            }
        }

        private void SingleTouchRaycast()
        {
            bool raycastResult = Utils.Utils.ScreenPointRaycast(_position, out RaycastHit hit, camera, -5, 100f, true, 2);

            if (raycastResult)
            {
                Vector3Int gridPoint = Utils.Utils.WorldPointAsGridPoint(hit);
                if (_portalMode)
                {
                    // ReSharper disable once PossibleNullReferenceException : _portalIndex is not null if _portalMode is true
                    _portals[_portalIndex.Item1][_portalIndex.Item2].SetPortal(hit.transform.parent.GetComponent<Room>(), gridPoint, hit.normal.ToSurface());
                    _portalMode = false;
                    return;
                }

                if (_playerSelected && hit.normal == cat.transform.up)
                {
                    cat.TryMovingTo(gridPoint);
                    return;
                }
                
                Portal portal = hit.collider.GetComponent<Portal>();

                if (portal != null)
                {
                    portal.UnsetPortal();
                    return;
                }
                
                GameObject hitGameObject = hit.transform.gameObject;
                if (!Utils.Utils.IsInLayerMask(hitGameObject, selectableLayerMask)) return;
                
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
                {
                    //Right swipe
                    _selectedMovableObject.MoveRight(cat);
                }
                else
                {
                    //Left swipe
                    _selectedMovableObject.MoveLeft(cat);
                }
            }
            else
            {
                if (_lastTouchPosition.y > _initialTouchPosition.y)
                {
                    //Up swipe
                    _selectedMovableObject.MoveForward(cat);
                }
                else
                {
                    //Down swipe
                    _selectedMovableObject.MoveBackward(cat);
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
                    _selectedMovableObject.MoveLeft(cat);
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    _selectedMovableObject.MoveRight(cat);
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    _selectedMovableObject.MoveForward(cat);
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _selectedMovableObject.MoveBackward(cat);
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

        private void ConstructPortalsDictionary()
        {
            _portals = new Dictionary<int, List<Portal>>();
            
            foreach (Transform parentTransform in portalsParentTransform)
            {
                for (int i = 0; i < parentTransform.childCount; i++)
                {
                    Portal portal = parentTransform.GetChild(i).GetComponent<Portal>();
                    
                    if (!_portals.ContainsKey(portal.Id))
                    {
                        _portals.Add(portal.Id, new List<Portal>());
                    }
                    
                    _portals[portal.Id].Add(portal);
                }
            }
        }

        private void Awake()
        {
            Surfaces = FindObjectsOfType<NavMeshSurface>();

            ConstructPortalsDictionary();
        }

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