using PuzzleCat.LevelElements;
using UnityEditor;
using UnityEngine;

namespace PuzzleCat.Editor
{
    [CustomEditor(typeof(Room))]
    [CanEditMultipleObjects]
    public class RoomEditor : UnityEditor.Editor
    {
        private SerializedProperty _gridWorldPosition;
        private SerializedProperty _gridSize;
        private bool _editorChanged;

        void OnEnable()
        {
            _gridWorldPosition = serializedObject.FindProperty("gridWorldPosition");
            _gridSize = serializedObject.FindProperty("gridSize");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            serializedObject.Update();
            EditorGUILayout.PropertyField(_gridWorldPosition);
            EditorGUILayout.PropertyField(_gridSize);

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                _editorChanged = true;
            }
        }

        public void OnSceneGUI()
        {
            if (!_editorChanged) return;
            
            _editorChanged = false;
            var room = (Room) target;
            Undo.RecordObjects(new Object[]{room.transform, room.transform.GetChild(0), room.transform.GetChild(1), room.transform.GetChild(2)}, "Room Transform");

            Vector3Int gridSize = _gridSize.vector3IntValue;
                
            room.transform.position = _gridWorldPosition.vector3IntValue + new Vector3(gridSize.x * 0.5f, 0, gridSize.z * 0.5f);
            room.transform.GetChild(1).localPosition = new Vector3(0, gridSize.y * 0.5f, gridSize.z * 0.5f);
            room.transform.GetChild(2).localPosition = new Vector3(-gridSize.x * 0.5f, gridSize.y * 0.5f, 0);
            room.transform.GetChild(0).localScale = new Vector3(gridSize.x, gridSize.z, 1);
            room.transform.GetChild(1).localScale = new Vector3(gridSize.x, gridSize.y, 1);
            room.transform.GetChild(2).localScale = new Vector3(gridSize.z, gridSize.y, 1);
            
            
        }
        
    }
}
