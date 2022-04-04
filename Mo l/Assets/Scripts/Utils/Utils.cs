using System;
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
			return ((layerMask.value & (1 << obj.layer)) > 0);
		}

		public static Vector3Int WorldPointAsGridPoint(RaycastHit hit)
		{
			Surface surface = hit.normal.ToSurface();
			Vector3 point = hit.point;

			switch (surface)
			{
				case Surface.Floor:
					return new Vector3Int(Mathf.FloorToInt(point.x), Mathf.RoundToInt(point.y),
						Mathf.FloorToInt(point.z));
				case Surface.SideWall:
					return new Vector3Int(Mathf.RoundToInt(point.x), Mathf.FloorToInt(point.y),
						Mathf.FloorToInt(point.z));
				case Surface.BackWall:
					return new Vector3Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y),
						Mathf.RoundToInt(point.z) - 1);
				default:
					return Vector3Int.back * 100;
			}
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
}