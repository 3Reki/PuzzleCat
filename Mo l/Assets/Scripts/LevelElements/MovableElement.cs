using System;
using System.Collections.Generic;
using PuzzleCat.Utils;
using PuzzleCat.Visuals;
using UnityEngine;
using PuzzleCat.Sound;

namespace PuzzleCat.LevelElements
{
    public class MovableElement : RoomElement
    {
        public static OnMovement onMovement;
        public static MovableElementDirectionIndicator DirectionIndicator;
        public Surface CurrentSurface;
        public bool InPortal { get; private set; }

        [SerializeField] private Transform objectTransform;
        [SerializeField] private MovableElement[] linkedMovables;
        
        private bool _onPortal;
        private Portal _steppedOnPortal;
        private Vector3Int _inPortalDirection;
        private Vector3Int _outPortalDirection;
        private Surface _surfaceBeforePortal;
        private Vector3Int _direction;

        public bool MoveLeft()
        {
            SetMovableDirections(Vector3Int.left, Vector3Int.back, Vector3Int.left);

            return TryMoving();
        }

        public bool MoveRight()
        {
            SetMovableDirections(Vector3Int.right, Vector3Int.forward, Vector3Int.right);

            return TryMoving();
        }

        public bool MoveForward()
        {
            SetMovableDirections(Vector3Int.forward, Vector3Int.up, Vector3Int.up);

            return TryMoving();
        }

        public bool MoveBackward()
        {
            SetMovableDirections(Vector3Int.back, Vector3Int.down, Vector3Int.down);

            return TryMoving();
        }

        public override void MoveTo(Vector3Int destination)
        {
            objectTransform.position = GetWorldPosition(destination);
            AudioManager.Instance.Play("MoveFurniture");
        }

        public void TeleportTo(Vector3Int coordinates, Surface newSurface, Vector3Int exitDirection)
        {
            objectTransform.position = GetWorldPosition(coordinates);
            CurrentSurface = newSurface;
        }

        public void PositionIndicator()
        {
            var onGround = new List<MovableElement>();
            foreach (MovableElement linkedMovable in linkedMovables)
            {
                if (!linkedMovable.InPortal &&
                    CurrentRoom.IsInContact(linkedMovable.RoomGridPosition, linkedMovable.CurrentSurface))
                {
                    onGround.Add(linkedMovable);
                }
            }

            if (onGround.Count == 0)
            {
                DirectionIndicator.SetAllIndicatorsActive(false);
                return;
            }
            
            DirectionIndicator.SetAllIndicatorsActive(true);

            var (movableElement1, movableElement2) = FindFarthestElements(onGround);
            
            float distX = (movableElement2.objectTransform.position - movableElement1.objectTransform.position)
                    .ApplyMask(movableElement1.CurrentSurface.Up());
            float distY = (movableElement2.objectTransform.position - movableElement1.objectTransform.position)
                    .ApplyMask(movableElement1.CurrentSurface.Right());
            
            distX = Mathf.Abs(Mathf.Round(distX));
            distY = Mathf.Abs(Mathf.Round(distY));

            DirectionIndicator.SetTransform(
                (movableElement1.GroundedPosition() + movableElement2.GroundedPosition()) / 2, 
                Quaternion.LookRotation(movableElement1.CurrentSurface.GetNormal()), 
                new Vector3(distX + 1, distY + 4, 1), new Vector3(distY + 1, distX + 4, 1));
        }
        
        public void Select()
        {
            if (IsInPortal())
            {
                CurrentRoom.SetSurfaceIndicatorActive(_outPortalDirection.ToSurface(), true);
            }
        }
        
        public void Deselect()
        {
            if (IsOutOfPortal())
            {
                ExitPortal();
            }
            
            CurrentRoom.SetAllSurfaceIndicatorActive(false);
        }

        public bool IsUnderCat()
        {
            foreach (MovableElement movable in linkedMovables)
            {
                if (GameManager.Instance.Cat.IsUnderCat(movable))
                {
                    return true;
                }
            }

            return false;
        }
        
        public bool AnyLinkedElementAt(Vector3Int position)
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
        

        private static (MovableElement, MovableElement) FindFarthestElements(List<MovableElement> movableElements)
        {
            float maxDistance = 0;
            (MovableElement, MovableElement) elements = (movableElements[0], movableElements[0]);
            for (var i = 0; i < movableElements.Count; i++)
            {
                for (var j = 1; j < movableElements.Count; j++)
                {
                    if (!((movableElements[i].objectTransform.position - movableElements[j].objectTransform.position)
                        .magnitude > maxDistance)) continue;
                    
                    elements = (movableElements[i], movableElements[j]);
                    maxDistance = (movableElements[i].objectTransform.position -
                                   movableElements[j].objectTransform.position).magnitude;
                }
            }

            return elements;
        }

        private Vector3 GroundedPosition()
        {
            return CurrentSurface switch
            {
                Surface.Floor => WorldGridPosition + new Vector3(0.5f, 0.01f, 0.5f),
                Surface.SideWall => WorldGridPosition + new Vector3(0.01f, 0.5f, 0.5f),
                Surface.BackWall => WorldGridPosition + new Vector3(0.5f, 0.5f, 0.99f),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void SetMovableDirections(Vector3Int floorDefault, Vector3Int sideDefault, Vector3Int backDefault)
        {
            foreach (MovableElement movable in linkedMovables)
            {
                if (!movable.InPortal)
                {
                    movable._direction = movable.CurrentSurface switch
                    {
                        Surface.Floor => floorDefault,
                        Surface.SideWall => sideDefault,
                        Surface.BackWall => backDefault,
                        _ => throw new ArgumentOutOfRangeException(nameof(CurrentSurface), CurrentSurface, null)
                    };
                    continue;
                }
                
                movable._direction = _surfaceBeforePortal switch
                {
                    Surface.Floor => floorDefault,
                    Surface.SideWall => sideDefault,
                    Surface.BackWall => backDefault,
                    _ => throw new ArgumentOutOfRangeException(nameof(CurrentSurface), CurrentSurface, null)
                };

                if (movable._direction == _inPortalDirection)
                {
                    movable._direction = _outPortalDirection;
                }
                else if (movable._direction == -_inPortalDirection)
                {
                    movable._direction = -_outPortalDirection;
                }
                else
                {
                    movable._direction = Vector3Int.zero;
                }
            }
        }

        private bool CanMove()
        {
            bool onPortal = IsOnPortal();

            if (IsOutOfPortal() && _direction != -_outPortalDirection) // TODO return to previous surface
            {
                return false;
            }

            foreach (MovableElement movable in linkedMovables)
            {
                if (movable.InPortal && movable._direction != _outPortalDirection && movable._direction != -_outPortalDirection)
                {
                    return false;
                }
            }

            Array.Sort(linkedMovables,
                (movable1, movable2) =>
                    ((movable2.RoomGridPosition - movable1.RoomGridPosition) * _direction).Sum()); // todo remove ?

            Portal currentPortal = null;

            foreach (MovableElement movable in linkedMovables)
            {
                if (GameManager.Instance.Cat.IsUnderCat(movable))
                {
                    return false;
                }
                
                if (AnyLinkedElementAt(movable.RoomGridPosition + movable._direction))
                {
                    continue;
                }

                Portal portal = 
                    movable.CurrentRoom.FindPortal(movable.RoomGridPosition, (-movable._direction).ToSurface());
                Portal sameSurfacePortal = movable.CurrentRoom.FindPortal(movable.RoomGridPosition + movable._direction,
                    movable.CurrentSurface);

                if (portal != null && portal.Active && !portal.CatPortal)
                {
                    if (!portal.CanMovableInteract(movable)) 
                        return false;
                    if (currentPortal != null && !portal.IsConnectedTo(currentPortal))
                        return false;
                    
                    currentPortal = portal;
                    continue;
                }

                if (onPortal && sameSurfacePortal != null && sameSurfacePortal.IsConnectedTo(movable._steppedOnPortal)) 
                    continue;
                
                if (!movable.CurrentRoom.CanMoveOnCell(movable, movable.RoomGridPosition + movable._direction, movable.CurrentSurface))
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
                DirectionIndicator.SetIncorrectColorFade();
                return false;
            }

            bool onPortal = IsOnPortal();

            Surface beforePortal = _surfaceBeforePortal != Surface.None ? _surfaceBeforePortal : CurrentSurface;

            foreach (MovableElement movable in linkedMovables)
            {
                movable.PrepareMovement(beforePortal, onPortal,
                    movable.CurrentRoom.FindPortal(movable.RoomGridPosition, (-movable._direction).ToSurface()),
                    movable.CurrentRoom.FindPortal(movable.RoomGridPosition + movable._direction, movable.CurrentSurface));
            }

            onMovement?.Invoke();

            return true;
        }

        private void PrepareMovement(Surface beforePortalSurface, bool onPortal, Portal portal, Portal sameSurfacePortal)
        {
            if (portal != null)
            {
                portal.Interact(this);

                if (InPortal)
                {
                    InPortal = false;
                    _onPortal = false;
                    CurrentSurface = _surfaceBeforePortal;
                    if (!IsInPortal())
                    {
                        CurrentRoom.SetSurfaceIndicatorActive(portal.ImpactedSurface, false);
                    }
                    return;
                }

                InPortal = true;
                _onPortal = true;
                _steppedOnPortal = CurrentRoom.FindPortal(RoomGridPosition, CurrentSurface);
                CurrentRoom.SetSurfaceIndicatorActive(portal.ArrivalSurface, true);
                foreach (MovableElement linkedMovable in linkedMovables)
                {
                    linkedMovable._inPortalDirection = -portal.ImpactedSurface.GetNormal();
                    linkedMovable._outPortalDirection = portal.ArrivalSurface.GetNormal();
                    linkedMovable._surfaceBeforePortal = beforePortalSurface;
                }

                return;
            }

            if (onPortal)
            {
                if (sameSurfacePortal != null && sameSurfacePortal.IsConnectedTo(_steppedOnPortal))
                {
                    _onPortal = true;
                    MoveTo(CurrentRoom.RoomToWorldCoordinates(RoomGridPosition + _direction));
                    return;
                }

                _onPortal = false;
            }

            CurrentRoom.MoveOnCell(this, RoomGridPosition + _direction, CurrentSurface);
        }

        private bool IsOnPortal()
        {
            foreach (MovableElement movable in linkedMovables)
            {
                if (movable._onPortal)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsInPortal()
        {
            foreach (MovableElement movable in linkedMovables)
            {
                if (movable.InPortal)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool IsOutOfPortal()
        {
            foreach (MovableElement movable in linkedMovables)
            {
                if (!movable.InPortal)
                {
                    return false;
                }
            }

            return true;
        }

        private void ExitPortal()
        {
            foreach (MovableElement movable in linkedMovables)
            {
                movable.InPortal = false;
            }
        }

        public delegate void OnMovement();
    }
}