using System;
using PuzzleCat.Sound;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Menus
{
    public class SecretMenu : MonoBehaviour
    {
        [SerializeField] private GameObject secret;
        
        private Direction[] _inputs;
        private int _currentIndex;

        private KonamiCode CurrentInput()
        {
            return _inputs[_currentIndex] switch
            {
                Direction.Up => position => position.y > Screen.height * 0.9f,
                Direction.Right => position => position.x > Screen.width * 0.9f,
                Direction.Down => position => position.y < Screen.height * 0.1f,
                Direction.Left => position => position.x < Screen.width * 0.1f,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void Start()
        {
            _inputs = new[]
            {
                Direction.Up, Direction.Up, Direction.Down, Direction.Down, 
                Direction.Left, Direction.Right, Direction.Left, Direction.Right
            };
        }

        private void Update()
        {
            if (Input.touchCount <= 0) 
                return;

            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;

            Debug.Log(Screen.height);
            Debug.Log(Input.GetTouch(0).position);
            if (!CurrentInput()(Input.GetTouch(0).position))
            {
                _currentIndex = 0;
                return;
            }

            _currentIndex++;
            if (_currentIndex == _inputs.Length)
            {
                secret.SetActive(true);
                AudioManager.Instance.StopPlaying("MenuMusic");
                AudioManager.Instance.Play("Important");
                _currentIndex = 0;
            }
        }

        private void OnDestroy()
        {
            AudioManager.Instance.StopPlaying("Important");
        }

        private delegate bool KonamiCode(Vector2 position);
    }
}
