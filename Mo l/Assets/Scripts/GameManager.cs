using System.Collections;
using PuzzleCat.Controller;
using PuzzleCat.LevelElements;
using PuzzleCat.Sound;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public static GameManagerCallback OnLevelEnd;
        
        public DefaultState DefaultState;
        public CatMovementState CatMovementState;
        public MovableSelectionState MovableSelectionState;
        public MovableMovementState MovableMovementState;
        public CameraMovementState CameraMovementState;
        public CameraZoomState CameraZoomState;
        public PortalState portalState;
        
        public Camera MainCamera => mainCamera;
        public Cat Cat => cat;
        public bool MenuOpened;

        [SerializeField] private Cat cat;
        [SerializeField] private Camera mainCamera;

        private NavMeshSurface[] _surfaces;
        private IEnumerator _updateNavEnumerator;
        private float _lastNavUpdateFrameCount;

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
            _surfaces = FindObjectsOfType<NavMeshSurface>();
            MovableElement.OnMovement += UpdateNavMeshes;
            
#if UNITY_EDITOR
            new GameObject("Game Data").AddComponent<GameData>();
            UnityEditor.PrefabUtility.InstantiatePrefab(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AudioManager.prefab"));
#endif
        }

        private void Start()
        {
            AudioManager.Instance.Play("LevelMusic");
        }

        private void OnDestroy()
        {
            MovableElement.OnMovement -= UpdateNavMeshes;
        }

        public delegate void GameManagerCallback();
    }
}