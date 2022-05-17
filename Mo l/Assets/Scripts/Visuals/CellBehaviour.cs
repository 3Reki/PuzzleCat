using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleCat
{
    public class CellBehaviour : MonoBehaviour
    {
        [SerializeField] private Color _walkableCell;
        [SerializeField] private Color _portalCell;
        [SerializeField] private Color _unWalkableCell;

        private void OnCollisionEnter(Collision collision)
        {
            
        }

        private void SetMatColor(string channelA, string channelB, string channelC, Material mat, Color color, List<GameObject> listGo, bool thirdParam = false)
        {
                mat.color = color;
                mat.DisableKeyword(channelA);
                mat.EnableKeyword(channelB);

                if (thirdParam)
                {
                    mat.DisableKeyword(channelC);
                }
        }
    }
}
