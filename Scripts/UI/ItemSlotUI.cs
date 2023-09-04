using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class ItemSlotUI : MonoBehaviour
	{
		[SerializeField] private Sprite emptySlotSprite;
    
		private Image _imageComponent;
		private TextMeshProUGUI _textComponent;

		private void Awake()
		{
			_imageComponent = transform.Find("ItemSlotBackground/ItemIcon").GetComponent<Image>();
			_imageComponent.sprite = emptySlotSprite;
			_textComponent = transform.Find("ItemSlotBackground/ItemCountText").GetComponent<TextMeshProUGUI>();
			_textComponent.text = "";
		}

		// Update is called once per frame
		private void Update()
		{
		}

		public void SetItemIcon(Sprite icon)
		{
			if (_imageComponent is null) return;
	    
			if (icon is null)
			{
				_imageComponent.sprite = emptySlotSprite;
				return;
			}
	    
			_imageComponent.sprite = icon;
		}
    
		public void SetItemCountText(string itemCount)
		{
			if (_textComponent is null) return;
	    
			if (itemCount is null)
			{
				_textComponent.text = "";
				return;
			}
	    
			_textComponent.text = itemCount;
		}
	}
}
