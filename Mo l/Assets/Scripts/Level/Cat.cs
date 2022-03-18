using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.Level
{
    public class Cat : RoomElement, IMovable
    {
        [SerializeField] private NavMeshAgent playerAgent;

        public static bool IsCat(GameObject gameObject) => gameObject.GetComponent<Cat>() != null;
        public static bool IsCat(object otherObject) => otherObject.GetType() == typeof(Cat);

        public void TryMovingTo(Vector3 worldDestination)
        {
            Vector3Int destination = CurrentRoom.WorldToRoomCoordinates(UtilsClass.WorldPointAsGridPoint(worldDestination));

            if (CurrentRoom.CanMoveOnCell(destination))
            {
                CurrentRoom.MoveOnCell(this, destination);
            }
        }

        public void MoveTo(Vector3Int coordinates)
        {
            playerAgent.SetDestination(coordinates + new Vector3(0.5f, 0, 0.5f));
        }

        public void TeleportTo(Vector3Int coordinates)
        {
            playerAgent.Warp(GetWorldPosition(coordinates));
        }
    }
}
