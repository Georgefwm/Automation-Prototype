using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot
{
	public ItemStack stack;

	public ItemSlot(ItemSO itemType)
	{
		stack = new ItemStack(itemType);
	}
	
	public ItemSlot()
	{
	}

	public bool IsEmpty()
	{
		return stack is null;
	}

	public override string ToString()
	{
		if (IsEmpty()) return "ItemSlot - Empty";

		return "ItemSlot - ItemType: " + stack.GetItemType().nameString + ", Amount: " + stack.GetItemCount();
	}
}
