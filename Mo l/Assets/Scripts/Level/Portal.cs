using System;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Level
{
	public class Portal : RoomElement
	{
		[SerializeField] private Portal linkedPortal;
		[SerializeField] private bool catPortal;

		private readonly Vector3 _offset = new(0.5f, 0.001f, 0.5f);

		private bool _active;
		private bool _placed;

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

			CurrentRoom.RemoveRoomElement(movable.RoomElement);
			movable.TeleportTo(linkedPortal.ArrivalWorldPosition());
			linkedPortal.CurrentRoom.AddRoomElement(movable.RoomElement, linkedPortal.ArrivalRoomPosition());
			movable.RoomElement.SetRoom(linkedPortal.CurrentRoom);
		}

		public void SetPortal(Room parentRoom, Vector3 worldPosition)
		{
			gameObject.SetActive(true);
			transform.position = UtilsClass.WorldPointAsGridPoint(worldPosition) + _offset;
			SetRoom(parentRoom);
			CurrentRoom.AddRoomElement(this, RoomGridPosition);
			_placed = true;

			if (linkedPortal._placed)
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
			_placed = false;
			_active = false;
			linkedPortal._active = false;
		}

		private Vector3Int ArrivalWorldPosition() => WorldGridPosition + Vector3Int.left;
		private Vector3Int ArrivalRoomPosition() => RoomGridPosition + Vector3Int.left;

		private void Start()
		{
			if (!catPortal) return;

			_active = true;
			_placed = true;
		}
	}
}