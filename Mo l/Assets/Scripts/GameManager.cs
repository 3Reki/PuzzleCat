using System;
using System.Collections;
using PuzzleCat.LevelElements;
using PuzzleCat.Sound;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PuzzleCat
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public static event Action<GameState> OnGameStateChanged;

        public Camera MainCamera => mainCamera;
        public GameState State { get; private set; }
        public Cat Cat => cat;

        [SerializeField] private Cat cat;
        [SerializeField] private Camera mainCamera;

        private NavMeshSurface[] _surfaces;
        private IEnumerator _updateNavEnumerator;
        private float _lastNavUpdateFrameCount;

        public void UpdateGameState(GameState newState)
        {
            State = newState;
            OnGameStateChanged?.Invoke(newState);
        }

        private void UpdateNavMeshes()
        {
            if (Time.frameCount - _lastNavUpdateFrameCount < 3)
            {
                StopCoroutine(_updateNavEnumerator);
            }
            
            _updateNavEnumerator = UpdateNavMeshesCoroutine();
            StartCoroutine(_updateNavEnumerator);
        }

        private IEnumerator UpdateNavMeshesCoroutine()
        {
            _lastNavUpdateFrameCount = Time.frameCount;
            for (int i = 0; i < 3; i++)
            {
                yield return null;
            }
            
            foreach (NavMeshSurface navMeshSurface in _surfaces)
            {
                navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
                yield return null;
            }
        }

        private void Awake()
        {
            Instance = this;
            State = GameState.PlayerMovement;
            _surfaces = FindObjectsOfType<NavMeshSurface>();
            MovableElement.onMovement += UpdateNavMeshes;
            
#if UNITY_EDITOR
            new GameObject("Game Data").AddComponent<GameData>();
            PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AudioManager.prefab"));
#endif
        }

        private void Start()
        {
            AudioManager.Instance.StopPlaying("MenuMusic");
            AudioManager.Instance.StopPlaying("LevelWin");
            AudioManager.Instance.Play("LevelMusic");
        }

        private void OnDestroy()
        {
            MovableElement.onMovement -= UpdateNavMeshes;
        }

        public enum GameState
        {
            PortalMode,
            FurnitureMovement,
            PlayerMovement,
            CameraMovement,
            Menu,
            End
        }
    }
}