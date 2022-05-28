using System.Collections;
using PuzzleCat.LevelElements;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.Controller
{
    public class CatAnimation : MonoBehaviour
    {
        [SerializeField] private Cat catController;
        [SerializeField] private NavMeshAgent playerAgent;
        [SerializeField] private Animator animator;
        [SerializeField] private float timeBeforeIdleDown;
        [SerializeField] private float jumpSpeed;

        private static readonly int _speed = Animator.StringToHash("Speed");
        private static readonly int _velocityX = Animator.StringToHash("VelX");
        private static readonly int _teleport = Animator.StringToHash("Teleport");
        private static readonly int _idleDown = Animator.StringToHash("IdleDown");
        private static readonly int _endMirrorReached = Animator.StringToHash("EndMirrorReached");
        private WaitForSeconds _idleDownWait;
        private IEnumerator _idleDownEnumerator;
        private float _defaultSpeed;

        public void GetUp()
        {
            animator.SetBool(_idleDown, false);
            StopAllCoroutines();
        }
        
        public void StartIdleDownTimer()
        {
            StartCoroutine(_idleDownEnumerator = IdleDownCoroutine());
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
        
        public void JumpInMirror()
        {
            animator.SetTrigger(_endMirrorReached);
        }

        public void Warp()
        {
            catController.CastTeleport();
        }

        public void TeleportAnimationEnd()
        {
            catController.EndTeleport();
        }

        public void EndLevel()
        {
            Cat.EndLevel();
        }

        private IEnumerator IdleDownCoroutine()
        {
            yield return _idleDownWait;
            
            animator.SetBool(_idleDown, true);
        }

        private void Awake()
        {
            _defaultSpeed = playerAgent.speed;
            _idleDownWait = new WaitForSeconds(timeBeforeIdleDown);
        }

        private void Update()
        {
            animator.SetFloat(_speed, playerAgent.velocity.magnitude);
            animator.SetFloat(_velocityX, playerAgent.transform.InverseTransformDirection(playerAgent.velocity).x);
        }
    }
}