using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.LevelElements
{
    public interface IMovable
    {
        RoomElement RoomElement => (RoomElement) this;

        void MoveTo(Vector3Int coordinates);
        void TeleportTo(Vector3Int coordinates, Surface newSurface, Vector3Int exitDirection);
    }
}