using System.Collections;
using System.Collections.Generic;
using ConveyorSystem;
using UnityEngine;

public interface IItemStorage
{
	
	public bool CanGetItem();
	public bool TryGetItem(out ConveyorItem item);

	public bool TryGiveItem(ConveyorItem item);
	public bool CanGiveItem(ItemSO itemType);
}
