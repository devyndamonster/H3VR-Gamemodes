using FistVR;
using Gamemodes;
using Sodalite.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KOTH
{
    public class KOTHSosig : SosigTeamMember
    {
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
    }
}
