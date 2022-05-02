using System.Collections.Generic;
using System.Linq;
using PuzzleCat.Level;
using PuzzleCat.Utils;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PuzzleCat.Editor
{
    public class LevelEditionTool : MonoBehaviour
    {
        [MenuItem("Level Edition/Link Scripts")]
        public static void LinkScripts()
        {
            CreateAndBakeNavMeshes();

            GameManager gameManager = CreateInputManager();

            UpdateRoomAndRoomElements();

            CreateUI(gameManager);

            //UpdateCatPortals();

            Scene scene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void CreateAndBakeNavMeshes()
        {
            if (FindObjectOfType<NavMeshSurface>() == null)
            {
                PrefabUtility.InstantiatePrefab(
                    AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LevelEditing/Navigation Meshes.prefab"));
            }

            var navMeshSurfaces = FindObjectsOfType<NavMeshSurface>();

            foreach (NavMeshSurface navMeshSurface in navMeshSurfaces)
            {
                navMeshSurface.BuildNavMesh();
                PrefabUtility.RecordPrefabInstancePropertyModifications(navMeshSurface);
            }
        }
        
        private static GameManager CreateInputManager()
        {
            var inputManager = FindObjectOfType<GameManager>();

            if (inputManager == null)
            {
                inputManager = new GameObject("Game Manager").AddComponent<GameManager>();
            }

            inputManager.Init(Camera.main, FindObjectOfType<Cat>(), 3, GetPortalsParentList(), CreateInvisibleQuad(), CreateCatIndicator());
            return inputManager;
        }

        private static Transform[] GetPortalsParentList()
        {
            var portalsParents = new HashSet<Transform>();
            
            foreach (Portal portal in Resources.FindObjectsOfTypeAll<Portal>())
            {
                if (portal.gameObject.scene.name == null || portal.catPortal)
                {
                    continue;
                }

                portalsParents.Add(portal.transform.parent);
            }

            return portalsParents.ToArray();
        }

        private static GameObject CreateInvisibleQuad()
        {
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            DestroyImmediate(quad.GetComponent<MeshFilter>());
            DestroyImmediate(quad.GetComponent<MeshRenderer>());
            quad.transform.localScale = new Vector3(40, 40, 1);
            quad.layer = LayerMask.NameToLayer("Invisible");
            quad.SetActive(false);

            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(go => Utils.Utils.IsInLayerMask(go, 1 << LayerMask.NameToLayer("Invisible")))
                .Where(gameObject => gameObject.scene.name != null && gameObject != quad))
            {
                DestroyImmediate(gameObject);
            }

            return quad;
        }

        private static Transform CreateCatIndicator()
        {
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            sphere.layer = LayerMask.NameToLayer("Invisible");
            sphere.tag = "Indicator";
            sphere.SetActive(false);

            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(go => Utils.Utils.IsInLayerMask(go, 1 << LayerMask.NameToLayer("Invisible")) && go.CompareTag("Indicator"))
                .Where(gameObject => gameObject.scene.name != null && gameObject != sphere))
            {
                DestroyImmediate(gameObject);
            }

            return sphere.transform;
        }
        
        private static void UpdateRoomAndRoomElements()
        {
            var rooms = FindObjectsOfType<Room>();

            foreach (Room room in rooms)
            {
                room.Init();
            }

            foreach (RoomElement roomElement in FindObjectsOfType<RoomElement>())
            {
                foreach (Room room in rooms.Where(room => room.AreCoordinatesValid(room.WorldToRoomCoordinates(roomElement.WorldGridPosition))))
                {
                    room.AddRoomElement(roomElement);
                    break;
                }
            }

            foreach (Room room in rooms)
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(room);
            }

            foreach (SerializedObject movable in FindObjectsOfType<SingleMovable>()
                .Select(movable => new SerializedObject(movable))
                .Where(movable => movable.FindProperty("linkedMovables").arraySize > 1))
            {
                for (int i = 1; i < movable.FindProperty("linkedMovables").arraySize; i++)
                {
                    var serializedObject = new SerializedObject(
                        movable.FindProperty("linkedMovables").GetArrayElementAtIndex(i).objectReferenceValue);
                    serializedObject.FindProperty("linkedMovables")
                        .SetAsSingleMovableArray(movable.FindProperty("linkedMovables").GetAsSingleMovableArray());
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private static void CreateUI(GameManager gameManager)
        {
            if (FindObjectOfType<Canvas>() == null)
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LevelEditing/In Game Canvas.prefab"));
            }

            if (FindObjectOfType<EventSystem>() == null)
            {
                new GameObject("EventSystem").AddComponent<EventSystem>().AddComponent<StandaloneInputModule>();
            }

            var portalButton = FindObjectOfType<Canvas>().transform.GetChild(0).GetComponent<Button>();

            while (portalButton.onClick.GetPersistentEventCount() > 0)
            {
                UnityEventTools.RemovePersistentListener(portalButton.onClick, 0);
            }
            
            UnityEventTools.AddIntPersistentListener(portalButton.onClick, gameManager.SwitchPortalMode, 1);
            PrefabUtility.RecordPrefabInstancePropertyModifications(portalButton);
        }
        
        private static void UpdateCatPortals()
        {
            foreach (SerializedObject catPortal in FindObjectsOfType<Portal>()
                .Select(portal => new SerializedObject(portal))
                .Where(portal => portal.FindProperty("catPortal").boolValue))
            {
                Vector3 rotation = ((Transform) catPortal.FindProperty("myTransform").objectReferenceValue).rotation
                    .eulerAngles;
                if (rotation.x % 360 <= 90.1f && rotation.x % 360 > 89.9f)
                {
                    catPortal.FindProperty("ImpactedSurface").SetEnumValue(Surface.Floor);
                    catPortal.FindProperty("arrivalPositionOffset").vector3IntValue = new Vector3Int(1, 0, 0);
                }
                else if (rotation.y % 360 <= 270.1f && rotation.y % 360 > 269.9f)
                {
                    catPortal.FindProperty("ImpactedSurface").SetEnumValue(Surface.SideWall);
                    catPortal.FindProperty("arrivalPositionOffset").vector3IntValue = new Vector3Int(0, 0, 1);
                }
                else
                {
                    catPortal.FindProperty("ImpactedSurface").SetEnumValue(Surface.BackWall);
                    catPortal.FindProperty("arrivalPositionOffset").vector3IntValue = new Vector3Int(-1, 0, 0);
                }

                catPortal.ApplyModifiedProperties();
            }
        }
    }
}
