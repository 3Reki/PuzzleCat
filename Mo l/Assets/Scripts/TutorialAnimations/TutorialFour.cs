using System.Collections;
using DG.Tweening;
using PuzzleCat.Controller;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.TutorialAnimations
{
    public class TutorialFour : Tutorial
    {
        [SerializeField] private InputManager inputManager;
        [SerializeField] private RectTransform handTransform;
        [SerializeField] private HandAnimation handAnimation;
        [SerializeField] private LayerMask invisibleLayerMask;
        [SerializeField] private Vector3Int[] desiredTouchPosition;
        [SerializeField] private Surface[] desiredTouchSurface;
        [SerializeField] private Vector2 handDestination;
        [SerializeField] private Vector2[] handPositions;
        [SerializeField] private Quaternion[] handRotations;
        [SerializeField] private float selectionDelay;
        
        private WaitForSeconds _selectionDelay;
        private IEnumerator _furnitureMovementEnumerator;
        private int _desiredPositionIndex;
        private int _desiredSurfaceIndex;
        private int _handPositionIndex = -1;
        private bool _tutorialEnded;

        public override bool CanChangePortalMode()
        {
            if (_tutorialEnded)
                return true;
            
            return _handPositionIndex is 0 or 4;
        }

        public override bool CanSelectPortal(int index)
        {
            if (_tutorialEnded)
                return true;
            
            return _handPositionIndex == 1;
        }

        public override bool CanPlacePortal()
        {
            if (_tutorialEnded)
                return true;

            if (_handPositionIndex is not (2 or 3))
                return false;

            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out RaycastHit hit,
                GameManager.Instance.MainCamera, -5, 100f, true, 2))
                return false;

            return Utils.Utils.WorldPointAsGridPoint(hit.normal, hit.point) ==
                   desiredTouchPosition[_desiredPositionIndex] &&
                   hit.normal.ToSurface() == desiredTouchSurface[_desiredSurfaceIndex];
        }

        public override bool CanSelectElement()
        {
            if (_tutorialEnded)
                return true;
            
            return _handPositionIndex >= 5;
        }

        public override bool CanMoveElement()
        {
            if (_tutorialEnded)
                return true;
            
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out RaycastHit hit,
                GameManager.Instance.MainCamera, invisibleLayerMask, 100f, true, 2))
                return false;

            return Utils.Utils.WorldPointAsGridPoint(hit.normal, hit.point) == desiredTouchPosition[_desiredPositionIndex];
        }

        public override bool CanMovePlayer()
        {
            return _tutorialEnded;
        }

        public override void OnPortalModeChanged()
        {
            if (!HasNextPosition())
            {
                _tutorialEnded = true;
                return;
            }
            
            NextPosition();
            if (_handPositionIndex == 5)
            {
                handAnimation.OnHalfComplete = MoveFurniture;
                handAnimation.OnStart = ResetPosition;
            }
        }

        public override void OnPortalSelected()
        {
            if (HasNextPosition())
            {
                NextPosition();
                return;
            }
            _tutorialEnded = true;
        }

        public override void OnPortalPlaced()
        {
            if (!HasNextPosition())
            {
                _tutorialEnded = true;
                return;
            }
            NextPosition();
            _desiredPositionIndex++;
            _desiredSurfaceIndex++;
        }

        public override void OnElementMovement()
        {
            if (!HasNextPosition())
            {
                _tutorialEnded = true;
                return;
            }

            NextPosition();
            handAnimation.OnHalfComplete = MoveFurniture;
            _desiredPositionIndex++;
        }

        public override void OnElementSelection()
        {
            handAnimation.StopAnimation();
            handTransform.DOComplete();
            if (_furnitureMovementEnumerator != null)
            {
                StopCoroutine(_furnitureMovementEnumerator);
            }
        }

        public override void OnElementDeselection()
        {
            if (!_tutorialEnded)
            {
                ResetPosition();
                handAnimation.PlayAnimation();
            }
        }
        
        private void MoveFurniture()
        {
            handAnimation.PauseAnimation();
            StartCoroutine(_furnitureMovementEnumerator = MoveFurnitureCoroutine());
        }
        
        private IEnumerator MoveFurnitureCoroutine()
        {
            yield return _selectionDelay;
            handTransform.DOAnchorPos(handDestination, .5f * (desiredTouchPosition.Length - _desiredPositionIndex)).SetEase(Ease.Linear).onComplete = 
                handAnimation.ResumeAnimation;
        }
        
        private void NextPosition()
        {
            _handPositionIndex++;
            handTransform.anchoredPosition = handPositions[_handPositionIndex];
            handTransform.rotation = handRotations[_handPositionIndex];
        }

        private void ResetPosition()
        {
            handTransform.anchoredPosition = handPositions[_handPositionIndex];
        }

        private bool HasNextPosition()
        {
            return _handPositionIndex + 1 < handPositions.Length;
        }
        
        private void Awake()
        {
            _selectionDelay = new WaitForSeconds(selectionDelay);
        }

        private void Start()
        {
            NextPosition();
            handAnimation.PlayAnimation();
        }
    }
}
