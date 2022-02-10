using System.Collections.Generic;
using UnityEngine;

namespace PuzzleCat.Level
{
	public class Room : MonoBehaviour
	{
		[SerializeField] private int maxLeft;
		[SerializeField] private int maxRight;
		[SerializeField] private int maxForward;
		[SerializeField] private int maxBackward;
		[SerializeField] private GameObject[] movableGameObjects;
		[SerializeField] private List<Portal> portals;
		
		private List<IMovable> _movables;

		#region ObjectMovements

		public void MoveObjectLeft(IMovable movable)
		{
			Vector3Int movableCoordinates = movable.GetCoordinates();
			Portal portal = PortalAtCoordinates(movableCoordinates + Vector3Int.left);
			
			if (portal != null)
			{
				TeleportObject(portal, movable);
				return;
			}
			
			if (movableCoordinates.x != maxLeft)
			{
				movable.MoveLeft();
			}
		}
		
		public void MoveObjectRight(IMovable movable)
		{
			Vector3Int movableCoordinates = movable.GetCoordinates();
			Portal portal = PortalAtCoordinates(movableCoordinates + Vector3Int.right);
			
			if (portal != null)
			{
				TeleportObject(portal, movable);
				return;
			}
			
			if (movableCoordinates.x != maxRight)
			{
				movable.MoveRight();
			}
		}
		
		public void MoveObjectForward(IMovable movable)
		{
			Vector3Int movableCoordinates = movable.GetCoordinates();
			Portal portal = PortalAtCoordinates(movableCoordinates + Vector3Int.forward);
			
			if (portal != null)
			{
				TeleportObject(portal, movable);
				return;
			}
			
			if (movableCoordinates.z != maxForward)
			{
				movable.MoveForward();
			}
		}
		
		public void MoveObjectBackward(IMovable movable)
		{
			Vector3Int movableCoordinates = movable.GetCoordinates();
			Portal portal = PortalAtCoordinates(movableCoordinates + Vector3Int.back);
			
			if (portal != null)
			{
				TeleportObject(portal, movable);
				return;
			}
			
			if (movableCoordinates.z != maxBackward)
			{
				movable.MoveBackward();
			}
		}

		private Portal PortalAtCoordinates(Vector3Int coordinates)
		{
			foreach (Portal portal in portals)
			{
				if (portal.GridCoordinates == coordinates)
				{
					return portal;
				}
			}

			return null;
		}

		private void TeleportObject(Portal portal, IMovable movable)
		{
			Portal linkedPortal = portal.GetLinkedPortal();
			movable.TeleportTo(linkedPortal.ArrivalPosition());
			linkedPortal.ParentRoom._movables.Add(movable);
			movable.SetRoom(linkedPortal.ParentRoom);
			_movables.Remove(movable);
		}
		
		#endregion

		private void SetRoomElements()
		{
			_movables = new List<IMovable>();

			for (var i = 0; i < movableGameObjects.Length; i++)
			{
				_movables.Add(movableGameObjects[i].GetComponent<IMovable>());
				_movables[i].SetRoom(this);
			}

			foreach (Portal portal in portals)
			{
				portal.ParentRoom = this;
			}
		}

		private void Awake()
		{
			SetRoomElements();
		}
	}
}