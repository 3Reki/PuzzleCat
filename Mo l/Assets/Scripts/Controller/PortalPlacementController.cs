using System;
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
        private Tuple<int, int> _portalIndex;
        
        public void SwitchPortalMode(int id)
        {
            _portalIndex = FindCurrentPortalIndex(id);

            GameManager.Instance.UpdateGameState(_portalIndex == null
                ? GameManager.GameState.PlayerMovement
                : GameManager.GameState.PortalMode);
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
                return;
            }
            
            _portals[_portalIndex.Item1][_portalIndex.Item2].SetPortal(hit.transform.parent.GetComponent<Room>(), 
                Utils.Utils.WorldPointAsGridPoint(hit.normal, hit.point), hit.normal.ToSurface());
            GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerMovement);
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
