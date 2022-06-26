using FistVR;
using Sodalite.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes
{
    public class SpawnArea : MonoBehaviour
    {

        public float spawnRadius = 1;
        public MeshRenderer areaMesh;

        [HideInInspector]
        public int IFF;

        [HideInInspector]
        public bool isPlayerInSpawn;

        [HideInInspector]
        public Color spawnColor;


        public Vector3 GetPositionInArea()
        {
            Vector3 random = transform.position + Random.insideUnitSphere * spawnRadius;
            random.y = transform.position.y;
            return random;
        }


        public Sosig SpawnSosig(SosigEnemyTemplate template, SosigAPI.SpawnOptions spawnOptions)
        {
            return SosigAPI.Spawn(template, spawnOptions, GetPositionInArea(), transform.rotation);
        }


        public void SetTeam(Team team, int teamIndex)
        {
            spawnColor = team.teamColor;
            IFF = teamIndex;

            if (!isPlayerInSpawn || IFF == 0)
            {
                areaMesh.material.SetColor("_ScrollColor", spawnColor);
            }
            else
            {
                areaMesh.material.SetColor("_ScrollColor", Color.grey);
            }
        }


        public bool CanSosigSpawn(int team)
        {
            //if the team matches the spawn, and the player is either not in the point or the point belongs to the player
            return IFF == team && (!isPlayerInSpawn || IFF == 0);
        }


        void OnTriggerEnter(Collider other)
        {
            if (PlayerTracker.currentPlayerState != PlayerState.playing) return;

            FVRPlayerHitbox player = other.GetComponent<FVRPlayerHitbox>();
            if (player != null)
            {
                isPlayerInSpawn = true;

                if (IFF != 0)
                {
                    areaMesh.material.SetColor("_ScrollColor", Color.grey);
                }
            }
        }


        void OnTriggerExit(Collider other)
        {
            FVRPlayerHitbox player = other.GetComponent<FVRPlayerHitbox>();
            if (player != null)
            {
                isPlayerInSpawn = false;
                areaMesh.material.SetColor("_ScrollColor", spawnColor);
            }
        }



        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, spawnRadius);
#endif
        }

    }
}


