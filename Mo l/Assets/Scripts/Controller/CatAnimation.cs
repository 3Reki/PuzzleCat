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
        private static readonly int _angularSpeed = Animator.StringToHash("AngularSpeed");
        private static readonly int _teleport = Animator.StringToHash("Teleport");
        private static readonly int _idleDown = Animator.StringToHash("IdleDown");
        private static readonly int _endMirrorReached = Animator.StringToHash("EndMirrorReached");
        private WaitForSeconds _idleDownWait;
        private float _defaultSpeed;
        private static readonly int _headPat = Animator.StringToHash("HeadPat");

        public void GetUp()
        {
            animator.SetBool(_idleDown, false);
            StopAllCoroutines();
        }

        public void HeadPat()
        {
            animator.SetTrigger(_headPat);
            animator.SetBool(_idleDown, true);
            StopAllCoroutines();
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

        public void UpdateSpeed(float speed, float angularSpeed)
        {
            animator.SetFloat(_speed, speed);
            animator.SetFloat(_angularSpeed, angularSpeed);
        }
        
        public void StartIdleDownTimer()
        {
            StartCoroutine(IdleDownCoroutine());
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
    }
}