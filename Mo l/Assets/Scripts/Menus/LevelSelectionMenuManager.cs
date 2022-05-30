using System.Collections;
using DG.Tweening;
using PuzzleCat.Sound;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PuzzleCat.Menus
{
    public class LevelSelectionMenuManager : MonoBehaviour
    {
        [SerializeField] private RectTransform[] levelSelectionTransforms;
        [SerializeField] private Button leftSelectionArrow;
        [SerializeField] private Button rightSelectionArrow;
        [SerializeField] private Button[] levelButtons;

        [SerializeField] private GameObject levelSelectionCanvas;
        [SerializeField] private GameObject introductionCanvas;
        
        private int _currentLevelSelectionIndex;
        
        public static void LoadLevel(int level)
        {
            SceneManager.LoadScene(level);
            AudioManager.Instance.Play("Ok");
        }

        public void UnlockAllLevels()
        {
            foreach (Button button in levelButtons)
            {
                button.interactable = true;
            }

            GameData.Instance.unlockedLevelsCount = levelButtons.Length;
            GameData.Instance.gameFinished = true;
            AudioManager.Instance.Play("Ok");
            SceneManager.LoadScene(0);
        }

        public void ResetSave()
        {
            for (var i = 1; i < levelButtons.Length; i++)
            {
                levelButtons[i].interactable = false;
            }

            GameData.Instance.ResetGameData();
            AudioManager.Instance.Play("Back");
            SceneManager.LoadScene(0);
        }

        public void ChangeLevelSelectionPanel(int direction)
        {
            _currentLevelSelectionIndex += direction;
            
            foreach (RectTransform selectionTransform in levelSelectionTransforms)
            {
                //selectionTransform.DOKill();
                selectionTransform.DOAnchorPosX(-Screen.width * _currentLevelSelectionIndex, 0.5f);
                AudioManager.Instance.Play("Ok");
            }

            rightSelectionArrow.interactable = _currentLevelSelectionIndex != levelSelectionTransforms.Length - 1;
            leftSelectionArrow.interactable = _currentLevelSelectionIndex != 0;
        }

        private void PlayIntroduction()
        {
            levelSelectionCanvas.SetActive(false);
            introductionCanvas.SetActive(true);
            StartCoroutine(IntroductionCoroutine());
        }

        private IEnumerator IntroductionCoroutine()
        {
            yield return new WaitForSeconds(10);
            levelSelectionCanvas.SetActive(true);
            introductionCanvas.SetActive(false);
            GameData.Instance.firstTime = false;
        }

        private void Awake()
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
#if UNITY_EDITOR
            new GameObject("Game Data").AddComponent<GameData>();
            PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AudioManager.prefab"));
#endif
        }

        private void Start()
        {
            if (GameData.Instance.firstTime)
            {
                PlayIntroduction();
            }

            for (int i = GameData.Instance.unlockedLevelsCount; i < levelButtons.Length; i++)
            {
                levelButtons[i].interactable = false;
            }

            leftSelectionArrow.interactable = false;
        }
    }
}