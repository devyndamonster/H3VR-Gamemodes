using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Gamemodes
{
	public class ButtonList : MonoBehaviour
	{

		public SelectableButton buttonPrefab;
		public Transform buttonOrigin;
		public float buttonSpacing;

		[HideInInspector]
		public List<SelectableButton> buttons = new List<SelectableButton>();

		public SelectableButton AddButton(string text, UnityAction onClick, bool selectable)
		{
			SelectableButton buttonComp = Instantiate(buttonPrefab, buttonOrigin.position, buttonOrigin.rotation, buttonOrigin);
			buttonComp.transform.localPosition += Vector3.down * buttons.Count * buttonSpacing;

			buttonComp.text.text = text;
			buttonComp.button.onClick = new Button.ButtonClickedEvent();
			buttonComp.button.onClick.AddListener(onClick);

            if (selectable)
            {
				buttonComp.button.onClick.AddListener(() => { buttonComp.SetSelected(); });

				buttonComp.otherButtons.AddRange(buttons);

				foreach(SelectableButton button in buttons)
                {
					button.otherButtons.Add(buttonComp);
                }
			}

			buttons.Add(buttonComp);

			return buttonComp;
		}

		public void ClearButtons()
		{
			for(int i = 0; i < buttons.Count; i++)
            {
				Destroy(buttons[i].gameObject);
            }

			buttons.Clear();
		}

	}
}

