using System;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class PlayerController : MonoBehaviour
    {
        private IPlayerState _playerState;
        
        private void Handle()
        {
            IPlayerState newPlayerState = _playerState.Handle();

            if (newPlayerState == null)
                return;
            
            _playerState.Exit();
            newPlayerState.Enter();
            _playerState = newPlayerState;
        }

        private void Start()
        {
            _playerState = GameManager.Instance.DefaultState;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop)
            {
                GameManager.Instance.CameraZoomState.HandleZoomInEditor();
            }
#endif

            if (GameManager.Instance.MenuOpened)
                return;
            
            Handle();
        }
    }
}
