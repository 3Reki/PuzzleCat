using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Level
{
	public class SingleMovable : RoomElement, IMovable
	{
		[SerializeField] private Transform objectTransform;

		public void MoveLeft()
		{
			Vector3Int newPosition = RoomGridPosition - objectTransform.right.ToVector3Int();
			TryMovingTo(newPosition);
		}

		public void MoveRight()
		{
			Vector3Int newPosition = RoomGridPosition + objectTransform.right.ToVector3Int();
			TryMovingTo(newPosition);
		}

		public void MoveForward()
		{
			Vector3Int newPosition = RoomGridPosition + objectTransform.forward.ToVector3Int();
			TryMovingTo(newPosition);
		}

		public void MoveBackward()
		{
			Vector3Int newPosition = RoomGridPosition - objectTransform.forward.ToVector3Int();
			TryMovingTo(newPosition);
		}

		public void MoveTo(Vector3Int coordinates)
		{
			objectTransform.position = GetWorldPosition(coordinates);
		}

		public void TeleportTo(Vector3Int coordinates)
		{
			objectTransform.position = GetWorldPosition(coordinates);
		}

		private void TryMovingTo(Vector3Int position)
		{
			Surface currentSurface = objectTransform.up.ToSurface();
			
			if (CurrentRoom.TryUsingPortal(this, RoomGridPosition, (RoomGridPosition - position).ToSurface()))
			{
				return;
			}
			
			if (CurrentRoom.CanMoveOnCell(position, currentSurface))
			{
				CurrentRoom.MoveOnCell(this, position, currentSurface);
			}
		}
	}
}