using System;
using System.Collections.Generic;
using System.Linq;
using PuzzleCat.Controller;
using PuzzleCat.LevelElements;
using PuzzleCat.Menus;
using PuzzleCat.Utils;
using PuzzleCat.Visuals;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
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
            CreateGameManagerAndControllers();
            UpdatePlayerController();
            UpdateCatIndicator();
            CreateUI();
            UpdateRoomAndRoomElements();

            Scene scene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void UpdateCatIndicator()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>()
                .Where(go => Utils.Utils.IsInLayerMask(go, 1 << LayerMask.NameToLayer("Invisible")) && go.CompareTag("Indicator"))
                .Where(gameObject => gameObject.scene.name != null))
            {
                DestroyImmediate(gameObject);
            }

            // TODO var movementIndicatorSO = new SerializedObject(FindObjectOfType<CatController>());
            // movementIndicatorSO.FindProperty("movementIndicator").objectReferenceValue = CreateCatIndicator();
            // movementIndicatorSO.ApplyModifiedProperties();
        }

        private static void CreateAndBakeNavMeshes()
        {
            if (FindObjectOfType<NavMeshSurface>() == null)
            {
                PrefabUtility.InstantiatePrefab(
                    AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Navigation Meshes.prefab"));
            }

            var navMeshSurfaces = FindObjectsOfType<NavMeshSurface>();

            foreach (NavMeshSurface navMeshSurface in navMeshSurfaces)
            {
                navMeshSurface.BuildNavMesh();
                PrefabUtility.RecordPrefabInstancePropertyModifications(navMeshSurface);
            }
        }

        private static void CreateGameManagerAndControllers()
        {
            var manager = FindObjectOfType<GameManager>();
            
            if (manager == null)
            {
                manager = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Game Manager.prefab")).GetComponent<GameManager>();
            }

            var serializedObjects = new List<SerializedObject> {new SerializedObject(manager)};

            serializedObjects[0].FindProperty("cat").objectReferenceValue = FindObjectOfType<Cat>();
            serializedObjects[0].FindProperty("mainCamera").objectReferenceValue = Camera.main;
            
            Transform controllers = manager.transform.GetChild(0);
            
            //serializedObjects.Add(new SerializedObject(controllers.GetComponent<CatController>()));
            serializedObjects[1].FindProperty("movementIndicator").objectReferenceValue = CreateCatIndicator();

            serializedObjects.Add(new SerializedObject(controllers.GetComponent<MovableElementsController>()));
            serializedObjects[2].FindProperty("invisibleQuad").objectReferenceValue = CreateInvisibleQuad();
            serializedObjects[2].FindProperty("movableElementDirectionIndicator").objectReferenceValue = CreateMovementIndicator();
            
            serializedObjects.Add(new SerializedObject(controllers.GetComponent<PortalPlacementController>()));
            serializedObjects[3].FindProperty("portalsParentTransform").SetAsTransformArray(GetPortalsParentList());
            
            serializedObjects.Add(new SerializedObject(controllers.GetComponent<CameraController>()));
            serializedObjects[4].FindProperty("cameraTransform").objectReferenceValue = Camera.main.transform;
            serializedObjects[4].FindProperty("camera").objectReferenceValue = Camera.main;

            foreach (SerializedObject serializedObject in serializedObjects)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
        
        private static void CreateUI()
        {
            if (FindObjectOfType<Canvas>() == null)
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/In Game Canvas.prefab"));
            }

            var menuManagerSO = new SerializedObject(FindObjectOfType<MenuManager>());
            menuManagerSO.FindProperty("portalPlacementController").objectReferenceValue = 
                FindObjectOfType<PortalPlacementController>();
            menuManagerSO.ApplyModifiedProperties();

            Transform[] portalParents = GetPortalsParentList();
            MenuManager menuManager = FindObjectOfType<MenuManager>();
            if (portalParents.Length == 0)
            {
                menuManager.transform.Cast<Transform>().First(child => child.CompareTag("PortalBook")).gameObject.SetActive(false);
                menuManager.portalToggles = Array.Empty<Toggle>();
            } 
            else
            {
                menuManager.transform.Cast<Transform>().First(child => child.CompareTag("PortalBook")).gameObject.SetActive(true);
                menuManager.portalToggles = new Toggle[portalParents.Length];
                GameObject[] portalTogglesGo = new GameObject[4];
                for (int i = 0; i < 4; i++)
                {
                    portalTogglesGo[i] = GameObject.FindGameObjectWithTag("PortalTogglesParent").transform.GetChild(i)
                        .gameObject;
                }

                int greyOffset = 0;
                if (portalParents.Any(parent => parent.GetChild(0).GetComponent<Portal>().GreyPortal))
                {
                    portalTogglesGo[0].SetActive(true);
                    menuManager.portalToggles[^1] = portalTogglesGo[0].GetComponent<Toggle>();
                    greyOffset = 1;
                }
                else
                {
                    portalTogglesGo[0].SetActive(false);
                }

                for (int i = 0; i < portalParents.Length - greyOffset; i++)
                {
                    portalTogglesGo[i + 1].SetActive(true);
                    RectTransform toggleRectTransform = (RectTransform) portalTogglesGo[i + 1].transform;
                    
                    toggleRectTransform.anchorMin = new Vector2(
                        0.046f + 0.225f * (i + greyOffset), toggleRectTransform.anchorMin.y);
                    
                    toggleRectTransform.anchorMax = new Vector2(
                        0.046f + 0.225f * (i + 1 + greyOffset), toggleRectTransform.anchorMax.y);
                    
                    toggleRectTransform.anchoredPosition = Vector2.zero;
                    menuManager.portalToggles[i] = portalTogglesGo[i + 1].GetComponent<Toggle>();
                }

                for (int i = portalParents.Length + 1 - greyOffset; i < 4; i++)
                {
                    portalTogglesGo[i].SetActive(false);
                }
                
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(menuManager);

            PortalPlacementController portalPlacement = FindObjectOfType<PortalPlacementController>();

            portalPlacement.PortalCountTexts = new TextMeshProUGUI[4];
            for (int i = 0; i < 4; i++)
            {
                portalPlacement.PortalCountTexts[i] = GameObject.FindGameObjectWithTag("PortalTogglesParent").transform
                    .GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(portalPlacement);

            if (FindObjectOfType<EventSystem>() == null)
            {
                new GameObject("EventSystem").AddComponent<EventSystem>().AddComponent<StandaloneInputModule>();
            }
        }
        
        private static void UpdatePlayerController()
        {
            SerializedObject playerControllerSO = new SerializedObject(FindObjectOfType<PlayerController>());
            playerControllerSO.FindProperty("furnitureSelectionIndicator").objectReferenceValue =
                FindObjectOfType<FurnitureSelectionIndicator>();
            playerControllerSO.FindProperty("holdTouchThreshold").floatValue = 0.3f;

            playerControllerSO.ApplyModifiedProperties();
        }

        private static Transform[] GetPortalsParentList()
        {
            var portalsParents = new HashSet<Transform>();
            
            foreach (Portal portal in Resources.FindObjectsOfTypeAll<Portal>())
            {
                if (portal.gameObject.scene.name == null || portal.CatPortal)
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
                .Where(go => Utils.Utils.IsInLayerMask(go, 1 << LayerMask.NameToLayer("Invisible")) && !go.CompareTag("Indicator"))
                .Where(gameObject => gameObject.scene.name != null && gameObject != quad))
            {
                DestroyImmediate(gameObject);
            }

            return quad;
        }

        private static MovableElementDirectionIndicator CreateMovementIndicator()
        {
            if (FindObjectOfType<MovableElementDirectionIndicator>())
            {
                return FindObjectOfType<MovableElementDirectionIndicator>();
            }

            return PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/Prefabs/Movable Furnitures Indicator.prefab")).GetComponent<MovableElementDirectionIndicator>();
        }

        private static PlayerMovementIndicator CreateCatIndicator()
        {
            PlayerMovementIndicator indicator = Resources.FindObjectsOfTypeAll<PlayerMovementIndicator>()
                .FirstOrDefault(indicator => indicator.gameObject.scene.name != null);
            if (indicator != null)
            {
                return indicator;
            }

            GameObject indicatorGO = (GameObject) PrefabUtility.InstantiatePrefab(
                AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/Art/Sprites/UI_Sprites/UI_InGame/UI_Shader/ICON_MovementIndicator.prefab"));
            
            indicatorGO.SetActive(false);

            return indicatorGO.GetComponent<PlayerMovementIndicator>();
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

            foreach (SerializedObject movable in FindObjectsOfType<MovableElement>()
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
    }
}
