using PuzzleCat.Controller;
using UnityEngine;

namespace PuzzleCat.TutorialAnimations
{
    public class TutorialOne : Tutorial
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private RectTransform handTransform;
        [SerializeField] private HandAnimation handAnimation;
        [SerializeField] private Vector3Int[] desiredTouchPosition;
        [SerializeField] private Vector2[] handPositions;
        [SerializeField] private Quaternion[] handRotations;

        private int _currentIndex = -1;

        public override bool CanMovePlayer()
        {
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out RaycastHit hit,
                GameManager.Instance.MainCamera, -5, 100f, true, 2))
                return false;

            return Utils.Utils.WorldPointAsGridPoint(hit.normal, hit.point) == desiredTouchPosition[_currentIndex];
        }

        public override void OnPlayerMovement()
        {
            if (HasNextPosition())
            {
                NextPosition();
                handAnimation.PlayAnimation();
                return;
            }
                        
            handAnimation.StopAnimation();
        }
        
        private void NextPosition()
        {
            _currentIndex++;
            handTransform.anchoredPosition = handPositions[_currentIndex];
            handTransform.rotation = handRotations[_currentIndex];
        }

        private bool HasNextPosition()
        {
            return _currentIndex + 1 < handPositions.Length;
        }
        
        private void Start()
        {
            NextPosition();
            handAnimation.PlayAnimation();
        }
    }
}
