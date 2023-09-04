using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class ProgressBarUI : MonoBehaviour
	{
		private Slider _sliderComponent;
	
		private void Awake()
		{
			_sliderComponent = transform.Find("ElementBackground/Slider").GetComponent<Slider>();
		}

		public void SetSliderPosition(float percentage)
		{
			_sliderComponent.value = percentage;
		}
	}
}
