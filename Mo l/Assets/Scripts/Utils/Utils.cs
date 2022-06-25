using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PuzzleCat.Utils
{
	public static class Utils
	{
		#region ScreenToWorldFunctions

		public static Vector3 GetMouseWorldPosition()
		{
			Debug.Assert(Camera.main != null, "Camera.main != null");
			return GetMouseWorldPosition(Camera.main);
		}

		public static Vector3 GetMouseWorldPosition(Camera camera)
		{
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
			if (IsPointerOverUI())
			{
				hit = new RaycastHit();
				return false;
			}
			
			Ray ray = camera.ScreenPointToRay(screenPoint);

			Physics.Simulate(0.01f);
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

		public static bool IsPointerOverUI(EventSystem eventSystem)
		{
			if (eventSystem.IsPointerOverGameObject())
			{
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

		public static bool IsInLayerMask(GameObject obj, LayerMask layerMask)
		{
			return (layerMask.value & (1 << obj.layer)) > 0;
		}
		
		public static GameObject FindGameObjectWithLayer(int layer) 
		{
			GameObject[] goArray = Object.FindObjectsOfType<GameObject>();
			foreach (GameObject gameObject in goArray)
			{
				if (gameObject.layer == layer)
				{
					return gameObject;
				}
			}
			
			return null;
		}

		public static Vector3Int WorldPointAsGridPoint(Vector3 upDirection, Vector3 point)
		{
			return WorldPointAsGridPoint(upDirection.ToSurface(), point);
		}

		public static Vector3Int WorldPointAsGridPoint(Surface surface, Vector3 point)
		{
			return surface switch
			{
				Surface.Floor => new Vector3Int(Mathf.FloorToInt(point.x), Mathf.RoundToInt(point.y),
					Mathf.FloorToInt(point.z)),
				Surface.SideWall => new Vector3Int(Mathf.RoundToInt(point.x), Mathf.FloorToInt(point.y),
					Mathf.FloorToInt(point.z)),
				Surface.BackWall => new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y),
					Mathf.RoundToInt(point.z) - 1),
				_ => Vector3Int.back * 100
			};
		}

		public static Vector3Int[] GetDirectionVectors(Surface surface)
		{
			return new[] { surface.Up(), surface.Right(), -surface.Up(), -surface.Right() };
		}
	}

	public enum Surface
	{
		None,
		Floor,
		SideWall,
		BackWall,
		All
	}
	
	public enum Direction
	{
		Up,
		Right,
		Down,
		Left
	}
}