using PuzzleCat.Controller;
using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.AI;
using PuzzleCat.Sound;

namespace PuzzleCat.LevelElements
{
    public class Cat : RoomElement
    {
        public CatCallback OnArrival;

        [SerializeField] private NavMeshAgent playerAgent;
        [SerializeField] private Transform myTransform;
        [SerializeField] private CatAnimation catAnimation;
        [SerializeField] private Surface currentSurface = Surface.Floor;

        private bool _isGrounded = true;
        private bool _canMove = true;
        private Vector3 _warpDestination;
        private Vector3 _lookAtDirection;
        private NavMeshPath _path;
        private Vector3 _agentDestination;

        private Vector3 offset
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

        public static void EndLevel()
        {
            GameManager.Instance.UpdateGameState(GameManager.GameState.End);
            AudioManager.Instance.Play("LevelWin");
        }

        public void HeadPat()
        {
            catAnimation.HeadPat();
        }

        public bool IsUnderCat(RoomElement roomElement)
        {
            return roomElement.WorldGridPosition == WorldGridPosition - currentSurface.GetNormal();
        }

        public bool TryMovingTo(Vector3Int worldGridDestination)
        {
            if (!_canMove)
                return false;

            _agentDestination = worldGridDestination + offset;
            _path = new NavMeshPath();

            if (!playerAgent.CalculatePath(_agentDestination, _path) || _path.status != NavMeshPathStatus.PathComplete)
            {
                return false;
            }

            Vector3Int destination = CurrentRoom.WorldToRoomCoordinates(worldGridDestination);
            OnArrival = () => { };

            if (CurrentRoom.CanMoveOnCell(this, destination, myTransform.up.ToSurface()))
            {
                CurrentRoom.MoveOnCell(this, destination, myTransform.up.ToSurface());
                AudioManager.Instance.Play("Click");
                catAnimation.GetUp();
                return true;
            }

            return false;
        }

        public override void MoveTo(Vector3Int destination)
        {
            MoveTo(destination + offset, 0);
        }

        public void MoveTo(Vector3 destination, float aimedDistance)
        {
            playerAgent.stoppingDistance = aimedDistance;
            MoveTo(destination);
        }

        private void MoveTo(Vector3 position)
        {
            if (position != _agentDestination)
            {
                _agentDestination = position;
                playerAgent.CalculatePath(_agentDestination, _path);

            }

            playerAgent.SetPath(_path);
            playerAgent.Move(position.normalized * Time.deltaTime);
            enabled = true;
        }

        public void TeleportTo(Vector3Int coordinates, Surface newSurface, Vector3Int exitDirection)
        {
            myTransform.rotation = Quaternion.LookRotation(_lookAtDirection, myTransform.up);
            catAnimation.StartTeleportAnimation();
            _warpDestination = GetWorldPosition(coordinates);
            _lookAtDirection = exitDirection;
            _canMove = false;
            currentSurface = newSurface;
            AudioManager.Instance.Play("TPIn");

        }

        public void CastTeleport()
        {
            playerAgent.areaMask = 1 + currentSurface.GetNavMeshAreaMask();
            playerAgent.Warp(_warpDestination);
            AudioManager.Instance.Play("TPOut");
            myTransform.rotation = Quaternion.LookRotation(_lookAtDirection, currentSurface.GetNormal());
        }

        public void EndTeleport()
        {
            _canMove = true;
        }

        public void JumpInMirror()
        {
            myTransform.rotation = Quaternion.LookRotation(_lookAtDirection, myTransform.up);
            catAnimation.JumpInMirror();
            AudioManager.Instance.Play("Mirror");

        }

        private void HandleJump()
        {
            if (playerAgent.isOnOffMeshLink)
            {
                if (!_isGrounded) return;

                _isGrounded = false;
                _canMove = false;
                _lookAtDirection = myTransform.InverseTransformDirection(playerAgent.steeringTarget - myTransform.position);

                if (_lookAtDirection.y > 0)
                {
                    catAnimation.StartJumpingUp();
                    AudioManager.Instance.Play("JumpUp");

                }
                else
                {
                    catAnimation.StartJumpingDown();
                    AudioManager.Instance.Play("JumpDown");

                }

                _lookAtDirection.y = 0;
                _lookAtDirection = myTransform.TransformDirection(_lookAtDirection);
                myTransform.rotation = Quaternion.LookRotation(_lookAtDirection, myTransform.up);
            }
            else if (!_isGrounded)
            {
                _isGrounded = true;
                _canMove = true;
                catAnimation.StopJumping();
            }
        }

        private void HandleMovementChecking()
        {
            if (playerAgent.remainingDistance > playerAgent.stoppingDistance)
            {
                Vector3 velocity = playerAgent.velocity;
                catAnimation.UpdateSpeed(velocity.magnitude, 
                    myTransform.InverseTransformDirection(velocity).x);
                return;
            }

            playerAgent.velocity = Vector3.zero;
            _lookAtDirection = myTransform.InverseTransformDirection(playerAgent.destination - myTransform.position);
            _lookAtDirection.y = 0;
            _lookAtDirection = myTransform.TransformDirection(_lookAtDirection);
            catAnimation.UpdateSpeed(0, 0);
            enabled = false;

            if (OnArrival != null)
            {
                OnArrival();
                OnArrival = null;
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
        
        public delegate void CatCallback();
    }
}
