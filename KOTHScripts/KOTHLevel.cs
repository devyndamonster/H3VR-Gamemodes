using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOTH
{
    public class KOTHLevel : MonoBehaviour
    {
        public string levelName;
        public Transform spectatePoint;
        public List<KOTHHill> hills;
        public Transform PlayerBackupSpawn;

        public void SetActiveLevel()
        {
            KOTHManager.instance.SetActiveLevel(this);
        }

    }
}


