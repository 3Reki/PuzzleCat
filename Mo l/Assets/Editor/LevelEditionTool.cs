using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PuzzleCat.Level;
using PuzzleCat.Utils;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace PuzzleCat.Editor
{
    public class LevelEditionTool : MonoBehaviour
    {
        [MenuItem("Level Edition/Link Scripts")]
        public static void LinkScripts()
        {
            if (FindObjectOfType<NavMeshSurface>() == null)
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LevelEditing/Navigation Meshes.prefab"));
            }
            
            var navMeshSurfaces = FindObjectsOfType<NavMeshSurface>();

            foreach (NavMeshSurface navMeshSurface in navMeshSurfaces)
            {
                navMeshSurface.BuildNavMesh();
                PrefabUtility.RecordPrefabInstancePropertyModifications(navMeshSurface);
            }
            
            var inputManager = FindObjectOfType<InputManager>();
            
            if (inputManager == null)
            {
                inputManager = new GameObject("Game Manager").AddComponent<InputManager>();
            }
            
            inputManager.Init(Camera.main, LayerMask.GetMask(new []{"Selectable"}), FindObjectOfType<Cat>(), 15, GetPortalsParentList());

            Room[] rooms = FindObjectsOfType<Room>();

            foreach (Room room in rooms)
            {
                room.transform.position = room.gridWorldPosition + new Vector3(room.gridSize.x * 0.5f, 0, room.gridSize.z * 0.5f);
                room.transform.GetChild(1).localPosition = new Vector3(0, room.gridSize.y * 0.5f, room.gridSize.z * 0.5f);
                room.transform.GetChild(2).localPosition = new Vector3(-room.gridSize.x * 0.5f, room.gridSize.y * 0.5f, 0);
                room.transform.GetChild(0).localScale = new Vector3(room.gridSize.x, room.gridSize.z, 1);
                room.transform.GetChild(1).localScale = new Vector3(room.gridSize.x, room.gridSize.y, 1);
                room.transform.GetChild(2).localScale = new Vector3(room.gridSize.z, room.gridSize.y, 1);
                room.Init();
            }

            foreach (RoomElement roomElement in FindObjectsOfType<RoomElement>())
            {
                foreach (Room room in rooms.Where(room => room.AreCoordinatesValid(roomElement.WorldGridPosition)))
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
            
            if (FindObjectOfType<Canvas>() == null)
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LevelEditing/In Game Canvas.prefab"));
            }

            if (FindObjectOfType<EventSystem>() == null)
            {
                new GameObject("EventSystem").AddComponent<EventSystem>().AddComponent<StandaloneInputModule>();
            }

            Transform canvas = FindObjectOfType<Canvas>().transform;
            var portalButton = canvas.GetChild(0).GetComponent<Button>();
            portalButton.onClick.RemoveAllListeners();
            portalButton.onClick.AddListener(() => inputManager.SwitchPortalMode(1));
            PrefabUtility.RecordPrefabInstancePropertyModifications(portalButton);
            
            foreach (SerializedObject catPortal in FindObjectsOfType<Portal>()
                .Select(portal => new SerializedObject(portal))
                .Where(portal => portal.FindProperty("catPortal").boolValue))
            {
                Vector3 rotation = ((Transform) catPortal.FindProperty("myTransform").objectReferenceValue).rotation.eulerAngles;
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

            Scene scene = SceneManager.GetActiveScene();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
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
    }
}
