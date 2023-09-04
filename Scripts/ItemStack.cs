using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStack
{
	private ItemSO _itemType;
	private int _amount;


	#region Constructors

	public ItemStack(ItemSO newItemType)
	{
		_itemType = newItemType;
		_amount = 0;
	}
	
	public ItemStack()
	{
		_amount = 0;
	}

	#endregion

	#region Item Count Methods
	
	public bool CanAddItem()
	{
		return _amount < _itemType.maxStacks;
	}

	public bool CanAddItem(int queryAmount)
	{
		return _amount + queryAmount < _itemType.maxStacks;
	}
	
	public void RemoveItem(int removeAmount)
	{
		_amount -= removeAmount;
	}
	
	public void Add(int amount)
	{
		_amount += amount;
	}
	
	public int GetMaxItems()
	{
		return _itemType.maxStacks;
	}

	public int GetFreeSpace()
	{
		return _itemType.maxStacks - _amount;
	}

	public int GetItemCount()
	{
		return _amount;
	}
	
	public void ClearItems()
	{
		_amount = 0;
	}

	#endregion

	#region Item Type Methods

	public void SetItemType(ItemSO newItemType)
	{
		if (newItemType is null) return;
		
		ClearItems();
		_itemType = newItemType;
	}

	public ItemSO GetItemType()
	{
		return _itemType;
	}

	#endregion
}
