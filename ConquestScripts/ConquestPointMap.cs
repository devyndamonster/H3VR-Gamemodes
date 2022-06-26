using FistVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamemodes.Conquest
{
	public class ConquestPointMap : MonoBehaviour
	{
		public Button ButtonPrefab;
		public LineRenderer LineRendererPrefab;
		public float mapNeighborButtonDistance = 0.5f;
		public Color defendColor;
		public Color attackColor;
		public Color defaultColor;

		private ConquestPoint point;
		private Button centerButton;
		private List<Button> buttons = new List<Button>();
		private List<LineRenderer> lineRenderers = new List<LineRenderer>();

		[ContextMenu("BuildButtonMap")]
		private void Start()
        {
			point = GetComponentInParent<ConquestPoint>();
			BuildButtonMap();
        }

		public void BuildButtonMap()
        {
			ClearMap();
			SpawnCenterButton();
			
			for(int i = 0; i < point.Neighbors.Count; i++)
            {
				ConquestPoint neighbor = point.Neighbors[i];
				LineRenderer lineToNeighbor = Instantiate(LineRendererPrefab.gameObject, transform).GetComponent<LineRenderer>();
				Vector3 offsetToNeighbor = GetButtonOffsetForNeighbor(neighbor);
				lineToNeighbor.SetPositions(new Vector3[] { transform.position, transform.position + offsetToNeighbor });

				Button newButton = Instantiate(ButtonPrefab.gameObject, transform.position + offsetToNeighbor + (Vector3.up * .01f), transform.rotation, transform).GetComponent<Button>();
				newButton.onClick = new Button.ButtonClickedEvent();
				newButton.onClick.AddListener(() => { NeighborButtonPressed(newButton, neighbor); });

				buttons.Add(newButton);
				lineRenderers.Add(lineToNeighbor);
				SetMapButtonColor(newButton, GetNeighborPointColor(neighbor));
				SetMapButtonText(newButton, neighbor.PointName);
			}
		}

		public void ClearMap()
        {
			foreach(Button button in buttons)
            {
				if(button != null) Destroy(button.gameObject);
			}

			foreach(LineRenderer lineRenderer in lineRenderers)
            {
				if(lineRenderer != null) Destroy(lineRenderer.gameObject);
			}

			if(centerButton != null) Destroy(centerButton.gameObject);

			buttons.Clear();
			lineRenderers.Clear();
        }

		public void SpawnCenterButton()
        {
			centerButton = Instantiate(ButtonPrefab.gameObject, transform.position, transform.rotation, transform).GetComponent<Button>();
			centerButton.onClick = new Button.ButtonClickedEvent();
			centerButton.onClick.AddListener(() => { CenterButtonPressed(centerButton); });
			SetMapButtonColor(centerButton, GetCenterPointColor());
			SetMapButtonText(centerButton, point.PointName);
		}

		public void CenterButtonPressed(Button button)
		{
			point.ToggleDefendStrategy();
			Color pointColor = GetCenterPointColor();
			SetMapButtonColor(button, pointColor);
		}

		public void NeighborButtonPressed(Button button, ConquestPoint neighbor)
        {
			ToggleNeighborStrategy(neighbor);
			Color pointColor = GetNeighborPointColor(neighbor);
			SetMapButtonColor(button, pointColor);
		}

		public void ToggleNeighborStrategy(ConquestPoint neighbor)
        {
			point.AttackNeighborPoints[neighbor] = !point.AttackNeighborPoints[neighbor];
		}

		public Vector3 GetButtonOffsetForNeighbor(ConquestPoint neighbor)
        {
			Vector3 offset = (neighbor.transform.position - transform.position).normalized * mapNeighborButtonDistance;
			offset.y = 0;
			return offset;
		}

		public Color GetCenterPointColor()
        {
			if (point.DefendThisPoint) return defendColor;
			return defaultColor;
        }

		public Color GetNeighborPointColor(ConquestPoint neighbor)
        {
            if (point.AttackNeighborPoints[neighbor]) return attackColor;
			return defaultColor;
        }

		public void SetMapButtonColor(Button button, Color color)
        {
			ColorBlock buttonColors = button.colors;
			buttonColors.normalColor = color;
			button.colors = buttonColors;
			button.GetComponent<FVRPointableButton>().ColorUnselected = color;
		}

		public void SetMapButtonText(Button button, string text)
        {
			button.GetComponentInChildren<Text>().text = text;
        }

		
	}
}

