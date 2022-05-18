using System.Collections.Generic;
using PuzzleCat.LevelElements;
using PuzzleCat.Utils;
using TMPro;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class PortalPlacementController : MonoBehaviour
    {
        public TextMeshProUGUI[] PortalCountTexts;
        
        [SerializeField] private InputManager inputManager;
        [SerializeField] private Transform[] portalsParentTransform;

        private Dictionary<int, List<Portal>> _portals;
        private int[] _portalCounts;
        private int _portalGroupId;
        private int _portalId;

        public void UpdateSelectedPortalGroup(int id)
        {
            if (_portalGroupId == id)
            {
                _portalGroupId = -1;
                _portalId = -1;
                return;
            }
            
            _portalGroupId = id;
            _portalId = FindCurrentPortalIndex(id);
        }
        
        public void HandlePortalPlacement()
        {
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out RaycastHit hit,
                GameManager.Instance.MainCamera, -5, 100f, true, 2))
            {
                return;
            }
                
            var portal = hit.collider.GetComponent<Portal>();

            if (portal != null)
            {
                portal.UnsetPortal();
                _portalCounts[portal.Id - 1]++;
                PortalCountTexts[portal.Id - 1].text = $"x{_portalCounts[portal.Id - 1]}";
                
                if (_portalGroupId == -1)
                {
                    return;
                }
                
                _portalId = FindCurrentPortalIndex(_portalGroupId);
            }

            if (_portalGroupId == -1)
            {
                return;
            }

            if (_portalId == -1)
            {
                Debug.LogWarning("Attempt to place portal when none is available");
                return;
            }

            Vector3Int gridPoint = Utils.Utils.WorldPointAsGridPoint(hit.normal, hit.point);
            
            if (_portals[_portalGroupId][_portalId].CanSetPortal(hit.transform, gridPoint, hit.normal.ToSurface()))
            {
                _portals[_portalGroupId][_portalId].SetPortal(hit.transform.parent.GetComponent<Room>(), 
                    gridPoint, hit.normal.ToSurface());
                _portalId = FindCurrentPortalIndex(_portalGroupId);
                _portalCounts[_portalGroupId - 1]--;
                PortalCountTexts[_portalGroupId - 1].text = $"x{_portalCounts[_portalGroupId - 1]}";
            }
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
                    PortalCountTexts[portal.Id - 1].text = $"x{_portalCounts[portal.Id - 1]}";
                }
            }
        }
        
        private void OnGameStateChanged(GameManager.GameState state)
        {
            if (state == GameManager.GameState.PortalMode)
            {
                _portalGroupId = -1;
                _portalId = -1;
            }
        }

        private void Awake()
        {
            ConstructPortalsDictionary();
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }
        
        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }

        private void Start()
        {
            if (portalsParentTransform.Length == 0)
            {
                // TODO
            }
        }
    }
}
