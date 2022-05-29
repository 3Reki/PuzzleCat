using System;
using System.Collections.Generic;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.LevelElements
{
	public class Room : MonoBehaviour
	{
		[SerializeField] private Vector3Int gridWorldPosition;
		[SerializeField] private Vector3Int gridSize;
		[SerializeField] private List<RoomElement> roomElements;
		[SerializeField] private MeshRenderer floorRenderer;
		[SerializeField] private MeshRenderer sideWallRenderer; // TODO show in editor
		[SerializeField] private MeshRenderer backWallRenderer;
		private static readonly int _onOff = Shader.PropertyToID("_OnOff");

#if UNITY_EDITOR
		public void Init()
		{
			roomElements = new List<RoomElement>();
		}
#endif

		public bool AreCoordinatesValid(Vector3Int coordinates)
		{
			return coordinates.x >= 0 && coordinates.x < gridSize.x && 
			       coordinates.y >= 0 && coordinates.y < gridSize.y && 
			       coordinates.z >= 0 && coordinates.z < gridSize.z;
		}

		public bool IsInContact(Vector3Int position, Surface elementSurface)
		{
			return elementSurface switch
			{
				Surface.Floor => position.y == 0,
				Surface.SideWall => position.x == 0,
				Surface.BackWall => position.z == gridSize.z - 1,
				_ => throw new ArgumentOutOfRangeException(nameof(elementSurface), elementSurface, null)
			};
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
				    && portal.Placed)
				{
					return portal;
				}
			}

			return null;
		}

		public bool CanMoveOnCell(RoomElement movableElement, Vector3Int coordinates, Surface surface)
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

		public void MoveOnCell(RoomElement movableElement, Vector3Int coordinates, Surface surface)
		{
			RoomElement element = GetElementAt(coordinates, surface);
			if (element == null)
			{
				movableElement.MoveTo(RoomToWorldCoordinates(coordinates));
				return;
			}

			if (element.CanInteract(movableElement))
			{
				element.Interact(movableElement);
			}
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

		public RoomElement GetElementAt(Vector3Int roomCoordinates, Surface surface)
		{
			foreach (RoomElement element in roomElements)
			{
				if ((element.ImpactedSurface == surface)
				    && element.RoomGridPosition.x == roomCoordinates.x
				    && element.RoomGridPosition.y == roomCoordinates.y
				    && element.RoomGridPosition.z == roomCoordinates.z)
				{
					return element;
				}
			}

			return null;
		}

		public bool ExistsElementAt(Vector3Int roomCoordinates, Surface surface)
		{
			foreach (RoomElement element in roomElements)
			{
				if ((element.ImpactedSurface == surface)
				    && element.RoomGridPosition.x == roomCoordinates.x
				    && element.RoomGridPosition.y == roomCoordinates.y
				    && element.RoomGridPosition.z == roomCoordinates.z)
				{
					return true;
				}
			}

			return false;
		}
		
		public void SetSurfaceIndicatorActive(Surface portalArrivalSurface, bool state)
		{
			(portalArrivalSurface switch
			{
				Surface.Floor => floorRenderer,
				Surface.SideWall => sideWallRenderer,
				Surface.BackWall => backWallRenderer,
				_ => throw new ArgumentOutOfRangeException(nameof(portalArrivalSurface), portalArrivalSurface, null)
			}).material.SetFloat(_onOff, state ? 1: 0);
		}
		
		public void SetAllSurfaceIndicatorActive(bool state)
		{
			if (state)
			{
				floorRenderer.material.SetFloat(_onOff, 1f);
				sideWallRenderer.material.SetFloat(_onOff, 1f);
				backWallRenderer.material.SetFloat(_onOff, 1f);
				return;
			}
			
			floorRenderer.material.SetFloat(_onOff, 0f);
			sideWallRenderer.material.SetFloat(_onOff, 0f);
			backWallRenderer.material.SetFloat(_onOff, 0f);
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