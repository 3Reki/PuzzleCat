using UnityEngine;

namespace PuzzleCat.Level
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private Vector3Int gridWorldPosition;
        [SerializeField] private Vector3Int gridSize;
        [SerializeField] private RoomElement[] roomElements;

        private RoomElement[,,] _grid;

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

            RoomElement element = _grid[coordinates.x, coordinates.y, coordinates.z];
            return element == null || element.Walkable;
        }

        public void MoveOnCell(SingleMovable movableElement, Vector3Int coordinates)
        {
            RoomElement element = _grid[coordinates.x, coordinates.y, coordinates.z];
            if (element == null)
            {
                RemoveRoomElement(movableElement);
                movableElement.MoveTo(RoomToWorldCoordinates(coordinates));
                AddRoomElement(movableElement, coordinates);
                return;
            }

            element.Interact(movableElement);
        }

        public void AddRoomElement(RoomElement element, Vector3Int position)
        {
            // TODO : check if not possible

            if (_grid[position.x, position.y, position.z] != null)
            {
                Debug.LogWarning("A Room Element got replaced", this);
            }

            _grid[position.x, position.y, position.z] = element;
        }

        public void RemoveRoomElement(RoomElement element)
        {
            RemoveRoomElementAt(element.RoomGridPosition);
        }

        private void RemoveRoomElementAt(Vector3Int position)
        {
            _grid[position.x, position.y, position.z] = null;
        }

        private void SetRoomElements()
        {
            foreach (RoomElement roomElement in roomElements)
            {
                roomElement.SetRoom(this);
            }
        }

        private void CreateRoomGrid()
        {
            _grid = new RoomElement[gridSize.x, gridSize.y, gridSize.z];

            for (int i = 1; i <= roomElements.Length; i++)
            {
                Vector3Int elementCoordinates = roomElements[^i].RoomGridPosition;
                _grid[elementCoordinates.x, elementCoordinates.y, elementCoordinates.z] = roomElements[^i];
            }
        }

        private void Awake()
        {
            SetRoomElements();
            CreateRoomGrid();
        }
    }
}