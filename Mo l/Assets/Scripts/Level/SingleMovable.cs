using UnityEngine;

namespace PuzzleCat.Level
{
	public class SingleMovable : MonoBehaviour, IMovable
	{
		[SerializeField] private Transform objectTransform;

		private Room _currentRoom;

		public Vector3Int GetCoordinates()
		{
			var position = transform.position;
			return new Vector3Int((int) (position.x + 0.5f), (int) (position.y + 0.5f), (int) (position.z + 0.5f));
		}

		public Room GetRoom()
		{
			return _currentRoom;
		}

		public void SetRoom(Room room)
		{
			_currentRoom = room;
		}

		public void MoveLeft()
		{
			objectTransform.position -= Vector3.right;
		}

		public void MoveRight()
		{
			objectTransform.position += Vector3.right;
		}

		public void MoveForward()
		{
			objectTransform.position += Vector3.forward;
		}

		public void MoveBackward()
		{
			objectTransform.position -= Vector3.forward;
		}
		
	}
}