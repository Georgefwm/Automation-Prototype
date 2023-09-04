using System.Linq;
using ConveyorSystem;
using UI;
using UnityEngine;

namespace PlaceableObjects
{
	public class Assembler1 : PlacedObject, IItemStorage
	{
		private RecipeSO _currentRecipe;
		private float _recipeProgress;

		[SerializeField] private RecipeSO defaultRecipe;

		private ItemSlot _inputItemSlot;
		private ItemSlot _outputItemSlot;
		private BuildingState _currentState;

		public enum BuildingState
		{
			Idle,
			Crafting,
			CraftingInterrupted
		}

		void Start()
		{
			_currentState = BuildingState.Idle;
        
			SetRecipe(defaultRecipe);
        
			UpdateNeighbors();
		}

		private void Update()
		{
			if (_currentRecipe is null) return;

			if (_currentState is BuildingState.Idle)
			{
				CheckForRecipe();
				return;
			}

			UpdateRecipeProgress();
		}

		public RecipeSO GetCurrentRecipe()
		{
			return _currentRecipe;
		}

		#region UI methods

		public override void ShowUI()
		{
			if (GridBuildingSystem.Instance.IsPlacing()) return;
	    
			if (GetPlacedObjectSO().UIPrefab is null) return;

			GameObject newUI = Instantiate(GetPlacedObjectSO().UIPrefab);
			Assembler1UI ui = newUI.GetComponent<Assembler1UI>();
			ui.SetReference(this);
		}

		#endregion

		#region Building-specific methods
    
		private void SetRecipe(RecipeSO newRecipe)
		{
			_inputItemSlot = new ItemSlot(newRecipe.inputItems.First().itemType);
			_outputItemSlot = new ItemSlot(newRecipe.outputItems.First().itemType);
	    
			_currentRecipe = defaultRecipe;
		}

		public ItemSlot GetInputItemSlot()
		{
			return _inputItemSlot;
		}
    
		public ItemSlot GetOutputItemSlot()
		{
			return _outputItemSlot;
		}

		// inPercentage returns value between 0 and 1
		public float GetCraftProgress(bool inPercentage = false)
		{
			if (inPercentage) return 1 - (_recipeProgress / _currentRecipe.craftTime);
	    
			return _recipeProgress;
		}

		public void CheckForRecipe()
		{
			// Don't have enough reagents
			if (_inputItemSlot.stack.GetItemCount() < _currentRecipe.inputItems.First().amount) return;
	    
			// Output is blocked
			if (_outputItemSlot.stack.GetItemCount() + _currentRecipe.outputItems.First().amount >
			    _outputItemSlot.stack.GetMaxItems()) return;
	    
			_currentState = BuildingState.Crafting;
			_recipeProgress = _currentRecipe.craftTime;
			_inputItemSlot.stack.RemoveItem(_currentRecipe.inputItems.First().amount);
		}

		public void UpdateRecipeProgress()
		{
			_recipeProgress -= Time.deltaTime;
			NotifyObservers();
	    
			if (_recipeProgress <= 0 && _outputItemSlot.stack.CanAddItem(_currentRecipe.outputItems.First().amount))
			{
				_outputItemSlot.stack.Add(_currentRecipe.outputItems.First().amount);
				_currentState = BuildingState.Idle;
		    
				// might need to do a 'CheckForRecipe()' here
			}
		}
    
		#endregion

		#region IItemStorage methods

		public override bool CanGetItem()
		{
			if (_outputItemSlot is null) return false;

			return _outputItemSlot.stack.GetItemCount() > 0;
		}

		public override bool TryGetItem(out ConveyorItem item)
		{
			if (!CanGetItem())
			{
				item = null;
				return false;
			}
			
			item = ConveyorItem.Create(_outputItemSlot.stack.GetItemType(),
				GridBuildingSystem.Instance.GetWorldPosition(GetOutputPosition()), 
				Quaternion.identity);
	    
			_outputItemSlot.stack.RemoveItem(1);

			NotifyObservers();
			return true;
		}

		public override bool TryGiveItem(ConveyorItem item)
		{
			if (!CanGiveItem(item.GetItemSO())) return false;

			_inputItemSlot.stack.Add(1);
			item.SelfDestruct();

			NotifyObservers();
			return true;
		}

		public override bool CanGiveItem(ItemSO itemType)
		{
			if (_inputItemSlot.stack.GetItemType() != itemType) return false;
	    
			return _inputItemSlot.stack.GetFreeSpace() > 0;
		}
    
		#endregion
	}
}
