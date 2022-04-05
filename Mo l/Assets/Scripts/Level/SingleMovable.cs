using System;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Level
{
	public class SingleMovable : RoomElement, IMovable
	{
		[SerializeField] private Transform objectTransform;
		[SerializeField] private SingleMovable[] linkedMovables;

		private Surface _currentSurface => objectTransform.up.ToSurface();
		private bool _inPortal;
		private Vector3Int _portalDirection;
		private Vector3Int _direction;

		public void MoveLeft()
		{
			foreach (SingleMovable movable in linkedMovables)
			{
				movable._direction = -movable.objectTransform.right.ToVector3Int();
			}
			TryMoving();
		}

		public void MoveRight()
		{
			foreach (SingleMovable movable in linkedMovables)
			{
				movable._direction = movable.objectTransform.right.ToVector3Int();
			}
			TryMoving();
		}

		public void MoveForward()
		{
			foreach (SingleMovable movable in linkedMovables)
			{
				movable._direction = movable.objectTransform.forward.ToVector3Int();
			}
			TryMoving();
		}

		public void MoveBackward()
		{
			foreach (SingleMovable movable in linkedMovables)
			{
				movable._direction = -movable.objectTransform.forward.ToVector3Int();
			}
			TryMoving();
		}

		public void MoveTo(Vector3Int coordinates)
		{
			objectTransform.position = GetWorldPosition(coordinates);
		}

		public void TeleportTo(Vector3Int coordinates)
		{
			objectTransform.position = GetWorldPosition(coordinates);
		}

		private void TryMoving()
		{
			Array.Sort(linkedMovables,
				(movable1, movable2) =>
					((movable2.RoomGridPosition - movable1.RoomGridPosition) * _direction).Sum());

			bool linkedThroughPortal = IsInPortal();

			if (!linkedThroughPortal)
			{
				ExitPortal();
			}

			if (linkedThroughPortal && _direction != _portalDirection)
			{
				return;
			}

			foreach (SingleMovable movable in linkedMovables)
			{
				if (AnyLinkedElementAt(movable.RoomGridPosition + movable._direction))
				{
					continue;
				}

				if (movable.CurrentRoom.FindPortal(movable.RoomGridPosition, (-movable._direction).ToSurface()) == null && 
				    !movable.CurrentRoom.CanMoveOnCell(movable, movable.RoomGridPosition + movable._direction, movable._currentSurface))
				{
					return;
				}
			}

			foreach (SingleMovable movable in linkedMovables)
			{
				Portal portal = movable.CurrentRoom.FindPortal(movable.RoomGridPosition, (-movable._direction).ToSurface());

				if (portal != null)
				{
					portal.Use(movable);
					movable._inPortal = true;
					foreach (SingleMovable linkedMovable in linkedMovables)
					{
						linkedMovable._portalDirection = _direction;
					}
					continue;
				}

				movable.CurrentRoom.MoveOnCell(movable, movable.RoomGridPosition + movable._direction, movable._currentSurface);
			}
		}

		private bool IsInPortal()
		{
			int inPortalCount = 0;

			foreach (SingleMovable movable in linkedMovables)
			{
				if (movable._inPortal)
				{
					inPortalCount++;
				}
			}

			return inPortalCount != 0 && inPortalCount != linkedMovables.Length;
		}

		private void ExitPortal()
		{
			foreach (SingleMovable movable in linkedMovables)
			{
				movable._inPortal = false;
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