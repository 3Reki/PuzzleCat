using UnityEngine;

namespace PuzzleCat.Visuals
{
    public class MovableElementDirectionIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject[] sideAxisIndicators;
        [SerializeField] private GameObject[] forwardAxisIndicators;

        public void SetSideIndicatorsActive(bool state)
        {
            foreach (GameObject indicator in sideAxisIndicators)
            {
                indicator.SetActive(state);
            }
        }

        public void SetForwardIndicatorsActive(bool state)
        {
            foreach (GameObject indicator in forwardAxisIndicators)
            {
                indicator.SetActive(state);
            }
        }
        
        public void SetAllIndicatorsActive(bool state)
        {
            SetSideIndicatorsActive(state);
            SetForwardIndicatorsActive(state);
        }
    }
}
