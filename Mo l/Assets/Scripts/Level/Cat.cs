using System;
using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.Level
{
    public class Cat : RoomElement, IMovable
    {
        [SerializeField] private NavMeshAgent playerAgent;

        private Transform _transform;
        private Vector3 _offset
        {
            get
            {
                Vector3 roundedUp = _transform.up.Round();
                if (roundedUp == Vector3.up)
                {
                    return new Vector3(0.5f, 0, 0.5f);
                }
                if (roundedUp == Vector3.right)
                {
                    return new Vector3(0, 0.5f, 0.5f);
                }
                if (roundedUp == Vector3.back)
                {
                    return new Vector3(0.5f, 0.5f, 0.5f);
                }
                
                Debug.LogWarning("Not on floor");
                return Vector3Int.zero;
            }
        }

        public static bool IsCat(GameObject gameObject) => gameObject.GetComponent<Cat>() != null;
        public static bool IsCat(object otherObject) => otherObject.GetType() == typeof(Cat);

        public void TryMovingTo(Vector3Int worldGridDestination)
        {
            Vector3Int destination = CurrentRoom.WorldToRoomCoordinates(worldGridDestination);

            if (CurrentRoom.CanMoveOnCell(destination))
            {
                CurrentRoom.MoveOnCell(this, destination);
            }
        }

        public void MoveTo(Vector3Int coordinates)
        {
            playerAgent.SetDestination(coordinates + _offset);
            //Debug.Log(playerAgent.navMeshOwner.name);
            Debug.Log(coordinates + _offset);
        }

        public void TeleportTo(Vector3Int coordinates)
        {
            playerAgent.Warp(GetWorldPosition(coordinates));
        }

        private void Awake()
        {
            _transform = transform;
        }
    }
}
