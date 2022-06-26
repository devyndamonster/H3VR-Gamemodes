using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gamemodes.Conquest
{
    public class ConquestLevel : MonoBehaviour
    {

        public List<ConquestPoint> points = new List<ConquestPoint>();

        public Transform PlayerBackupSpawn;

        public Transform PlayerSpectatePoint;

        public void ResetLevel()
        {
            foreach(ConquestPoint point in points)
            {
                point.ResetPoint(ConquestManager.instance.teams, ConquestManager.instance.captureUpdateFrequency);
            }
        }

    }
}
