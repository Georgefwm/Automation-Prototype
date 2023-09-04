using UnityEngine;

namespace ConveyorSystem
{
	public class ConveyorItem : MonoBehaviour
	{
		private ItemSO _item;
		private float _distance;
		private Vector3 _desiredLocation;
		private Vector3 _velocity = Vector3.zero;

		public static ConveyorItem Create(ItemSO itemType, Vector3 location, Quaternion rotation)
		{
			Transform transform = Instantiate(itemType.prefab, location, rotation);

			Instantiate(itemType.visual, transform);

			ConveyorItem visualComponent = transform.GetComponent<ConveyorItem>();
		
			visualComponent._item = itemType;
			visualComponent._distance = 0;
		
			return visualComponent;
		}

		public float GetDistance()
		{
			return _distance;
		}

		public void SetDistance(float newDistance)
		{
			_distance = newDistance;
		}

		public ItemSO GetItemSO()
		{
			return _item;
		}

		public Transform GetVisual()
		{
			return transform;
		}

		public void SetNewVisualLocation(Vector3 worldPosition)
		{
			worldPosition.y = 0;
			_desiredLocation = worldPosition;
		}

		// Lerp to location
		public void Tick()
		{
			//Vector3 newTransformPosition = Vector3.SmoothDamp(visual.position, _desiredLocation, ref _velocity, _smoothTime);
			transform.position = _desiredLocation;
		}

		public void SelfDestruct()
		{
			Destroy(gameObject);
		}
	}
}
