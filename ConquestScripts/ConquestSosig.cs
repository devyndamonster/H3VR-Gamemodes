using Sodalite.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.Conquest
{
	public class ConquestSosig : SosigTeamMember
	{
        CapturablePoint currentPoint = null;

        void OnDestroy()
        {
            team.sosigs.Remove(this);
            team.score -= ConquestManager.instance.ticketKillCost;

            if (currentPoint != null)
            {
                currentPoint.capturingSosigs.Remove(this);
            }
        }

        public void OrderToAssault(ConquestPoint point)
        {
            sosig.CommandAssaultPoint(point.AttackPoints.GetRandom().position);
        }

        public void OrderToDefend(ConquestPoint point)
        {
            sosig.CommandAssaultPoint(point.DefendPoints.GetRandom().position);
            sosig.SetDominantGuardDirection(Random.onUnitSphere);
        }

    }
}

