using FistVR;
using Gamemodes;
using KOTH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamemodes
{
    public class KillBox : MonoBehaviour
    {
        public bool killPlayer;

        void OnTriggerEnter(Collider other)
        {
            SosigLink link = other.GetComponentInParent<SosigLink>();
            if (link != null && link.S != null)
            {
                link.S.KillSosig();

                return;
            }

            if (!killPlayer) return;

            //Don't kill the player if they aren't playing
            if (PlayerTracker.currentPlayerState != PlayerState.playing) return;

            FVRPlayerHitbox player = other.GetComponent<FVRPlayerHitbox>();
            if (player != null)
            {
                player.Body.HarmPercent(1000);

                return;
            }
        }

    }
}

