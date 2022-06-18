using FistVR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Gamemodes
{
	public class SelectableButton : MonoBehaviour
	{

		public Color selectedColor;
		public List<SelectableButton> otherButtons;
		public Text text;

		[HideInInspector]
		public FVRPointableButton fvrButton;

		[HideInInspector]
		public Button button;

		[HideInInspector]
		public Color normalColor;

		void Awake()
		{
			button = GetComponent<Button>();
			fvrButton = GetComponent<FVRPointableButton>();
			normalColor = fvrButton.ColorUnselected;
		}

		public void SetSelected()
		{
			fvrButton.ColorUnselected = selectedColor;
			fvrButton.Image.color = selectedColor;

			foreach (SelectableButton other in otherButtons)
			{
				other.SetUnselected();
			}
		}

		public void SetUnselected()
		{
			fvrButton.ColorUnselected = normalColor;
			fvrButton.Image.color = normalColor;
		}
	}
}

