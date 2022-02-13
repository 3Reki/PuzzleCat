using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Level
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private new Camera camera;
        [SerializeField] private LayerMask movableLayerMask;

        private SingleMovable _selectedMovableObject;

        private void SetSelectedMovableObject(GameObject selectedGameObject)
        {
            _selectedMovableObject = selectedGameObject != null
                ? selectedGameObject.GetComponent<SingleMovable>()
                : null;
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

            SetSelectedMovableObject(
                UtilsClass.ScreenPointRaycast(position, out RaycastHit hit, camera, movableLayerMask)
                    ? hit.transform.gameObject
                    : null);
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