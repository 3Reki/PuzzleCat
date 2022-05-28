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
        [SerializeField] private Action[] authorizedAction;
        [SerializeField] private Vector2[] handPositions;
        [SerializeField] private Quaternion[] handRotations;
        [SerializeField] private Vector3Int[] desiredTouchPosition;
        [SerializeField] private int[] desiredPortalGroup;
        [SerializeField] private Surface[] desiredTouchSurface;
        [SerializeField] private Vector2[] handDestinations;
        [SerializeField] private float selectionDelay;
        [SerializeField] private bool horribleBool;
        
        private WaitForSeconds _selectionDelay;
        private IEnumerator _furnitureMovementEnumerator;
        private int _desiredPositionIndex;
        private int _desiredPortalGroupIndex;
        private int _desiredSurfaceIndex;
        private int _handPositionIndex = -1;
        private int _handDestinationIndex;
        private int _elementMovementsLeft;
        private bool _tutorialEnded;

        public override bool CanChangePortalMode()
        {
            if (_tutorialEnded)
                return true;
            
            return authorizedAction[_handPositionIndex] == Action.PortalMode;
        }

        public override bool CanSelectPortal(int index)
        {
            if (_tutorialEnded)
                return true;

            return authorizedAction[_handPositionIndex] == Action.PortalSelect &&
                   desiredPortalGroup[_desiredPortalGroupIndex] == index;
        }

        public override bool CanPlacePortal()
        {
            if (_tutorialEnded)
                return true;

            if (authorizedAction[_handPositionIndex] != Action.PortalPlace)
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
            
            if (authorizedAction[_handPositionIndex] != Action.ElementMove)
                return false;

            if (!horribleBool)
                return true;
            
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out RaycastHit hit,
                GameManager.Instance.MainCamera, -5, 100f, true, 2))
                return false;

            return hit.point.z < 0;
        }

        public override bool CanMoveElement()
        {
            if (_tutorialEnded)
                return true;
            
            if (authorizedAction[_handPositionIndex] != Action.ElementMove)
                return false;
            
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out RaycastHit hit,
                GameManager.Instance.MainCamera, invisibleLayerMask, 100f, true, 2))
                return false;

            return Utils.Utils.WorldPointAsGridPoint(hit.normal, hit.point) == desiredTouchPosition[_desiredPositionIndex];
        }

        public override bool CanMovePlayer()
        {
            if (_tutorialEnded)
                return true;

            return authorizedAction[_handPositionIndex] == Action.PlayerMove;
        }

        public override void OnPortalModeChanged()
        {
            if (_tutorialEnded)
                return;
            
            if (!HasNextPosition())
            {
                _tutorialEnded = true;
                return;
            }
            
            NextPosition();
            handAnimation.PlayAnimation();
            if (authorizedAction[_handPositionIndex] == Action.ElementMove)
            {
                OnElementMovementStart();
            }
        }

        public override void OnPortalSelected()
        {
            if (_tutorialEnded)
                return;
            
            if (HasNextPosition())
            {
                NextPosition();
                handAnimation.PlayAnimation();
                _desiredPortalGroupIndex++;
                return;
            }
            _tutorialEnded = true;
        }

        public override void OnPortalPlaced()
        {
            if (_tutorialEnded)
                return;
            
            if (!HasNextPosition())
            {
                _tutorialEnded = true;
                return;
            }
            NextPosition();
            handAnimation.PlayAnimation();
            _desiredPositionIndex++;
            _desiredSurfaceIndex++;
        }

        public override void OnElementMovement()
        {
            if (_tutorialEnded)
                return;
            
            if (!HasNextPosition())
            {
                _tutorialEnded = true;
                return;
            }

            NextPosition();
            handAnimation.OnHalfComplete = MoveFurniture;
            _desiredPositionIndex++;
            _elementMovementsLeft--;
            
            if (authorizedAction[_handPositionIndex] != Action.ElementMove)
            {
                handAnimation.OnHalfComplete = null;
                handAnimation.OnStart = null;
                _handDestinationIndex++;
            }
            
        }

        public override void OnElementSelection()
        {
            if (_tutorialEnded)
                return;
            
            handAnimation.StopAnimation();
            handTransform.DOComplete();
            if (_furnitureMovementEnumerator != null)
            {
                StopCoroutine(_furnitureMovementEnumerator);
            }
        }

        public override void OnElementDeselection()
        {
            if (_tutorialEnded) 
                return;
            
            ResetPosition();
            handAnimation.PlayAnimation();
        }

        private void OnElementMovementStart()
        {
            for (_elementMovementsLeft = 1; _elementMovementsLeft < authorizedAction.Length - _handPositionIndex; _elementMovementsLeft++)
            {
                if (authorizedAction[_handPositionIndex + _elementMovementsLeft - 1] != Action.ElementMove)
                {
                    break;
                }
            }
                
            handAnimation.OnHalfComplete = MoveFurniture;
            handAnimation.OnStart = ResetPosition;
        }
        
        private void MoveFurniture()
        {
            handAnimation.PauseAnimation();
            StartCoroutine(_furnitureMovementEnumerator = MoveFurnitureCoroutine());
        }
        
        private IEnumerator MoveFurnitureCoroutine()
        {
            yield return _selectionDelay;
            handTransform.DOAnchorPos(handDestinations[_handDestinationIndex], .5f * _elementMovementsLeft).SetEase(Ease.Linear).onComplete = 
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
