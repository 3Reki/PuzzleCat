using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleCat
{
    public class GridSpawner : MonoBehaviour
    {
        public enum GridDirection { Floor, BackWall, SideWall }
        public GridDirection Direction;

        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private Transform _instancesParent;
        [Range(.05f, 0.2f)][SerializeField] private float _cellSurfaceSpacing = .05f;

        private GameObject _cellInstance;
        private float _x;
        private float _z;
        private int _directionIndex;
        private List<GameObject> _cellList;
        private List<Material> _materialList;

        private void Awake()
        {
            _x = (int)Mathf.Floor(transform.localScale.x);
            _z = (int)Mathf.Floor(transform.localScale.z);
            _directionIndex = (int)GridDirection.Floor;
            _cellList = new List<GameObject>();
        }
        private void Start()
        {
            SpawnDirection();
        }

        private void SpawnDirection()
        {
            
            switch(_directionIndex)
            {
                case 0:
                    SpawnCell(new Vector3(_x, _cellSurfaceSpacing, _z), Quaternion.Euler(90f, 0f, 0f), _x, _z); //floor
                    break;
            } 
        }



        private void SpawnCell(Vector3 spawnPosition, Quaternion spawnRotation, float x, float z)
        {
            for (int i = 0; i < _z; i++)
            {
                for (int j = 0; j < _x; j++)
                {
                    _cellInstance = Instantiate(_cellPrefab, new Vector3(j, _cellSurfaceSpacing, i), spawnRotation, _instancesParent);
                    _cellList.Add(_cellInstance);
                }
            }
            for (int j = 0; j < _cellList.Count; j++)
            {
                Transform transformCellInstance = _cellList[j].transform;
                
                if (x % 2 == 0 && z % 2 == 0)
                {
                    transformCellInstance.Translate((-z / 2f) + .5f, (-x / 2f), 0); //even
                }
                else if(z % 2 == 0)
                {
                    transformCellInstance.Translate((-z / 2f), (-x / 2f) + .5f, 0);
                }
            }
        }
    }
}
