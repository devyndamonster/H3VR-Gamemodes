using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamemodes
{
	public class ValueButton : MonoBehaviour
	{
		public Button DecreaseButton;
		public Button IncreaseButton;
		public Text DescriptionText;
		public Text ValueText;

		protected virtual void Awake()
        {
			DecreaseButton.onClick = new Button.ButtonClickedEvent();
			DecreaseButton.onClick.AddListener(OnDecreasePressed);

			IncreaseButton.onClick = new Button.ButtonClickedEvent();
			IncreaseButton.onClick.AddListener(OnIncreasePressed);
		}

		public virtual void OnIncreasePressed()
        {
			Debug.Log("Increase Pressed!");
        }

		public virtual void OnDecreasePressed()
		{
			Debug.Log("Decrease Pressed!");
		}

	}
}

