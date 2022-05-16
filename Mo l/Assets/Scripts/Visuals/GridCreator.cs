using System.Collections.Generic;
using UnityEngine;

namespace PuzzleCat.Visuals
{
    public class GridCreator : MonoBehaviour
    {
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private Material cellMat;
        [SerializeField] private float cellHeight = .05f;
        [SerializeField] private Transform cellParent;

        public Color walkableCells = Color.green;
        public Color nonWalkableCells = Color.red;
        public Color portalCells = Color.blue;
        
        [Range(0.0f, 1.0f)] public float alphaValue = 1.0f; //ici coco
        // 0.5
        //0.0

        private int _heightReceived;
        private int _widthReceived;

        private List<Material> _cellMatList;
        private List<GameObject> _cellList;
        private GameObject _cellInst;
        
        private Vector3 direction = new Vector3(0f, 1.5f, 0f);
        private float maxDist = 1.5f;

        [SerializeField] private LayerMask furnitureMask;
        [SerializeField] private LayerMask catPortalMask;

        private void Awake()
        {
            float x = Mathf.Floor(transform.localScale.x);
            float z = Mathf.Floor(transform.localScale.z);
            _heightReceived = (int)z;
            _widthReceived = (int)x;
            furnitureMask = LayerMask.GetMask("furniture");
            catPortalMask = LayerMask.GetMask("catPortal");

        }

        private void Start()
        {
            SpawnCell();
            NonWalkableCell();
            SetMatAlphaInit();
        }


        private void Update()//debug
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                VisualizeGrid(0.5f);
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                VisualizeGrid(.0f);
            }
        }

        private void VisualizeGrid(float value)
        {
            for (int i = 0; i < _cellMatList.Count; i++)
            {
                _cellMatList[i].SetFloat("_Alpha", value);
            }
        }

        private void NonWalkableCell()
        {
            _cellMatList = new List<Material>();

            for (int i = 0; i < _cellList.Count; i++)
            {
                cellMat.EnableKeyword("_CHANNELSELECTION");

                Vector3 cellPos = _cellList[i].transform.position;
                Vector3 downRay = cellPos - new Vector3(0f, 0.2f, 0f);

                

                Material matList = _cellList[i].GetComponent<MeshRenderer>().material;
                
                if (Physics.Raycast(downRay, direction, maxDist, furnitureMask))
                {
                    SetMatColor("_CHANNELSELECTION_B", "_CHANNELSELECTION_A", "null", matList, walkableCells, _cellList);
                }
                else if (Physics.Raycast(downRay, direction, maxDist, catPortalMask))
                {
                    SetMatColor("_CHANNELSELECTION_A", "_CHANNELSELECTION_C", "null", matList, walkableCells, _cellList);
                }
                else
                {
                    SetMatColor("_CHANNELSELECTION_A", "_CHANNELSELECTION_C", "_CHANNELSELECTION_B", matList, walkableCells, _cellList, true);
                }
                _cellMatList.Add(_cellList[i].GetComponent<MeshRenderer>().material);

            }
        }
        private void SpawnCell()
        {
            _cellList = new List<GameObject>();

            for (int x = 0; x < _widthReceived; x++)
            {
                for (int y = 0; y < _heightReceived; y++)
                {
                    _cellInst = Instantiate(cellPrefab, new Vector3(x, cellHeight, y), Quaternion.Euler(90, 0, 0), cellParent);
                    _cellList.Add(_cellInst);
                }
            }
            foreach (GameObject item in _cellList)
            {
                if (_heightReceived % 2 == 0 && _widthReceived % 2 == 0)
                {
                    item.transform.Translate((-_widthReceived / 2) + .5f, (-_heightReceived / 2) + .5f, 0);
                }
                else
                {
                    item.transform.Translate((-_widthReceived / 2), (-_heightReceived / 2), 0);
                }
            }
        }
        private void SetMatAlphaInit()
        {
            cellMat.SetFloat("_AlphaValue", alphaValue);
        }
        private void SetMatColor(string channelA, string channelB, string channelC, Material mat, Color color, List<GameObject> listGo, bool thirdParam = false)
        {

            // channelA = delete, channelB enable
            foreach (GameObject item in _cellList)
            {
                mat.color = walkableCells;
                mat.DisableKeyword(channelA);
                mat.EnableKeyword(channelB);

                if (thirdParam)
                {
                    mat.DisableKeyword(channelC);
                }
            }
        }
    }
}
