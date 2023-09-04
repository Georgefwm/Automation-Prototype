using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grid<TGridObject>
{
	public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
	public class OnGridObjectChangedEventArgs : EventArgs
	{
		public int x;
		public int z;
	}
	
	private int _width;
	private int _length;
	private float _cellSize;
	private Vector3 _originPosition;
	private TGridObject[][] _gridArray;
	private TextMesh[][] _debugTextArray;

	public Grid(int width, int length, float cellSize, Vector3 originPosition, Func<Grid<TGridObject>, int, int, TGridObject> gridDefaultObject)
	{
		_width = width;
		_length = length;
		_cellSize = cellSize;
		_originPosition = originPosition;

		_gridArray = new TGridObject[width][];
		_debugTextArray = new TextMesh[width][];
		for (int widthIndex = 0; widthIndex < width; widthIndex++)
		{
			_gridArray[widthIndex] = new TGridObject[length];
			_debugTextArray[widthIndex] = new TextMesh[length];
			
			for (int lengthIndex = 0; lengthIndex < length; lengthIndex++)
			{
				_gridArray[widthIndex][lengthIndex] = gridDefaultObject(this, widthIndex, lengthIndex);
			}
		}

		bool debugEnabled = true;
		if (debugEnabled)
		{
			for (int x = 0; x < _gridArray.Length; x++)
			{
				for (int z = 0; z < _gridArray[0].Length; z++)
				{
					_debugTextArray[x][z] = Utils.CreateWorldText(null, _gridArray[x][z]?.ToString(),
						CellCenterToWorld(x, z), TextAnchor.MiddleCenter,
						TextAlignment.Center, 0);

					Debug.DrawLine(CellToWorld(x, z), CellToWorld(x + 1, z), Color.white, 1000f);
					Debug.DrawLine(CellToWorld(x, z), CellToWorld(x, z + 1), Color.white, 1000f);
				}
			}

			Debug.DrawLine(CellToWorld(width, 0), CellToWorld(width, length), Color.white, 1000f);
			Debug.DrawLine(CellToWorld(0, length), CellToWorld(width, length), Color.white, 1000f);

			OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
			{
				_debugTextArray[eventArgs.x][eventArgs.z].text = _gridArray[eventArgs.x][eventArgs.z]?.ToString();
			};
		}
	}

	public float GetCellSize()
	{
		return _cellSize;
	}

	public Vector3 CellToWorld(int x, int z)
	{
		return new Vector3(x, 0, z) * _cellSize + _originPosition;
	}
	
	public Vector3 CellCenterToWorld(int x, int z)
	{
		return CellToWorld(x, z) + new Vector3(_cellSize, 0, _cellSize) * 0.5f;
	}
	
	public void WorldToCell(Vector3 worldPosition, out int x, out int z)
	{
		x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
		z = Mathf.FloorToInt((worldPosition - _originPosition).z / _cellSize);
	}

	public void SetGridObject(int x, int z, TGridObject gridObject)
	{
		if (!IsValidGridPosition(x, z)) return;
			
		_gridArray[x][z] = gridObject;
		_debugTextArray[x][z].text = _gridArray[x][z].ToString();
	}
	
	public void SetGridObject(Vector3 worldPosition, TGridObject gridObject)
	{
		int x, z;
		WorldToCell(worldPosition, out x, out z);
		
		SetGridObject(x, z, gridObject);
	}
	
	public TGridObject GetGridObject(Vector3 worldPosition)
	{
		int x, z;
		WorldToCell(worldPosition, out x, out z);

		return GetGridObject(x, z);
	}
	
	public TGridObject GetGridObject(int x, int z)
	{
		if (!IsValidGridPosition(x, z)) return default(TGridObject);
		
		return _gridArray[x][z];
	}

	public bool IsValidGridPosition(int x, int z)
	{
		return !(x < 0 || z < 0 || x >= _width || z >= _length);
	}

	public void TriggerGridObjectChanged(int x, int z)
	{
		OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, z = z });
	}
	
}
