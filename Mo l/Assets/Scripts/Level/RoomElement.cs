using UnityEngine;

namespace PuzzleCat.Level
{
    public class RoomElement : MonoBehaviour
    {
        public bool Walkable;
        public Vector3Int RoomGridPosition => CurrentRoom.WorldToRoomCoordinates(WorldGridPosition);

        protected Room CurrentRoom;

        protected virtual Vector3Int WorldGridPosition
        {
            get
            {
                Vector3 worldPosition = transform.position;
                return new Vector3Int((int) (worldPosition.x - 0.5f), (int) (worldPosition.y - 0.5f),
                    (int) (worldPosition.z - 0.5f));
            }
        }

        public void SetRoom(Room room)
        {
            CurrentRoom = room;
        }

        public virtual void Interact(SingleMovable movable)
        {
        }

        protected virtual Vector3 GetWorldPosition(Vector3Int gridCoordinates)
        {
            return new Vector3(gridCoordinates.x + 0.5f, gridCoordinates.y + 0.5f, gridCoordinates.z + 0.5f);
        }
    }
}