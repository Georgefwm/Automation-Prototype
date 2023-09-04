using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlaceableObjectSO : ScriptableObject
{
	public string nameString;
	public Transform prefab;
	public Transform visual;
	public int width = 1;
	public int length = 1;
	public Vector2Int inputPosition;
	public Vector2Int outputPosition;
	public GameObject UIPrefab;

	public enum Direction
	{
		Backward,
		Left,
		Forward,
		Right,
	}
	
	public static Direction GetNextDirection(Direction direction)
	{
		switch (direction)
		{
			case Direction.Forward: return Direction.Right;
			case Direction.Right: return Direction.Backward;
			case Direction.Backward: return Direction.Left;
			case Direction.Left: return Direction.Forward;
		}

		return Direction.Forward;
	}

	public int DirectionToRotation(Direction direction)
	{
		switch (direction)
		{
			case Direction.Backward: return 0;
			case Direction.Left: return 90;
			case Direction.Forward: return 180;
			case Direction.Right: return 270;
		}

		return 0;
	}

	public Vector2Int GetRelativeRotatedPosition(Vector2Int objectCenter, Direction direction, Vector2Int location)
	{
		Vector2Int rotatedLocation = location;

		if (direction is Direction.Left or Direction.Right)
		{
			rotatedLocation = new Vector2Int(location.y, location.x);
		}
		
		return objectCenter + rotatedLocation * GetGridForwardVector(direction);
	}

	public Vector2Int GetRotationOffset(Direction direction)
	{
		switch (direction)
		{
			case Direction.Backward: return new Vector2Int(0, 0);
			case Direction.Left:     return new Vector2Int(0, width);
			case Direction.Forward:  return new Vector2Int(width, length);
			case Direction.Right:    return new Vector2Int(length, 0);
		}

		return new Vector2Int(0, 0);
	}

	public static Vector2Int GetGridForwardVector(Direction direction) {
		switch (direction) {
			default:
			case Direction.Backward: return new Vector2Int( 0, -1);
			case Direction.Left:     return new Vector2Int(-1,  0);
			case Direction.Forward:  return new Vector2Int( 0, 1);
			case Direction.Right:    return new Vector2Int(1,  0);
		}
	}

	// Assumes that all placed objects have an odd height/width
	public Vector2Int GetCenterCellPosition(Vector2Int offset, Direction direction)
	{
		switch (direction)
		{
			case Direction.Backward:
			case Direction.Forward: return offset + new Vector2Int((width + 1) / 2 - 1, (length + 1) / 2 - 1);
			case Direction.Left:
			case Direction.Right: return offset + new Vector2Int((length + 1) / 2 - 1, (width + 1) / 2 - 1);
		}
		
		return offset + new Vector2Int((width + 1) / 2 - 1, (length + 1) / 2 - 1);
	}

	public List<Vector2Int> GetGridPositions(Vector2Int offset, Direction direction)
	{
		List<Vector2Int> gridPositions = new List<Vector2Int>();
		
		switch (direction)
		{
			case Direction.Backward:
			case Direction.Forward:
			{
				for (int x = 0; x < width; x++)
				{
					for (int z = 0; z < length; z++)
					{
						gridPositions.Add((offset + new Vector2Int(x, z)));
					}
				}
				break;
			}
			case Direction.Left:
			case Direction.Right:
			{
				for (int x = 0; x < length; x++)
				{
					for (int z = 0; z < width; z++)
					{
						gridPositions.Add((offset + new Vector2Int(x, z)));
					}
				}
				break;
			}
		}

		return gridPositions;
	}
}
