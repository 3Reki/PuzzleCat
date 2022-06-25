using System;
using PuzzleCat.Visuals;
using UnityEngine;

namespace PuzzleCat.Controller
{
    public class CatMovementState : MonoBehaviour, IPlayerState
    {
        [SerializeField] private InputManager inputManager;
        
        private PlayerMovementIndicator _movementIndicator;

        public void Enter()
        {
            HandlePlayerMovement();
        }

        public IPlayerState Handle()
        {
            return GameManager.Instance.DefaultState;
        }

        public void Exit()
        {
        }

        private bool HandlePlayerMovement()
        {
            if (!Utils.Utils.ScreenPointRaycast(inputManager.FirstTouchPosition, out RaycastHit hit, 
                GameManager.Instance.MainCamera, -5, 100f, true, 2)) 
                return false;
            
            Vector3Int gridPoint = Utils.Utils.WorldPointAsGridPoint(hit.normal, hit.point);
            
            if (hit.transform == GameManager.Instance.Cat.transform)
            {
                GameManager.Instance.Cat.HeadPat();
                return false;
            }

            if (hit.normal != GameManager.Instance.Cat.transform.up)
                return false;

            _movementIndicator.Play(hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
            
            return GameManager.Instance.Cat.TryMovingTo(gridPoint);
        }

        private void Awake()
        {
            _movementIndicator = FindObjectOfType<PlayerMovementIndicator>();
        }
    }
}
