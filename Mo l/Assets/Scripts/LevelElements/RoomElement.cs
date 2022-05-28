using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.LevelElements
{
	public class RoomElement : MonoBehaviour
	{
		public Surface ImpactedSurface = Surface.All;
		public bool IsObstacle = true;
		public Vector3Int RoomGridPosition => CurrentRoom.WorldToRoomCoordinates(WorldGridPosition);
		public Room CurrentRoom { get; private set; }

		public virtual Vector3Int WorldGridPosition
		{
			get
			{
				Vector3 worldPosition = transform.position;
				return new Vector3Int(
					Mathf.FloorToInt(worldPosition.x),
					Mathf.FloorToInt(worldPosition.y),
					Mathf.FloorToInt(worldPosition.z));
			}
		}

		public void SetRoom(Room room)
		{
			CurrentRoom = room;
		}

		public virtual bool CanInteract(RoomElement movable)
		{
			return !IsObstacle;
		}

		public virtual void Interact(RoomElement movable)
		{
			Debug.LogWarning("No interaction implemented");
		}

		public virtual void MoveTo(Vector3Int destination)
		{
			Debug.LogWarning("No interaction implemented");
		}

		protected virtual Vector3 GetWorldPosition(Vector3Int gridCoordinates)
		{
			return new Vector3(gridCoordinates.x + 0.5f, gridCoordinates.y + 0.5f, gridCoordinates.z + 0.5f);
		}
	}
}