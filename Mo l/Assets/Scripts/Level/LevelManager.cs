using System;
using UnityEngine;

namespace PuzzleCat.Level
{
	public class LevelManager : MonoBehaviour
	{
		[SerializeField] private Room[] rooms;
		[SerializeField] private GameObject movable;

		private IMovable _selectedMovableObject;

		public void SetSelectedMovableObject(GameObject selectedGameObject)
		{
			_selectedMovableObject = selectedGameObject != null
				? selectedGameObject.GetComponent<IMovable>()
				: null;
		}

		private void MoveObject()
		{
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
				print(_selectedMovableObject);
				_selectedMovableObject.GetRoom().MoveObjectForward(_selectedMovableObject);
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				_selectedMovableObject.GetRoom().MoveObjectBackward(_selectedMovableObject);
			}
		}

		private void Awake()
		{
			SetSelectedMovableObject(movable);
		}

		private void Update()
		{
			if (_selectedMovableObject != null)
			{
				MoveObject();
			}
			
		}
	}
}