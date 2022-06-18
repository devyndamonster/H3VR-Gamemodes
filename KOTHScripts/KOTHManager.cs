using FistVR;
using Gamemodes;
using Sodalite.Api;
using Sodalite.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KOTH
{
    public class KOTHManager : MonoBehaviour
    {
        public Transform playerSpawn;
        public List<Team> teams;
        public List<KOTHLevel> levels;
        public List<TimePeriodOption> timePeriodOptions;
        public WristMapController wristMap;

        public float sosigSpawnFrequency = 10;
        public float captureUpdateFrequency = 1;
        public float playerHealth = 1000f;
        public string levelName = "GenericKOTH";


        private static KOTHManager instanceRef;
        public static KOTHManager instance {
            get
            {
                if (instanceRef == null)
                {
                    instanceRef = FindObjectOfType<KOTHManager>();
                }

                return instanceRef;
            }
            set
            {
                instanceRef = value;
            } 
        }

        [HideInInspector]
        public KOTHLevel currentLevel;

        [HideInInspector]
        public TimePeriodOption currentTimePeriod;

        [HideInInspector]
        public PlayerLoadout currentPlayerLoadout;

        [HideInInspector]
        public int currentHillIndex = 0;

        [HideInInspector]
        public float timeTillSpawn = 0;

        [HideInInspector]
        public bool hasInit = false;



        void Awake()
        {
            instance = this;
        }


		void Update()
		{
            if (!hasInit) return;

            if (timeTillSpawn <= 0)
            {
                for(int i = 0; i < teams.Count; i++)
                {
                    if (currentLevel.hills[currentHillIndex].currentTeam == i || teams[i].sosigs.Count >= teams[i].maxSosigs) continue;

                    SpawnSosig(i);
                }

                timeTillSpawn = sosigSpawnFrequency;
            }

            timeTillSpawn -= Time.deltaTime;
		}


        public void DelayedInit()
        {
            hasInit = true;
            Debug.Log("Delayed init of koth manager!");

            KOTHSaveManager.InitSaveManager(this);

            //First, setup all the level stuff
            SetActiveLevel(levels[0]);

            //Afterwards, set the time period
            SetActiveTimePeriod(timePeriodOptions[0]);

            GM.CurrentSceneSettings.SosigKillEvent += OnSosigKill;
            GM.CurrentSceneSettings.PlayerDeathEvent += OnPlayerDeath;
            GM.CurrentPlayerBody.SetHealthThreshold(playerHealth);
        }


        public void SetActiveTimePeriod(TimePeriodOption timePeriod)
        {
            ResetKOTH();
            currentTimePeriod = timePeriod;
            teams.Clear();
            teams.AddRange(timePeriod.Teams);
            SetPlayerLoadout(timePeriod.Loadouts[0]);
        }


        public void ResetKOTH()
        {
            Debug.Log("Resetting KOTH");

            foreach (Team team in teams)
            {
                team.KillAllSosigs();
            }

            currentLevel.hills[currentHillIndex].ResetHill();
            currentLevel.hills[currentHillIndex].ResetDefenses();
        }


        public void SetNextHill()
        {
            //Deactivate all hills
            foreach(KOTHHill hill in currentLevel.hills)
            {
                hill.gameObject.SetActive(false);
            }

            currentHillIndex += 1;
            if (currentHillIndex >= currentLevel.hills.Count) currentHillIndex = 0;

            KOTHHill nextHill = currentLevel.hills[currentHillIndex];
            nextHill.gameObject.SetActive(true);
            nextHill.capturingSosigs.Clear();
            nextHill.ResetHill();
            nextHill.ResetDefenses();

            foreach(Team team in teams)
            {
                foreach(KOTHSosig sosig in team.sosigs)
                {
                    sosig.OrderToAssault(nextHill);
                }
            }

            if (PlayerTracker.currentPlayerState == PlayerState.waitingToRespawn)
            {
                KOTHMenuController.instance.SetPlay();
            }

            wristMap.targets.Clear();
            wristMap.targets.Add(new WristMapTarget(nextHill.transform, Color.cyan, 0.1f));
        }



        public void SetActiveLevel(KOTHLevel level)
        {
            foreach (KOTHLevel l in levels)
            {
                l.gameObject.SetActive(false);
            }
            level.gameObject.SetActive(true);

            currentLevel = level;
            currentHillIndex = UnityEngine.Random.Range(0, level.hills.Count);

            SetNextHill();
            ResetKOTH();
        }

        public void SetPlayerLoadout(PlayerLoadout loadout)
        {
            currentPlayerLoadout = loadout;
        }


        void OnDestroy()
        {
            GM.CurrentSceneSettings.SosigKillEvent -= OnSosigKill;
            GM.CurrentSceneSettings.PlayerDeathEvent -= OnPlayerDeath;
        }

        private void OnSosigKill(Sosig sosig)
        {
            sosig.ClearSosig();
        }

        private void OnPlayerDeath(bool killedSelf)
        {
            //We must move the spawnpoint before the player actually respawns
            MovePlayerSpawnpoint();
            PlayerTracker.currentPlayerState = PlayerState.spawningIn;
            
            Invoke("OnPlayerFadeComplete", GM.CurrentSceneSettings.PlayerDeathFade);
        }


        private void MoveSpawnToSpectate()
        {
            playerSpawn.position = currentLevel.spectatePoint.position;
            playerSpawn.rotation = currentLevel.spectatePoint.rotation;
            GM.CurrentSceneSettings.DeathResetPoint = playerSpawn;
        }


        public void MovePlayerSpawnpoint()
        {
            bool spawnAnywhere = currentLevel.hills[currentHillIndex].currentTeam != -1 && currentLevel.hills[currentHillIndex].currentTeam != 0;
            SpawnArea newSpawn = GetRandomSpawnPoint(0, spawnAnywhere);
           
            if(newSpawn != null)
            {
                playerSpawn.position = newSpawn.GetPositionInArea();
            }
            else
            {
                //playerSpawn.position = currentLevel.spectatePoint.position;

                playerSpawn.position = currentLevel.PlayerBackupSpawn.position;
                Debug.Log("Spawning player at backup spawn. Position: " + playerSpawn.position);
            }

            GM.CurrentSceneSettings.DeathResetPoint = playerSpawn;
        }


        public void OnHillNeutralized()
        {
            if (PlayerTracker.currentPlayerState == PlayerState.waitingToRespawn)
            {
                KOTHMenuController.instance.SetPlay();
            }
        }


        public void OnPlayerFadeComplete()
        {
            KOTHMenuController.instance.CleanupItems();

            //If the player has to wait to respawn, set them as a spectator
            /*
            if (currentLevel.hills[currentHillIndex].currentTeam == 0)
            {
                MoveSpawnToSpectate();
                gameController.SetPlayerSpectator(currentLevel.spectatePoint);
                PlayerTracker.currentPlayerState = PlayerState.waitingToRespawn;
            }
            
            //If the player is to respawn, get them ready to spawn
            else
            {
                PlayerTracker.ClearPlayerItems();
                PlayerTracker.EquipPlayer(currentPlayerLoadout);
                PlayerTracker.currentPlayerState = PlayerState.playing;
            }
            */


            //The player will always respawn
            PlayerTracker.ClearPlayerItems();
            PlayerTracker.EquipPlayer(currentPlayerLoadout);
            PlayerTracker.currentPlayerState = PlayerState.playing;
        }


        private void SpawnSosig(int team)
        {
            KOTHHill currentHill = currentLevel.hills[currentHillIndex];

            SosigAPI.SpawnOptions spawnOptions = new SosigAPI.SpawnOptions
            {
                SpawnState = Sosig.SosigOrder.Assault,
                SpawnActivated = true,
                EquipmentMode = SosigAPI.SpawnOptions.EquipmentSlots.All,
                SpawnWithFullAmmo = true,
                IFF = team,
                SosigTargetPosition = currentHill.attackPoints.GetRandom().position
            };

            bool spawnAnywhere = currentHill.currentTeam != team && currentHill.currentTeam != -1;
            SpawnArea spawnArea = GetRandomSpawnPoint(team, spawnAnywhere);
            if (spawnArea == null) return;

            SosigEnemyTemplate template = teams[team].builtSosigTemplates.GetRandom();
            Sosig sosig = spawnArea.SpawnSosig(template, spawnOptions);

            KOTHSosig kSosig = sosig.Links[0].gameObject.AddComponent<KOTHSosig>();
            kSosig.sosig = sosig;
            kSosig.team = teams[team];

            kSosig.EquipSlothingItem(kSosig.team.teamClothingPrefab, 2);

            teams[team].sosigs.Add(kSosig);
        }

        public SpawnArea GetRandomSpawnPoint(int team, bool spawnAnywhere)
        {
            if (spawnAnywhere)
            {
                SpawnArea area = currentLevel.hills[currentHillIndex].teamSpawnPoints.GetRandom().spawnPoints.GetRandom();

                if (area.CanSosigSpawn(team)) return area;

                foreach(SpawnPointCollection collection in currentLevel.hills[currentHillIndex].teamSpawnPoints)
                {
                    foreach(SpawnArea newArea in collection.spawnPoints)
                    {
                        if (newArea.CanSosigSpawn(team)) return newArea;
                    }
                }
            }
            else
            {
                SpawnArea area = currentLevel.hills[currentHillIndex].teamSpawnPoints.GetRandom().spawnPoints.GetRandom();

                if (area.CanSosigSpawn(team)) return area;

                foreach (SpawnArea newArea in currentLevel.hills[currentHillIndex].teamSpawnPoints[team].spawnPoints)
                {
                    if (newArea.CanSosigSpawn(team)) return newArea;
                }
            }

            return null;
        }
    }
}

