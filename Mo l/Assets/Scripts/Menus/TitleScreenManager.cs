using System.Collections;
using DG.Tweening;
using PuzzleCat.Sound;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PuzzleCat.Menus
{
    public class TitleScreenManager : MonoBehaviour
    {
        [SerializeField] private RectTransform startTextTransform;
        [SerializeField] private GameObject mainCanvas;
        [SerializeField] private GameObject endGameCanvas;

        public void StartGame()
        {
            AudioManager.Instance.Play("Ok");
            SceneManager.LoadSceneAsync(1);
        }

        private void EndGameScreen()
        {
            mainCanvas.SetActive(false);
            endGameCanvas.SetActive(true);
            StartCoroutine(EndGameCoroutine());
        }

        private IEnumerator EndGameCoroutine()
        {
            while (Input.touchCount == 0)
            {
#if UNITY_EDITOR
                if (UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    break;
                }
#endif
                yield return null;
            }
            GameData.Instance.gameFinished = false;
            mainCanvas.SetActive(true);
            endGameCanvas.SetActive(false);
            AudioManager.Instance.Play("Click");
        }

        private void Start()
        {
            startTextTransform.DOAnchorPosY(0.02f * Screen.height, 1.2f).SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
            AudioManager.Instance.StopPlaying("LevelMusic");
            AudioManager.Instance.StopPlaying("LevelWin");
            AudioManager.Instance.Play("MenuMusic");

            if (GameData.Instance.gameFinished)
            {
                EndGameScreen();
            }
        }

        private void OnDestroy()
        {
            startTextTransform.DOKill();
        }
    }
}
