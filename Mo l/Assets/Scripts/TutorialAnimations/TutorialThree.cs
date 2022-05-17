using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PuzzleCat
{
    public class TutorialThree : MonoBehaviour
    {
        [SerializeField] private RectTransform handTransform;
        [SerializeField] private Animator handAnimator;
        
        [SerializeField] private Vector2 firstPosition = new Vector2(115,-63);


        void Start()
        {
            HoldMove();
        }
        
        
        //Animation de la main qui descend selectionne le meuble deplacable
        public void DownMove()
        {
            handTransform.anchoredPosition = firstPosition;
            handAnimator.Play("HandDown_Animation");
        }

        //Animation de la main fixe sur le meuble et le meuble glisse
        public void HoldMove()
        {
            handAnimator.Play("HandHold_Animation");
            //La main se deplace ici
        }

        //Animation de la main qui remonte
        public void UpMove()
        {
            handAnimator.Play("HandUp_Animation");
        }
        
    }
}
