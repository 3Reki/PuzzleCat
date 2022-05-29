using System;
using DG.Tweening;
using PuzzleCat.Sound;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PuzzleCat.Menus
{
    public class TitleScreenManager : MonoBehaviour
    {
        [SerializeField] private RectTransform startTextTransform;

        public void StartGame()
        {
            SceneManager.LoadSceneAsync(1);
        }

        private void Start()
        {
            startTextTransform.DOAnchorPosY(0.02f * Screen.height, 1.2f).SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
            AudioManager.Instance.StopPlaying("LevelMusic");
            AudioManager.Instance.StopPlaying("LevelWin");
            AudioManager.Instance.Play("MenuMusic");
        }

        private void OnDestroy()
        {
            startTextTransform.DOKill();
        }
    }
}
