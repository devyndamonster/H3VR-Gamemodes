using Gamemodes;
using KOTH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gamemodes
{
	public class TimePeriodOption : MonoBehaviour
	{
		public string TimePeriodName;
		public List<Team> Teams;
		public List<PlayerLoadout> Loadouts;

		public List<LoadoutPool> FirearmPools;
		public List<LoadoutPool> EquipmentPools;

		public void InitializeLoadouts()
		{
			foreach (LoadoutPool pool in FirearmPools)
			{
				pool.InitializeTables();
			}

			foreach (LoadoutPool pool in EquipmentPools)
			{
				pool.InitializeTables();
			}
		}
	}
}


