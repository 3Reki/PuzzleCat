using UnityEngine;

namespace PuzzleCat.LevelElements
{
    public class EndOfLevel : RoomElement
    {
        public override Vector3Int WorldGridPosition
        {
            get
            {
                Vector3 worldPosition = transform.position;
                return new Vector3Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y),
                    Mathf.FloorToInt(worldPosition.z));
            }
        }
        
        public override bool CanInteract(RoomElement movable)
        {
            return Cat.IsCat(movable);
        }

        public override void Interact(RoomElement movable)
        {
            if (movable is not Cat cat)
            {
                Debug.LogWarning("RoomElement that's not the cat is interacting with the mirror");
                return;
            }
            cat.MoveTo(transform.position, 1);
            cat.onArrival = () => cat.JumpInMirror();
        }
    }
}
