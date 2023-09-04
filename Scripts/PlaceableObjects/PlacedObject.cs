using System;
using System.Collections.Generic;
using ConveyorSystem;
using GenericObserver;
using UnityEngine;

namespace PlaceableObjects
{
	public class PlacedObject : Subject, IItemStorage
	{
		// public event EventHandler OnItemStorageChanged;
	
		private PlaceableObjectSO _placeableObjectSo;
		private Vector2Int _gridOrigin;
		private PlaceableObjectSO.Direction _direction;
	
	
		private Vector2Int _inputOffsetPosition;
		private Vector2Int _outputOffsetPosition;

		public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlaceableObjectSO.Direction direction,
			PlaceableObjectSO placeableObjectSo)
		{
			Transform placedObjectTransform = Instantiate(
				placeableObjectSo.prefab,
				worldPosition, 
				Quaternion.Euler(0, placeableObjectSo.DirectionToRotation(direction), 0)
			);

			PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

			placedObject._placeableObjectSo = placeableObjectSo;
			placedObject._gridOrigin = origin;
			placedObject._direction = direction;
			placedObject._inputOffsetPosition = placeableObjectSo.inputPosition;
			placedObject._outputOffsetPosition = placeableObjectSo.outputPosition;

			return placedObject;
		}

		#region UI methods

		public virtual void UpdateUI()
		{
			throw new NotImplementedException();
		}

		public virtual void ShowUI()
		{
			throw new NotImplementedException();
		}

		#endregion

		public PlaceableObjectSO GetPlacedObjectSO()
		{
			return _placeableObjectSo;
		}

		protected virtual void UpdateNeighbors()
		{
			if (GridBuildingSystem.Instance.GetObjectAt(GetOutputPosition()) is ConveyorBelt beltInFront)
			{
				if (beltInFront.GetGridOriginPosition() == GetOutputPosition())
				{
					beltInFront.UpdateSegment();
				}
			}
	    
			if (GridBuildingSystem.Instance.GetObjectAt(GetInputPosition()) is ConveyorBelt beltBehind)
			{
				if (beltBehind.GetGridOriginPosition() == GetInputPosition())
				{
					beltBehind.UpdateSegment();
				}
			}
		}

		#region Object grid position query methods
	
		public List<Vector2Int> GetGridPositions()
		{
			return _placeableObjectSo.GetGridPositions(_gridOrigin, _direction);
		}
	
		public Vector2Int GetInputPosition()
		{
			Vector2Int objectCenter = _placeableObjectSo.GetCenterCellPosition(_gridOrigin, _direction);

			return _placeableObjectSo.GetRelativeRotatedPosition(objectCenter, _direction, _inputOffsetPosition);
		}
	
		public Vector2Int GetOutputPosition()
		{
			Vector2Int objectCenter = _placeableObjectSo.GetCenterCellPosition(_gridOrigin, _direction);
		
			return _placeableObjectSo.GetRelativeRotatedPosition(objectCenter, _direction, _outputOffsetPosition);
		}

		public PlaceableObjectSO.Direction GetDirection()
		{
			return _direction;
		}

		public Vector2Int GetGridOriginPosition()
		{
			return _gridOrigin;
		}
	
		#endregion
	
		#region Utils methods

		public void DestroySelf()
		{
			Destroy(gameObject);
		}

		public override string ToString()
		{
			return _placeableObjectSo.nameString + " (" + _gridOrigin.x + ", " + _gridOrigin.y + ")";
		}

		#endregion

		#region IItemStorage methods
	
		public virtual bool CanGetItem()
		{
			throw new NotImplementedException();
		}

		public virtual bool TryGetItem(out ConveyorItem item)
		{
			throw new NotImplementedException();
		}

		public virtual bool TryGiveItem(ConveyorItem item)
		{
			throw new NotImplementedException();
		}

		public virtual bool CanGiveItem(ItemSO itemType)
		{
			throw new NotImplementedException();
		}
	
		#endregion
		
	}
}
