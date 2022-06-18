using FistVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gamemodes
{
    [CreateAssetMenu(fileName = "New Atlas Enemy Template", menuName = "MeatKit/Gamemodes/AtlasEnemyTemplate", order = 0)]
    public class AtlasSosigEnemyTemplate : ScriptableObject
    {
        public string DisplayName;
        public SosigEnemyCategory SosigEnemyCategory;
        public List<string> SosigPrefabs;
        public List<AtlasSosigConfigTemplate> Configs;
        public List<AtlasSosigConfigTemplate> ConfigsEasy;
        public List<AtlasOutfitConfigTemplate> OutfitConfigs;
        public List<string> WeaponOptions;
        public List<string> WeaponOptionsSecondary;
        public List<string> WeaponOptionsTertiary;
        public float SecondaryChance;
        public float TertiaryChance;

        public SosigEnemyTemplate GetSosigEnemyTemplate()
        {
            SosigEnemyTemplate template = (SosigEnemyTemplate)ScriptableObject.CreateInstance(typeof(SosigEnemyTemplate));

            template.DisplayName = DisplayName;
            template.SosigEnemyCategory = SosigEnemyCategory;
            template.SosigPrefabs = SosigPrefabs.Select(o => IM.OD[o]).ToList();
            template.ConfigTemplates = Configs.Select(o => o.GetConfigTemplate()).ToList();
            template.ConfigTemplates_Easy = ConfigsEasy.Select(o => o.GetConfigTemplate()).ToList();
            template.OutfitConfig = OutfitConfigs.Select(o => o.GetOutfitConfig()).ToList();
            template.WeaponOptions = WeaponOptions.Select(o => IM.OD[o]).ToList();
            template.WeaponOptions_Secondary = WeaponOptionsSecondary.Select(o => IM.OD[o]).ToList();
            template.WeaponOptions_Tertiary = WeaponOptionsTertiary.Select(o => IM.OD[o]).ToList();
            template.SecondaryChance = SecondaryChance;
            template.TertiaryChance = TertiaryChance;

            return template;
        }
    }
}




