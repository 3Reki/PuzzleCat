using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleCat
{
    public class TutorialTwo : MonoBehaviour
    {
        [SerializeField] private RectTransform handTransform;
        
        [SerializeField] private Vector2 firstPosition = new Vector2(-260,35);

        void Start()
        {
            
        }

 
        
        
        //Premiere animation de la main (sur le meuble apres le tabouret)
        public void FirstHand()
        {
            handTransform.anchoredPosition = firstPosition;
        }
    }
}
