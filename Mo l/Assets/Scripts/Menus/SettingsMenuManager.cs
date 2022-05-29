using DG.Tweening;
using PuzzleCat.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleCat.Menus
{
    public class SettingsMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject settingsCanvasGameObject;
        [SerializeField] private RectTransform settingsMenuTransform;
        [SerializeField] private Image settingsBackgroundImage;
        
        [SerializeField] private GameObject creditsCanvasGameObject;
        [SerializeField] private RectTransform creditsMenuTransform;
        
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Toggle musicToggle;
        
        private bool _menuAlreadyClosed;
        
        public void CloseSettingsMenu()
        {
            if (_menuAlreadyClosed)
            {
                return;
            }

            _menuAlreadyClosed = true;
            AudioManager.Instance.Play("Back");
            settingsBackgroundImage.DOFade(0f, 0.6f);
            settingsMenuTransform.DOAnchorPosY(0, .6f).SetEase(Ease.InBack).onComplete =
                () =>
                {
                    settingsCanvasGameObject.SetActive(false);
                    _menuAlreadyClosed = false;
                };
        }

        public void OpenSettingsMenu()
        {
            settingsCanvasGameObject.SetActive(true);
            settingsBackgroundImage.DOFade(0.733f, 0.45f);
            settingsMenuTransform.DOMoveY(Screen.height * 0.88f, .6f).SetEase(Ease.OutBack);
            AudioManager.Instance.Play("Back");
        }

        public void CloseCredits()
        {
            if (_menuAlreadyClosed)
            {
                return;
            }

            _menuAlreadyClosed = true;
            AudioManager.Instance.Play("Ok");
            creditsMenuTransform.DOAnchorPosY(0, .9f).SetEase(Ease.InBack).onComplete =
                () =>
                {
                    settingsBackgroundImage.enabled = true;
                    creditsCanvasGameObject.SetActive(false);
                    _menuAlreadyClosed = false;
                };
        }

        public void OpenCredits()
        {
            creditsCanvasGameObject.SetActive(true);
            settingsBackgroundImage.enabled = false;
            creditsMenuTransform.DOMoveY(Screen.height * 0.5f, .9f).SetEase(Ease.OutBack);
            AudioManager.Instance.Play("Ok");
        }

        public void SwitchSFX(bool state)
        {
            GameData.Instance.sfxOn = state;
            AudioManager.Instance.Play("Ok");

            if (sfxToggle.isOn)
            {
                AudioManager.Instance.SfxMixerVolumeOn();
            }   
            else
            {
                AudioManager.Instance.SfxMixerVolumeOff();
            }
        }
        
        public void SwitchMusic(bool state)
        {
            GameData.Instance.musicOn = state;
            AudioManager.Instance.Play("Ok");

            if (musicToggle.isOn)
            {
                AudioManager.Instance.MusicMixerVolumeOn();
            }
            else
            {
                AudioManager.Instance.MusicMixerVolumeOff();
            }
        }

        private void Start()
        {
            sfxToggle.isOn = GameData.Instance.sfxOn;
            musicToggle.isOn = GameData.Instance.musicOn;
        }
    }
}
