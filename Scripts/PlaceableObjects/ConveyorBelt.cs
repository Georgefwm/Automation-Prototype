using System;
using System.Collections.Generic;
using ConveyorSystem;
using UnityEditor;
using UnityEngine;

namespace PlaceableObjects
{
	public class ConveyorBelt : PlacedObject
	{
		[SerializeField] private ConveyorSegment segmentPrefab;
		[SerializeField] private Mesh straightBelt;
		[SerializeField] private Mesh rightTurnBelt;
		
		private ConveyorSegment _segment;
		private MeshFilter _meshFilter;
		private Transform _visualTransform;
		

		// Start is called before the first frame update
		void Start()
		{
			// Debug.Log(GetInputPosition() + " -> " + GetGridOriginPosition() + " -> " + GetOutputPosition());
	    
			// ConveyorSegment conveyorSegment = Instantiate(segmentPrefab, FactoryExecutionManager.Instance.transform);
			var prefabObject = Instantiate(segmentPrefab, FactoryExecutionManager.Instance.transform);

			ConveyorSegment conveyorSegment = prefabObject.GetComponent<ConveyorSegment>();
			FactoryExecutionManager.Instance.AddSegment(conveyorSegment);
			transform.SetParent(conveyorSegment.transform);
			_segment = conveyorSegment;
			
			_meshFilter = transform.Find("ConveyorBeltVisual/ConveyorBelt_Straight").GetComponent<MeshFilter>();
			
			_visualTransform = transform.Find("ConveyorBeltVisual/ConveyorBelt_Straight").GetComponent<Transform>();

			UpdateNeighbors();
		}

		#region Building-specific methods
    
		public ConveyorSegment GetConveyorSegment()
		{
			return _segment;
		}
    
		public void SetConveyorSegment(ConveyorSegment segment)
		{
			transform.SetParent(segment.transform);
			_segment = segment;
		}

		public void UpdateSegment()
		{
			if (_segment is null)
			{
				Debug.Log("_segment is null");
				return;
			}
	    
			_segment.UpdateSegment();
		}
		
		public void SetConveyorMesh(int directionOffset)
		{
			if (rightTurnBelt is null)
			{
				Debug.Log("rightTurn is null");
				return;
			}

			if (straightBelt is null)
			{
				Debug.Log("straightBelt is null");
				return;
			}

			Vector3 defaultMeshScale = new Vector3(1f, 0.2f, 1f);

			if (directionOffset == 0)
			{
				_meshFilter.mesh = rightTurnBelt;
				
				defaultMeshScale.x = -1;
				_visualTransform.localScale = defaultMeshScale;
				
				
				return;
			}
			
			if (directionOffset == 2)
			{
				_meshFilter.mesh = rightTurnBelt;
				_visualTransform.localScale = defaultMeshScale;
				
				return;
			}

			_meshFilter.mesh = straightBelt;
			_visualTransform.localScale = defaultMeshScale;
		}
    
		// Finds the first conveyor in a sequential line
		public ConveyorBelt GetBeltLineStart()
		{
			Vector2Int nextBeltPosition = GetInputPosition();
		
			ConveyorBelt nextBelt = GridBuildingSystem.Instance.GetObjectAt(nextBeltPosition) as ConveyorBelt;
			if (nextBelt == null) return this;
	    
			if (nextBelt.GetOutputPosition() != GetGridOriginPosition() || nextBelt.GetGridOriginPosition() != GetInputPosition()) return this;
	    
			return nextBelt.GetBeltLineStart();
		}

		// Finds and returns a line of conveyors from a start position
		public List<ConveyorBelt> GetBeltLine(List<ConveyorBelt> line)
		{
			line.Add(this);
	    
			Vector2Int nextBeltPosition = GetOutputPosition();

			ConveyorBelt nextBelt = GridBuildingSystem.Instance.GetObjectAt(nextBeltPosition) as ConveyorBelt;
			if (nextBelt != null)
			{
				if(nextBelt.GetInputPosition() == GetGridOriginPosition() && nextBelt.GetGridOriginPosition() == GetOutputPosition())
					nextBelt.GetBeltLine(line);
			}
	    
			return line;
		}

		protected override void UpdateNeighbors()
		{
			if (GridBuildingSystem.Instance.GetObjectAt(GetOutputPosition()) is ConveyorBelt beltInFront)
			{
				if (beltInFront.GetInputPosition() == GetGridOriginPosition())
				{
					beltInFront.UpdateSegment();
				}
			}
	    
			foreach (PlaceableObjectSO.Direction direction in Enum.GetValues(typeof(PlaceableObjectSO.Direction)))
			{
				if (GetDirection() == direction) continue;
			
				Vector2Int positionOffset = PlaceableObjectSO.GetGridForwardVector(direction) * Vector2Int.one;
				PlacedObject queryObject = GridBuildingSystem.Instance.GetObjectAt(GetGridOriginPosition() + positionOffset);
				
				if (queryObject is null) continue;
				if (queryObject.GetOutputPosition() != GetGridOriginPosition()) continue;

				if (queryObject is ConveyorBelt beltBehind)
				{
					beltBehind.UpdateSegment();
				}
			}
		}

		public void SetPreviousObject(PlacedObject placedObject)
		{
			_segment.SetPreviousObject(placedObject);
		}
    
		public void SetNextObject(PlacedObject placedObject)
		{
			_segment.SetNextObject(placedObject);
		}
    
		#endregion
    
		#region IItemStorage methods

		public override bool CanGetItem()
		{
			return _segment.CanGetItem();
		}

		public override bool TryGetItem(out ConveyorItem item)
		{
			if (!CanGetItem())
			{
				item = null;
				return false;
			}

			return _segment.TryGetItem(out item);
		}
    
		public override bool TryGiveItem(ConveyorItem item)
		{
			throw new System.NotImplementedException();
		}

		public override bool CanGiveItem(ItemSO itemType)
		{
			throw new System.NotImplementedException();
		}
    
		#endregion
	}
}
