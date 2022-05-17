using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleCat
{
    public class TutorialOne : MonoBehaviour
    {
        
        [SerializeField] private RectTransform handTransform;

        [SerializeField] private Vector2 firstPosition = new Vector2(137,-289);
        [SerializeField] private Vector2 secondPosition = new Vector2(408, -245);
        [SerializeField] private Vector2 thirdPosition = new Vector2(13, 74);




        void Start()
        {
            
        }


        //Premiere animation de la main (devant le meuble au pied du lit)
        public void FirstHand()
        {
            handTransform.anchoredPosition = firstPosition;
        }

        
        //Deuxieme animation de la main (sur le point de teleportaiton du chat)
        public void SecondHand()
        {
            handTransform.anchoredPosition = secondPosition;
        }
        
        
        //Troisieme animation de la main (sur le miroir de fin de niveau)
        public void ThirdHand()
        {
            handTransform.anchoredPosition = thirdPosition;
            handTransform.rotation = Quaternion.Euler(0,0, -125);
        }
        
        
    }
}
