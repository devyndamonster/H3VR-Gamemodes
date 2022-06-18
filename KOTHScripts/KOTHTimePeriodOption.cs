using Gamemodes;
using KOTH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KOTH
{
	public class KOTHTimePeriodOption : MonoBehaviour
	{

		public string timePeriodName;
		public List<KOTHTeam> kothTeams;
		public List<PlayerLoadout> loadouts;

		public List<LoadoutPool> firearmPools;
		public List<LoadoutPool> equipmentPools;

		public void SetActiveTimePeriod()
		{
			KOTHManager.instance.ResetKOTH();
			InitializeLoadouts();

			KOTHManager.instance.currentTimePeriod = this;
			KOTHMenuController.instance.InitLoadoutButtons();
			SetupTeams();

			KOTHManager.instance.SetPlayerLoadout(loadouts[0]);
		}


		public void InitializeLoadouts()
		{
			foreach (LoadoutPool pool in firearmPools)
			{
				pool.InitializeTables();
			}

			foreach (LoadoutPool pool in equipmentPools)
			{
				pool.InitializeTables();
			}
		}




		public void SetupTeams()
		{
			Debug.Log("Setting up teams");
			KOTHManager.instance.teams.Clear();
			KOTHManager.instance.teams.AddRange(kothTeams);
		}
	}
}


