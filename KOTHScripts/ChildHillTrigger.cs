using FistVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOTH
{
    public class ChildHillTrigger : MonoBehaviour
    {
        public KOTHHill hill;

        [HideInInspector]
        public List<KOTHSosig> capturingSosigs = new List<KOTHSosig>();

        [HideInInspector]
        public bool isPlayerInHill;

        void OnTriggerEnter(Collider other)
        {
            KOTHSosig sosig = other.GetComponent<KOTHSosig>();
            if (sosig != null)
            {
                if (!capturingSosigs.Contains(sosig))
                {
                    capturingSosigs.Add(sosig);
                }

                hill.OnSosigEnteredHill(sosig);

                return;
            }

            FVRPlayerHitbox player = other.GetComponent<FVRPlayerHitbox>();
            if (player != null)
            {
                isPlayerInHill = true;
                hill.OnPlayerEnteredHill(player);

                return;
            }
        }


        void OnTriggerExit(Collider other)
        {
            KOTHSosig sosig = other.GetComponent<KOTHSosig>();
            if (sosig != null)
            {
                capturingSosigs.Remove(sosig);
                hill.OnSosigExitedHill(sosig);

                return;
            }

            FVRPlayerHitbox player = other.GetComponent<FVRPlayerHitbox>();
            if (player != null)
            {
                isPlayerInHill = false;
                hill.OnPlayerExitedHill(player);

                return;
            }
        }
    }
}

