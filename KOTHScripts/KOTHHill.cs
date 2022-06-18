using FistVR;
using Gamemodes;
using Sodalite.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace KOTH
{
    public class KOTHHill : MonoBehaviour
    {

        public List<KOTHSpawnPointCollection> teamSpawnPoints;
        public List<Transform> attackPoints;
        public List<Transform> defendPoints;
        public List<MeshRenderer> hillRendererList;
        public KOTHPurchasePanel purchasePanel;
        public List<BuildableCover> coverList;
        public float captureRate = 0.1f;
        public float timeToCapture = 180f;
        public List<Text> progressTextList;
        public List<ChildHillTrigger> childHillTriggers = new List<ChildHillTrigger>();

        [NonSerialized]
        public List<KOTHSosig> capturingSosigs = new List<KOTHSosig>();

        [NonSerialized]
        public List<int> captureCount = new List<int>();

        [NonSerialized]
        public int currentTeam = -1;

        [NonSerialized]
        public bool isPlayerInHill;

        [NonSerialized]
        public List<AutoMeater> turretList = new List<AutoMeater>();

        private float timeTillCaptureUpdate = 0;
        private float captureProgress = 0;
        private float timeOfCapture;
        private float timeRemaining;


        void Awake()
        {
            if(purchasePanel != null)
            {
                purchasePanel.hill = this;
            }
        }


        void Start()
        {
            for(int i = 0; i < KOTHManager.instance.teams.Count; i++)
            {
                captureCount.Add(0);
            }

            timeOfCapture = Time.time;
        }


        void Update()
        {
            if (!KOTHManager.instance.hasInit) return;

            timeTillCaptureUpdate -= Time.deltaTime;

            if (timeTillCaptureUpdate <= 0)
            {
                timeTillCaptureUpdate = KOTHManager.instance.captureUpdateFrequency;
                UpdateCaptureProgress();
            }

            UpdateTime();
        }


        public void ResetHill()
        {
            captureProgress = 0;
            currentTeam = -1;
            timeRemaining = timeToCapture;
            
            SetAllSpawnsToNormal();

            foreach(Text text in progressTextList)
            {
                text.text = GetTimeString();
                text.color = new Color(1, 1, 1, 0.5f);
            }

            foreach(MeshRenderer renderer in hillRendererList)
            {
                renderer.material.SetFloat("_CaptureProgress", captureProgress);
            }
        }


        public void ResetDefenses()
        {
            if (purchasePanel != null)
            {
                purchasePanel.SetPoints(0);
                purchasePanel.ClearSpawnedItems();
            }

            foreach (BuildableCover cover in coverList)
            {
                cover.ResetCover();
            }

            for(int i = 0; i < turretList.Count; i++)
            {
                if(turretList[i] != null)
                {
                    Destroy(turretList[i].gameObject);
                }
            }

            turretList.Clear();
        }


        public void ShowBuildableAreas()
        {
            foreach (BuildableCover cover in coverList)
            {
                if (!cover.built)
                {
                    cover.areaCollider.enabled = true;
                    cover.areaMesh.enabled = true;
                }
            }
        }


        public void HideBuildableAreas()
        {
            foreach (BuildableCover cover in coverList)
            {
                cover.areaCollider.enabled = false;
                cover.areaMesh.enabled = false;
            }
        }



        private string GetTimeString()
        {
            return ((int)(timeRemaining / 60)) + " : " + string.Format("{0:00}", (int)(timeRemaining % 60));
        }


        private void UpdateTime()
        {
            if(currentTeam != -1)
            {
                float prevTime = timeRemaining;
                timeRemaining = timeToCapture - (Time.time - timeOfCapture);

                foreach (Text text in progressTextList)
                {
                    text.text = GetTimeString();
                }

                //If the hill has been captured, reset everything and tell the manager to move hills
                if (timeRemaining <= 0)
                {
                    capturingSosigs.Clear();
                    ResetDefenses();
                    ResetHill();
                    KOTHManager.instance.SetNextHill();
                }
            }
        }


        private void ResetCaptureCount()
        {
            for (int i = 0; i < captureCount.Count; i++)
            {
                captureCount[i] = 0;
            }
        }


        /// <summary>
        /// Updates the capture count based on who's in the hill, and returns the index of the max count
        /// </summary>
        /// <returns>Index of max team</returns>
        private int UpdateCaptureCount()
        {
            if (isPlayerInHill && PlayerTracker.currentPlayerState == PlayerState.playing) captureCount[0] += 1;

            int maxIndex = 0;
            for (int i = 0; i < capturingSosigs.Count; i++)
            {
                KOTHSosig sosig = capturingSosigs[i];

                if (sosig == null)
                {
                    capturingSosigs.RemoveAt(i);
                    i -= 1;
                    continue;
                }

                if (sosig.sosig.E.IFFCode < 0 || sosig.sosig.E.IFFCode >= captureCount.Count)
                {
                    continue;
                }

                captureCount[sosig.sosig.E.IFFCode] += 1;

                if (captureCount[sosig.sosig.E.IFFCode] > captureCount[maxIndex]) maxIndex = sosig.sosig.E.IFFCode;
            }

            return maxIndex;
        }


        private void UpdateCaptureProgress()
        {
            ResetCaptureCount();

            int maxIndex = UpdateCaptureCount();

            if (captureCount[maxIndex] > 0)
            {
                //If the point is neutralized, give it to the current controlling team
                if (currentTeam == -1)
                {
                    currentTeam = maxIndex;
                    captureProgress += captureRate;

                    foreach (MeshRenderer renderer in hillRendererList)
                    {
                        renderer.material.SetColor("_TeamColor", KOTHManager.instance.teams[maxIndex].teamColor);
                    }

                    timeOfCapture = Time.time;

                    foreach (Text text in progressTextList)
                    {
                        text.color = KOTHManager.instance.teams[maxIndex].teamColor;
                    }

                    //Add a point every time the hill changes hands
                    if (purchasePanel != null)
                    {
                        purchasePanel.AddPoints(2);
                    }

                    //Swap over turrets to controlling team
                    foreach(AutoMeater turret in turretList)
                    {
                        turret.E.IFFCode = currentTeam;
                    }

                    //This is horrible code that only works with two teams!
                    if(maxIndex == 0)
                    {
                        SetAllSpawnsToTeam(1);
                    }
                    else
                    {
                        SetAllSpawnsToTeam(0);
                    }
                }

                //If the controlling team does not own the point, decap it
                else if (currentTeam != maxIndex)
                {
                    captureProgress -= captureRate;
                }

                //Otherwise, the controlling team owns the point and we cap it 
                else
                {
                    if (captureProgress < 1 && captureProgress + captureRate >= 1) Captured();
                    captureProgress = Mathf.Min(captureProgress + captureRate, 1);
                }

                //If the point is uncapped even with sosigs in it, then we just neutralized it
                if (captureProgress <= 0)
                {
                    Neutralized(currentTeam);
                }

                foreach (MeshRenderer renderer in hillRendererList)
                {
                    renderer.material.SetFloat("_CaptureProgress", captureProgress);
                }
            }
        }


        private void Captured()
        {
            foreach (KOTHSosig sosig in capturingSosigs)
            {
                if(sosig.sosig.E.IFFCode == currentTeam)
                {
                    sosig.OrderToDefend(this);
                }
            }
        }

        private void Neutralized(int originalTeam)
        {
            ResetHill();
            KOTHManager.instance.OnHillNeutralized();

            foreach(KOTHSosig sosig in KOTHManager.instance.teams[originalTeam].sosigs)
            {
                sosig.OrderToAssault(this);
            }
        }


        private void SetAllSpawnsToTeam(int team)
        {
            foreach (KOTHSpawnPointCollection teamSpawns in teamSpawnPoints)
            {
                foreach (SpawnArea area in teamSpawns.spawnPoints)
                {
                    area.SetTeam(team, KOTHManager.instance.teams[team].teamColor);
                }
            }
        }


        private void SetAllSpawnsToNormal()
        {
            for(int team = 0; team < teamSpawnPoints.Count; team++)
            {
                foreach (SpawnArea area in teamSpawnPoints[team].spawnPoints)
                {
                    area.SetTeam(team, KOTHManager.instance.teams[team].teamColor);
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            KOTHSosig sosig = other.GetComponent<KOTHSosig>();
            if(sosig != null)
            {
                OnSosigEnteredHill(sosig);
                return;
            }

            FVRPlayerHitbox player = other.GetComponent<FVRPlayerHitbox>();
            if(player != null)
            {
                OnPlayerEnteredHill(player);
                return;
            }
        }


        void OnTriggerExit(Collider other)
        {
            KOTHSosig sosig = other.GetComponent<KOTHSosig>();
            if (sosig != null)
            {
                OnSosigExitedHill(sosig);
                return;
            }

            FVRPlayerHitbox player = other.GetComponent<FVRPlayerHitbox>();
            if (player != null)
            {
                OnPlayerExitedHill(player);
                return;
            }
        }


        public void OnSosigEnteredHill(KOTHSosig sosig)
        {
            if (!capturingSosigs.Contains(sosig))
            {
                capturingSosigs.Add(sosig);
            }

            sosig.currentHill = this;

            if (sosig.sosig.E.IFFCode == currentTeam && captureProgress >= 1)
            {
                sosig.OrderToDefend(this);
            }
        }

        public void OnSosigExitedHill(KOTHSosig sosig)
        {
            //If this sosig is not also in any child hill triggers, remove it
            if(!childHillTriggers.Any(o => o.capturingSosigs.Contains(sosig)))
            {
                capturingSosigs.Remove(sosig);
            }

            sosig.currentHill = null;
        }

        public void OnPlayerEnteredHill(FVRPlayerHitbox player)
        {
            isPlayerInHill = true;
        }

        public void OnPlayerExitedHill(FVRPlayerHitbox player)
        {
            //If the player is not also in any hill triggers, they are not in this hill
            if(!childHillTriggers.Any(o => o.isPlayerInHill))
            {
                isPlayerInHill = false;
            }
        }


        void OnDrawGizmos()
        {
#if UNITY_EDITOR

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.5f);

            if (KOTHManager.instance == null) return;

            for (int team = 0; team < teamSpawnPoints.Count; team++)
            {
                Gizmos.color = KOTHManager.instance.teams[team].teamColor;
                
                foreach (SpawnArea point in teamSpawnPoints[team].spawnPoints)
                {
                    Gizmos.DrawSphere(point.transform.position, 0.25f);
                    Gizmos.DrawLine(point.transform.position, transform.position);
                }
            }

            Gizmos.color = Color.magenta;
            foreach(Transform point in defendPoints)
            {
                Gizmos.DrawSphere(point.position, 0.25f);
            }

            Gizmos.color = Color.cyan;
            foreach (Transform point in attackPoints)
            {
                Gizmos.DrawSphere(point.position, 0.25f);
            }
#endif
        }

    }


}
