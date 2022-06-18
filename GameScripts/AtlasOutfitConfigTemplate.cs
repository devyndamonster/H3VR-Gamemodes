using FistVR;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes
{
	[CreateAssetMenu(fileName = "New Atlas Outfit Config", menuName = "MeatKit/Gamemodes/AtlasOutfitConfig", order = 0)]
	public class AtlasOutfitConfigTemplate : ScriptableObject
	{
		public List<string> Headwear;
		public float Chance_Headwear;
		public List<string> Eyewear;
		public float Chance_Eyewear;
		public List<string> Facewear;
		public float Chance_Facewear;
		public List<string> Torsowear;
		public float Chance_Torsowear;
		public List<string> Pantswear;
		public float Chance_Pantswear;
		public List<string> Pantswear_Lower;
		public float Chance_Pantswear_Lower;
		public List<string> Backpacks;
		public float Chance_Backpacks;


		public SosigOutfitConfig GetOutfitConfig()
		{
			SosigOutfitConfig template = (SosigOutfitConfig)ScriptableObject.CreateInstance(typeof(SosigOutfitConfig));

			template.Chance_Headwear = Chance_Headwear;
			template.Headwear = Headwear.Select(o => IM.OD[o]).ToList();
			template.Chance_Eyewear = Chance_Eyewear;
			template.Eyewear = Eyewear.Select(o => IM.OD[o]).ToList();
			template.Chance_Facewear = Chance_Facewear;
			template.Facewear = Facewear.Select(o => IM.OD[o]).ToList();
			template.Chance_Torsowear = Chance_Torsowear;
			template.Torsowear = Torsowear.Select(o => IM.OD[o]).ToList();
			template.Chance_Pantswear = Chance_Pantswear;
			template.Pantswear = Pantswear.Select(o => IM.OD[o]).ToList();
			template.Chance_Pantswear_Lower = Chance_Pantswear_Lower;
			template.Pantswear_Lower = Pantswear_Lower.Select(o => IM.OD[o]).ToList();
			template.Chance_Backpacks = Chance_Backpacks;
			template.Backpacks = Backpacks.Select(o => IM.OD[o]).ToList();

			return template;
		}
	}
}


