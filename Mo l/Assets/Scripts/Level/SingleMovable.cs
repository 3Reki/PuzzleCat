using System;
using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.Level
{
	public class SingleMovable : RoomElement, IMovable
	{
		public Surface CurrentSurface;
		
		[SerializeField] private Transform objectTransform;
		[SerializeField] private SingleMovable[] linkedMovables;

		private bool _inPortal;
		private Vector3Int _portalDirection;
		private Surface _surfaceBeforePortal;
		private Vector3Int _direction;

		public void MoveLeft(Cat cat)
		{
			foreach (SingleMovable movable in linkedMovables)
			{
				movable._direction = CurrentSurface switch
				{
					Surface.Floor => Vector3Int.left,
					Surface.SideWall => Vector3Int.back,
					Surface.BackWall => Vector3Int.left,
					_ => throw new ArgumentOutOfRangeException(nameof(CurrentSurface), CurrentSurface, null)
				};
			}
			TryMoving(cat);
		}

		public void MoveRight(Cat cat)
		{
			foreach (SingleMovable movable in linkedMovables)
			{
				movable._direction = CurrentSurface switch
				{
					Surface.Floor => Vector3Int.right,
					Surface.SideWall => Vector3Int.forward,
					Surface.BackWall => Vector3Int.right,
					_ => throw new ArgumentOutOfRangeException(nameof(CurrentSurface), CurrentSurface, null)
				};
			}
			TryMoving(cat);
		}

		public void MoveForward(Cat cat)
		{
			foreach (SingleMovable movable in linkedMovables)
			{
				movable._direction = CurrentSurface switch
				{
					Surface.Floor => Vector3Int.forward,
					Surface.SideWall => Vector3Int.up,
					Surface.BackWall => Vector3Int.up,
					_ => throw new ArgumentOutOfRangeException(nameof(CurrentSurface), CurrentSurface, null)
				};
			}
			TryMoving(cat);
		}

		public void MoveBackward(Cat cat)
		{
			foreach (SingleMovable movable in linkedMovables)
			{
				movable._direction = CurrentSurface switch
				{
					Surface.Floor => Vector3Int.back,
					Surface.SideWall => Vector3Int.down,
					Surface.BackWall => Vector3Int.down,
					_ => throw new ArgumentOutOfRangeException(nameof(CurrentSurface), CurrentSurface, null)
				};
			}
			TryMoving(cat);
		}

		public void MoveTo(Vector3Int coordinates)
		{
			objectTransform.position = GetWorldPosition(coordinates);
			foreach (NavMeshSurface navMeshSurface in InputManager.Surfaces)
			{
				navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
			}
		}

		public void TeleportTo(Vector3Int coordinates, Surface newSurface, Vector3Int exitDirection)
		{
			objectTransform.position = GetWorldPosition(coordinates);
			CurrentSurface = newSurface;
		}

		private void TryMoving(Cat cat)
		{
			bool linkedThroughPortal = IsInPortal();

			if (!linkedThroughPortal)
			{
				ExitPortal();
			}
			else
			{
				if (_direction != _portalDirection && _direction != -_portalDirection)
				{
					return;
				}
			}

			foreach (SingleMovable movable in linkedMovables)
			{
				if (movable._inPortal)
				{
					movable._direction = movable.CurrentSurface.GetNormal();

					if (_direction == -_portalDirection)
					{
						movable._direction *= -1;
					}
				}
			}
			
			Array.Sort(linkedMovables,
				(movable1, movable2) =>
					((movable2.RoomGridPosition - movable1.RoomGridPosition) * _direction).Sum());

			Vector3Int underCatPosition = cat.RoomGridPosition - cat.transform.up.ToVector3Int();
			
			foreach (SingleMovable movable in linkedMovables)
			{
				if (AnyLinkedElementAt(movable.RoomGridPosition + movable._direction))
				{
					continue;
				}

				if (movable.CurrentRoom.FindPortal(movable.RoomGridPosition, (-movable._direction).ToSurface()) == null &&
					!movable.CurrentRoom.CanMoveOnCell(movable, movable.RoomGridPosition + movable._direction,
						movable.CurrentSurface))
				{
					return;
				}

				if (cat.CurrentRoom == CurrentRoom && underCatPosition == movable.RoomGridPosition)
				{
					return;
				}
			}

			Surface currentSurfaceCopy = CurrentSurface;
			foreach (SingleMovable movable in linkedMovables)
			{
				Portal portal = movable.CurrentRoom.FindPortal(movable.RoomGridPosition, (-movable._direction).ToSurface());

				if (portal != null)
				{
					portal.Use(movable);
					
					if (movable._inPortal)
					{
						movable._inPortal = false;
						movable.CurrentSurface = _surfaceBeforePortal;
						continue;
					}
					
					movable._inPortal = true;
					foreach (SingleMovable linkedMovable in linkedMovables)
					{
						linkedMovable._portalDirection = _direction;
						_surfaceBeforePortal = currentSurfaceCopy;
					}
					continue;
				}

				movable.CurrentRoom.MoveOnCell(movable, movable.RoomGridPosition + movable._direction, movable.CurrentSurface);
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