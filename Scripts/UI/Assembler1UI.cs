using System;
using System.Linq;
using GenericObserver;
using PlaceableObjects;
using UnityEngine;

namespace UI
{
	public class Assembler1UI : MonoBehaviour, IObserver
	{
		private Assembler1 _assembler1Ref;
		private ItemSlotUI _inputSlotUI;
		private ItemSlotUI _outputSlotUI;
		private ProgressBarUI _progressBarUI;
		private TextUI _inputRateText;
		private TextUI _outputRateText;
    
    
		public void SetReference(Assembler1 assembler)
		{
			if (assembler is null)
			{
				Destroy(gameObject);
				return;
			};
	    
			_assembler1Ref = assembler;
			_assembler1Ref.AddObserver(this);
	    
			const string contentPath = "BuildingUI/Canvas/Window/ContentPanel/";
	    
			_inputSlotUI = transform.Find(contentPath + "InputSlot").GetComponent<ItemSlotUI>();
			_outputSlotUI = transform.Find(contentPath + "OutputSlot").GetComponent<ItemSlotUI>();
			_progressBarUI = transform.Find(contentPath + "ProgressBarUI").GetComponent<ProgressBarUI>();
			_progressBarUI = transform.Find(contentPath + "ProgressBarUI").GetComponent<ProgressBarUI>();
			_progressBarUI = transform.Find(contentPath + "ProgressBarUI").GetComponent<ProgressBarUI>();
			_inputRateText = transform.Find(contentPath + "InputSlot/InputRateText").GetComponent<TextUI>();
			_outputRateText = transform.Find(contentPath + "OutputSlot/OutputRateText").GetComponent<TextUI>();

			UpdateUI();
		}
		
		public void OnNotify()
		{
			UpdateUI();
		}

		private void OnDisable()
		{
			_assembler1Ref.RemoveObserver(this);
		}

		public void UpdateUI()
		{
			if (_assembler1Ref is null) return;
	    
			float inputItemsPerMinute = RecipeSO.GetItemsPerMinute(_assembler1Ref.GetCurrentRecipe().inputItems.First(),
				_assembler1Ref.GetCurrentRecipe().craftTime);
	    
			float outputItemsPerMinute = RecipeSO.GetItemsPerMinute(_assembler1Ref.GetCurrentRecipe().outputItems.First(),
				_assembler1Ref.GetCurrentRecipe().craftTime);
	    
			if (_assembler1Ref.GetInputItemSlot().IsEmpty())
			{
				_inputSlotUI.SetItemIcon(null);
				_inputSlotUI.SetItemCountText("");
				_inputRateText.SetText("");
			}
			else
			{
				_inputSlotUI.SetItemIcon(_assembler1Ref.GetInputItemSlot().stack.GetItemType().icon);
				_inputSlotUI.SetItemCountText(_assembler1Ref.GetInputItemSlot().stack.GetItemCount().ToString());
				_inputRateText.SetText(inputItemsPerMinute.ToString("0.#####" + " items per minute"));
			}

			if (_assembler1Ref.GetOutputItemSlot().IsEmpty())
			{
				_outputSlotUI.SetItemIcon(null);
				_outputSlotUI.SetItemCountText("");
				_outputRateText.SetText("");
			}
			else
			{
				_outputSlotUI.SetItemIcon(_assembler1Ref.GetOutputItemSlot().stack.GetItemType().icon);
				_outputSlotUI.SetItemCountText(_assembler1Ref.GetOutputItemSlot().stack.GetItemCount().ToString());
				_outputRateText.SetText(outputItemsPerMinute.ToString("0.#####") + " items per minute");
			}

			_progressBarUI.SetSliderPosition(_assembler1Ref.GetCraftProgress(true));
		}
		
	}
}
