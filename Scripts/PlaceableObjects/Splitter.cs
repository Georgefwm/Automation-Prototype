using System.Collections.Generic;
using ConveyorSystem;
using UnityEngine;

namespace PlaceableObjects
{
    public class Splitter : PlacedObject
    {
        private ConveyorItem _inputItem;
        
        #region IItemStorage methods
            
        public override bool CanGetItem()
        {
            return _inputItem is not null;
        }

        public override bool TryGetItem(out ConveyorItem item)
        {
	        if (!CanGetItem())
	        {
		        item = null;
		        return false;
	        }

	        item = _inputItem;
            _inputItem = null;
            return false;
        }

        public override bool TryGiveItem(ConveyorItem item)
        {
            if (_inputItem is not null || item is null) return false;

            _inputItem = item;
            return true;
        }

        public override bool CanGiveItem(ItemSO itemType)
        {
            return _inputItem is null;
        }
    
        #endregion
    }
}
