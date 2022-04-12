using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleCat
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

        private void Awake()
        {
            float x = Mathf.Floor(transform.localScale.x);
            float z = Mathf.Floor(transform.localScale.z);
            _heightReceived = (int)z;
            _widthReceived = (int)x;
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

                LayerMask furnitureMask = LayerMask.GetMask("furniture");
                LayerMask catPortalMask = LayerMask.GetMask("catPortal");

                Material matList = _cellList[i].GetComponent<MeshRenderer>().material;

                if (Physics.Raycast(downRay, direction, maxDist, furnitureMask))
                {
                    //cellMat = cellList[i].GetComponent<MeshRenderer>().material;
                    foreach (GameObject item in _cellList)
                    {
                        

                        matList.color = nonWalkableCells;
                        matList.DisableKeyword("_CHANNELSELECTION_B");
                        matList.EnableKeyword("_CHANNELSELECTION_A");
                    }

                }
                else if (Physics.Raycast(downRay, direction, maxDist, catPortalMask))
                {
                    foreach (GameObject item in _cellList)
                    {
                        matList.color = portalCells;
                        matList.DisableKeyword("_CHANNELSELECTION_A");
                        matList.EnableKeyword("_CHANNELSELECTION_C");
                    }
                }
                else
                {
                    foreach (GameObject item in _cellList)
                    {
                        matList.color = walkableCells;
                        matList.DisableKeyword("_CHANNELSELECTION_A");
                        matList.DisableKeyword("_CHANNELSELECTION_C");
                        matList.EnableKeyword("_CHANNELSELECTION_B");
                    }
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
    }

}
