using TMPro;
using UnityEngine;

namespace UI
{
	public class TextUI : MonoBehaviour
	{
		private TextMeshProUGUI _textComponent;

		private void Awake()
		{
			_textComponent = transform.Find("Text").GetComponent<TextMeshProUGUI>();
			_textComponent.SetText("");
		}

		public void SetText(string newText)
		{
			_textComponent.SetText(newText); 
		}
	}
}
