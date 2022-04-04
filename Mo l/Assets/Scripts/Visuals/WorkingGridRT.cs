using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleCat
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


        private int heightReceived;
        private int widthReceived;

        private List<Material> cellMatList;
        private List<GameObject> cellList;
        private GameObject cellInst;

        private Vector3 direction = new Vector3(0f, 1.5f, 0f);
        private float maxDist = 1.5f;


        
        private void Awake()
        {
            float x = Mathf.Floor(transform.localScale.x);
            float z = Mathf.Floor(transform.localScale.z);
            heightReceived = (int)z;
            widthReceived = (int)x;
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
                for (int i = 0; i < cellList.Count; i++)
                {
                    //cellInst.gameObject.
                }             
            }
            NonWalkableCell();
            setMatAlpha();
        }
        private void NonWalkableCell()
        {
            for (int i = 0; i < cellList.Count; i++)
            {
                cellMat.EnableKeyword("_CHANNELSELECTION");

                Vector3 cellPos = cellList[i].transform.position;
                Vector3 downRay = cellPos - new Vector3(0f, 0.2f, 0f);

                LayerMask furnitureMask = LayerMask.GetMask("furniture");
                LayerMask catPortalMask = LayerMask.GetMask("catPortal");

                if (Physics.Raycast(downRay, direction, maxDist, furnitureMask))
                {
                    //cellMat = cellList[i].GetComponent<MeshRenderer>().material;
                    foreach (GameObject item in cellList)
                    {
                        cellList[i].GetComponent<MeshRenderer>().material.color = nonWalkableCells;
                    }

                }
                else if (Physics.Raycast(downRay, direction, maxDist, catPortalMask))
                {
                    foreach (GameObject item in cellList)
                    {
                        cellList[i].GetComponent<MeshRenderer>().material.color = portalCells;
                    }
                }
                else
                {
                    foreach (GameObject item in cellList)
                    {
                        cellList[i].GetComponent<MeshRenderer>().material.color = walkableCells;
                    }
                }
            }
        }

        private void spawnCell()
        {
            cellList = new List<GameObject>();

            for (int x = 0; x < widthReceived; x++)
            {
                for (int y = 0; y < heightReceived; y++)
                {
                    cellInst = Instantiate(cellPrefab, new Vector3(x, 0.05f, y), Quaternion.Euler(90, 0, 0), parent.transform);
                    cellList.Add(cellInst);
                }
            }
            foreach (GameObject item in cellList)
            {
                if (heightReceived % 2 == 0 && widthReceived % 2 == 0)
                {
                    item.transform.Translate((-widthReceived / 2) + .5f, (-heightReceived / 2) + .5f, 0);
                }
                else
                {
                    item.transform.Translate((-widthReceived / 2), (-heightReceived / 2), 0);
                }
            }
        }

        private void setMatAlpha()
        {
            cellMat.SetFloat("_AlphaValue", alphaValue);
        }
    }
}

