using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PuzzleCat.Utils
{
    public static class UtilsClass
    {
        #region ScreenToWorldFunctions

        public static Vector3 GetMouseWorldPosition() {
            Debug.Assert(Camera.main != null, "Camera.main != null");
            return GetMouseWorldPosition(Camera.main);
        }
        
        public static Vector3 GetMouseWorldPosition(Camera camera) {
            Vector3 vec = camera.ScreenToWorldPoint(Input.mousePosition);
            return vec;
        }

        public static bool ScreenPointRaycast(Vector3 screenPoint, out RaycastHit hit)
        {
            Debug.Assert(Camera.main != null, "Camera.main != null");
            return ScreenPointRaycast(screenPoint, out hit, Camera.main, LayerMask.GetMask("Default"));
        }
        
        public static bool ScreenPointRaycast(Vector3 screenPoint, out RaycastHit hit, LayerMask layerMask)
        {
            Debug.Assert(Camera.main != null, "Camera.main != null");
            return ScreenPointRaycast(screenPoint, out hit, Camera.main, layerMask);
        }

        public static bool ScreenPointRaycast(Vector3 screenPoint, out RaycastHit hit, Camera camera)
            => ScreenPointRaycast(screenPoint, out hit, camera, -5);

        public static bool ScreenPointRaycast(Vector3 screenPoint, out RaycastHit hit, Camera camera,
            LayerMask layerMask, float maxDistance = 100, bool drawRay = false, float drawRayDuration = 0f)
        {
            Ray ray = camera.ScreenPointToRay(screenPoint);
			
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                if (drawRay)
                {
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, drawRayDuration);
                }
                
                return true;
            }

            if (drawRay)
            {
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, drawRayDuration);
            }
            
            return false;
        }

        #endregion

        public static bool IsPointerOverUI() => IsPointerOverUI(EventSystem.current);
        
        public static bool IsPointerOverUI(EventSystem eventSystem) {
            if (eventSystem.IsPointerOverGameObject()) {
                return true;
            } 
            
            var pe = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };
            var hits = new List<RaycastResult>();
            eventSystem.RaycastAll(pe, hits);
            return hits.Count > 0;
        }
        
        /*
        public static TextMeshPro CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3),
            Vector2 sizeDelta = default(Vector2), int fontSize = 40, Color? color = null, TextAnchor anchor = TextAnchor.UpperLeft, 
            TextAlignmentOptions alignment = TextAlignmentOptions.Left, int sortingOrder = 5000)
        {
            color ??= Color.white;

            return CreateWorldText(parent, text, localPosition, sizeDelta, fontSize, (Color) color, anchor, alignment, sortingOrder);
        }
        
        public static TextMeshPro CreateWorldText(Transform parent, string text, Vector3 localPosition, Vector2 sizeDelta, 
            int fontSize, Color color, TextAnchor anchor, TextAlignmentOptions alignment, int sortingOrder)
        {
            var go = new GameObject("World Text", typeof(TextMeshPro));
            var transform = (RectTransform) go.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.sizeDelta = sizeDelta;
            var textMesh = go.GetComponent<TextMeshPro>();
            
            textMesh.alignment = alignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }
        
        public static void DrawLine(Vector3 start, Vector3 end, Color color, Material mat, Transform parent = null)
        {
            var lineGO = new GameObject("Line");
            var transform = lineGO.transform;
            transform.position = start;
            transform.SetParent(parent);
            lineGO.AddComponent<LineRenderer>();
            var lr = lineGO.GetComponent<LineRenderer>();
            lr.material = mat;
            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }
        
        public static float Sum(List<float> floatList)
        {
            float sum = 0;

            foreach (var nbr in floatList)
            {
                sum += nbr;
            }

            return sum;
        }

        public static float ToPercentage(float nbr, float total) => nbr / total * 100;

        public static List<float> ToPercentageList(List<float> floatList)
        {
            float total = Sum(floatList);
            List<float> percentageList = new List<float>();

            foreach (float nbr in floatList)
            {
                percentageList.Add(ToPercentage(nbr, total));
            }

            return percentageList;
        }

        public static float GetListAverage(List<float> floatList)
        {
            return Sum(floatList) / floatList.Count;
        }

        public static float GetAverageHeight(List<Vector3> positionList)
        {
            List<float> floatList = new List<float>();

            foreach (var pos in positionList)
            {
                floatList.Add(pos.y);
            }

            return GetListAverage(floatList);
        }

        public static Vector3 RandomGroundedCirclePoint(float range, float baseHeight)
        {
            Vector2 randomCirclePoint = Random.insideUnitCircle * range;
            Vector3 currentPoint = new Vector3(randomCirclePoint.x, baseHeight + range, randomCirclePoint.y);

            if (Physics.Raycast(currentPoint, Vector3.down, out RaycastHit hit))
            {
                //Debug.DrawRay(currentPoint, Vector3.down * hit.distance, Color.green, 3);

                currentPoint.y = baseHeight - (hit.distance - range);
            }
            else
            {
                //Debug.DrawRay(currentPoint, Vector3.down * 1000, Color.red, 3);

                currentPoint = Vector3.zero;
            }
            
            //Debug.DrawRay(currentPoint, Vector3.right * 3, Color.yellow, 3);
            
            return currentPoint;
        }
        
        */
    }
}
