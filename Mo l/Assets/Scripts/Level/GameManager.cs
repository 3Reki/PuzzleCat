using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.Level
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private GameObject invisibleQuad;
        [SerializeField] private Transform catDirectionIndicator;
        [SerializeField] private LayerMask selectableLayerMask;
        [SerializeField] private LayerMask invisibleLayerMask;
        [SerializeField] private Cat cat;
        [SerializeField] private float dragDistance = 15;
        [SerializeField] private Transform[] portalsParentTransform;
        [SerializeField] private float holdTouchThreshold = 0.3f;

        
        private static NavMeshSurface[] _surfaces;
        
        private Dictionary<int, List<Portal>> _portals;
        private SingleMovable _selectedMovableObject;
        [CanBeNull] private Tuple<int, int> _portalIndex;
        private Touch _touch;
        private Vector3Int _initialObjectPosition;
        private Vector3Int _currentObjectPosition;
        private Vector3Int _currentObjectDirection;
        private Vector3 _initialTouchPosition;
        private Vector3 _lastTouchPosition;
        private float _touchStartTime;
        private bool _touchMoved;
        private bool _holdingTouch;
        private bool _playerSelected = true;
        private bool _doRaycast;
        private bool _portalMode;
        private Func<bool> _forwardMovementFunction = () => true;
        private Func<bool> _backwardMovementFunction = () => true;
        private RaycastHit _hit;

#if UNITY_EDITOR
        public void Init(Camera cam, Cat sceneCat, float defaultDragDistance, Transform[] furniturePortals, GameObject quad, Transform catIndicator)
        {
            camera = cam;
            selectableLayerMask = 1 << LayerMask.NameToLayer("Selectable");
            invisibleLayerMask = 1 << LayerMask.NameToLayer("Invisible");
            cat = sceneCat;
            dragDistance = defaultDragDistance;
            portalsParentTransform = furniturePortals;
            invisibleQuad = quad;
            catDirectionIndicator = catIndicator;
        }
#endif

        public static void UpdateNavMeshes()
        {
            Debug.Log("update");
            foreach (NavMeshSurface navMeshSurface in _surfaces)
            {
                navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
            }
        }

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
                _playerSelected = true;
                invisibleQuad.SetActive(false);
                return;
            }

            _selectedMovableObject = selectedGameObject.GetComponent<SingleMovable>();
            _playerSelected = false;
            _initialObjectPosition = _selectedMovableObject.WorldGridPosition;
            _currentObjectPosition = _initialObjectPosition;
            invisibleQuad.SetActive(true);
            invisibleQuad.transform.position = _selectedMovableObject.WorldGridPosition;
            invisibleQuad.transform.rotation = Quaternion.LookRotation(-_selectedMovableObject.CurrentSurface.GetNormal());
            selectedGameObject.transform.DOScale(Vector3.one * 1.5f, 0.2f).onComplete =
                () => selectedGameObject.transform.DOScale(Vector3.one, 0.2f);
        }

        private void SingleTouch()
        {
            _touch = Input.GetTouch(0);
            switch (_touch.phase)
            {
                
                case TouchPhase.Began:
                    _initialTouchPosition = _touch.position;
                    _lastTouchPosition = _touch.position;
                    _touchStartTime = Time.time;
                    _touchMoved = false;
                    break;
                
                case TouchPhase.Stationary:
                    if (!_touchMoved && !_holdingTouch && Time.time - _touchStartTime > holdTouchThreshold)
                    {
                        _holdingTouch = true;
                        bool raycastResult = Utils.Utils.ScreenPointRaycast(_touch.position, out _hit, camera, selectableLayerMask, 100f, true, 2);

                        if (raycastResult)
                        {
                            SetSelectedMovableObject(_hit.transform.gameObject);
                        }
                    }
                    break;
                
                case TouchPhase.Moved:
                    _lastTouchPosition = _touch.position;
                    
                    if ((_lastTouchPosition - _initialTouchPosition).magnitude > dragDistance)
                    {
                        _touchMoved = true;
                        if (!_playerSelected)
                        {
                            HandleSwipe();
                        } 
                    }
                    
                    break;

                case TouchPhase.Ended:
                    _holdingTouch = false;
                    
                    if (_playerSelected)
                    {
                        _lastTouchPosition = _touch.position;
                        _doRaycast = true;
                    }
                    else
                    {
                        SetSelectedMovableObject(null);
                    }
                    break;
            }
        }

        private void SingleTouchRaycast()
        {
            if (!Utils.Utils.ScreenPointRaycast(_lastTouchPosition, out _hit, camera, -5, 100f, true, 2)) 
                return;
            
            Vector3Int gridPoint = Utils.Utils.WorldPointAsGridPoint(_hit.normal, _hit.point);
            
            if (_portalMode)
            {
                var portal = _hit.collider.GetComponent<Portal>();

                if (portal != null)
                {
                    portal.UnsetPortal();
                    return;
                }
                    
                // ReSharper disable once PossibleNullReferenceException : _portalIndex is not null if _portalMode is true
                _portals[_portalIndex.Item1][_portalIndex.Item2].SetPortal(_hit.transform.parent.GetComponent<Room>(), gridPoint, _hit.normal.ToSurface());
                _portalMode = false;
                cat.SetIdle(false);
                    
                return;
            }

            if (_playerSelected && _hit.normal == cat.transform.up)
            {
                cat.TryMovingTo(gridPoint);
                
                catDirectionIndicator.position = _hit.point;
                catDirectionIndicator.gameObject.SetActive(true);
                StartCoroutine(DisableIndicator());
            }
        }

        private IEnumerator DisableIndicator()
        {
            yield return new WaitForSeconds(0.2f);
            catDirectionIndicator.gameObject.SetActive(false);
        }

        

        private void HandleSwipe()
        {
            if (!Utils.Utils.ScreenPointRaycast(_lastTouchPosition, out _hit, camera, invisibleLayerMask, 100f, true, 2))
                return;
            
            Vector3Int gridPoint = Utils.Utils.WorldPointAsGridPoint(_hit.normal, _hit.point);

            if (_initialObjectPosition == _currentObjectPosition)
            {
                if ((gridPoint - _currentObjectPosition).ApplyMask(invisibleQuad.transform.up.ToVector3Int()) is >= 1 or <= -1)
                {
                    _currentObjectDirection = invisibleQuad.transform.up.ToVector3Int();
                    _forwardMovementFunction = () => _selectedMovableObject.MoveForward(cat);
                    _backwardMovementFunction = () => _selectedMovableObject.MoveBackward(cat);
                }
                else if ((gridPoint - _currentObjectPosition).ApplyMask(invisibleQuad.transform.right.ToVector3Int()) is >= 1 or <= -1)
                {
                    _currentObjectDirection = invisibleQuad.transform.right.ToVector3Int();
                    _forwardMovementFunction = () => _selectedMovableObject.MoveRight(cat);
                    _backwardMovementFunction = () => _selectedMovableObject.MoveLeft(cat);
                }
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
                _lastTouchPosition = Input.mousePosition;
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
            _surfaces = FindObjectsOfType<NavMeshSurface>();
            SingleMovable.onMovement += UpdateNavMeshes;

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
            //DebugInputs();
#endif

            if (_doRaycast)
            {
                SingleTouchRaycast();
                _doRaycast = false;
            }
        }
    }
}