using System.Collections.Generic;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Level
{
	public class Room : MonoBehaviour
	{
		public Vector3Int gridWorldPosition;
		public Vector3Int gridSize;
		[SerializeField] private List<RoomElement> roomElements;

#if UNITY_EDITOR
		public void Init()
		{
			roomElements = new List<RoomElement>();
		}
#endif

		public bool AreCoordinatesValid(Vector3Int coordinates)
		{
			coordinates = WorldToRoomCoordinates(coordinates);
			return coordinates.x >= 0 && coordinates.x < gridSize.x && 
			       coordinates.y >= 0 && coordinates.y < gridSize.y && 
			       coordinates.z >= 0 && coordinates.z < gridSize.z;
		}

		public Vector3Int WorldToRoomCoordinates(Vector3Int worldGridCoordinates)
		{
			return worldGridCoordinates - gridWorldPosition;
		}

		public Vector3Int RoomToWorldCoordinates(Vector3Int roomGridCoordinates)
		{
			return roomGridCoordinates + gridWorldPosition;
		}

		public Portal FindPortal(Vector3Int portalPosition, Surface portalSurface)
		{
			foreach (RoomElement roomElement in roomElements)
			{
				if (roomElement is not Portal portal) continue;

				if (portal.RoomGridPosition.x == portalPosition.x
				    && portal.RoomGridPosition.y == portalPosition.y
				    && portal.RoomGridPosition.z == portalPosition.z
				    && portal.ImpactedSurface == portalSurface
				    && portal.Active)
				{
					return portal;
				}
			}

			return null;
		}

		public bool CanMoveOnCell(IMovable movableElement, Vector3Int coordinates, Surface surface)
		{
			if (coordinates.x < 0 || coordinates.x >= gridSize.x ||
			    coordinates.y < 0 || coordinates.y >= gridSize.y ||
			    coordinates.z < 0 || coordinates.z >= gridSize.z)
			{
				return false;
			}

			foreach (RoomElement element in roomElements)
			{
				if ((element.ImpactedSurface == surface || element.ImpactedSurface == Surface.All)
				    && element.RoomGridPosition.x == coordinates.x
				    && element.RoomGridPosition.y == coordinates.y
				    && element.RoomGridPosition.z == coordinates.z
				    && !element.CanInteract(movableElement))
				{
					return false;
				}
			}

			return true;
		}

		public void MoveOnCell(IMovable movableElement, Vector3Int coordinates, Surface surface)
		{
			RoomElement element = GetElementAt(coordinates, surface);
			if (element == null)
			{
				movableElement.MoveTo(RoomToWorldCoordinates(coordinates));
				return;
			}

			element.Interact(movableElement);
		}

		public void AddRoomElement(RoomElement roomElement)
		{
			roomElements.Add(roomElement);
		}

		public void RemoveRoomElement(RoomElement roomElement)
		{
			roomElements.Remove(roomElement);
		}
		
		public RoomElement GetElementAt(Vector3Int roomCoordinates)
		{
			foreach (RoomElement element in roomElements)
			{
				if (element.RoomGridPosition.x == roomCoordinates.x
				    && element.RoomGridPosition.y == roomCoordinates.y
				    && element.RoomGridPosition.z == roomCoordinates.z)
				{
					return element;
				}
			}

			return null;
		}

		private RoomElement GetElementAt(Vector3Int roomCoordinates, Surface surface)
		{
			foreach (RoomElement element in roomElements)
			{
				if ((element.ImpactedSurface == surface || element.ImpactedSurface == Surface.All)
				    && element.RoomGridPosition.x == roomCoordinates.x
				    && element.RoomGridPosition.y == roomCoordinates.y
				    && element.RoomGridPosition.z == roomCoordinates.z)
				{
					return element;
				}
			}

			return null;
		}

		private void SetRoomElements()
		{
			foreach (RoomElement roomElement in roomElements)
			{
				roomElement.SetRoom(this);
			}
		}

		private void Awake()
		{
			SetRoomElements();
		}
	}
}