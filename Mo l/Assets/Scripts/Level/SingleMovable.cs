using System;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Level
{
	public class SingleMovable : RoomElement, IMovable
	{
		[SerializeField] private Transform objectTransform;
		[SerializeField] private SingleMovable[] linkedMovables;
		[SerializeField] private Surface currentSurface;
		
		private bool _inPortal;
		private Vector3Int _portalDirection;
		private Vector3Int _direction;

		public void MoveLeft()
		{
			foreach (SingleMovable movable in linkedMovables)
			{
				movable._direction = currentSurface switch
				{
					Surface.Floor => Vector3Int.left,
					Surface.SideWall => Vector3Int.back,
					Surface.BackWall => Vector3Int.left,
					_ => throw new ArgumentOutOfRangeException(nameof(currentSurface), currentSurface, null)
				};
			}
			TryMoving();
		}

		public void MoveRight()
		{
			foreach (SingleMovable movable in linkedMovables)
			{
				movable._direction = currentSurface switch
				{
					Surface.Floor => Vector3Int.right,
					Surface.SideWall => Vector3Int.forward,
					Surface.BackWall => Vector3Int.right,
					_ => throw new ArgumentOutOfRangeException(nameof(currentSurface), currentSurface, null)
				};
			}
			TryMoving();
		}

		public void MoveForward()
		{
			foreach (SingleMovable movable in linkedMovables)
			{
				movable._direction = currentSurface switch
				{
					Surface.Floor => Vector3Int.forward,
					Surface.SideWall => Vector3Int.up,
					Surface.BackWall => Vector3Int.up,
					_ => throw new ArgumentOutOfRangeException(nameof(currentSurface), currentSurface, null)
				};
			}
			TryMoving();
		}

		public void MoveBackward()
		{
			foreach (SingleMovable movable in linkedMovables)
			{
				movable._direction = currentSurface switch
				{
					Surface.Floor => Vector3Int.back,
					Surface.SideWall => Vector3Int.down,
					Surface.BackWall => Vector3Int.down,
					_ => throw new ArgumentOutOfRangeException(nameof(currentSurface), currentSurface, null)
				};
			}
			TryMoving();
		}

		public void MoveTo(Vector3Int coordinates)
		{
			objectTransform.position = GetWorldPosition(coordinates);
		}

		public void TeleportTo(Vector3Int coordinates, Surface newSurface)
		{
			objectTransform.position = GetWorldPosition(coordinates);
			currentSurface = newSurface;
		}

		private void TryMoving()
		{
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
				if (movable._inPortal)
				{
					movable._direction = movable.currentSurface.GetNormal();
				}
			}
			
			Array.Sort(linkedMovables,
				(movable1, movable2) =>
					((movable2.RoomGridPosition - movable1.RoomGridPosition) * _direction).Sum());

			foreach (SingleMovable movable in linkedMovables)
			{
				if (AnyLinkedElementAt(movable.RoomGridPosition + movable._direction))
				{
					continue;
				}

				if (movable.CurrentRoom.FindPortal(movable.RoomGridPosition, (-movable._direction).ToSurface()) == null && 
				    !movable.CurrentRoom.CanMoveOnCell(movable, movable.RoomGridPosition + movable._direction, movable.currentSurface))
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

				movable.CurrentRoom.MoveOnCell(movable, movable.RoomGridPosition + movable._direction, movable.currentSurface);
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