using System.Linq;
using ConveyorSystem;
using GenericObserver;
using UI;
using UnityEngine;

namespace PlaceableObjects
{
	public class StorageContainer : PlacedObject
	{
		private ItemSlot[] _itemSlots;
		[SerializeField] public ItemSO testItem;
		[SerializeField] private int itemSlotCount = 10;

		// Start is called before the first frame update
		void Start()
		{
			_itemSlots = new ItemSlot[itemSlotCount];

			for (int stackCount = 0; stackCount < itemSlotCount; stackCount++)
			{
				_itemSlots[stackCount] = new ItemSlot();
			}

			var newStack = new ItemStack(testItem);
			newStack.Add(testItem.maxStacks);
			_itemSlots[0].stack = newStack;
	    
			var newStack2 = new ItemStack(testItem);
			newStack2.Add(testItem.maxStacks);
			_itemSlots[3].stack = newStack2;
	    
			UpdateNeighbors();
		}

		#region UI methods

		public override void ShowUI()
		{
			if (GridBuildingSystem.Instance.IsPlacing()) return;

			if (GetPlacedObjectSO().UIPrefab is null) return;

			GameObject newUI = Instantiate(GetPlacedObjectSO().UIPrefab);
			StorageContainerUI ui = newUI.GetComponent<StorageContainerUI>();
			ui.SetReference(this);
		}

		#endregion

		#region Building-specific methods

		public ItemSlot[] GetAllItemSlots()
		{
			return _itemSlots;
		}

		public int GetItemSlotCount()
		{
			return itemSlotCount;
		}
    
		#endregion
    
		#region IItemStorage methods
    
		public override bool CanGetItem()
		{
			foreach (ItemSlot itemSlot in _itemSlots)
			{
				if (itemSlot.IsEmpty()) continue;
				if (itemSlot.stack.GetItemCount() > 0) return true;
			}
	    
			return false;
		}

		public override bool TryGetItem(out ConveyorItem item)
		{
			foreach (ItemSlot itemSlot in _itemSlots.Reverse())
			{
				if (itemSlot.IsEmpty()) continue;
				if (itemSlot.stack.GetItemCount() <= 0) continue;
		    
				itemSlot.stack.RemoveItem(1);
		    
				item = ConveyorItem.Create(itemSlot.stack.GetItemType(), GridBuildingSystem.Instance.GetWorldPosition(GetOutputPosition()), Quaternion.identity);
		    
				if (itemSlot.stack.GetItemCount() <= 0) itemSlot.stack = null;
		    
				NotifyObservers();
				return true;
			}

			item = null;
			return false;
		}

		public override bool TryGiveItem(ConveyorItem item)
		{
			// item.SetNewVisualLocation(GridBuildingSystem.Instance.GetWorldPosition(GetOutputPosition()));
			ItemSlot nextFreeItemSlot = null;
	    
			foreach (ItemSlot itemSlot in _itemSlots)
			{
				if (itemSlot.IsEmpty())
				{
					if (nextFreeItemSlot is null) nextFreeItemSlot = itemSlot;
					
					continue;
				}

				if (itemSlot.stack.GetItemType() != item.GetItemSO() || itemSlot.stack.GetFreeSpace() <= 0) continue;
		    
				itemSlot.stack.Add(1);
		    
				NotifyObservers();
				return true;
			}

			if (nextFreeItemSlot is null) return false;

			ItemStack newStack = new ItemStack(item.GetItemSO());
			newStack.Add(1);
			nextFreeItemSlot.stack = newStack;
	    
			NotifyObservers();
			return true;
		}

		public override bool CanGiveItem(ItemSO itemType)
		{
			foreach (ItemSlot itemSlot in _itemSlots)
			{
				if (itemSlot.IsEmpty()) continue;
		    
				if (itemSlot.stack.GetItemType() == itemType && itemSlot.stack.GetFreeSpace() > 0) return true;
			}
	    
			return false;
		}
    
		#endregion
	}
}
