using UnityEngine;

namespace PuzzleCat.TutorialAnimations
{
    public abstract class Tutorial : MonoBehaviour
    {
        public virtual bool CanPlacePortal() => false;
        public virtual bool CanMoveElement() => false;
        public virtual bool CanMovePlayer() => false;
        public virtual void OnPortalPlaced() {}
        public virtual void OnElementMovement() {}
        public virtual void OnPlayerMovement() {}
    }
}
