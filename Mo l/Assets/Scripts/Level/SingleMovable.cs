using System;
using System.Collections;
using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace PuzzleCat.Level
{
	public class SingleMovable : RoomElement, IMovable
	{
		[SerializeField] private Transform objectTransform;
		[SerializeField] private SingleMovable[] linkedMovables;
		
		private Surface currentSurface => objectTransform.up.ToSurface();

		public void MoveLeft()
		{
			Array.Sort(linkedMovables,
				(movable1, movable2) => movable1.RoomGridPosition.x - movable2.RoomGridPosition.x);
			TryMovingTo(-objectTransform.right.ToVector3Int());
		}

		public void MoveRight()
		{
			Array.Sort(linkedMovables,
				(movable1, movable2) => movable2.RoomGridPosition.x - movable1.RoomGridPosition.x);
			TryMovingTo(objectTransform.right.ToVector3Int());
		}

		public void MoveForward()
		{
			Array.Sort(linkedMovables,
				(movable1, movable2) => movable2.RoomGridPosition.z - movable1.RoomGridPosition.z);
			TryMovingTo(objectTransform.forward.ToVector3Int());
		}

		public void MoveBackward()
		{
			Array.Sort(linkedMovables,
				(movable1, movable2) => movable1.RoomGridPosition.z - movable2.RoomGridPosition.z);
			TryMovingTo(-objectTransform.forward.ToVector3Int());
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
			/*if (CurrentRoom.FindPortal(RoomGridPosition, position.ToSurface()))
			{
				return;
			}*/

			foreach (SingleMovable movable in linkedMovables)
			{
				if (AnyLinkedElementAt(movable.RoomGridPosition + position))
				{
					continue;
				}
				
				if (!CurrentRoom.CanMoveOnCell(movable.RoomGridPosition + position, movable.currentSurface))
				{
					return;
				}
			}

			foreach (SingleMovable movable in linkedMovables)
			{
				CurrentRoom.MoveOnCell(movable, movable.RoomGridPosition + position, movable.currentSurface);
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