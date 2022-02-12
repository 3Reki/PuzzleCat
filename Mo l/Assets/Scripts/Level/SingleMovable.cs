using System;
using UnityEngine;

namespace PuzzleCat.Level
{
    public class SingleMovable : GridElement, IMovable
    {
        [SerializeField] private Transform objectTransform;
        
        private Room _currentRoom;

        public Vector3Int GetCoordinates() => GetGridCoordinates(transform.position);

        public Room GetRoom() => _currentRoom;

        public void SetRoom(Room room)
        {
            _currentRoom = room;
        }

        public void MoveLeft()
        {
            objectTransform.position -= Vector3.right;
        }

        public void MoveRight()
        {
            objectTransform.position += Vector3.right;
        }

        public void MoveForward()
        {
            objectTransform.position += Vector3.forward;
        }

        public void MoveBackward()
        {
            objectTransform.position -= Vector3.forward;
        }

        public void TeleportTo(Vector3Int coordinates)
        {
            objectTransform.position = GetWorldPosition(coordinates);
        }
    }
}