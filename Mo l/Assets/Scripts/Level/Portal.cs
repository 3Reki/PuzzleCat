using UnityEngine;

namespace PuzzleCat.Level
{
    public class Portal : GridElement
    {
        [HideInInspector] public Room ParentRoom;
        public Vector3Int GridCoordinates { get; private set; }
        [SerializeField] private Portal linkedPortal;

        public Portal GetLinkedPortal() => linkedPortal;
        
        public Vector3Int ArrivalPosition() => GridCoordinates + Vector3Int.left;

        protected override Vector3Int GetGridCoordinates(Vector3 worldPosition)
        {
            return new Vector3Int((int) (worldPosition.x - 0.5f), Mathf.FloorToInt(worldPosition.y), (int) (worldPosition.z - 0.5f));
        }

        private void Awake()
        {
            GridCoordinates = GetGridCoordinates(transform.position);
        }
    }
}