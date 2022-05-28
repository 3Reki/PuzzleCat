using PuzzleCat.Visuals;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class CatController : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private PlayerMovementIndicator movementIndicator;

        private RaycastHit _hit;
        
        public bool HandlePlayerMovement()
        {
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out _hit, GameManager.Instance.MainCamera, -5, 100f, true, 2)) 
                return false;
            
            Vector3Int gridPoint = Utils.Utils.WorldPointAsGridPoint(_hit.normal, _hit.point);

            if (_hit.normal != GameManager.Instance.Cat.transform.up)
                return false;

            movementIndicator.Play(_hit.point + _hit.normal * 0.01f, Quaternion.LookRotation(_hit.normal));
            
            return GameManager.Instance.Cat.TryMovingTo(gridPoint);
        }
    }
}
