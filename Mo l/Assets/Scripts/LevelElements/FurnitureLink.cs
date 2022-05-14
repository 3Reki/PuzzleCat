using System.Collections.Generic;
using PuzzleCat.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace PuzzleCat.LevelElements
{
    public class FurnitureLink : RoomElement
    {
        [SerializeField] private NavMeshLink NavMeshLink;

        private static readonly List<FurnitureLink> _furnitureLinks = new();
        private RoomElement _roomElement;

        private Surface _surface;

        private static void UpdateLinksInstances()
        {
            foreach (FurnitureLink furnitureLink in _furnitureLinks)
            {
                furnitureLink.NavMeshLink.enabled = furnitureLink.IsValidLink();
            }
        }

        private Vector3Int GetRoomCoordinates(Vector3 point)
        {
            return CurrentRoom.WorldToRoomCoordinates(Utils.Utils.WorldPointAsGridPoint(
                _surface, transform.position + transform.TransformDirection(point)));
        }

        private bool IsValidLink()
        {
            foreach (Vector3Int point in new[]
            {
                GetRoomCoordinates(NavMeshLink.endPoint), GetRoomCoordinates(NavMeshLink.startPoint)
            })
            {
                _roomElement = CurrentRoom.GetElementAt(point);

                if (_roomElement != null && _roomElement.ImpactedSurface == Surface.All && !Cat.IsCat(_roomElement))
                {
                    return false;
                }

                _roomElement = CurrentRoom.GetElementAt(point - _surface.GetNormal(), Surface.All);

                if (_roomElement == null)
                {
                    if (CurrentRoom.AreCoordinatesValid(point - _surface.GetNormal()))
                    {
                        return false;
                    }
                        
                }
                else if (_roomElement.ImpactedSurface != Surface.All)
                {
                    return false;
                }
            }

            return true;
        }

        private void OnEnable()
        {
            if (_furnitureLinks.Count == 0)
                MovableElement.onMovement += UpdateLinksInstances;

            _furnitureLinks.Add(this);
            NavMeshLink.enabled = IsValidLink();
        }

        private void OnDisable()
        {
            _furnitureLinks.Remove(this);

            if (_furnitureLinks.Count == 0)
                MovableElement.onMovement -= UpdateLinksInstances;
        }

        private void Awake()
        {
            _surface = transform.up.ToSurface();
        }
    }
}