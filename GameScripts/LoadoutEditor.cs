using Gamemodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamemodes
{
	public class LoadoutEditor : MonoBehaviour
	{
		public int MaxPoints = 20;
		public Text pointsText;

		public ButtonList firearmsList;
		public ButtonList equipmentList;
		public ButtonList primaryList;
		public ButtonList quickbeltList;

		[HideInInspector]
		public int selectedLoadoutSlot = -1;


		public void InitLoadoutEditor(PlayerLoadout loadout, List<LoadoutPool> firearmPools, List<LoadoutPool> equipmentPools)
        {
			selectedLoadoutSlot = -1;

			InitFirearmButtons(firearmPools, loadout);
			InitEquipmentButtons(equipmentPools, loadout);
			InitPrimaryButtons(loadout);
			InitQuickbeltButtons(loadout);
			LinkEquippedButtons();

			pointsText.text = GetAvailablePoints(loadout).ToString();
        }

		public void InitFirearmButtons(List<LoadoutPool> firearmPools, PlayerLoadout loadout)
        {
			firearmsList.ClearButtons();

			foreach (LoadoutPool pool in firearmPools)
			{
				firearmsList.AddButton("[" + pool.poolCost + "] " + pool.poolName, () => { EquipIntoSlot(pool, loadout); }, false);
			}
		}

		public void InitEquipmentButtons(List<LoadoutPool> equipmentPools, PlayerLoadout loadout)
		{
			equipmentList.ClearButtons();

			foreach (LoadoutPool pool in equipmentPools)
			{
				equipmentList.AddButton("[" + pool.poolCost + "] " + pool.poolName, () => { EquipIntoSlot(pool, loadout); }, false);
			}
		}


		public void InitPrimaryButtons(PlayerLoadout loadout)
		{
			primaryList.ClearButtons();

			string poolName = "Empty";

			if (loadout.rightHandTable != null)
			{
				poolName = "[" + loadout.rightHandTable.poolCost + "] " + loadout.rightHandTable.poolName;
			}

			int slot = 0;
			primaryList.AddButton(poolName, () => { selectedLoadoutSlot = slot; }, true);
		}

		public void InitQuickbeltButtons(PlayerLoadout loadout)
		{
			quickbeltList.ClearButtons();

			for(int i = 0; i < loadout.quickbeltTables.Count; i++)
            {
				string poolName = "Empty";

				if(loadout.quickbeltTables[i] != null)
                {
					poolName = "[" + loadout.quickbeltTables[i].poolCost + "] " + loadout.quickbeltTables[i].poolName;
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

		public void EquipIntoSlot(LoadoutPool pool, PlayerLoadout loadout)
        {
			Debug.Log("Selected slot: " + selectedLoadoutSlot);

			if (selectedLoadoutSlot == -1) return;

			//Remove the cost of the item that's being replaced from this equation
			int negatedCost = 0;
			if (selectedLoadoutSlot == 0 && loadout.rightHandTable != null) negatedCost = loadout.rightHandTable.poolCost;
			else if (loadout.quickbeltTables[selectedLoadoutSlot - 1] != null) negatedCost = loadout.quickbeltTables[selectedLoadoutSlot - 1].poolCost;

			//If too expensive, do nothing
			if (pool.poolCost > GetAvailablePoints(loadout) + negatedCost) return;


			//Change the pool on the player loadout
			if(selectedLoadoutSlot == 0)
            {
				loadout.rightHandTable = pool;
			}

            else
            {
				loadout.quickbeltTables[selectedLoadoutSlot - 1] = pool;
			}

			//Refresh the buttons
			InitPrimaryButtons(loadout);
			InitQuickbeltButtons(loadout);
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
			pointsText.text = GetAvailablePoints(loadout).ToString();
		}


		public int GetAvailablePoints(PlayerLoadout loadout)
        {
			//Subtract from allowed points based on what's equipped
			int currentPoints = MaxPoints;
			if (loadout.rightHandTable != null)
			{
				currentPoints -= loadout.rightHandTable.poolCost;
			}
			foreach (LoadoutPool quickbelt in loadout.quickbeltTables)
			{
				if (quickbelt == null) continue;
				currentPoints -= quickbelt.poolCost;
			}

			return currentPoints;
		}


	}
}

