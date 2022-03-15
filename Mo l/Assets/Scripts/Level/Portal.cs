using UnityEngine;

namespace PuzzleCat.Level
{
    public class Portal : RoomElement
    {
        [SerializeField] private Portal linkedPortal;

        protected override Vector3Int WorldGridPosition
        {
            get
            {
                Vector3 worldPosition = transform.position;
                return new Vector3Int((int) (worldPosition.x - 0.5f), Mathf.FloorToInt(worldPosition.y),
                    (int) (worldPosition.z - 0.5f));
            }
        }

        public override void Interact(IMovable movable)
        {
            CurrentRoom.RemoveRoomElement(movable.RoomElement);
            movable.TeleportTo(linkedPortal.ArrivalWorldPosition());
            linkedPortal.CurrentRoom.AddRoomElement(movable.RoomElement, linkedPortal.ArrivalRoomPosition());
            movable.RoomElement.SetRoom(linkedPortal.CurrentRoom);
        }

        private Vector3Int ArrivalWorldPosition() => WorldGridPosition + Vector3Int.left;
        private Vector3Int ArrivalRoomPosition() => RoomGridPosition + Vector3Int.left;
    }
}