using PuzzleCat.Controller;
using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.LevelElements
{
    public class Cat : RoomElement, IMovable
    {
        public delegate void OnArrival();

        public OnArrival onArrival;

        [SerializeField] private NavMeshAgent playerAgent;
        [SerializeField] private Transform myTransform;
        [SerializeField] private CatAnimation catAnimation;
        [SerializeField] private Surface currentSurface = Surface.Floor;

        private bool _isMoving;
        private bool _isGrounded = true;
        private bool _canMove = true;
        private Vector3 _warpDestination;
        private Vector3 _lookAtDirection;
        private NavMeshPath _path;
        private Vector3 _agentDestination;

        private Vector3 _offset
        {
            get
            {
                switch (currentSurface)
                {
                    case Surface.Floor:
                        return new Vector3(0.5f, 0, 0.5f);
                    case Surface.SideWall:
                        return new Vector3(0, 0.5f, 0.5f);
                    case Surface.BackWall:
                        return new Vector3(0.5f, 0.5f, 1);
                }

                Debug.LogWarning("Not on a valid surface");
                return Vector3Int.zero;
            }
        }

        public static bool IsCat(GameObject gameObject) => gameObject.GetComponent<Cat>() != null;
        public static bool IsCat(object otherObject) => otherObject.GetType() == typeof(Cat);

        public bool IsUnderCat(RoomElement roomElement)
        {
            return roomElement.WorldGridPosition == WorldGridPosition - currentSurface.GetNormal();
        }

        public void SetIdle(bool idleState)
        {
            catAnimation.SetIdleDown(idleState);
        }

        public void TryMovingTo(Vector3Int worldGridDestination)
        {
            if (!_canMove)
                return;

            _agentDestination = worldGridDestination + _offset;
            _path = new NavMeshPath();

            if (!playerAgent.CalculatePath(_agentDestination, _path) || _path.status != NavMeshPathStatus.PathComplete)
            {
                return;
            }

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
            if (position == _agentDestination)
            {
                playerAgent.SetPath(_path);
            }
            else
            {
                playerAgent.SetDestination(position);
            }
            
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
            currentSurface = newSurface;
        }

        public void CastTeleport()
        {
            playerAgent.areaMask = 1 + currentSurface.GetNavMeshAreaMask();
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
                
                if (_lookAtDirection.ApplyMask(currentSurface.GetNormal()) > 0)
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
            playerAgent.areaMask = 1 + currentSurface.GetNavMeshAreaMask();
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