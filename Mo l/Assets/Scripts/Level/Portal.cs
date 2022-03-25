using System;
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

		private bool _active;
		private Transform _transform;

		protected override Vector3Int WorldGridPosition
		{
			get
			{
				Vector3 worldPosition = _transform.position;
				return new Vector3Int((int) (worldPosition.x - 0.5f), Mathf.FloorToInt(worldPosition.y),
					(int) (worldPosition.z - 0.5f));
			}
		}

		public override void Interact(IMovable movable)
		{
			if (!_active || Cat.IsCat(movable) != catPortal)
			{
				print("Can't use");
				return;
			}

			Room linkedRoom = linkedPortal.CurrentRoom;
			if (!linkedRoom.CanMoveOnCell(linkedPortal.ArrivalRoomPosition()))
			{
				print("Can't move");
				return;
			}

			RoomElement roomElement = movable.RoomElement;

			roomElement.transform.rotation = linkedPortal.ArrivalElementRotation();
			movable.TeleportTo(linkedPortal.ArrivalWorldPosition());
			CurrentRoom.RemoveRoomElement(roomElement);
			linkedRoom.AddRoomElement(roomElement);
			roomElement.SetRoom(linkedRoom);
		}

		public void SetPortal(Room parentRoom, Vector3Int worldGridPosition, Surface surfaceType)
		{
			gameObject.SetActive(true);
			_transform.position = worldGridPosition + GetOffset(surfaceType);
			_transform.rotation = Quaternion.FromToRotation(_transform.up, surfaceType.GetNormal()) *
			                      _transform.rotation *
			                      Quaternion.Euler(90, 0, 0);
			parentRoom.AddRoomElement(this);
			SetRoom(parentRoom);
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

			CurrentRoom.RemoveRoomElement(this);
			gameObject.SetActive(false);
			Placed = false;
			_active = false;
			linkedPortal._active = false;
		}

		private Vector3 GetOffset(Surface surfaceType)
		{
			return surfaceType switch
			{
				Surface.Floor => new Vector3(0.5f, 0.001f, 0.5f),
				Surface.SideWall => new Vector3(0.001f, 0.5f, 0.5f),
				Surface.BackWall => new Vector3(0.5f, 0.5f, 0.999f),
				_ => throw new ArgumentOutOfRangeException(nameof(surfaceType), surfaceType, null)
			};
		}

		private Quaternion ArrivalElementRotation()
		{
			Surface currentSurface = (-_transform.forward).ToSurface();

			return currentSurface switch
			{
				Surface.Floor => Quaternion.identity,
				Surface.SideWall => Quaternion.Euler(-90, -90, 0),
				Surface.BackWall => Quaternion.Euler(-90, 0, 0),
				_ => throw new ArgumentOutOfRangeException(nameof(currentSurface), currentSurface, null)
			};
		}

		private Vector3Int ArrivalWorldPosition() => WorldGridPosition + arrivalPositionOffset;
		private Vector3Int ArrivalRoomPosition() => RoomGridPosition + arrivalPositionOffset;

		private void Awake()
		{
			_transform = transform;
		}

		private void Start()
		{
			if (!catPortal) return;

			_active = true;
			Placed = true;
		}
	}
}