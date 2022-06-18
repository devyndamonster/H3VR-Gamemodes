using FistVR;
using Gamemodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gamemodes
{
    public class Team : MonoBehaviour
    {
        [HideInInspector]
        public List<SosigTeamMember> sosigs;

        public List<AtlasSosigEnemyTemplate> sosigTemplates;

        public Color teamColor = Color.red;

        public SosigWearable teamClothingPrefab;

        [HideInInspector]
        public int maxSosigs = 20;

        [HideInInspector]
        public List<SosigEnemyTemplate> builtSosigTemplates;

        void Awake()
        {
            builtSosigTemplates = sosigTemplates.Select(o => o.GetSosigEnemyTemplate()).ToList();
        }

        public void KillAllSosigs()
        {
            foreach(SosigTeamMember sosig in sosigs)
            {
                sosig.sosig.KillSosig();
            }
        }

    }
}
