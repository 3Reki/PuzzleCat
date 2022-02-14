using UnityEngine;

namespace PuzzleCat.Level
{
    public class Portal : RoomElement
    {
        [HideInInspector] public Room ParentRoom;
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

        public override void Interact(SingleMovable movable)
        {
            base.Interact(movable);
            CurrentRoom.RemoveRoomElement(movable);
            movable.TeleportTo(linkedPortal.ArrivalWorldPosition());
            linkedPortal.CurrentRoom.AddRoomElement(movable, linkedPortal.ArrivalRoomPosition());
            movable.SetRoom(linkedPortal.CurrentRoom);
        }

        private Vector3Int ArrivalWorldPosition() => WorldGridPosition + Vector3Int.left;
        private Vector3Int ArrivalRoomPosition() => RoomGridPosition + Vector3Int.left;
    }
}