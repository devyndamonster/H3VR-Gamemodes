using FistVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes
{
	public class SosigTeamMember : MonoBehaviour
	{
		public Sosig sosig;
		public Team team;

		public void EquipSlothingItem(SosigWearable item, int sosigLink)
		{
			SosigLink link = sosig.Links[sosigLink];
			SosigWearable wearable = Instantiate<SosigWearable>(item, link.transform.position + Vector3.up * 0.15f, link.transform.rotation, link.transform);
			wearable.RegisterWearable(link);
		}
	}
}

