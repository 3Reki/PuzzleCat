using System;
using System.Collections.Generic;
using PuzzleCat.Level;
using UnityEditor;
using UnityEngine;

namespace PuzzleCat.Utils
{
    public static class ExtensionMethods
    {
        #region Vector3Int

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

        public static Vector3Int GetNormal(this Surface surface)
        {
            return surface switch
            {
                Surface.Floor => Vector3Int.up,
                Surface.SideWall => Vector3Int.right,
                Surface.BackWall => Vector3Int.back,
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

        #endregion

        #region SerializedProperty

        public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty property)
        {
            property = property.Copy();
            var nextElement = property.Copy();
            bool hasNextElement = nextElement.NextVisible(false);
            if (!hasNextElement)
            {
                nextElement = null;
            }

            property.NextVisible(true);
            while (true)
            {
                if ((SerializedProperty.EqualContents(property, nextElement)))
                {
                    yield break;
                }

                yield return property;

                bool hasNext = property.NextVisible(false);
                if (!hasNext)
                {
                    break;
                }
            }
        }
        
        public static SingleMovable[] GetAsSingleMovableArray(this SerializedProperty prop)
        {
            if (prop == null) throw new System.ArgumentNullException("prop");
            if (!prop.isArray) throw new System.ArgumentException("SerializedProperty does not represent an Array.", "prop");

            var arr = new SingleMovable[prop.arraySize];
            for(int i = 0; i < arr.Length; i++)
            {
                arr[i] = (SingleMovable) prop.GetArrayElementAtIndex(i).objectReferenceValue;
            }
            return arr;
        }

        public static void SetAsSingleMovableArray(this SerializedProperty prop, SingleMovable[] arr)
        {
            if (prop == null) throw new System.ArgumentNullException("prop");
            if (!prop.isArray) throw new System.ArgumentException("SerializedProperty does not represent an Array.", "prop");

            int sz = arr?.Length ?? 0;
            prop.arraySize = sz;
            for(int i = 0; i < sz; i++)
            {
                prop.GetArrayElementAtIndex(i).objectReferenceValue = arr[i];
            }
        }

        #endregion
    }
}
