using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlaceableObjects;
using UnityEngine;
using UnityEngine.Assertions;

namespace ConveyorSystem
{
	public class ConveyorSegment : PlacedObject, IItemStorage
	{
		private List<ConveyorBelt> _conveyorPieces = new List<ConveyorBelt>();
		private Queue<ConveyorItem> _items = new Queue<ConveyorItem>();
		private PlacedObject _previousPlacedObject;
		private PlacedObject _nextPlacedObject;
		private Vector3 _start;
		private Vector3 _end;
		private Vector3 _initialPreviousItemTransform;
		
		
		// parameters
		private const float BeltSpeed = 60f;			// Dictates how fast items move along belt (items/min)
		private const float MinimumItemSpacing = 1.5f;	// Dictates the minimum distance between items on belt

		private void Start()
		{
			Transform t = transform.GetChild(0);

			_items = new Queue<ConveyorItem>();

			if (t.GetComponent<ConveyorBelt>() is { } rootBelt)
			{
				AddPiece(rootBelt);
			}
		}
	
		public void SetPreviousObject(PlacedObject givingObject)
		{
			_previousPlacedObject = givingObject;
		}
	
		public void SetNextObject(PlacedObject givingObject)
		{
			_previousPlacedObject = givingObject;
		}
	
		public ConveyorBelt GetFirstBelt()
		{
			if (_conveyorPieces.Count <= 0) return null;
		
			return _conveyorPieces.First();
		}

		public ConveyorBelt GetLastBelt()
		{
			if (_conveyorPieces.Count <= 0) return null;
		
			return _conveyorPieces.Last();
		}

		public void AddPiece(ConveyorBelt belt)
		{
			Assert.IsNotNull(belt);
		
			_conveyorPieces.Add(belt);
			belt.SetConveyorSegment(this);
		}


		public List<ConveyorBelt> GetBeltList()
		{
			return _conveyorPieces;
		}

		// TODO: Fix calculation, currently step size seems to be about 25-30% too high
		private float CalculateMaxStepSize()
		{
			float speed = GridBuildingSystem.Instance.GetCellSize() / (BeltSpeed / 60);
			
			return speed * Time.deltaTime;
		}

		public void Tick()
		{
			Vector3 previousItemTransform = _initialPreviousItemTransform;

			float maxStepSize = CalculateMaxStepSize();
		
			// First, check if we can give an item to the next placedObject
			if (_items?.Count > 0 && _nextPlacedObject is not null && _nextPlacedObject is not ConveyorBelt)
			{
				ConveyorItem firstItem = _items.Peek();
				if (Vector3.Distance(firstItem.GetVisual().transform.position, _end) < 0.02)
				{
					if (_nextPlacedObject.TryGiveItem(firstItem))
					{
						ConveyorItem givenItem = _items.Dequeue();
						givenItem.SelfDestruct();
					}
				}
			}
		
			// Then update item positions/progress
			foreach (ConveyorItem item in _items)
			{
				Vector3 itemPosition = item.GetVisual().transform.position;
				var desiredPosition = Vector3.MoveTowards(itemPosition, _end, maxStepSize);

				if (Vector3.Distance(desiredPosition, previousItemTransform) > MinimumItemSpacing)
				{
					item.SetNewVisualLocation(desiredPosition);
					item.SetDistance(Vector3.Distance(itemPosition, previousItemTransform));
				}

				item.Tick();
			
				previousItemTransform = itemPosition;
			}
		
		
			if (_previousPlacedObject is null) return;
		
			if (Vector3.Distance(_start, previousItemTransform) < MinimumItemSpacing) return;
		
			// Lastly, check if we can pull an item from the previous placedObject
			if (_previousPlacedObject.TryGetItem(out ConveyorItem newItem))
			{
				newItem.SetDistance(Vector3.Distance(_start, previousItemTransform));
				newItem.SetNewVisualLocation(_start);
				newItem.Tick();
				_items.Enqueue(newItem);
			}
		}
	
		public void UpdateSegment()
		{
			if (_conveyorPieces.Count <= 0) return;
			
			// Reset the mesh to straight conveyor
			GetFirstBelt().SetConveyorMesh(1);

			ConveyorBelt newStart = _conveyorPieces.First().GetBeltLineStart();
			_conveyorPieces = newStart.GetBeltLine(new List<ConveyorBelt>());

			Queue<ConveyorItem> newItemQueue = new Queue<ConveyorItem>();
		
			for (int beltIndex = _conveyorPieces.Count - 1; beltIndex >= 0; beltIndex--)
			{
				ConveyorSegment segment = _conveyorPieces[beltIndex].GetConveyorSegment();
			
				// Merge all items into one queue
				while (segment.GetItemQueue().TryDequeue(out ConveyorItem dequeuedItem))
				{
					newItemQueue.Enqueue(dequeuedItem);
				}

				if (_conveyorPieces[beltIndex].GetConveyorSegment() == this) continue;

				_conveyorPieces[beltIndex].SetConveyorSegment(this);
				Destroy(segment.gameObject);
			}
			
			_items = newItemQueue;
			// _maxDistance = _conveyorPieces.Count;

			// Check forwards
			_nextPlacedObject = GridBuildingSystem.Instance.GetObjectAt(GetLastBelt().GetOutputPosition());
		
			if (_nextPlacedObject is ConveyorBelt beltInFront)
			{
				if (beltInFront.GetGridOriginPosition() == GetLastBelt().GetOutputPosition())
				{
					_nextPlacedObject = beltInFront;
					Assert.IsNotNull(_nextPlacedObject);
					beltInFront.SetPreviousObject(GetLastBelt());
				}
			}

			// Check backwards
			if (_previousPlacedObject is null) _previousPlacedObject = GridBuildingSystem.Instance.GetObjectAt(GetFirstBelt().GetInputPosition());
		
			foreach (PlaceableObjectSO.Direction direction in Enum.GetValues(typeof(PlaceableObjectSO.Direction)))
			{
				if (GetFirstBelt().GetDirection() == direction) continue;
			
				Vector2Int positionOffset = PlaceableObjectSO.GetGridForwardVector(direction) * Vector2Int.one;
				PlacedObject queryObject = GridBuildingSystem.Instance.GetObjectAt(GetFirstBelt().GetGridOriginPosition() + positionOffset);
			
			
				if (queryObject is null) continue;
				if (queryObject.GetOutputPosition() != GetFirstBelt().GetGridOriginPosition()) continue;
			
				_previousPlacedObject = queryObject;
				if (_previousPlacedObject is ConveyorBelt beltBehind) beltBehind.UpdateSegment();

				break;
			}
		
			UpdateStartEndPositions();
		
			Vector2Int forwardVector = PlaceableObjectSO.GetGridForwardVector(GetLastBelt().GetDirection());
			Vector2Int endGridPosition = GridBuildingSystem.Instance.GetGridPosition(_end);
			Vector2Int nextPosition = endGridPosition + forwardVector;
			_initialPreviousItemTransform = GridBuildingSystem.Instance.GetWorldPosition(nextPosition);
		
			// Debug output
			if (_items.Count > 0) Debug.Log(ToString());
		}

		public int GetItemQueueLength()
		{
			return _items.Count;
		}

		public Queue<ConveyorItem> GetItemQueue()
		{
			return _items;
		}

		public void DestroyAllItems()
		{
			while (_items.Count > 0)
			{
				if (_items.Peek() is null) continue;
				_items.Dequeue().SelfDestruct();
			}
		}

		public void RefreshItems()
		{
			Vector3 previousItemTransform = _initialPreviousItemTransform;
		
			foreach (ConveyorItem item in _items)
			{
				Vector3 itemPosition = item.GetVisual().transform.position;
			
				item.SetDistance(Vector3.Distance(itemPosition, previousItemTransform));
			
				previousItemTransform = itemPosition;
			}
		}
	
		private void OnDestroy()
		{
			//DestroyAllItems();
		}

		public void UpdateStartEndPositions()
		{
			UpdateStartPosition();
			UpdateEndPosition();
		}

		public void UpdateEndPosition()
		{
			// If there isn't a next object stop items before they fall off the end
			if (_nextPlacedObject is null)
			{
				_end = GridBuildingSystem.Instance.GetWorldPosition(GetLastBelt().GetGridOriginPosition());
				return;
			}
		
			// If there is a next object then 'push' the item onto it 
			_end = GridBuildingSystem.Instance.GetWorldPosition(GetLastBelt().GetOutputPosition());
		}
	
		public void UpdateStartPosition()
		{
			// Not sure if needed since no items are starting
			if (_previousPlacedObject is null)
			{
				_start = GridBuildingSystem.Instance.GetWorldPosition(GetFirstBelt().GetGridOriginPosition());
				return;
			}
		
			// If the previous object is a conveyor then start from the 'pushed' position. see UpdateEndPosition()
			if (_previousPlacedObject is ConveyorBelt)
			{
				_start = GridBuildingSystem.Instance.GetWorldPosition(GetFirstBelt().GetGridOriginPosition());

				/////////////////////////////////////////////////////////////////////////////////

				
				ConveyorBelt firstBelt = GetFirstBelt();

				PlaceableObjectSO.Direction currentDirection =
					PlaceableObjectSO.GetNextDirection(firstBelt.GetDirection());
				
				for(int positionIndex = 0; positionIndex < 3; positionIndex++)
				{
					Vector2Int searchPosition = firstBelt.GetGridOriginPosition() + PlaceableObjectSO.GetGridForwardVector(currentDirection);
					
					Debug.Log("Actual position: " + _previousPlacedObject.GetGridOriginPosition() + ", " +
					          "Checked position: " + searchPosition);
					
					currentDirection = PlaceableObjectSO.GetNextDirection(currentDirection);

					if (_previousPlacedObject.GetGridOriginPosition() != searchPosition) continue;
					
					Debug.Log("Setting mesh with direction: " + positionIndex);
					firstBelt.SetConveyorMesh(positionIndex);
					break;
				}
				
				
				/////////////////////////////////////////////////////////////////////////////////
				
				return;
			}
		
			// Pull items from inside non-conveyor objects
			_start = GridBuildingSystem.Instance.GetWorldPosition(GetFirstBelt().GetInputPosition());
		}

		

		public override bool CanGetItem()
		{
			if (_items.Count <= 0) return false;
		
			float distance = Vector3.Distance(_items.Peek().GetVisual().transform.position, _end);
		
			return distance < 0.001;
		}

		public override bool TryGetItem(out ConveyorItem item)
		{
			item = _items.Dequeue();
			return item is not null;
		}

		public override bool TryGiveItem(ConveyorItem item)
		{
			return true;
		}

		public override bool CanGiveItem(ItemSO itemType)
		{
			// TODO: implement CanTakeItem for conveyorSegment
			return true;
		}

		public override string ToString()
		{
			if (_items.Count <= 0) return "Conveyor Segment : Empty";
		
			StringBuilder newString = new StringBuilder();
			newString.Append("Conveyor segment: ");

			if (_previousPlacedObject is not null)
			{
				newString.Append(_previousPlacedObject.ToString());
			}
			else
			{
				newString.Append("NONE");
			}
			newString.Append(" -> ");
		
			foreach (ConveyorBelt belt in _conveyorPieces)
			{
				newString.Append(belt.ToString());
			
				if (belt != _conveyorPieces.Last()) newString.Append(", ");
			}

			newString.Append(" -> ");
			if (_nextPlacedObject is not null)
			{
				newString.Append(_nextPlacedObject.ToString());
			}
			else
			{
				newString.Append("NONE");
			}
		
			return newString.ToString();
		}

	}
}
