using System;
using System.Collections;
using UnityEngine;

namespace PuzzleCat.TutorialAnimations
{
    public class TutorialOne : MonoBehaviour
    {
        [SerializeField] private RectTransform handTransform;
        [SerializeField] private Animator handAnimator;
        [SerializeField] private float secondsToRepeat;
        [SerializeField] private Vector3Int[] desiredTouchPosition;
        [SerializeField] private Vector2[] handPositions;
        [SerializeField] private Quaternion[] handRotations;

        private int _currentIndex = -1;
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

        public void NextPosition()
        {
            _currentIndex++;
            handTransform.anchoredPosition = handPositions[_currentIndex];
            handTransform.rotation = handRotations[_currentIndex];
        }

        public bool HasNextPosition()
        {
            return _currentIndex + 1 < handPositions.Length;
        }

        public bool IsValidTouch(Vector3Int position)
        {
            return position == desiredTouchPosition[_currentIndex];
        }

        private IEnumerator LoopAnimation()
        {
            yield return _waitForSeconds;
            handAnimator.SetTrigger(_replay);

            StartCoroutine(_loopEnumerator = LoopAnimation());
        }

        private void Awake()
        {
            _waitForSeconds = new WaitForSeconds(secondsToRepeat);
        }

        private void Start()
        {
            NextPosition();
            StartCoroutine(_loopEnumerator = LoopAnimation());
        }

        //Premiere animation de la main (devant le meuble au pied du lit)
        //Deuxieme animation de la main (sur le point de teleportation du chat)
        //Troisieme animation de la main (sur le miroir de fin de niveau)
    }
}
