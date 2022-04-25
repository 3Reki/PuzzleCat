using System;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Level
{
	public class Portal : RoomElement
	{
		public int Id;
		[HideInInspector] public bool Placed;
		public bool catPortal;
		public bool Active { get; private set; }

		[SerializeField] private Portal linkedPortal;
		[SerializeField] private Vector3Int arrivalPositionOffset;
		[SerializeField] private Transform myTransform;

		public override Vector3Int WorldGridPosition
		{
			get
			{
				Vector3 worldPosition = myTransform.position;
				return new Vector3Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y),
					Mathf.FloorToInt(worldPosition.z));
			}
		}

		public override bool CanInteract(IMovable movable)
		{
			if (!Active || !Cat.IsCat(movable) || !catPortal)
			{
				print("Can't use");
				return false;
			}
			
			if (!linkedPortal.CurrentRoom.CanMoveOnCell(movable, ArrivalRoomPosition(), ImpactedSurface))
			{
				print("Can't move");
				return false;
			}
			
			return true;
		}

		public override void Interact(IMovable movable)
		{
			if (CanInteract(movable))
			{
				Use(movable);
			}
		}

		public void Use(IMovable movable)
		{
			Room linkedRoom = linkedPortal.CurrentRoom;
			RoomElement roomElement = movable.RoomElement;

			roomElement.transform.rotation = ArrivalElementRotation(roomElement);
			movable.TeleportTo(ArrivalWorldPosition(), linkedPortal.ImpactedSurface);
			CurrentRoom.RemoveRoomElement(roomElement);
			linkedRoom.AddRoomElement(roomElement);
			roomElement.SetRoom(linkedRoom);
		}

		public void SetPortal(Room parentRoom, Vector3Int worldGridPosition, Surface surfaceType)
		{
			gameObject.SetActive(true);
			myTransform.position = worldGridPosition + GetOffset(surfaceType);
			myTransform.rotation = Quaternion.FromToRotation(myTransform.up, surfaceType.GetNormal()) *
			                      myTransform.rotation *
			                      Quaternion.Euler(90, 0, 0);
			ImpactedSurface = surfaceType;
			parentRoom.AddRoomElement(this);
			SetRoom(parentRoom);
			Placed = true;

			if (linkedPortal.Placed)
			{
				Active = true;
				linkedPortal.Active = true;
			}
		}

		public void UnsetPortal()
		{
			if (catPortal)
				return;

			CurrentRoom.RemoveRoomElement(this);
			gameObject.SetActive(false);
			Placed = false;
			Active = false;
			linkedPortal.Active = false;
		}

		private static Vector3 GetOffset(Surface surfaceType)
		{
			return surfaceType switch
			{
				Surface.Floor => new Vector3(0.5f, 0.001f, 0.5f),
				Surface.SideWall => new Vector3(0.001f, 0.5f, 0.5f),
				Surface.BackWall => new Vector3(0.5f, 0.5f, 0.999f),
				_ => throw new ArgumentOutOfRangeException(nameof(surfaceType), surfaceType, null)
			};
		}

		private Quaternion ArrivalElementRotation(RoomElement roomElement)
		{
			return Quaternion.FromToRotation(myTransform.position - roomElement.transform.position, linkedPortal.ImpactedSurface.GetNormal()) *
			       roomElement.transform.rotation;
		}

		private Vector3Int ArrivalWorldPosition() => linkedPortal.WorldGridPosition + linkedPortal.arrivalPositionOffset;
		private Vector3Int ArrivalRoomPosition() => linkedPortal.RoomGridPosition + linkedPortal.arrivalPositionOffset;

		private void Start()
		{
			if (!catPortal) return;

			Active = true;
			Placed = true;
			ImpactedSurface = (-myTransform.forward).ToSurface();
		}
	}
}