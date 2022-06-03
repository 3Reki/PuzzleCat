using UnityEngine;

namespace PuzzleCat.Controller
{
    public class InputManager : MonoBehaviour
    {
        public int TouchCount { get; private set; }
        public bool TwoTouchesDone { get; private set; }
        public Vector2 FirstTouchPosition { get; private set; }
        public TouchPhase FirstTouchPhase { get; private set; }
        public Vector2 SecondTouchPosition { get; private set; }
        public TouchPhase SecondTouchPhase { get; private set; }
        public Vector2 FirstTouchDeltaPosition;

        public bool CameraTouchesFinished => FirstTouchPhase is TouchPhase.Ended or TouchPhase.Canceled || 
                                           SecondTouchPhase is TouchPhase.Ended or TouchPhase.Canceled;

        public bool CameraTouchesMoved => FirstTouchPhase == TouchPhase.Moved || SecondTouchPhase == TouchPhase.Moved;

        private Touch _firstTouch;
        private Touch _secondTouch;

#if UNITY_EDITOR
        public float MouseScroll { get; private set; }
        public bool IsScrolling => MouseScroll != 0;
        private Vector2 _lastMousePosition;
        
        private void EditorInputs()
        {
            MouseScroll = Input.mouseScrollDelta.y;
            
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                FirstTouchPhase = TouchPhase.Began;
                _lastMousePosition = Input.mousePosition;
                FirstTouchDeltaPosition = Vector2.zero;
            } 
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                if (_lastMousePosition != (Vector2) Input.mousePosition)
                {
                    FirstTouchPhase = TouchPhase.Moved;
                    FirstTouchDeltaPosition = (Vector2) Input.mousePosition - _lastMousePosition;
                    _lastMousePosition = Input.mousePosition;
                }
                else
                {
                    FirstTouchPhase = TouchPhase.Stationary;
                    FirstTouchDeltaPosition = Vector2.zero;
                }
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                FirstTouchPhase = TouchPhase.Ended;
                FirstTouchDeltaPosition = (Vector2) Input.mousePosition - _lastMousePosition;
            }
            else
            {
                TouchCount = 0;
                return;
            }
            
            FirstTouchPosition = _lastMousePosition;
            TouchCount = 1;
        }
#endif

        private void Update()
        {
            TouchCount = Input.touchCount;

#if UNITY_EDITOR
            if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop)
            {
                EditorInputs();
                TwoTouchesDone = false;
                return;
            }
#endif

            if (TouchCount <= 0)
            {
                TwoTouchesDone = false;
                return;
            }
            
            _firstTouch = Input.GetTouch(0);
            FirstTouchPosition = _firstTouch.position;
            FirstTouchPhase = _firstTouch.phase;
            FirstTouchDeltaPosition = _firstTouch.deltaPosition;

            if (TouchCount <= 1) return;
            
            TwoTouchesDone = true;
            _secondTouch = Input.GetTouch(1);
            SecondTouchPosition = _secondTouch.position;
            SecondTouchPhase = _secondTouch.phase;

        }
    }
}
