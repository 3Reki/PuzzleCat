using UnityEngine;

namespace PuzzleCat
{
    public class Cat : MonoBehaviour
    {
        public bool IsCat(GameObject otherGameObject) => gameObject == otherGameObject;
    }
}
