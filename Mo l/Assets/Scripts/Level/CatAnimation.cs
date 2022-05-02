using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.Level
{
    public class CatAnimation : MonoBehaviour
    {
        [SerializeField] private Cat catController;
        [SerializeField] private NavMeshAgent playerAgent;
        [SerializeField] private Animator animator;
        [SerializeField] private float jumpSpeed;
        
        private static readonly int _speed = Animator.StringToHash("Speed");
        private static readonly int _teleport = Animator.StringToHash("Teleport");
        private static readonly int _idleDown = Animator.StringToHash("IdleDown");
        private float _defaultSpeed;

        public void SetIdleDown(bool idleState)
        {
			animator.SetBool(_idleDown, idleState);
        }
        
        public void StartJumpingUp()
        {
            animator.Play("Jump Up");
            playerAgent.speed = jumpSpeed;
        }
        
        public void StartJumpingDown()
        {
            animator.Play("Jump Down");
            playerAgent.speed = jumpSpeed;
        }

        public void StopJumping()
        {
            playerAgent.speed = _defaultSpeed;
        }

        public void StartTeleportAnimation()
        {
            animator.SetTrigger(_teleport);
        }

        public void Warp()
        {
            catController.CastTeleport();
        }

        public void TeleportAnimationEnd()
        {
            catController.EndTeleport();
        }

        private void Awake()
        {
            _defaultSpeed = playerAgent.speed;
        }

        private void Update()
        {
            animator.SetFloat(_speed, playerAgent.velocity.magnitude);
        }
    }
}
