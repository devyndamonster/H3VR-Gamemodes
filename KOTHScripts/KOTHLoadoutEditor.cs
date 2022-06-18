using Gamemodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KOTH
{
	public class KOTHLoadoutEditor : MonoBehaviour
	{
		public int MaxPoints = 20;
		public Text pointsText;

		public ButtonList firearmsList;
		public ButtonList equipmentList;
		public ButtonList primaryList;
		public ButtonList quickbeltList;

		[HideInInspector]
		public int selectedLoadoutSlot = -1;


		public void InitLoadoutEditor()
        {
			selectedLoadoutSlot = -1;

			InitFirearmButtons();
			InitEquipmentButtons();
			InitPrimaryButtons();
			InitQuickbeltButtons();
			LinkEquippedButtons();

			pointsText.text = GetAvailablePoints().ToString();
        }

		public void InitFirearmButtons()
        {
			firearmsList.ClearButtons();

			foreach (LoadoutPool pool in KOTHManager.instance.currentTimePeriod.firearmPools)
			{
				firearmsList.AddButton("[" + pool.poolCost + "] " + pool.poolName, () => { EquipIntoSlot(pool); }, false);
			}
		}

		public void InitEquipmentButtons()
		{
			equipmentList.ClearButtons();

			foreach (LoadoutPool pool in KOTHManager.instance.currentTimePeriod.equipmentPools)
			{
				equipmentList.AddButton("[" + pool.poolCost + "] " + pool.poolName, () => { EquipIntoSlot(pool); }, false);
			}
		}


		public void InitPrimaryButtons()
		{
			primaryList.ClearButtons();

			string poolName = "Empty";

			if (KOTHManager.instance.currentPlayerLoadout.rightHandTable != null)
			{
				poolName = "[" + KOTHManager.instance.currentPlayerLoadout.rightHandTable.poolCost + "] " + KOTHManager.instance.currentPlayerLoadout.rightHandTable.poolName;
			}

			int slot = 0;
			primaryList.AddButton(poolName, () => { selectedLoadoutSlot = slot; }, true);
		}

		public void InitQuickbeltButtons()
		{
			quickbeltList.ClearButtons();

			for(int i = 0; i < KOTHManager.instance.currentPlayerLoadout.quickbeltTables.Count; i++)
            {
				string poolName = "Empty";

				if(KOTHManager.instance.currentPlayerLoadout.quickbeltTables[i] != null)
                {
					poolName = "[" + KOTHManager.instance.currentPlayerLoadout.quickbeltTables[i].poolCost + "] " + KOTHManager.instance.currentPlayerLoadout.quickbeltTables[i].poolName;
				}

				int slot = i + 1;
				quickbeltList.AddButton(poolName, () => { selectedLoadoutSlot = slot; }, true);
			}

		}

		public void LinkEquippedButtons()
        {
			foreach(SelectableButton button in primaryList.buttons)
            {
				button.otherButtons.AddRange(quickbeltList.buttons);
            }

			foreach(SelectableButton button in quickbeltList.buttons)
            {
				button.otherButtons.AddRange(primaryList.buttons);
            }
        }

		public void EquipIntoSlot(LoadoutPool pool)
        {
			Debug.Log("Selected slot: " + selectedLoadoutSlot);

			if (selectedLoadoutSlot == -1) return;

			//Remove the cost of the item that's being replaced from this equation
			int negatedCost = 0;
			if (selectedLoadoutSlot == 0 && KOTHManager.instance.currentPlayerLoadout.rightHandTable != null) negatedCost = KOTHManager.instance.currentPlayerLoadout.rightHandTable.poolCost;
			else if (KOTHManager.instance.currentPlayerLoadout.quickbeltTables[selectedLoadoutSlot - 1] != null) negatedCost = KOTHManager.instance.currentPlayerLoadout.quickbeltTables[selectedLoadoutSlot - 1].poolCost;

			//If too expensive, do nothing
			if (pool.poolCost > GetAvailablePoints() + negatedCost) return;


			//Change the pool on the player loadout
			if(selectedLoadoutSlot == 0)
            {
				KOTHManager.instance.currentPlayerLoadout.rightHandTable = pool;
			}

            else
            {
				KOTHManager.instance.currentPlayerLoadout.quickbeltTables[selectedLoadoutSlot - 1] = pool;
			}

			//Refresh the buttons
			InitPrimaryButtons();
			InitQuickbeltButtons();
			LinkEquippedButtons();

			//Set the proper selected button
			if(selectedLoadoutSlot == 0)
            {
				primaryList.buttons[0].SetSelected();
            }
            else
            {
				quickbeltList.buttons[selectedLoadoutSlot - 1].SetSelected();
            }

			//Update point text
			pointsText.text = GetAvailablePoints().ToString();

			KOTHSaveManager.SaveData(KOTHManager.instance);
		}


		public int GetAvailablePoints()
        {
			//Subtract from allowed points based on what's equipped
			int currentPoints = MaxPoints;
			if (KOTHManager.instance.currentPlayerLoadout.rightHandTable != null)
			{
				currentPoints -= KOTHManager.instance.currentPlayerLoadout.rightHandTable.poolCost;
			}
			foreach (LoadoutPool quickbelt in KOTHManager.instance.currentPlayerLoadout.quickbeltTables)
			{
				if (quickbelt == null) continue;
				currentPoints -= quickbelt.poolCost;
			}

			return currentPoints;
		}


	}
}

