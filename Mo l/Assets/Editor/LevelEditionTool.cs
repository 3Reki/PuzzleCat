using System;
using System.Collections.Generic;
using System.Linq;
using PuzzleCat.Controller;
using PuzzleCat.LevelElements;
using PuzzleCat.Utils;
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
            CreateUI();
            UpdateRoomAndRoomElements();

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

        private static void CreateGameManagerAndControllers()
        {
            if (FindObjectOfType<GameManager>())
            {
                return;
            }
            
            var manager = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LevelEditing/Game Manager.prefab")).GetComponent<GameManager>();
            var serializedObjects = new List<SerializedObject>();
            
            serializedObjects.Add(new SerializedObject(manager));
            serializedObjects[0].FindProperty("cat").objectReferenceValue = FindObjectOfType<Cat>();
            serializedObjects[0].FindProperty("mainCamera").objectReferenceValue = Camera.main;
            
            Transform controllers = manager.transform.GetChild(0);
            
            serializedObjects.Add(new SerializedObject(controllers.GetComponent<CatController>()));
            serializedObjects[1].FindProperty("catDirectionIndicator").objectReferenceValue = CreateCatIndicator();
            
            serializedObjects.Add(new SerializedObject(controllers.GetComponent<MovableElementsController>()));
            serializedObjects[2].FindProperty("invisibleQuad").objectReferenceValue = CreateInvisibleQuad();
            
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
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LevelEditing/In Game Canvas.prefab"));
            }

            var menuManagerSO = new SerializedObject(FindObjectOfType<MenuManager>());
            menuManagerSO.FindProperty("portalPlacementController").objectReferenceValue = 
                FindObjectOfType<PortalPlacementController>();
            menuManagerSO.ApplyModifiedProperties();

            Transform[] portalParents = GetPortalsParentList();
            MenuManager menuManager = FindObjectOfType<MenuManager>();
            if (portalParents.Length == 0)
            {
                menuManager.transform.GetChild(1).gameObject.SetActive(false);
                menuManager.portalToggles = Array.Empty<Toggle>();
            } 
            else
            {
                menuManager.transform.GetChild(1).gameObject.SetActive(true);

                menuManager.portalToggles = new Toggle[portalParents.Length];
                GameObject[] portalTogglesGo = new GameObject[4];
                for (int i = 0; i < 4; i++)
                {
                    portalTogglesGo[i] = menuManager.transform.GetChild(2).GetChild(0).GetChild(i).gameObject;
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
                portalPlacement.PortalCountTexts[i] = menuManager.transform.GetChild(2).GetChild(0).GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(portalPlacement);

            if (FindObjectOfType<EventSystem>() == null)
            {
                new GameObject("EventSystem").AddComponent<EventSystem>().AddComponent<StandaloneInputModule>();
            }
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
