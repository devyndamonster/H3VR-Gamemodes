using FistVR;
using Sodalite.Api;
using Sodalite.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamemodes.Conquest
{
    public class ConquestManager : MonoBehaviour
    {

        public Transform playerSpawn;
        public List<Team> teams;
        public List<ConquestLevel> levels;
        public List<TimePeriodOption> timePeriodOptions;
        public WristMapController wristMap;

        public float sosigSpawnFrequency = 10;
        public float captureUpdateFrequency = 1;
        public float playerHealth = 1000f;
        public string levelName = "GenericConquest";


        private static ConquestManager instanceRef;
        public static ConquestManager instance
        {
            get
            {
                if (instanceRef == null)
                {
                    instanceRef = FindObjectOfType<ConquestManager>();
                }

                return instanceRef;
            }
            set
            {
                instanceRef = value;
            }
        }

        [HideInInspector]
        public ConquestLevel currentLevel;

        [HideInInspector]
        public TimePeriodOption currentTimePeriod;

        [HideInInspector]
        public PlayerLoadout currentPlayerLoadout;

        [HideInInspector]
        public float timeTillSpawn = 0;

        [HideInInspector]
        public bool hasInit = false;


        void Update()
        {
            if (!hasInit) return;

            if (timeTillSpawn <= 0)
            {
                for (int i = 0; i < teams.Count; i++)
                {
                    SpawnSosig(i);
                }

                timeTillSpawn = sosigSpawnFrequency;
            }

            timeTillSpawn -= Time.deltaTime;
        }


        public void DelayedInit()
        {
            hasInit = true;
            Debug.Log("Delayed init of conquest manager!");

            //KOTHSaveManager.InitSaveManager(this);

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
            ResetConquest();
            currentTimePeriod = timePeriod;
            teams.Clear();
            teams.AddRange(timePeriod.Teams);
            SetPlayerLoadout(timePeriod.Loadouts[0]);
        }


        public void ResetConquest()
        {
            Debug.Log("Resetting KOTH");

            foreach (Team team in teams)
            {
                team.KillAllSosigs();
            }
        }

        public void SetActiveLevel(ConquestLevel level)
        {
            foreach (ConquestLevel l in levels)
            {
                l.gameObject.SetActive(false);
            }
            level.gameObject.SetActive(true);

            currentLevel = level;

            ResetConquest();
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
            playerSpawn.position = currentLevel.PlayerSpectatePoint.position;
            playerSpawn.rotation = currentLevel.PlayerSpectatePoint.rotation;
            GM.CurrentSceneSettings.DeathResetPoint = playerSpawn;
        }


        public void MovePlayerSpawnpoint()
        {
            SpawnArea newSpawn = GetRandomSpawnPoint(0);

            if (newSpawn != null)
            {
                playerSpawn.position = newSpawn.GetPositionInArea();
            }
            else
            {
                playerSpawn.position = currentLevel.PlayerBackupSpawn.position;
                Debug.Log("Spawning player at backup spawn. Position: " + playerSpawn.position);
            }

            GM.CurrentSceneSettings.DeathResetPoint = playerSpawn;
        }


        public void OnPlayerFadeComplete()
        {
            ConquestMenuController.Instance.CleanupItems();

            PlayerTracker.ClearPlayerItems();
            PlayerTracker.EquipPlayer(currentPlayerLoadout);
            PlayerTracker.currentPlayerState = PlayerState.playing;
        }


        private void SpawnSosig(int team)
        {
            
        }

        public SpawnArea GetRandomSpawnPoint(int team)
        {
            return null;
        }

    }
}

