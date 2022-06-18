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

        public LayerMask layerMask;
        public bool killPlayer;

        void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & layerMask) == 0) return;

            Sosig sosig = other.GetComponentInParent<Sosig>();
            if (sosig != null)
            {
                sosig.KillSosig();

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

