using System;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.Level
{
    public class CatAnimation : MonoBehaviour
    {

        [SerializeField] private Cat catController;
        [SerializeField] private NavMeshAgent playerAgent;
        [SerializeField] private Animator animator;
        
        private static readonly int _speed = Animator.StringToHash("Speed");
        private static readonly int _teleport = Animator.StringToHash("Teleport");
        private static readonly int _idleDown = Animator.StringToHash("IdleDown");

        public void SetIdleDown(bool idleState)
        {
			animator.SetBool(_idleDown, idleState);
        }

        public void StartTeleportAnimation()
        {
            animator.SetTrigger(_teleport);
        }

        public void Warp()
        {
            catController.CastTeleport();
        }

        private void Update()
        {
            animator.SetFloat(_speed, playerAgent.velocity.magnitude);
        }
    }
}
