using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleCat
{
    public class FurnitureSelectionIndicator : MonoBehaviour
    {
        [SerializeField] private Image indicatorImage;
        [SerializeField] private RectTransform indicatorRectTransform;
        [SerializeField] private Color initialColor;
        [SerializeField] private Color finalColor;

        public void Play(Vector2 position, float desiredDuration)
        {
            indicatorRectTransform.localScale = Vector3.one;
            indicatorRectTransform.anchoredPosition = FindCorrectPosition(position);
            indicatorImage.fillAmount = 0;
            indicatorImage.color = initialColor;
            
            indicatorImage.DOColor(finalColor, desiredDuration + 0.2f);
            indicatorImage.DOFillAmount(1, desiredDuration);
            indicatorRectTransform.DOScale(1.2f, desiredDuration + 0.2f).SetEase(Ease.Flash).onComplete = () =>
            {
                indicatorImage.DOFade(0f, 0.1f);
            };
        }

        public void Stop()
        {
            indicatorRectTransform.DOKill();
            indicatorImage.DOKill();
            indicatorImage.color = new Color(0, 0, 0, 0);
        }

        private static Vector2 FindCorrectPosition(Vector2 position)
        {
            position.x += Screen.width * (position.x < Screen.width * .15f ? .12f : -.12f);
            position.y += Screen.height * (position.y > Screen.height * .85f ? -.10f : .12f);
            return position;
        }
    }
}
