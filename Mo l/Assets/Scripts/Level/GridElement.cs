using UnityEngine;

namespace PuzzleCat.Level
{
    public class GridElement : MonoBehaviour
    {
        protected virtual Vector3Int GetGridCoordinates(Vector3 worldPosition)
        {
            return new Vector3Int((int) (worldPosition.x - 0.5f), (int) (worldPosition.y - 0.5f), (int) (worldPosition.z - 0.5f));
        }

        protected virtual Vector3 GetWorldPosition(Vector3Int gridCoordinates)
        {
            return new Vector3(gridCoordinates.x + 0.5f, gridCoordinates.y + 0.5f, gridCoordinates.z + 0.5f);
        }
    }
}
