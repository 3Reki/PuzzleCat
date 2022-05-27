using UnityEngine;

namespace PuzzleCat.TutorialAnimations
{
    public class HandAnimation : MonoBehaviour
    {
        public delegate void HandAnimationCallback();

        public HandAnimationCallback OnHalfComplete;
        public HandAnimationCallback OnStart;
        
        [SerializeField] private Animator handAnimator;

        public void PlayAnimation()
        {
            handAnimator.gameObject.SetActive(true);
            handAnimator.enabled = true;
        }
        
        public void StopAnimation()
        {
            handAnimator.enabled = false;
            handAnimator.gameObject.SetActive(false);
        }

        public void PauseAnimation()
        {
            handAnimator.speed = 0;
        }
        
        public void ResumeAnimation()
        {
            handAnimator.speed = 1;
        }

        private void HandleHalfAnimation()
        {
            OnHalfComplete?.Invoke();
        }
        
        private void HandleAnimationStart()
        {
            OnStart?.Invoke();
        }
    }
}
