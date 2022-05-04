using UnityEngine;

namespace PuzzleCat.Controller
{
    public class InputManager : MonoBehaviour
    {
        public int TouchCount { get; private set; }
        public bool TwoTouchesDone { get; private set; }
        public Vector3 FirstTouchPosition { get; private set; }
        public TouchPhase FirstTouchPhase { get; private set; }
        public Vector3 SecondTouchPosition { get; private set; }
        public TouchPhase SecondTouchPhase { get; private set; }

        public bool CameraTouchesFinished => FirstTouchPhase is TouchPhase.Ended or TouchPhase.Canceled || 
                                           SecondTouchPhase is TouchPhase.Ended or TouchPhase.Canceled;

        public bool CameraTouchesMoved => FirstTouchPhase == TouchPhase.Moved || SecondTouchPhase == TouchPhase.Moved;

        private Touch _firstTouch;
        private Touch _secondTouch;

        private void Update()
        {
            TouchCount = Input.touchCount;

            if (TouchCount <= 0)
            {
                TwoTouchesDone = false;
                return;
            }
            
            _firstTouch = Input.GetTouch(0);
            FirstTouchPosition = _firstTouch.position;
            FirstTouchPhase = _firstTouch.phase;

            if (TouchCount <= 1) return;
            
            TwoTouchesDone = true;
            _secondTouch = Input.GetTouch(1);
            SecondTouchPosition = _secondTouch.position;
            SecondTouchPhase = _secondTouch.phase;

        }
    }
}
