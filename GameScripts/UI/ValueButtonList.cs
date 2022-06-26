using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamemodes
{
    public class ValueButtonList : MonoBehaviour
	{
		public ValueButtonInt buttonIntPrefab;
		public ValueButtonFloat buttonFloatPrefab;
		public Transform buttonOrigin;
		public float buttonSpacing;

		[HideInInspector]
		public List<ValueButton> buttons = new List<ValueButton>();

		public ValueButtonInt AddButton(string text, int minValue, int maxValue, int startingValue, int increment, UnityAction<int> OnValueChanged)
		{
			ValueButtonInt buttonComp = Instantiate(buttonIntPrefab, buttonOrigin.position, buttonOrigin.rotation, buttonOrigin);
			buttonComp.transform.localPosition += Vector3.down * buttons.Count * buttonSpacing;
			buttonComp.DescriptionText.text = text;
			buttonComp.MinValue = minValue;
			buttonComp.MaxValue = maxValue;
			buttonComp.Increment = increment;
			buttonComp.OnValueChanged = OnValueChanged;
			buttonComp.Value = startingValue;

			buttons.Add(buttonComp);

			return buttonComp;
		}

		public ValueButtonFloat AddButton(string text, float minValue, float maxValue, float startingValue, float increment, UnityAction<float> OnValueChanged)
		{
			ValueButtonFloat buttonComp = Instantiate(buttonFloatPrefab, buttonOrigin.position, buttonOrigin.rotation, buttonOrigin);
			buttonComp.transform.localPosition += Vector3.down * buttons.Count * buttonSpacing;
			buttonComp.DescriptionText.text = text;
			buttonComp.MinValue = minValue;
			buttonComp.MaxValue = maxValue;
			buttonComp.Increment = increment;
			buttonComp.OnValueChanged = OnValueChanged;
			buttonComp.Value = startingValue;

			buttons.Add(buttonComp);

			return buttonComp;
		}

		public void ClearButtons()
		{
			for (int i = 0; i < buttons.Count; i++)
			{
				Destroy(buttons[i].gameObject);
			}

			buttons.Clear();
		}
	}
}

