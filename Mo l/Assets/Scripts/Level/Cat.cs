using System;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.Level
{
    public class Cat : RoomElement, IMovable
    {
        [SerializeField] private NavMeshAgent playerAgent;

        public bool IsCat(GameObject otherGameObject) => gameObject == otherGameObject;

        public void TryMovingTo(Vector3 worldDestination)
        {
            Vector3Int destination = CurrentRoom.WorldToRoomCoordinates(new Vector3Int(
                Mathf.FloorToInt(worldDestination.x), Mathf.RoundToInt(worldDestination.y), Mathf.FloorToInt(worldDestination.z)));

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
            //transform.position = GetWorldPosition(coordinates);
            playerAgent.Warp(GetWorldPosition(coordinates));
            //playerAgent.SetDestination(GetWorldPosition(coordinates));
        }
    }
}
