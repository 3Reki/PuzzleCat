using System.Collections.Generic;
using PuzzleCat.LevelElements;
using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleCat.Controller
{
    public class PortalPlacementController : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private Transform[] portalsParentTransform;
        [SerializeField] private Button portalButton;
        
        private Dictionary<int, List<Portal>> _portals;
        private int _portalGroupId;
        private int? _portalId;
        
        public void SwitchPortalMode(int id)
        {
            if (GameManager.Instance.State == GameManager.GameState.PortalMode)
            {
                GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerMovement);
                return;
            }

            _portalGroupId = id;
            _portalId = FindCurrentPortalIndex(id);

            GameManager.Instance.UpdateGameState(GameManager.GameState.PortalMode);
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
                _portalId = FindCurrentPortalIndex(_portalGroupId);
                return;
            }

            if (_portalId == null)
            {
                Debug.LogWarning("Attempt to place portal when none is available");
                return;
            }

            Vector3Int gridPoint = Utils.Utils.WorldPointAsGridPoint(hit.normal, hit.point);
            
            if (_portals[_portalGroupId][_portalId.Value].CanSetPortal(hit.transform, gridPoint, hit.normal.ToSurface()))
            {
                _portals[_portalGroupId][_portalId.Value].SetPortal(hit.transform.parent.GetComponent<Room>(), 
                    gridPoint, hit.normal.ToSurface());
                _portalId = FindCurrentPortalIndex(_portalGroupId);
            }
        }
        
        private int? FindCurrentPortalIndex(int portalGroupId)
        {
            for (var i = 0; i < _portals[portalGroupId].Count; i++)
            {
                if (!_portals[portalGroupId][i].Placed)
                {
                    return i;
                }
            }

            return null;
        }

        private void OnGameStateChanged(GameManager.GameState state)
        {
            portalButton.interactable = state != GameManager.GameState.Menu;
        }

        private void ConstructPortalsDictionary()
        {
            _portals = new Dictionary<int, List<Portal>>();
            
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
                }
            }
        }

        private void Awake()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;
            ConstructPortalsDictionary();
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }
    }
}
