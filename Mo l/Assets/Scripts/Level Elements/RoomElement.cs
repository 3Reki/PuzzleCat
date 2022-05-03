using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Level_Elements
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
					Mathf.RoundToInt(worldPosition.x - 0.5f),
					Mathf.RoundToInt(worldPosition.y - 0.5f),
					Mathf.RoundToInt(worldPosition.z - 0.5f));
			}
		}

		public void SetRoom(Room room)
		{
			CurrentRoom = room;
		}

		public virtual bool CanInteract(IMovable movable)
		{
			return !IsObstacle;
		}

		public virtual void Interact(IMovable movable)
		{
			Debug.LogWarning("No interaction implemented");
		}

		protected virtual Vector3 GetWorldPosition(Vector3Int gridCoordinates)
		{
			return new Vector3(gridCoordinates.x + 0.5f, gridCoordinates.y + 0.5f, gridCoordinates.z + 0.5f);
		}
	}
}