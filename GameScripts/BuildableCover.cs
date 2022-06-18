using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamemodes
{
    public class BuildableCover : MonoBehaviour
    {

        public GameObject coverGeo;
        public MeshRenderer areaMesh;
        public SphereCollider areaCollider;

        public bool built;

        void OnTriggerEnter(Collider other)
        {
            if (built) return;

            Supplies supplies = other.GetComponent<Supplies>();
            if (supplies != null)
            {
                if (supplies.m_hand != null)
                {
                    supplies.ForceBreakInteraction();
                }

                Destroy(supplies.gameObject);
                BuildCover();
            }
        }

        public void BuildCover()
        {
            built = true;
            coverGeo.SetActive(true);
            areaCollider.enabled = false;
            areaMesh.enabled = false;
        }

        public void ResetCover()
        {
            built = false;
            coverGeo.SetActive(false);
            areaCollider.enabled = false;
            areaMesh.enabled = false;
        }

    }
}

