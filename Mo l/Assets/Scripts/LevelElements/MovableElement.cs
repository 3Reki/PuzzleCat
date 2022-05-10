using System;
using PuzzleCat.Controller;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.LevelElements
{
    public class MovableElement : RoomElement, IMovable
    {
        public static OnMovement onMovement;
        public Surface CurrentSurface;

        [SerializeField] private Transform objectTransform;
        [SerializeField] private MovableElement[] linkedMovables;

        private bool _inPortal;
        private Vector3Int _portalDirection;
        private Surface _surfaceBeforePortal;
        private Vector3Int _direction;

        public bool MoveLeft()
        {
            foreach (MovableElement movable in linkedMovables)
            {
                movable._direction = CurrentSurface switch
                {
                    Surface.Floor => Vector3Int.left,
                    Surface.SideWall => Vector3Int.back,
                    Surface.BackWall => Vector3Int.left,
                    _ => throw new ArgumentOutOfRangeException(nameof(CurrentSurface), CurrentSurface, null)
                };
            }

            return TryMoving();
        }

        public bool MoveRight()
        {
            foreach (MovableElement movable in linkedMovables)
            {
                movable._direction = CurrentSurface switch
                {
                    Surface.Floor => Vector3Int.right,
                    Surface.SideWall => Vector3Int.forward,
                    Surface.BackWall => Vector3Int.right,
                    _ => throw new ArgumentOutOfRangeException(nameof(CurrentSurface), CurrentSurface, null)
                };
            }

            return TryMoving();
        }

        public bool MoveForward()
        {
            foreach (MovableElement movable in linkedMovables)
            {
                movable._direction = CurrentSurface switch
                {
                    Surface.Floor => Vector3Int.forward,
                    Surface.SideWall => Vector3Int.up,
                    Surface.BackWall => Vector3Int.up,
                    _ => throw new ArgumentOutOfRangeException(nameof(CurrentSurface), CurrentSurface, null)
                };
            }

            return TryMoving();
        }

        public bool MoveBackward()
        {
            foreach (MovableElement movable in linkedMovables)
            {
                movable._direction = CurrentSurface switch
                {
                    Surface.Floor => Vector3Int.back,
                    Surface.SideWall => Vector3Int.down,
                    Surface.BackWall => Vector3Int.down,
                    _ => throw new ArgumentOutOfRangeException(nameof(CurrentSurface), CurrentSurface, null)
                };
            }

            return TryMoving();
        }

        public void MoveTo(Vector3Int coordinates)
        {
            objectTransform.position = GetWorldPosition(coordinates);
        }

        public void TeleportTo(Vector3Int coordinates, Surface newSurface, Vector3Int exitDirection)
        {
            objectTransform.position = GetWorldPosition(coordinates);
            CurrentSurface = newSurface;
        }

        private bool CanMove()
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
                    return false;
                }
            }

            foreach (MovableElement movable in linkedMovables)
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

            foreach (MovableElement movable in linkedMovables)
            {
                if (AnyLinkedElementAt(movable.RoomGridPosition + movable._direction))
                {
                    continue;
                }

                if (movable.CurrentRoom.FindPortal(movable.RoomGridPosition, (-movable._direction).ToSurface()) ==
                    null &&
                    !movable.CurrentRoom.CanMoveOnCell(movable, movable.RoomGridPosition + movable._direction,
                        movable.CurrentSurface))
                {
                    return false;
                }

                if (GameManager.Instance.Cat.IsUnderCat(movable))
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryMoving()
        {
            if (!CanMove())
            {
                return false;
            }

            Surface currentSurfaceCopy = CurrentSurface;
            foreach (MovableElement movable in linkedMovables)
            {
                Portal portal =
                    movable.CurrentRoom.FindPortal(movable.RoomGridPosition, (-movable._direction).ToSurface());

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
                    foreach (MovableElement linkedMovable in linkedMovables)
                    {
                        linkedMovable._portalDirection = _direction;
                        _surfaceBeforePortal = currentSurfaceCopy;
                    }

                    continue;
                }

                movable.CurrentRoom.MoveOnCell(movable, movable.RoomGridPosition + movable._direction,
                    movable.CurrentSurface);
            }

            onMovement?.Invoke();

            return true;
        }

        private bool IsInPortal()
        {
            int inPortalCount = 0;

            foreach (MovableElement movable in linkedMovables)
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
            foreach (MovableElement movable in linkedMovables)
            {
                movable._inPortal = false;
            }
        }

        private bool AnyLinkedElementAt(Vector3Int position)
        {
            foreach (MovableElement linkedElement in linkedMovables)
            {
                if (linkedElement.RoomGridPosition == position)
                {
                    return true;
                }
            }

            return false;
        }

        public delegate void OnMovement();
    }
}