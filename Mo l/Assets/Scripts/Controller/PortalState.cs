using System.Collections.Generic;
using PuzzleCat.LevelElements;
using PuzzleCat.Menus;
using PuzzleCat.Sound;
using PuzzleCat.Utils;
using TMPro;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class PortalState : MonoBehaviour, IPlayerState
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private TextMeshProUGUI[] portalCountTexts;
        [SerializeField] private Transform[] portalsParentTransform;
        
        private MenuManager _menuManager;
        private Dictionary<int, List<Portal>> _portals;
        private int[] _portalCounts;
        private int _portalGroupId = -1;
        private int _portalId = -1;
        
        public void Enter()
        {
            _menuManager.OpenPortalBook();
        }

        public IPlayerState Handle()
        {
            if (!inputManager.PortalMode)
            {
                _menuManager.ClosePortalBook();
                ResetSelectedGroup();
                return GameManager.Instance.DefaultState;
            }
                
            
            if (inputManager.TouchCount > 1)
                return GameManager.Instance.CameraZoomState;

            if (inputManager.TouchCount == 0)
                return null;

            if (inputManager.FirstTouchPhase == TouchPhase.Moved)
                return GameManager.Instance.CameraMovementState;

            if (inputManager.FirstTouchPhase == TouchPhase.Ended)
                HandlePortalPlacement();

            return null;
        }

        public void Exit()
        {
        }
        
        public void UpdateSelectedPortalGroup(int id)
        {
            if (_portalGroupId == id) // TODO : change toggles or check if false
            {
                ResetSelectedGroup();
                return;
            }
            
            _portalGroupId = id;
            _portalId = FindCurrentPortalIndex(id);
        }

        private bool HandlePortalPlacement()
        {
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out RaycastHit hit,
                GameManager.Instance.MainCamera, -5, 100f, true, 2))
            {
                return false;
            }
                
            var portal = hit.collider.GetComponent<Portal>();

            if (portal != null)
            {
                int unsetGreyPortals = portal.UnsetPortal() - 1;
                _portalCounts[0] += unsetGreyPortals;
                portalCountTexts[0].text = $"x{_portalCounts[0]}";
                _portalCounts[portal.Id - 1]++;
                AudioManager.Instance.Play("PortalRemoved");
                portalCountTexts[portal.Id - 1].text = $"x{_portalCounts[portal.Id - 1]}";
                
                if (_portalGroupId == -1)
                {
                    return false;
                }
                
                _portalId = FindCurrentPortalIndex(_portalGroupId);
                return false;
            }

            if (_portalGroupId == -1)
            {
                return false;
            }

            if (_portalId == -1)
            {
                Debug.LogWarning("Attempt to place portal when none is available");
                AudioManager.Instance.Play("InvalidPortal");
                return false;
            }

            Vector3Int gridPoint = Utils.Utils.WorldPointAsGridPoint(hit.normal, hit.point);
            
            if (!_portals[_portalGroupId][_portalId].CanSetPortal(hit.transform, gridPoint, hit.normal.ToSurface()))
            {
                AudioManager.Instance.Play("InvalidPortal");
                return false;
            }

            _portals[_portalGroupId][_portalId].SetPortal(hit.transform.parent.GetComponent<Room>(), 
                gridPoint, hit.normal.ToSurface());
            _portalId = FindCurrentPortalIndex(_portalGroupId);
            AudioManager.Instance.Play("PortalPlaced");
            _portalCounts[_portalGroupId - 1]--;
            portalCountTexts[_portalGroupId - 1].text = $"x{_portalCounts[_portalGroupId - 1]}";
            return true;
        }
        
        private int FindCurrentPortalIndex(int portalGroupId)
        {
            for (var i = 0; i < _portals[portalGroupId].Count; i++)
            {
                if (!_portals[portalGroupId][i].Placed)
                {
                    return i;
                }
            }

            return -1;
        }
        
        private void ResetSelectedGroup()
        {
            _portalGroupId = -1;
            _portalId = -1;
        }
        
        private void ConstructPortalsDictionary()
        {
            _portals = new Dictionary<int, List<Portal>>();
            _portalCounts = new int[4];
            
            foreach (Transform parentTransform in portalsParentTransform)
            {
                for (int i = 0; i < parentTransform.childCount; i++)
                {
                    var portal = parentTransform.GetChild(i).GetComponent<Portal>();
                    
                    if (!_portals.ContainsKey(portal.Id))
                    {
                        _portals.Add(portal.Id, new List<Portal>());
                    }
                    
                    _portals[portal.Id].Add(portal);
                    _portalCounts[portal.Id - 1]++;
                    portalCountTexts[portal.Id - 1].text = $"x{_portalCounts[portal.Id - 1]}";
                }
            }
        }

        private void Awake()
        {
            _menuManager = FindObjectOfType<MenuManager>();
            ConstructPortalsDictionary();
        }
    }
}
