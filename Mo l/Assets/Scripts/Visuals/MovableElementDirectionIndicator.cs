using System;
using DG.Tweening;
using PuzzleCat.Utils;
using UnityEngine;

namespace PuzzleCat.Visuals
{
    public class MovableElementDirectionIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject sideAxisIndicator;
        [SerializeField] private GameObject forwardAxisIndicator;
        [SerializeField] private Material indicatorMaterial;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color incorrectColor;

        public void SetTransform(
            Vector3 position, Quaternion rotation, Vector3 sideIndicatorScale, Vector3 forwardIndicatorScale)
        {
            sideAxisIndicator.transform.CopyValues(position, rotation, sideIndicatorScale);
            sideAxisIndicator.transform.rotation *= Quaternion.Euler(180, 0, 90);
            forwardAxisIndicator.transform.CopyValues(position, rotation, forwardIndicatorScale);
            forwardAxisIndicator.transform.rotation *= Quaternion.Euler(180, 0, 0);
        }

        public void SetSideIndicatorsActive(bool state)
        {
            sideAxisIndicator.SetActive(state);
        }

        public void SetForwardIndicatorsActive(bool state)
        {
            forwardAxisIndicator.SetActive(state);
        }
        
        public void SetAllIndicatorsActive(bool state)
        {
            SetSideIndicatorsActive(state);
            SetForwardIndicatorsActive(state);
        }

        public void SetIncorrectColor()
        {
            indicatorMaterial.DOComplete();
            indicatorMaterial.color = incorrectColor;
            indicatorMaterial.DOColor(defaultColor, 1.5f);
        }

        private void OnDestroy()
        {
            indicatorMaterial.DOKill();
            indicatorMaterial.color = defaultColor;
        }
    }
}
