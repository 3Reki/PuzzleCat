using UnityEngine;

namespace PuzzleCat.Level
{
    public interface IMovable
    {
        RoomElement RoomElement => (RoomElement) this;

        void MoveTo(Vector3Int coordinates);
        void TeleportTo(Vector3Int coordinates);
    }
}