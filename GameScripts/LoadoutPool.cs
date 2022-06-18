using FistVR;
using Sodalite.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes
{
	[CreateAssetMenu(fileName = "New Loadout Pool", menuName = "Loadout/LoadoutPool", order = 0)]
	public class LoadoutPool : ScriptableObject
	{
		public string poolName;
		public int poolCost;
		public List<AtlasObjectTable> tables;
		public List<string> additionalItems;

		public int minMagCapacity = -1;
		public int maxMagCapacity = -1;
		public int itemsToSpawn = 1;

		[HideInInspector]
		public List<FVRObject> items = new List<FVRObject>();

		private int lastSpawnedIndex = 0;

		public void InitializeTables()
		{
			items.Clear();

			foreach (AtlasObjectTable tableDef in tables)
			{

				tableDef.GenerateTable();
				items.AddRange(tableDef.generatedObjects);
			}

			foreach (string item in additionalItems)
			{
				if (IM.OD.ContainsKey(item))
				{
					items.Add(IM.OD[item]);
				}
			}

			items.Shuffle();
			items.Shuffle();

			lastSpawnedIndex = Random.Range(0, items.Count);
		}

		public FVRObject GetItem()
		{
			lastSpawnedIndex += 1;

			if (lastSpawnedIndex >= items.Count) lastSpawnedIndex = 0;

			return items[lastSpawnedIndex];
		}
	}
}


