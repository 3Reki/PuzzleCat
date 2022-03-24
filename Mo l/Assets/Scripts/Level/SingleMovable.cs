using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Level
{
	public class SingleMovable : RoomElement, IMovable
	{
		[SerializeField] private Transform objectTransform;

		public void MoveLeft()
		{
			Vector3Int newPosition = RoomGridPosition - transform.right.ToVector3Int();
			if (CurrentRoom.CanMoveOnCell(newPosition))
			{
				CurrentRoom.MoveOnCell(this, newPosition);
			}
		}

		public void MoveRight()
		{
			Vector3Int newPosition = RoomGridPosition + transform.right.ToVector3Int();
			if (CurrentRoom.CanMoveOnCell(newPosition))
			{
				CurrentRoom.MoveOnCell(this, newPosition);
			}
		}

		public void MoveForward()
		{
			Vector3Int newPosition = RoomGridPosition + transform.forward.ToVector3Int();
			if (CurrentRoom.CanMoveOnCell(newPosition))
			{
				CurrentRoom.MoveOnCell(this, newPosition);
			}
		}

		public void MoveBackward()
		{
			Vector3Int newPosition = RoomGridPosition - transform.forward.ToVector3Int();
			if (CurrentRoom.CanMoveOnCell(newPosition))
			{
				CurrentRoom.MoveOnCell(this, newPosition);
			}
		}

		public void MoveTo(Vector3Int coordinates)
		{
			objectTransform.position = GetWorldPosition(coordinates);
		}

		public void TeleportTo(Vector3Int coordinates)
		{
			objectTransform.position = GetWorldPosition(coordinates);
		}
	}
}