using System;
using PuzzleCat.LevelElements;
using UnityEngine;
using UnityEngine.AI;

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

        private static NavMeshSurface[] _surfaces;
        

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