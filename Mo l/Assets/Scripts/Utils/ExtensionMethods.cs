using System;
using UnityEngine;

namespace PuzzleCat.Utils
{
    public static class ExtensionMethods
    {
        public static Vector3 Round(this Vector3 vector3)
        {
            return new Vector3(
                Mathf.Round(vector3.x),
                Mathf.Round(vector3.y),
                Mathf.Round(vector3.z));
        }
        
        public static Vector3Int ToVector3Int(this Vector3 vector3)
        {
            return new Vector3Int(
                Mathf.RoundToInt(vector3.x),
                Mathf.RoundToInt(vector3.y),
                Mathf.RoundToInt(vector3.z));
        }
        
        public static int Sum(this Vector3Int vector3)
        {
            return vector3.x + vector3.y + vector3.z;
        }
        
        public static Vector3 GetNormal(this Surface surface)
        {
            return surface switch
            {
                Surface.Floor => Vector3.up,
                Surface.SideWall => Vector3.right,
                Surface.BackWall => Vector3.back,
                _ => throw new ArgumentOutOfRangeException(nameof(surface), surface, null)
            };
        }
        
        public static Surface ToSurface(this Vector3 surfaceNormal)
        {
            return surfaceNormal.ToVector3Int().ToSurface();
        }
        
        public static Surface ToSurface(this Vector3Int surfaceNormal)
        {
            if (surfaceNormal == Vector3Int.up)
            {
                return Surface.Floor;
            }
            if (surfaceNormal == Vector3Int.right)
            {
                return Surface.SideWall;
            }
            if (surfaceNormal == Vector3Int.back)
            {
                return Surface.BackWall;
            }

            Debug.LogWarning("Not a plane surface");
            return Surface.None;
        }
    }
}
