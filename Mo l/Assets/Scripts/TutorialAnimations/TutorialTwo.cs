using PuzzleCat.Controller;
using UnityEngine;

namespace PuzzleCat.TutorialAnimations
{
    public class TutorialTwo : Tutorial
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private RectTransform handTransform;
        [SerializeField] private HandAnimation handAnimation;
        [SerializeField] private Vector2 handPosition = new(-140,29);
        [SerializeField] private Vector3Int[] desiredFirstTouch;
        
        private bool _movedOnce;

        public override bool CanMovePlayer()
        {
            if (_movedOnce)
                return true;
            
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out RaycastHit hit,
                GameManager.Instance.MainCamera, -5, 100f, true, 2))
                return false;

            Vector3Int touchGridPosition = Utils.Utils.WorldPointAsGridPoint(hit.normal, hit.point);
            
            foreach (Vector3Int touchPosition in desiredFirstTouch)
            {
                if (touchGridPosition == touchPosition)
                {
                    return true;
                }
            }
            
            return false;
        }

        public override void OnPlayerMovement()
        {
            if (_movedOnce)
                return;

            _movedOnce = true;
                        
            handAnimation.StopAnimation();
        }

        private void Start()
        {
            handTransform.anchoredPosition = handPosition;
            handAnimation.PlayAnimation();
        }
    }
}
