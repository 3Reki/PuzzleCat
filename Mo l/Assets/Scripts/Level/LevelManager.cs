using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private LayerMask selectableLayerMask;
        [SerializeField] private Cat cat;
        [SerializeField] private NavMeshAgent playerAgent;

        private SingleMovable _selectedMovableObject;
        private bool _playerSelected;

        private void SetSelectedMovableObject(GameObject selectedGameObject)
        {
            if (selectedGameObject == null)
            {
                _selectedMovableObject = null;
                _playerSelected = false;
                return;
            }

            if (_playerSelected || _selectedMovableObject != null)
            {
                return;
            }

            if (cat.IsCat(selectedGameObject))
            {
                _playerSelected = true;
                return;
            }

            _selectedMovableObject = selectedGameObject.GetComponent<SingleMovable>();
        }

        private void MoveObject()
        {
#if UNITY_EDITOR

            // Debug only
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _selectedMovableObject.MoveLeft();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _selectedMovableObject.MoveRight();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _selectedMovableObject.MoveForward();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _selectedMovableObject.MoveBackward();
            }

#endif
        }

        private void HandleTouch()
        {
            Vector3 position;

#if UNITY_EDITOR
            if (Input.touchCount > 0) //&& !UtilsClass.IsPointerOverUI())
            {
                position = Input.GetTouch(0).position;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                position = Input.mousePosition;
            }
            else
            {
                return;
            }
#else
			if (Input.touchCount == 0)
				return;
			
			position = Input.GetTouch(0).position;
#endif
            bool raycastResult = UtilsClass.ScreenPointRaycast(position, out RaycastHit hit, camera);

            if (_playerSelected && raycastResult)
            {
                playerAgent.SetDestination(hit.point);
            }
            
            if (raycastResult)
            {
                GameObject hitGameObject = hit.transform.gameObject;
                if (!UtilsClass.IsInLayerMask(hitGameObject, selectableLayerMask)) return;
                
                SetSelectedMovableObject(hitGameObject);
                print("it works");
            }
            else
            {
                SetSelectedMovableObject(null);
            }
        }

        private void Update()
        {
            if (_selectedMovableObject != null)
            {
                MoveObject();
            }

            HandleTouch();
        }
    }
}