using System.Collections.Generic;
using UnityEngine;

namespace PuzzleCat.Visuals
{
    public class GridCreator : MonoBehaviour
    {
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

        private Vector3 direction = new Vector3(0f, 1.5f, 0f);
        private float maxDist = 1.5f;

        //private float minAlpha = 0.2f;
        //private float maxAlpha = 0.7f;

        private LayerMask furnitureMask;
        private LayerMask catPortalMask;


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
            SetMatAlpha();
        }

        private void NonWalkableCell()
        {
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
            }
        }

        private void SpawnCell()
        {
            _cellList = new List<GameObject>();

            for (int x = 0; x < _widthReceived; x++)
            {
                for (int y = 0; y < _heightReceived; y++)
                {
                    _cellInst = Instantiate(cellPrefab, new Vector3(x, 0.05f, y), Quaternion.Euler(90, 0, 0));
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

        private void SetMatAlpha()
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
