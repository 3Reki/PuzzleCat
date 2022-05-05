using System;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.LevelElements
{
	public class Portal : RoomElement
	{
		public int Id;
		[HideInInspector] public bool Placed;
		public bool catPortal;
		public bool Active { get; private set; }

		[SerializeField] private Portal defaultLinkedPortal;
		[SerializeField] private Vector3Int arrivalPositionOffset;
		[SerializeField] private Transform myTransform;
		[SerializeField] private bool isGreyPortal;
		
		private Portal _linkedPortal;
		private Portal _adjacentPortal;
		private Direction _adjacentDirection;

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
			
			if (!_linkedPortal.CurrentRoom.CanMoveOnCell(movable, ArrivalRoomPosition(), ImpactedSurface))
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
			Room linkedRoom = _linkedPortal.CurrentRoom;
			RoomElement roomElement = movable.RoomElement;

			roomElement.transform.rotation = ArrivalElementRotation(roomElement);
			if (catPortal)
			{
				((Cat) movable).MoveTo(myTransform.position + (movable.RoomElement.transform.position - myTransform.position).normalized);
				((Cat) movable).onArrival = () => movable.TeleportTo(ArrivalWorldPosition(), _linkedPortal.ImpactedSurface, _linkedPortal.arrivalPositionOffset);
			}
			else
			{
				movable.TeleportTo(ArrivalWorldPosition(), _linkedPortal.ImpactedSurface, ImpactedSurface.GetNormal());
			}
			CurrentRoom.RemoveRoomElement(roomElement);
			linkedRoom.AddRoomElement(roomElement);
			roomElement.SetRoom(linkedRoom);
		}
		
		public bool CanSetPortal(Transform hit, Vector3Int worldGridPosition, Surface surfaceType)
		{
			if (hit.GetComponent<RoomElement>() != null)
			{
				return false;
			}

			if (!isGreyPortal)
			{
				return true;
			}

			Room parentRoom = hit.parent.GetComponent<Room>();
			Vector3Int roomGridPosition = parentRoom.WorldToRoomCoordinates(worldGridPosition);

			Vector3Int[] directionVectors =
			{
				hit.up.ToVector3Int(), hit.right.ToVector3Int(), (-hit.up).ToVector3Int(),
				(-hit.right).ToVector3Int()
			};
			Direction[] directions =
			{
				Direction.Up, Direction.Right, Direction.Down, Direction.Left
			};

			for (var i = 0; i < directionVectors.Length; i++)
			{
				_adjacentPortal = parentRoom.FindPortal(roomGridPosition + directionVectors[i], surfaceType);

				if (_adjacentPortal == null)
				{
					continue;
				}

				_adjacentPortal._adjacentPortal = this;
				_adjacentDirection = directions[i];

				return true;
			}

			return false;
		}

		public void SetPortal(Room parentRoom, Vector3Int worldGridPosition, Surface surfaceType)
		{
			_linkedPortal = defaultLinkedPortal;
			gameObject.SetActive(true);
			myTransform.position = worldGridPosition + GetOffset(surfaceType);
			myTransform.rotation = Quaternion.FromToRotation(myTransform.up, surfaceType.GetNormal()) *
			                      myTransform.rotation *
			                      Quaternion.Euler(90, 0, 0);
			ImpactedSurface = surfaceType;
			parentRoom.AddRoomElement(this);
			SetRoom(parentRoom);
			Placed = true;

			if (isGreyPortal)
			{
				_linkedPortal = _adjacentPortal._linkedPortal._adjacentPortal;
				if (_linkedPortal == null)
				{
					_adjacentPortal.Active = false;
					_adjacentPortal._linkedPortal.Active = false;
				}
				else
				{
					if (_adjacentDirection == _linkedPortal._adjacentDirection)
					{
						_linkedPortal = _adjacentPortal._linkedPortal;
						_adjacentPortal._linkedPortal = _linkedPortal._adjacentPortal;
						_linkedPortal._linkedPortal = this;
						_adjacentPortal._linkedPortal._linkedPortal = _adjacentPortal;
					}

					_linkedPortal._linkedPortal = this;
					Active = true;
					_linkedPortal.Active = true;
					_adjacentPortal.Active = true;
					_adjacentPortal._linkedPortal.Active = true;
				}

				return;
			}

			if (_linkedPortal.Placed && _linkedPortal._adjacentPortal == null)
			{
				Active = true;
				_linkedPortal.Active = true;
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
			
			if (isGreyPortal)
			{
				if (_linkedPortal != null)
				{
					_linkedPortal.Active = false;
				}

				_adjacentPortal.Active = false;
				_adjacentPortal._linkedPortal.Active = false;
				_adjacentPortal._adjacentPortal = null;

				return;
			}

			if (_adjacentPortal != null)
			{
				_adjacentPortal.UnsetPortal();
				return;
			}

			_linkedPortal.Active = false;
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
			return Quaternion.FromToRotation(myTransform.position - roomElement.transform.position, _linkedPortal.ImpactedSurface.GetNormal()) *
			       roomElement.transform.rotation;
		}

		private Vector3Int ArrivalWorldPosition() => _linkedPortal.WorldGridPosition + _linkedPortal.arrivalPositionOffset;
		private Vector3Int ArrivalRoomPosition() => _linkedPortal.RoomGridPosition + _linkedPortal.arrivalPositionOffset;

		private void Awake()
		{
			_linkedPortal = defaultLinkedPortal;
		}

		private void Start()
		{
			if (!catPortal) return;

			Active = true;
			Placed = true;
		}
	}
}