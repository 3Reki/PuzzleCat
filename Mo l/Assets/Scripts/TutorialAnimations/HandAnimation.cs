using System.Collections;
using UnityEngine;

namespace PuzzleCat.TutorialAnimations
{
    public class HandAnimation : MonoBehaviour
    {
        [SerializeField] private Animator handAnimator;
        [SerializeField] private float secondsToRepeat = 2f;

        private WaitForSeconds _waitForSeconds;
        private IEnumerator _loopEnumerator;
        private static readonly int _replay = Animator.StringToHash("Replay");

        public void PlayAnimation()
        {
            handAnimator.enabled = true;
            handAnimator.SetTrigger(_replay);
            handAnimator.gameObject.SetActive(true);
            StopCoroutine(_loopEnumerator);
            StartCoroutine(_loopEnumerator = LoopAnimation());
        }
        
        public void StopAnimation()
        {
            handAnimator.enabled = false;
            handAnimator.gameObject.SetActive(false);
            StopCoroutine(_loopEnumerator);
        }

        private IEnumerator LoopAnimation()
        {
            while (true)
            {
                yield return _waitForSeconds;
                handAnimator.SetTrigger(_replay);
            }
        }

        private void Awake()
        {
            _waitForSeconds = new WaitForSeconds(secondsToRepeat);
            _loopEnumerator = LoopAnimation();
        }
    }
}
