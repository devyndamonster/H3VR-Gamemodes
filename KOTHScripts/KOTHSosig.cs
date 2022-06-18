using FistVR;
using Sodalite.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KOTH
{
    public class KOTHSosig : MonoBehaviour
    {
        public Sosig sosig;
        public KOTHTeam team;
        public KOTHHill currentHill;

        void OnDestroy()
        {
            team.sosigs.Remove(this);

            if(currentHill != null)
            {
                currentHill.capturingSosigs.Remove(this);
            }
        }

        public void OrderToAssault(KOTHHill hill)
        {
            sosig.CommandAssaultPoint(hill.attackPoints.GetRandom().position);
        }

        public void OrderToDefend(KOTHHill hill)
        {
            sosig.CommandAssaultPoint(hill.defendPoints.GetRandom().position);
            sosig.SetDominantGuardDirection(UnityEngine.Random.onUnitSphere);
        }

        public void EquipSlothingItem(SosigWearable item, int sosigLink)
        {
            SosigLink link = sosig.Links[sosigLink];
            SosigWearable wearable = Instantiate<SosigWearable>(item, link.transform.position + Vector3.up * 0.15f, link.transform.rotation, link.transform);
            wearable.RegisterWearable(link);
        }

    }
}
