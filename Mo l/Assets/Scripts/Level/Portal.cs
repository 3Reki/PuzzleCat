using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Level
{
	public class Portal : RoomElement
	{
		public int Id;
		[HideInInspector] public bool Placed;
		
		[SerializeField] private Portal linkedPortal;
		[SerializeField] private bool catPortal;
		[SerializeField] private Vector3Int arrivalPositionOffset;

		private readonly Vector3 _offset = new(0.5f, 0.001f, 0.5f);

		private bool _active;

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
			if (!_active || Cat.IsCat(movable) != catPortal)
				return;

			Room linkedRoom = linkedPortal.CurrentRoom;
			if (!linkedRoom.CanMoveOnCell(linkedPortal.ArrivalRoomPosition()))
			{
				return;
			}

			RoomElement roomElement = movable.RoomElement;
			CurrentRoom.RemoveRoomElement(roomElement);
			roomElement.transform.rotation = linkedPortal.transform.rotation * Quaternion.Euler(-90, 0, 0);
			movable.TeleportTo(linkedPortal.ArrivalWorldPosition());
			linkedRoom.AddRoomElement(roomElement, linkedPortal.ArrivalRoomPosition());
			
			roomElement.SetRoom(linkedRoom);
		}

		public void SetPortal(Room parentRoom, Vector3Int worldGridPosition)
		{
			gameObject.SetActive(true);
			transform.position = worldGridPosition + _offset;
			SetRoom(parentRoom);
			CurrentRoom.AddRoomElement(this, RoomGridPosition);
			Placed = true;

			if (linkedPortal.Placed)
			{
				_active = true;
				linkedPortal._active = true;
			}
		}

		public void UnsetPortal()
		{
			if (catPortal)
				return;

			gameObject.SetActive(false);
			CurrentRoom.RemoveRoomElement(this);
			Placed = false;
			_active = false;
			linkedPortal._active = false;
		}

		private Vector3Int ArrivalWorldPosition() => WorldGridPosition + arrivalPositionOffset;
		private Vector3Int ArrivalRoomPosition() => RoomGridPosition + arrivalPositionOffset;

		private void Start()
		{
			if (!catPortal) return;

			_active = true;
			Placed = true;
		}
	}
}