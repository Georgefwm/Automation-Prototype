using ConveyorSystem;
using UnityEngine;

namespace PlaceableObjects
{
    public class Merger : PlacedObject
    {
        
        
        #region IItemStorage methods
            
        public override bool CanGetItem()
        {
            return false;
        }

        public override bool TryGetItem(out ConveyorItem item)
        {
	        item = null;
            return false;
        }

        public override bool TryGiveItem(ConveyorItem item)
        {
            return false;
        }

        public override bool CanGiveItem(ItemSO itemType)
        {
            return false;
        }
    
        #endregion
    }
}
