using System;
using System.Collections.Generic;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.LevelElements
{
	public class Portal : RoomElement
	{
		public int Id;
		[HideInInspector] public bool Placed;
		public bool CatPortal;
		public bool Active { get; private set; }
		public bool GreyPortal;

		[SerializeField] private Portal defaultLinkedPortal;
		[SerializeField] private Vector3Int arrivalPositionOffset;
		[SerializeField] private Transform myTransform;

		private static bool _canLink;
		private readonly Portal[] _adjacentPortals = new Portal[4]; // 4 directions : up, right, down and left
		private Portal _linkedPortal;
		private float _rotationOffset;

		public override Vector3Int WorldGridPosition
		{
			get
			{
				Vector3 worldPosition = myTransform.position;
				return new Vector3Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y),
					Mathf.FloorToInt(worldPosition.z));
			}
		}

		public override bool CanInteract(RoomElement movable)
		{
			if (!Active || !Cat.IsCat(movable) || !CatPortal)
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

		public override void Interact(RoomElement movable)
		{
			Room linkedRoom = _linkedPortal.CurrentRoom;
			
			if (CatPortal)
			{
				((Cat) movable).MoveTo(myTransform.position, 1);
				((Cat) movable).onArrival = () => ((Cat) movable).TeleportTo(ArrivalWorldPosition(), _linkedPortal.ImpactedSurface, _linkedPortal.arrivalPositionOffset);
			}
			else
			{
				movable.transform.rotation = ArrivalElementAddedRotation() * movable.transform.rotation;
				((MovableElement) movable).TeleportTo(ArrivalWorldPosition(), _linkedPortal.ImpactedSurface, ImpactedSurface.GetNormal());
			}
			CurrentRoom.RemoveRoomElement(movable);
			linkedRoom.AddRoomElement(movable);
			movable.SetRoom(linkedRoom);
		}

		public bool IsConnectedTo(Portal portal)
		{
			var checkedPortals = new HashSet<Portal>();
			return IsConnectedTo(ref checkedPortals, p => p == portal);
		}

		#region SetAndLink

		public bool CanSetPortal(Transform hit, Vector3Int worldGridPosition, Surface surfaceType)
		{
			if (hit.GetComponent<RoomElement>() != null)
			{
				return false;
			}

			if (!GreyPortal)
			{
				return true;
			}

			Room parentRoom = hit.parent.GetComponent<Room>();
			Vector3Int roomGridPosition = parentRoom.WorldToRoomCoordinates(worldGridPosition);

			Vector3Int[] directionVectors = Utils.Utils.GetDirectionVectors(surfaceType);

			for (var i = 0; i < directionVectors.Length; i++)
			{
				if (parentRoom.FindPortal(roomGridPosition + directionVectors[i], surfaceType) != null)
				{
					return true;
				}
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

			if (GreyPortal)
			{
				Vector3Int roomGridPosition = parentRoom.WorldToRoomCoordinates(worldGridPosition);
				Vector3Int[] directionVectors = Utils.Utils.GetDirectionVectors(surfaceType);

				for (var i = 0; i < directionVectors.Length; i++)
				{
					_adjacentPortals[i] = parentRoom.FindPortal(roomGridPosition + directionVectors[i], surfaceType);

					if (_adjacentPortals[i] != null)
					{
						_adjacentPortals[i]._adjacentPortals[(i + 2) % 4] = this;
					}
				}

				TryLinkingPortals();

				return;
			}

			if (!_linkedPortal.Placed) return;
			
			foreach (Portal portal in _linkedPortal._adjacentPortals)
			{
				if (portal != null)
				{
					return;
				}
			}
			
			print("Linked");
			Active = true;
			_rotationOffset = 0;
			_linkedPortal.Active = true;
			_linkedPortal._rotationOffset = 0;
		}

		public void UnsetPortal()
		{
			if (CatPortal)
				return;
			
			CurrentRoom.RemoveRoomElement(this);
			gameObject.SetActive(false);
			Placed = false;
			Active = false;

			for (var i = 0; i < _adjacentPortals.Length; i++)
			{
				if (_adjacentPortals[i] != null)
				{
					_adjacentPortals[i]._adjacentPortals[(i + 2) % 4] = null;
				}
			}

			var checkedPortals = new HashSet<Portal>();
			for (var i = 0; i < _adjacentPortals.Length; i++)
			{
				if (_adjacentPortals[i] != null)
				{
					checkedPortals.Clear();
					if (_adjacentPortals[i].IsConnectedTo(ref checkedPortals, portal => !portal.GreyPortal))
					{
						_adjacentPortals[i].TryLinkingPortals();
					}
					else
					{
						_adjacentPortals[i].UnsetWithoutVerification();
					}
					_adjacentPortals[i] = null;
				}
			}
		}



		private void UnsetWithoutVerification()
		{
			CurrentRoom.RemoveRoomElement(this);
			gameObject.SetActive(false);
			Placed = false;
			Active = false;

			for (var i = 0; i < _adjacentPortals.Length; i++)
			{
				if (_adjacentPortals[i] != null)
				{
					_adjacentPortals[i]._adjacentPortals[(i + 2) % 4] = null;
				}
			}
			
			for (var i = 0; i < _adjacentPortals.Length; i++)
			{
				if (_adjacentPortals[i] != null)
				{
					_adjacentPortals[i].UnsetWithoutVerification();
					_adjacentPortals[i] = null;
				}
			}
		}
		
		private void TryLinkingPortals()
		{
			var allAdjacentPortals = new HashSet<Portal>();
			Portal defaultLinked = null;
			GetAllAdjacentPortals(ref allAdjacentPortals, ref defaultLinked);

			var checkedPortals = new HashSet<Portal>();
			for (int i = 0; i < 4; i++)
			{
				foreach (Portal adjacentPortal in allAdjacentPortals)
				{
					checkedPortals.Clear();
					if (CanLinkPortals(ref checkedPortals, adjacentPortal, defaultLinked, i))
					{
						print("Linked");
						checkedPortals.Clear();
						LinkPortals(ref checkedPortals, adjacentPortal, defaultLinked, i);
						return;
					}
				}
			}

			checkedPortals.Clear();
			Unlink(ref checkedPortals);
			checkedPortals.Clear();
			defaultLinked.Unlink(ref checkedPortals);
		}

		private void Unlink(ref HashSet<Portal> checkedPortals)
		{
			if (!checkedPortals.Add(this))
			{
				return;
			}
			
			Active = false;
			
			foreach (Portal adjacentPortal in _adjacentPortals)
			{
				if (adjacentPortal != null)
				{
					adjacentPortal.Unlink(ref checkedPortals);
				}
			}
		}

		private static void LinkPortals(ref HashSet<Portal> checkedPortals, Portal portal1, Portal portal2, int rotationOffset)
		{
			if (!checkedPortals.Add(portal1))
			{
				return;
			}
			
			portal1._linkedPortal = portal2;
			portal1.Active = true;
			portal1._rotationOffset = -rotationOffset * 90;
			portal2._linkedPortal = portal1;
			portal2.Active = true;
			portal2._rotationOffset = -rotationOffset * 90;

			for (int i = 0; i < 4; i++)
			{
				if (portal1._adjacentPortals[(i + rotationOffset) % 4] != null)
				{
					LinkPortals(ref checkedPortals, portal1._adjacentPortals[(i + rotationOffset) % 4],
						portal2._adjacentPortals[(i % 2 == 1) ? (i + 2) % 4 : i], rotationOffset);
				}
			}
		}

		private static bool CanLinkPortals(ref HashSet<Portal> checkedPortals, Portal portal1, Portal portal2, int rotationOffset)
		{
			if (!checkedPortals.Add(portal1))
			{
				return true;
			}

			for (int i = 0; i < 4; i++)
			{
				if ((portal1._adjacentPortals[(i + rotationOffset) % 4] == null) != (portal2._adjacentPortals[
					(i % 2 == 1) ? (i + 2) % 4 : i] == null))
				{
					return false;
				}

				if (portal1._adjacentPortals[(i + rotationOffset) % 4] == null)
				{
					continue;
				}

				_canLink = CanLinkPortals(ref checkedPortals, portal1._adjacentPortals[(i + rotationOffset) % 4],
					portal2._adjacentPortals[(i % 2 == 1) ? (i + 2) % 4 : i], rotationOffset);

				if (!_canLink)
				{
					return false;
				}
			}

			return true;
		}
		
		#endregion

		private void GetAllAdjacentPortals(ref HashSet<Portal> portals, ref Portal defaultLinked)
		{
			if (!portals.Add(this))
			{
				return;
			}

			if (defaultLinkedPortal != null)
			{
				defaultLinked = defaultLinkedPortal;
			}

			foreach (Portal adjacentPortal in _adjacentPortals)
			{
				if (adjacentPortal != null)
				{
					adjacentPortal.GetAllAdjacentPortals(ref portals, ref defaultLinked);
				}
			}
		}

		private bool IsConnectedTo(ref HashSet<Portal> portals, Func<Portal, bool> predicate)
		{
			if (!portals.Add(this))
			{
				return false;
			}

			if (predicate.Invoke(this))
			{
				return true;
			}

			foreach (Portal adjacentPortal in _adjacentPortals)
			{
				if (adjacentPortal == null) continue;
				
				if (adjacentPortal.IsConnectedTo(ref portals, predicate))
				{
					return true;
				}
			}

			return false;
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

		private Quaternion ArrivalElementAddedRotation()
		{
			return (ImpactedSurface, _linkedPortal.ImpactedSurface) switch
			{
				(Surface.Floor, Surface.Floor) => Quaternion.Euler(180, -_rotationOffset + 180, 0),
				(Surface.Floor, Surface.BackWall) => Quaternion.Euler(0, 0, _rotationOffset + 180) * Quaternion.Euler(-270, 0, 0),
				(Surface.Floor, Surface.SideWall) => Quaternion.Euler(-(_rotationOffset + 90), 0, -270),
				(Surface.BackWall, Surface.Floor) => Quaternion.Euler(270, -_rotationOffset + 180, 0),
				(Surface.BackWall, Surface.BackWall) => Quaternion.Euler(180, 0, -_rotationOffset + 180),
				(Surface.BackWall, Surface.SideWall) => Quaternion.Euler(-_rotationOffset, 0, 0) * Quaternion.Euler(0, -270, 0),
				(Surface.SideWall, Surface.Floor) => Quaternion.Euler(0, -(_rotationOffset + 90), 270),
				(Surface.SideWall, Surface.BackWall) => Quaternion.Euler(_rotationOffset, 270, 0),
				(Surface.SideWall, Surface.SideWall) => Quaternion.Euler(0, 0, 180) * Quaternion.Euler(_rotationOffset + 180, 0, 0),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		private Vector3Int ArrivalWorldPosition() => _linkedPortal.WorldGridPosition + _linkedPortal.arrivalPositionOffset;
		private Vector3Int ArrivalRoomPosition() => _linkedPortal.RoomGridPosition + _linkedPortal.arrivalPositionOffset;

		private void Awake()
		{
			_linkedPortal = defaultLinkedPortal;
		}

		private void Start()
		{
			if (!CatPortal) return;

			Active = true;
			Placed = true;
		}
	}
}