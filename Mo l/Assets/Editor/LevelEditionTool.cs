using System.Collections.Generic;
using System.Linq;
using PuzzleCat.Level;
using PuzzleCat.Utils;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
                Vector3Int size = new Vector3(room.transform.GetChild(0).localScale.x,
                    room.transform.GetChild(1).localScale.y, room.transform.GetChild(0).localScale.y).ToVector3Int();
                Vector3 position = room.transform.position;
                room.Init(
                    new Vector3(position.x - size.x * 0.5f, position.y,
                        position.z - size.z * 0.5f).ToVector3Int(), size);
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
            
            if (FindObjectOfType<Canvas>() == null)
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/LevelEditing/In Game Canvas.prefab"));
            }

            Transform canvas = FindObjectOfType<Canvas>().transform;
            canvas.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            canvas.GetChild(0).GetComponent<Button>().onClick.AddListener(() => inputManager.SwitchPortalMode(1));

            var scene = SceneManager.GetActiveScene();
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
