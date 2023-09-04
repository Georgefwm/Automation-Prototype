using System;
using System.Collections;
using System.Collections.Generic;
using PlaceableObjects;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class GridBuildingSystem : MonoBehaviour
{
	public static GridBuildingSystem Instance { get; private set; }

	public event EventHandler OnSelectedChanged;
	public event EventHandler OnObjectPlaced;

	[SerializeField] private int gridWidth = 10;
	[SerializeField] private int gridLength = 10;
	[SerializeField] private int cellSize = 2;
	
	[SerializeField] private List<PlaceableObjectSO> placeableObjectSOList;
	
	private PlaceableObjectSO _selectedPlaceableObjectSo;
	private Grid<GridObject> _grid;
	private PlaceableObjectSO.Direction _direction = PlaceableObjectSO.Direction.Backward;

	private PlacedObject _lastObjectPlaced;

	private void Awake()
	{
		Instance = this;

		_grid = new Grid<GridObject>(gridWidth, gridLength, cellSize, Vector3.zero,
			(Grid<GridObject> grid, int x, int z) => new GridObject(grid, x, z));

		_selectedPlaceableObjectSo = null;
	}

	public class GridObject
	{
		private Grid<GridObject> _grid;
		private int _x;
		private int _z;
		private PlacedObject _placedObject;

		public GridObject(Grid<GridObject> grid, int x, int z)
		{
			_grid = grid;
			_x = x;
			_z = z;
		}
		
		public void SetPlacedObject(PlacedObject placedObject)
		{
			_placedObject = placedObject;
			_grid.TriggerGridObjectChanged(_x, _z);
		}

		public PlacedObject GetPlacedObject()
		{
			return _placedObject;
		}

		public void ClearPlacedObject()
		{
			_placedObject = null;
			_grid.TriggerGridObjectChanged(_x, _z);
		}

		public bool CanBuild()
		{
			return _placedObject is null;
		}

		public override string ToString()
		{
			return _x + ", " + _z + "\n" + CanBuild().ToString();
		}
	}

	public int GetCellSize()
	{
		return cellSize;
	}
	
	public bool IsPlacing()
	{
		return _selectedPlaceableObjectSo is not null;
	}

	public PlacedObject GetObjectAt(Vector2Int position)
	{
		if (!_grid.IsValidGridPosition(position.x, position.y)) return null;
		
		return _grid.GetGridObject(position.x, position.y).GetPlacedObject();
	}

	public Vector2Int GetGridPosition(Vector3 worldPosition)
	{
		_grid.WorldToCell(worldPosition, out int x, out int z);
		return new Vector2Int(x, z);
	}

	public Vector3 GetWorldPosition(Vector2Int gridPosition)
	{
		return _grid.CellCenterToWorld(gridPosition.x, gridPosition.y);
	}

	public Vector3 GetMouseWorldSnappedPosition()
	{
		Vector3 mousePosition = Utils.GetMouseWorldPosition3D();
		_grid.WorldToCell(mousePosition, out int x, out int z);
		
		if (_selectedPlaceableObjectSo is null) return mousePosition;
		
		Vector2Int rotationOffset = _selectedPlaceableObjectSo.GetRotationOffset(_direction);

		Vector3 placedObjectWorldPosition = new Vector3(rotationOffset.x, 0f, rotationOffset.y) * _grid.GetCellSize() + _grid.CellToWorld(x, z);

		return placedObjectWorldPosition;
	}

	public Quaternion GetPlacedObjectRotation()
	{
		if (_selectedPlaceableObjectSo is null) return Quaternion.identity;
		
		return Quaternion.Euler(0, _selectedPlaceableObjectSo.DirectionToRotation(_direction), 0);
	}
	
	public PlaceableObjectSO GetSelectedPlaceableObject() {
		return _selectedPlaceableObjectSo;
	}

	private void DeselectObjectType()
	{
		_selectedPlaceableObjectSo = null;
		RefreshSelectedObjectType();
	}

	private void RefreshSelectedObjectType()
	{
		OnSelectedChanged?.Invoke(this, EventArgs.Empty);
	}
	
	public PlacedObject GetLastPlacedObject()
	{
		return _lastObjectPlaced;
	}

	public void PlaceObject(Vector2Int desiredGridPosition, PlaceableObjectSO.Direction desiredDirection, PlaceableObjectSO desiredObjectSo)
	{
		List<Vector2Int> gridPositions = desiredObjectSo.GetGridPositions(desiredGridPosition, desiredDirection);

		GridObject gridObject = _grid.GetGridObject(desiredGridPosition.x, desiredGridPosition.y);

		if (gridObject == null) return;

		foreach (Vector2Int gridPosition in gridPositions)
		{
			if (!_grid.IsValidGridPosition(gridPosition.x, gridPosition.y))
			{
				Debug.Log("Out of bounds!");
				return;
			}

			if (!_grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
			{
				Debug.Log("Can't build here!");
				return;
			}
		}

		Vector2Int rotationOffset = desiredObjectSo.GetRotationOffset(desiredDirection);

		Vector3 placedObjectWorldPosition = new Vector3(rotationOffset.x, 0f, rotationOffset.y) * _grid.GetCellSize() + _grid.CellToWorld(desiredGridPosition.x, desiredGridPosition.y);

		PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, desiredGridPosition, desiredDirection, desiredObjectSo);
			
		foreach (Vector2Int gridPosition in gridPositions)
		{
			_grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
		}

		_lastObjectPlaced = placedObject;
		OnObjectPlaced?.Invoke(this, EventArgs.Empty);
	}
	

	private void Update()
	{
		// Build
		if (Input.GetMouseButtonDown(0) && _selectedPlaceableObjectSo is not null)
		{
			_grid.WorldToCell(Utils.GetMouseWorldPosition3D(), out int x, out int z);
			
			PlaceObject(new Vector2Int(x, z), _direction, _selectedPlaceableObjectSo);
			
			// List<Vector2Int> gridPositions = _selectedPlaceableObjectSo.GetGridPositions(new Vector2Int(x, z), _direction);
			//
			// GridObject gridObject = _grid.GetGridObject(x, z);
			//
			// if (gridObject == null) return;
			//
			// foreach (Vector2Int gridPosition in gridPositions)
			// {
			// 	if (!_grid.IsValidGridPosition(gridPosition.x, gridPosition.y))
			// 	{
			// 		Debug.Log("Out of bounds!");
			// 		return;
			// 	}
			//
			// 	if (!_grid.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
			// 	{
			// 		Debug.Log("Can't build here!");
			// 		return;
			// 	}
			// }
			//
			// Vector2Int rotationOffset = _selectedPlaceableObjectSo.GetRotationOffset(_direction);
			//
			// Vector3 placedObjectWorldPosition = new Vector3(rotationOffset.x, 0f, rotationOffset.y) * _grid.GetCellSize() + _grid.CellToWorld(x, z);
			//
			// PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), _direction, _selectedPlaceableObjectSo);
			//
			// foreach (Vector2Int gridPosition in gridPositions)
			// {
			// 	_grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
			// }
			//
			// _lastObjectPlaced = placedObject;
			// OnObjectPlaced?.Invoke(this, EventArgs.Empty);
		}

		// Demolish
		if (Input.GetMouseButtonDown(1) && _selectedPlaceableObjectSo is null)
		{
			GridObject gridObject = _grid.GetGridObject(Utils.GetMouseWorldPosition3D());
			PlacedObject placedObject = gridObject.GetPlacedObject();

			if (placedObject is null) return;

			placedObject.DestroySelf();


			List<Vector2Int> gridPositions = placedObject.GetGridPositions();

			foreach (Vector2Int gridPosition in gridPositions)
			{
				_grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
			}
		}
		
		if (Input.GetMouseButtonDown(1) && _selectedPlaceableObjectSo is not null)
		{
			DeselectObjectType();
		}

		// Rotate
		if (Input.GetKeyDown(KeyCode.R))
		{
			_direction = PlaceableObjectSO.GetNextDirection(_direction);
			Debug.Log("Current direction: " + _direction.ToString());
		}
		
		// Select Building
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			_selectedPlaceableObjectSo = placeableObjectSOList[0];
			RefreshSelectedObjectType();
			Debug.Log("Selected building: " + _selectedPlaceableObjectSo.nameString);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			_selectedPlaceableObjectSo = placeableObjectSOList[1];
			RefreshSelectedObjectType();
			Debug.Log("Selected building: " + _selectedPlaceableObjectSo.nameString);
		}
		
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			_selectedPlaceableObjectSo = placeableObjectSOList[2];
			RefreshSelectedObjectType();
			Debug.Log("Selected building: " + _selectedPlaceableObjectSo.nameString);
		}
	}
	
}
