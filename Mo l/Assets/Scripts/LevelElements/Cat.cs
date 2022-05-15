using PuzzleCat.Controller;
using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.LevelElements
{
    public class Cat : RoomElement
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
        
        public override void MoveTo(Vector3Int destination)
        {
            MoveTo(destination + _offset, 0);
        }
        
        public void MoveTo(Vector3 destination, float aimedDistance)
        {
            playerAgent.stoppingDistance = aimedDistance;
            MoveTo(destination);
        }

        private void MoveTo(Vector3 position)
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
            playerAgent.isStopped = false;
        }

        public void TeleportTo(Vector3Int coordinates, Surface newSurface, Vector3Int exitDirection)
        {
            print(_lookAtDirection);
            myTransform.rotation = Quaternion.LookRotation(_lookAtDirection);
            catAnimation.StartTeleportAnimation();
            _warpDestination = GetWorldPosition(coordinates);
            _lookAtDirection = exitDirection;
            _canMove = false;
            currentSurface = newSurface;
        }

        public void CastTeleport()
        {
            playerAgent.areaMask = 1 + currentSurface.GetNavMeshAreaMask();
            playerAgent.Warp(_warpDestination);
            //transform.Rotate(_lookAtDirection);
            myTransform.rotation = Quaternion.LookRotation(_lookAtDirection);
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

        private void HandleMovementChecking()
        {
            if (playerAgent.remainingDistance > playerAgent.stoppingDistance)
            {
                _isMoving = true;
                return;
            }
            
            if (!_isMoving) return;

            playerAgent.velocity = Vector3.zero;
            _lookAtDirection = myTransform.InverseTransformDirection(playerAgent.destination - myTransform.position);
            _lookAtDirection.y = 0;
            _lookAtDirection = myTransform.TransformDirection(_lookAtDirection);
            _isMoving = false;

            if (onArrival != null)
            {
                onArrival();
                onArrival = null;
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
            HandleMovementChecking();
        }
    }
}