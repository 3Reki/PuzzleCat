using System.Collections;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class CatController : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private Transform catDirectionIndicator;
        
        private RaycastHit _hit;
        
        public void HandlePlayerMovement()
        {
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out _hit, GameManager.Instance.MainCamera, -5, 100f, true, 2)) 
                return;
            
            Vector3Int gridPoint = Utils.Utils.WorldPointAsGridPoint(_hit.normal, _hit.point);

            if (_hit.normal == GameManager.Instance.Cat.transform.up)
            {
                GameManager.Instance.Cat.TryMovingTo(gridPoint);
                
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
        
        private void OnGameStateChanged(GameManager.GameState state)
        {
            GameManager.Instance.Cat.SetIdle(state == GameManager.GameState.PortalMode);
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
