using UnityEngine;

namespace PuzzleCat.Level
{
	public interface IMovable
	{
		public Vector3Int GetCoordinates();
		public Room GetRoom();
		public void SetRoom(Room room);
		public void MoveLeft();
		public void MoveRight();
		public void MoveForward();
		public void MoveBackward();
	}
}
