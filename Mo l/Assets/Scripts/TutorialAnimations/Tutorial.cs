using PuzzleCat.TutorialAnimations.TutorialActions;
using UnityEngine;

namespace PuzzleCat.TutorialAnimations
{

    public abstract class Tutorial : MonoBehaviour
    {
        [SerializeField] private ITutorialAction action;
        
        public virtual bool CanChangePortalMode() => false;
        public virtual bool CanSelectPortal(int index) => false;
        public virtual bool CanPlacePortal() => false;
        public virtual bool CanSelectElement() => false;
        public virtual bool CanMoveElement() => false;
        public virtual bool CanMovePlayer() => false;
        public virtual void OnPortalModeChanged() {}
        public virtual void OnPortalSelected() {}
        public virtual void OnPortalPlaced() {}
        public virtual void OnElementMovement() {}
        public virtual void OnElementSelection() {}
        public virtual void OnElementDeselection() {}
        public virtual void OnPlayerMovement() {}
        
        public enum Action
        {
            PortalMode,
            PortalSelect,
            PortalPlace,
            ElementMove,
            PlayerMove
        }
    }
}
