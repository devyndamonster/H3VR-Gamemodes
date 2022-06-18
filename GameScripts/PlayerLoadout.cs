using FistVR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes
{
	[CreateAssetMenu(fileName = "New Player Loadout", menuName = "Loadout/PlayerLoadout", order = 0)]
	public class PlayerLoadout : ScriptableObject
	{

		public string LoadoutName;
		public LoadoutPool rightHandTable;
		public LoadoutPool leftHandTable;
		public List<LoadoutPool> quickbeltTables;

		public bool magInOtherHand;
		public bool bespokeInOtherHand;

		
	}
}


