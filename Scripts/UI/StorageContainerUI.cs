using GenericObserver;
using PlaceableObjects;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace UI
{
	public class StorageContainerUI : MonoBehaviour, IObserver
	{
		[SerializeField] private GameObject itemGridObject;
		[SerializeField] private GameObject itemSlotPrefab;

		private StorageContainer _storageContainer;
		private ItemSlotUI[] _uiItemSlots;

		public void SetReference(StorageContainer storageContainer)
		{
			if (storageContainer is null)
			{
				Destroy(gameObject);
				return;
			};
	    
			_storageContainer = storageContainer;
			_storageContainer.AddObserver(this);
	    
			_uiItemSlots = new ItemSlotUI[_storageContainer.GetItemSlotCount()];

			for (int itemSlotIndex = 0; itemSlotIndex < _storageContainer.GetItemSlotCount(); itemSlotIndex++)
			{
				var newObject = Instantiate(itemSlotPrefab, itemGridObject.transform);
				if (newObject is null)
				{
					Debug.Log("ItemSlotUI ref is not created properly");
					return;
				}
		    
				ItemSlotUI itemSlotUIComponent = newObject.GetComponent<ItemSlotUI>();
		    
				_uiItemSlots[itemSlotIndex] = itemSlotUIComponent;
			}

			UpdateUI();
		}

		public void UpdateUI()
		{
			if (_storageContainer is null) return;

			var itemSlots = _storageContainer.GetAllItemSlots();

			if (itemSlots.Length <= 0) return;
	    
			for (int itemSlotIndex = 0; itemSlotIndex < _storageContainer.GetItemSlotCount(); itemSlotIndex++)
			{
				if (itemSlots[itemSlotIndex].IsEmpty())
				{
					_uiItemSlots[itemSlotIndex].SetItemIcon(null);
					_uiItemSlots[itemSlotIndex].SetItemCountText("");
			    
					continue;
				}

				_uiItemSlots[itemSlotIndex].SetItemIcon(itemSlots[itemSlotIndex].stack.GetItemType().icon);
				_uiItemSlots[itemSlotIndex].SetItemCountText(itemSlots[itemSlotIndex].stack.GetItemCount().ToString());
			}
		}

		public void OnNotify()
		{
			UpdateUI();
		}

		private void OnDisable()
		{
			_storageContainer.RemoveObserver(this);
		}
	}
}
