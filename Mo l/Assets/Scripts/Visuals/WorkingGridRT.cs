using System.Collections.Generic;
using UnityEngine;

namespace PuzzleCat.Visuals
{
[ExecuteInEditMode]
    public class WorkingGridRT : MonoBehaviour
    {
        public bool action = false;

        [SerializeField]
        private GameObject parent;
        [SerializeField]
        private GameObject cellPrefab;
        [SerializeField]
        private Material cellMat;

        public Color walkableCells = Color.green;
        public Color nonWalkableCells = Color.red;
        public Color portalCells = Color.blue;
        [Range(0.0f, 1.0f)]
        public float alphaValue = 1.0f;


        private int _heightReceived;
        private int _widthReceived;

        private List<Material> _cellMatList;
        private List<GameObject> _cellList;
        private GameObject _cellInst;

        private Vector3 _direction = new Vector3(0f, 1.5f, 0f);
        private float _maxDist = 1.5f;


        
        private void Awake()
        {
            float x = Mathf.Floor(transform.localScale.x);
            float z = Mathf.Floor(transform.localScale.z);
            _heightReceived = (int)z;
            _widthReceived = (int)x;
        }
        
        private void Start()
        {
            if (action == true)
            {
                spawnCell();
                NonWalkableCell();
                setMatAlpha();
            }
        }
        
        private void Update()
        {
            if (action == false)
            {
                for (int i = 0; i < _cellList.Count; i++)
                {
                    //cellInst.gameObject.
                }             
            }
            NonWalkableCell();
            setMatAlpha();
        }
        private void NonWalkableCell()
        {
            for (int i = 0; i < _cellList.Count; i++)
            {
                cellMat.EnableKeyword("_CHANNELSELECTION");

                Vector3 cellPos = _cellList[i].transform.position;
                Vector3 downRay = cellPos - new Vector3(0f, 0.2f, 0f);

                LayerMask furnitureMask = LayerMask.GetMask("furniture");
                LayerMask catPortalMask = LayerMask.GetMask("catPortal");

                if (Physics.Raycast(downRay, _direction, _maxDist, furnitureMask))
                {
                    //cellMat = cellList[i].GetComponent<MeshRenderer>().material;
                    foreach (GameObject item in _cellList)
                    {
                        _cellList[i].GetComponent<MeshRenderer>().material.color = nonWalkableCells;
                    }

                }
                else if (Physics.Raycast(downRay, _direction, _maxDist, catPortalMask))
                {
                    foreach (GameObject item in _cellList)
                    {
                        _cellList[i].GetComponent<MeshRenderer>().material.color = portalCells;
                    }
                }
                else
                {
                    foreach (GameObject item in _cellList)
                    {
                        _cellList[i].GetComponent<MeshRenderer>().material.color = walkableCells;
                    }
                }
            }
        }

        private void spawnCell()
        {
            _cellList = new List<GameObject>();

            for (int x = 0; x < _widthReceived; x++)
            {
                for (int y = 0; y < _heightReceived; y++)
                {
                    _cellInst = Instantiate(cellPrefab, new Vector3(x, 0.05f, y), Quaternion.Euler(90, 0, 0), parent.transform);
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

        private void setMatAlpha()
        {
            cellMat.SetFloat("_AlphaValue", alphaValue);
        }
    }
}

