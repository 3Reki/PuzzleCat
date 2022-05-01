using System;
using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.Level
{
    public class Cat : RoomElement, IMovable
    {
        public delegate void OnArrival();

        public OnArrival onArrival;

        [SerializeField] private NavMeshAgent playerAgent;
        [SerializeField] private Transform myTransform;
        [SerializeField] private CatAnimation catAnimation;

        private bool _isMoving;
        private bool _isGrounded = true;
        private bool _canMove = true;
        private Vector3 _warpDestination;
        private Vector3 _lookAtDirection;
        private Surface _currentSurface = Surface.Floor;

        private Vector3 _offset
        {
            get
            {
                Vector3 roundedUp = myTransform.up.Round();
                if (roundedUp == Vector3.up)
                {
                    return new Vector3(0.5f, 0, 0.5f);
                }

                if (roundedUp == Vector3.right)
                {
                    return new Vector3(0, 0.5f, 0.5f);
                }

                if (roundedUp == Vector3.back)
                {
                    return new Vector3(0.5f, 0.5f, 0.5f);
                }

                Debug.LogWarning("Not on floor");
                return Vector3Int.zero;
            }
        }

        public static bool IsCat(GameObject gameObject) => gameObject.GetComponent<Cat>() != null;
        public static bool IsCat(object otherObject) => otherObject.GetType() == typeof(Cat);

        public void SetIdle(bool idleState)
        {
            catAnimation.SetIdleDown(idleState);
        }

        public void TryMovingTo(Vector3Int worldGridDestination)
        {
            if (!_canMove)
                return;
            
            Vector3Int destination = CurrentRoom.WorldToRoomCoordinates(worldGridDestination);

            if (CurrentRoom.CanMoveOnCell(this, destination, myTransform.up.ToSurface()))
            {
                CurrentRoom.MoveOnCell(this, destination, myTransform.up.ToSurface());
            }
        }

        public void MoveTo(Vector3Int coordinates)
        {
            MoveTo(coordinates + _offset);
        }

        public void MoveTo(Vector3 position)
        {
            playerAgent.SetDestination(position);
            _lookAtDirection = position - myTransform.position;
            _isMoving = true;
        }

        public void TeleportTo(Vector3Int coordinates, Surface newSurface, Vector3Int exitDirection)
        {
            transform.rotation = Quaternion.LookRotation(_lookAtDirection);
            catAnimation.StartTeleportAnimation();
            _warpDestination = GetWorldPosition(coordinates);
            _lookAtDirection = exitDirection;
            _isMoving = false;
            _canMove = false;
            _currentSurface = newSurface;
        }

        public void CastTeleport()
        {
            playerAgent.Warp(_warpDestination);
            transform.rotation = Quaternion.LookRotation(_lookAtDirection);
        }

        public void EndTeleport()
        {
            _canMove = true;
        }

        private void HandleJump()
        {
            if (playerAgent.isOnOffMeshLink)
            {
                if (!_isGrounded) return;

                _isGrounded = false;
                _lookAtDirection = playerAgent.steeringTarget - myTransform.position;
                myTransform.rotation = Quaternion.LookRotation(_lookAtDirection);
                
                if (_lookAtDirection.ApplyMask(_currentSurface.GetNormal()) > 0)
                {
                    catAnimation.StartJumpingUp();
                }
                else
                {
                    catAnimation.StartJumpingDown();
                }

                
            }
            else if (!_isGrounded)
            {
                _isGrounded = true;
                catAnimation.StopJumping();
            }
        }

        private void Awake()
        {
            playerAgent.enabled = true;
        }

        private void Update()
        {
            HandleJump();

            if (!_isMoving || playerAgent.remainingDistance > 0) return;

            _isMoving = false;

            if (onArrival != null)
            {
                onArrival();
                onArrival = null;
            }
        }
    }
}