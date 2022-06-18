using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Gamemodes;
using Valve.Newtonsoft.Json;
using System.Linq;
using System;

namespace KOTH
{
	public static class KOTHSaveManager
	{

		public static string saveFolderPath;
		public static string saveFilePath;

		public static void InitSaveManager(KOTHManager manager)
        {
			//Set the base save directory
			saveFolderPath = Application.dataPath.Replace("/h3vr_Data", "/GamemodeSaves");
            if (!Directory.Exists(saveFolderPath)) Directory.CreateDirectory(saveFolderPath);

			//Set the koth save directory
			saveFolderPath = Path.Combine(saveFolderPath, "KOTH");
			if (!Directory.Exists(saveFolderPath)) Directory.CreateDirectory(saveFolderPath);

			//If the save file for this level does not exist, create it and save the data
			saveFilePath = Path.Combine(saveFolderPath, manager.levelName + ".json");
			if (!File.Exists(saveFilePath))
			{
				SaveData(manager);
			}
            else
            {
				LoadData(manager);
            }
		}

		public static void SaveData(KOTHManager manager)
        {
			Debug.Log("Saving data to file");

			if (File.Exists(saveFilePath))
			{
				File.Delete(saveFilePath);
			}

			using (StreamWriter sw = File.CreateText(saveFilePath))
			{
				string json = JsonConvert.SerializeObject(new KOTHSaveData(manager));
				sw.WriteLine(json);
				sw.Close();
			}
		}

		public static void LoadData(KOTHManager manager)
        {
			Debug.Log("Loading save data from file");

			string json = File.ReadAllText(saveFilePath);
			KOTHSaveData saveData = JsonConvert.DeserializeObject<KOTHSaveData>(json);

            try
            {
				LoadSettings(manager, saveData);
				LoadTimePeriods(manager, saveData);
			}
			catch(Exception e)
            {
				SaveData(manager);
            }
			
		}


		private static void LoadSettings(KOTHManager manager, KOTHSaveData saveData)
        {
			manager.sosigSpawnFrequency = saveData.settingsData.respawnDelay;
			manager.playerHealth = saveData.settingsData.playerHealth;
			manager.wristMap.displayMode = saveData.settingsData.wristMapMode;

			foreach (TimePeriodOption timePeriod in manager.timePeriodOptions)
			{
				timePeriod.Teams[0].maxSosigs = saveData.settingsData.numGreen;
				timePeriod.Teams[1].maxSosigs = saveData.settingsData.numRed;
			}
		}


		private static void LoadTimePeriods(KOTHManager manager, KOTHSaveData saveData)
        {
			for(int i = 0; i < saveData.timePeriodList.Count; i++)
            {
				LoadLoadouts(manager.timePeriodOptions[i], saveData.timePeriodList[i]);
            }
        }


		private static void LoadLoadouts(TimePeriodOption timePeriod, TimePeriodData timeData)
		{
			for (int i = 0; i < timeData.loadoutData.Count; i++)
			{
				PlayerLoadout playerLoadout = timePeriod.Loadouts[i];
				LoadoutData loadoutData = timeData.loadoutData[i];

				//Setup the primary item
				LoadoutPool primary = null;
				primary = timePeriod.FirearmPools.FirstOrDefault(o => o.poolName == loadoutData.primaryPoolName);

				if(primary == null)
                {
					primary = timePeriod.EquipmentPools.FirstOrDefault(o => o.poolName == loadoutData.primaryPoolName);
				}

				if(primary != null)
                {
					playerLoadout.rightHandTable = primary;
                }

				//Now go through all of the quickbelt items
				for(int j = 0; j < loadoutData.quickbeltPoolNames.Count; j++)
                {
					LoadoutPool quickbelt = null;
					quickbelt = timePeriod.FirearmPools.FirstOrDefault(o => o.poolName == loadoutData.quickbeltPoolNames[j]);

					if (quickbelt == null)
					{
						quickbelt = timePeriod.EquipmentPools.FirstOrDefault(o => o.poolName == loadoutData.quickbeltPoolNames[j]);
					}

					if (quickbelt != null)
					{
						playerLoadout.quickbeltTables[j] = quickbelt;
					}
				}

			}
		}



		


		private class KOTHSaveData
		{
			public KOTHSettingsData settingsData;
			public List<TimePeriodData> timePeriodList = new List<TimePeriodData>();

			public KOTHSaveData() { }
			public KOTHSaveData(KOTHManager manager)
            {
				settingsData = new KOTHSettingsData(manager);

				foreach (TimePeriodOption timePeriod in manager.timePeriodOptions)
                {
					timePeriodList.Add(new TimePeriodData(timePeriod));
                }
            }

		}


		private class KOTHSettingsData
		{
			public int numRed;
			public int numGreen;
			public float respawnDelay;
			public int playerHealth;
			public WristMapDisplayMode wristMapMode;


			public KOTHSettingsData() { }
			public KOTHSettingsData(KOTHManager manager)
			{
				playerHealth = (int)manager.playerHealth;
				wristMapMode = manager.wristMap.displayMode;
				respawnDelay = manager.sosigSpawnFrequency;
				numGreen = manager.timePeriodOptions[0].Teams[0].maxSosigs;
				numRed = manager.timePeriodOptions[0].Teams[1].maxSosigs;
			}

		}

		private class TimePeriodData
		{
			public List<LoadoutData> loadoutData = new List<LoadoutData>();

			public TimePeriodData() { }

			public TimePeriodData(TimePeriodOption timePeriod)
            {
				foreach(PlayerLoadout loadout in timePeriod.Loadouts)
                {
					loadoutData.Add(new LoadoutData(loadout));
                }
            }

		}

		

		private class LoadoutData
        {
			public string primaryPoolName;

			public List<string> quickbeltPoolNames = new List<string>();

			public LoadoutData() { }

			public LoadoutData(PlayerLoadout loadout)
            {
				if (loadout != null) primaryPoolName = loadout.rightHandTable.poolName;

				foreach(LoadoutPool pool in loadout.quickbeltTables)
                {
					if (pool == null) continue;
					quickbeltPoolNames.Add(pool.poolName);
                }
            }
		}

	}


	

	


}

