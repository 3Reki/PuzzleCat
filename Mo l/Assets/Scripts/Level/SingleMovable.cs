using System;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Level
{
	public class SingleMovable : RoomElement, IMovable
	{
		[SerializeField] private Transform objectTransform;
		[SerializeField] private SingleMovable[] linkedMovables;
		
		private Surface currentSurface => objectTransform.up.ToSurface();

		public void MoveLeft()
		{
			TryMovingTowards(-objectTransform.right.ToVector3Int());
		}

		public void MoveRight()
		{
			TryMovingTowards(objectTransform.right.ToVector3Int());
		}

		public void MoveForward()
		{
			TryMovingTowards(objectTransform.forward.ToVector3Int());
		}

		public void MoveBackward()
		{
			TryMovingTowards(-objectTransform.forward.ToVector3Int());
		}

		public void MoveTo(Vector3Int coordinates)
		{
			objectTransform.position = GetWorldPosition(coordinates);
		}

		public void TeleportTo(Vector3Int coordinates)
		{
			objectTransform.position = GetWorldPosition(coordinates);
		}

		private void TryMovingTowards(Vector3Int direction)
		{
			Array.Sort(linkedMovables,
				(movable1, movable2) =>
					((movable2.RoomGridPosition - movable1.RoomGridPosition) * direction).Sum());
			
			/*if (CurrentRoom.FindPortal(RoomGridPosition, position.ToSurface()))
			{
				return;
			}*/

			foreach (SingleMovable movable in linkedMovables)
			{
				if (AnyLinkedElementAt(movable.RoomGridPosition + direction))
				{
					continue;
				}
				
				if (!CurrentRoom.CanMoveOnCell(movable.RoomGridPosition + direction, movable.currentSurface))
				{
					return;
				}
			}

			foreach (SingleMovable movable in linkedMovables)
			{
				CurrentRoom.MoveOnCell(movable, movable.RoomGridPosition + direction, movable.currentSurface);
			}
			
		}

		private bool AnyLinkedElementAt(Vector3Int position)
		{
			foreach (SingleMovable linkedElement in linkedMovables)
			{
				if (linkedElement.RoomGridPosition == position)
				{
					return true;
				}
			}

			return false;
		}
	}
}