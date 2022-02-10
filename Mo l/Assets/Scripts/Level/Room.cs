using UnityEngine;

namespace PuzzleCat.Level
{
	public class Room : MonoBehaviour
	{
		public IMovable[] movables;

		[SerializeField] private int maxLeft;
		[SerializeField] private int maxRight;
		[SerializeField] private int maxForward;
		[SerializeField] private int maxBackward;
		[SerializeField] private GameObject[] movableGameObjects;

		public void MoveObjectLeft(IMovable movable)
		{
			if (movable.GetCoordinates().x != maxLeft)
			{
				movable.MoveLeft();
			}
		}
		
		public void MoveObjectRight(IMovable movable)
		{
			if (movable.GetCoordinates().x != maxRight)
			{
				movable.MoveRight();
			}
		}
		
		public void MoveObjectForward(IMovable movable)
		{
			if (movable.GetCoordinates().z != maxForward)
			{
				movable.MoveForward();
			}
		}
		
		public void MoveObjectBackward(IMovable movable)
		{
			if (movable.GetCoordinates().z != maxBackward)
			{
				movable.MoveBackward();
			}
		}

		private void SetMovables()
		{
			movables = new IMovable[movableGameObjects.Length];

			for (var i = 0; i < movableGameObjects.Length; i++)
			{
				movables[i] = movableGameObjects[i].GetComponent<IMovable>();
				movables[i].SetRoom(this);
			}
		}

		private void Awake()
		{
			SetMovables();
		}
	}
}