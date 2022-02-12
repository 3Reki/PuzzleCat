using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Level
{
	public class LevelManager : MonoBehaviour
	{
		[SerializeField] private new Camera camera;
		[SerializeField] private LayerMask movableLayerMask;

		private IMovable _selectedMovableObject;

		public void SetSelectedMovableObject(GameObject selectedGameObject)
		{
			_selectedMovableObject = selectedGameObject != null
				? selectedGameObject.GetComponent<IMovable>()
				: null;
		}

		private void MoveObject()
		{
			
#if UNITY_EDITOR
			
			// Debug only
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				_selectedMovableObject.GetRoom().MoveObjectLeft(_selectedMovableObject);
			}
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				_selectedMovableObject.GetRoom().MoveObjectRight(_selectedMovableObject);
			}
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				_selectedMovableObject.GetRoom().MoveObjectForward(_selectedMovableObject);
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				_selectedMovableObject.GetRoom().MoveObjectBackward(_selectedMovableObject);
			}
			
#endif
			
		}

		private void HandleTouch()
		{
#if UNITY_EDITOR
			Vector3 position;
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
			
			Ray ray = camera.ScreenPointToRay(position);
#else
			if (Input.touchCount == 0)
				return;
			
			Ray ray = camera.ScreenPointToRay(Input.GetTouch(0).position);
#endif

			if (Physics.Raycast(ray, out RaycastHit hit, 100, movableLayerMask))
			{
				Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 1.5f);
				SetSelectedMovableObject(hit.transform.gameObject);
			}
			else
			{
				Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1.5f);
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