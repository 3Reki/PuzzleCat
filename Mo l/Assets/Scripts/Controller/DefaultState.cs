using PuzzleCat.LevelElements;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class DefaultState : MonoBehaviour, IPlayerState
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private LayerMask selectableLayerMask;
        
        public void Enter()
        {
        }

        public IPlayerState Handle()
        {
            if (inputManager.PortalMode)
                return GameManager.Instance.portalState;
            
            if (inputManager.TouchCount > 1)
                return GameManager.Instance.CameraZoomState;

            if (inputManager.TouchCount == 0)
                return null;

            switch (inputManager.FirstTouchPhase)
            {
                case TouchPhase.Began:
                    if (CanEnterFurnitureMode())
                        return GameManager.Instance.MovableSelectionState;
                    break;
                
                case TouchPhase.Moved:
                    return GameManager.Instance.CameraMovementState;
                
                case TouchPhase.Ended:
                    return GameManager.Instance.CatMovementState;
            }
            
            return null;
        }

        public void Exit()
        {
        }
        
        public bool CanEnterFurnitureMode()
        {
            return Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out RaycastHit hit,
                       GameManager.Instance.MainCamera, selectableLayerMask, 100f, true, 2)
                   && !hit.transform.GetComponent<MovableElement>().InPortal;
        }
    }
}
