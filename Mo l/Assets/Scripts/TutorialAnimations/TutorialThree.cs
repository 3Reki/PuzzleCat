using System.Collections;
using DG.Tweening;
using PuzzleCat.Controller;
using UnityEngine;

namespace PuzzleCat.TutorialAnimations
{
    public class TutorialThree : Tutorial
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private RectTransform handTransform;
        [SerializeField] private HandAnimation handAnimation;
        [SerializeField] private LayerMask invisibleLayerMask;
        [SerializeField] private Vector3Int[] desiredTouchPosition;
        [SerializeField] private Vector2[] HandPositions;
        [SerializeField] private Vector2[] HandDestinations;
        [SerializeField] private float selectionDelay;

        private Vector2 _currentHandDestination;
        private WaitForSeconds _selectionDelay;
        private int _currentIndex = -1;
        private bool _tutorialEnded;

        public override bool CanMovePlayer()
        {
            return _tutorialEnded;
        }

        public override bool CanSelectElement()
        {
            return true;
        }

        public override bool CanMoveElement()
        {
            if (_tutorialEnded)
                return true;
            
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out RaycastHit hit,
                GameManager.Instance.MainCamera, invisibleLayerMask, 100f, true, 2))
                return false;

            return Utils.Utils.WorldPointAsGridPoint(hit.normal, hit.point) == desiredTouchPosition[_currentIndex];
        }

        public override void OnElementSelection()
        {
            handAnimation.StopAnimation();
        }

        public override void OnElementMovement()
        {
            if (HasNextPosition())
            {
                NextPosition();
                return;
            }

            _tutorialEnded = true;
        }
        
        public override void OnElementDeselection()
        {
            if (!_tutorialEnded)
            {
                handAnimation.PlayAnimation();
            }
        }

        private void MoveFurniture()
        {
            handAnimation.PauseAnimation();
            StartCoroutine(MoveFurnitureCoroutine());
        }
        
        private IEnumerator MoveFurnitureCoroutine()
        {
            yield return _selectionDelay;
            handTransform.DOAnchorPos(_currentHandDestination, .5f).SetEase(Ease.Linear).onComplete = 
                handAnimation.ResumeAnimation;
        }

        private void NextPosition()
        {
            _currentIndex++;
            handTransform.anchoredPosition = HandPositions[_currentIndex];
            _currentHandDestination = HandDestinations[_currentIndex];
            handAnimation.OnHalfComplete = MoveFurniture;
        }

        private void ResetPosition()
        {
            handTransform.anchoredPosition = HandPositions[_currentIndex];
        }

        private bool HasNextPosition()
        {
            return _currentIndex + 1 < HandPositions.Length;
        }

        private void Awake()
        {
            _selectionDelay = new WaitForSeconds(selectionDelay);
        }

        private void Start()
        {
            NextPosition();
            handAnimation.PlayAnimation();
            handAnimation.OnStart = ResetPosition;
        }
    }
}
