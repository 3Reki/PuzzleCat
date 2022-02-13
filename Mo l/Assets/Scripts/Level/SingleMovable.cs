using UnityEngine;

namespace PuzzleCat.Level
{
    public class SingleMovable : RoomElement
    {
        [SerializeField] private Transform objectTransform;

        public void MoveLeft()
        {
            Vector3Int newPosition = RoomGridPosition - Vector3Int.right;
            if (CurrentRoom.CanMoveOnCell(newPosition))
            {
                CurrentRoom.MoveOnCell(this, newPosition);
            }
        }

        public void MoveRight()
        {
            Vector3Int newPosition = RoomGridPosition + Vector3Int.right;
            if (CurrentRoom.CanMoveOnCell(newPosition))
            {
                CurrentRoom.MoveOnCell(this, newPosition);
            }
        }

        public void MoveForward()
        {
            Vector3Int newPosition = RoomGridPosition + Vector3Int.forward;
            if (CurrentRoom.CanMoveOnCell(newPosition))
            {
                CurrentRoom.MoveOnCell(this, newPosition);
            }
        }

        public void MoveBackward()
        {
            Vector3Int newPosition = RoomGridPosition - Vector3Int.forward;
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