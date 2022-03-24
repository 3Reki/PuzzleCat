using UnityEngine;

namespace PuzzleCat.Level
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private Vector3Int gridWorldPosition;
        [SerializeField] private Vector3Int gridSize;
        [SerializeField] private RoomElement[] roomElements;

        public Vector3Int WorldToRoomCoordinates(Vector3Int worldGridCoordinates)
        {
            return worldGridCoordinates - gridWorldPosition;
        }

        public Vector3Int RoomToWorldCoordinates(Vector3Int roomGridCoordinates)
        {
            return roomGridCoordinates + gridWorldPosition;
        }

        public bool CanMoveOnCell(Vector3Int coordinates)
        {
            if (coordinates.x < 0 || coordinates.x >= gridSize.x ||
                coordinates.y < 0 || coordinates.y >= gridSize.y ||
                coordinates.z < 0 || coordinates.z >= gridSize.z)
            {
                return false;
            }

            foreach (RoomElement element in RoomElement.AllElements)
            {
                if (element.IsObstacle
                    && element.RoomGridPosition.x == coordinates.x 
                    && element.RoomGridPosition.y == coordinates.y 
                    && element.RoomGridPosition.z == coordinates.z)
                {
                    return false;
                }
            }

            return true;
        }

        public void MoveOnCell(IMovable movableElement, Vector3Int coordinates)
        {
            RoomElement element = GetElementAt(coordinates);
            if (element == null)
            {
                movableElement.MoveTo(RoomToWorldCoordinates(coordinates));
                return;
            }

            element.Interact(movableElement);
        }

        private RoomElement GetElementAt(Vector3Int coordinates)
        {
            foreach (RoomElement element in RoomElement.AllElements)
            {
                if (element.RoomGridPosition.x == coordinates.x 
                    && element.RoomGridPosition.y == coordinates.y 
                    && element.RoomGridPosition.z == coordinates.z)
                {
                    return element;
                }
            }

            return null;
        }

        private void SetRoomElements()
        {
            foreach (RoomElement roomElement in roomElements)
            {
                roomElement.SetRoom(this);
            }
        }

        private void Awake()
        {
            SetRoomElements();
        }
    }
}