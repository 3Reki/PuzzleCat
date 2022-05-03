using System;
using PuzzleCat.Level_Elements;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.Controller
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public static event Action<GameState> OnGameStateChanged;

        public Camera MainCamera => _mainCamera;
        public GameState State { get; private set; }
        public Cat Cat => cat;

        [SerializeField] private Cat cat;
        [SerializeField] private Camera _mainCamera;

        private static NavMeshSurface[] _surfaces;


#if UNITY_EDITOR
        public void Init(Camera cam, Cat sceneCat, float defaultDragDistance, Transform[] furniturePortals,
            GameObject quad, Transform catIndicator)
        {
            // MainCamera = cam;
            // selectableLayerMask = 1 << LayerMask.NameToLayer("Selectable");
            // invisibleLayerMask = 1 << LayerMask.NameToLayer("Invisible");
            cat = sceneCat;
            // dragDistance = defaultDragDistance;
            // portalsParentTransform = furniturePortals;
            // invisibleQuad = quad;
            // catDirectionIndicator = catIndicator;
        }
#endif

        public void UpdateGameState(GameState newState)
        {
            State = newState;

            OnGameStateChanged?.Invoke(newState);
        }


        private static void UpdateNavMeshes()
        {
            foreach (NavMeshSurface navMeshSurface in _surfaces)
            {
                navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
            }
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;
            State = GameState.PlayerMovement;
            _surfaces = FindObjectsOfType<NavMeshSurface>();
            MovableElement.onMovement += UpdateNavMeshes;
        }

        public enum GameState
        {
            PortalMode,
            FurnitureMovement,
            PlayerMovement,
            Menu
        }
    }
}