using FistVR;
using Sodalite.Api;
using Sodalite.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes.Conquest
{
    public class ConquestManager : MonoBehaviour
    {
        public Transform playerSpawn;
        public List<ConquestLevel> levels;
        public List<TimePeriodOption> timePeriodOptions;
        public List<TicketDisplay> ticketDisplays;
        public WristMapController wristMap;
        public string levelName = "GenericConquest";
        public float sosigSpawnFrequency = 10;
        public int additionalEnemySosigSpawns = 1;
        public float ticketUpdateFrequency = 10;
        public int ticketPointControlCost = 10;
        public int ticketKillCost = 5;
        public float captureUpdateFrequency = 1;
        public float playerHealth = 1000f;
        public int startingTickets = 1000;

        [HideInInspector]
        public List<Team> teams;
        [HideInInspector]
        public ConquestLevel currentLevel;
        [HideInInspector]
        public TimePeriodOption currentTimePeriod;
        [HideInInspector]
        public PlayerLoadout currentPlayerLoadout;
        [HideInInspector]
        public float timeTillSpawn = 0;
        [HideInInspector]
        public float timeTillTicketUpdate = 0;
        [HideInInspector]
        public bool hasInit = false;
        
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

        void Update()
        {
            if (!hasInit) return;

            SosigSpawnUpdate();
            TicketUpdate();
        }

        private void SosigSpawnUpdate()
        {
            if (timeTillSpawn <= 0)
            {
                for (int team = 0; team < teams.Count; team++)
                {
                    for(int sosigIndex = 0; sosigIndex < 1 || (team != 0 && sosigIndex < 1 + additionalEnemySosigSpawns); sosigIndex++)
                    {
                        if (teams[team].HasRoomForMoreMembers())
                        {
                            SpawnSosig(team);
                        }
                    }
                }
                timeTillSpawn = sosigSpawnFrequency;
            }

            timeTillSpawn -= Time.deltaTime;
        }

        private void TicketUpdate()
        {
            if (timeTillTicketUpdate <= 0)
            {
                foreach(ConquestPoint point in currentLevel.points)
                {
                    if (point.DoesTeamControlPoint(0))
                    {
                        teams[1].score -= ticketPointControlCost;
                    }
                    else if (point.DoesTeamControlPoint(1))
                    {
                        teams[0].score -= ticketPointControlCost;
                    }
                }

                UpdateTicketDisplays();

                timeTillTicketUpdate = ticketUpdateFrequency;
            }

            timeTillTicketUpdate -= Time.deltaTime;
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

            ResetConquest();

            GM.CurrentSceneSettings.SosigKillEvent += OnSosigKill;
            GM.CurrentSceneSettings.PlayerDeathEvent += OnPlayerDeath;
            GM.CurrentPlayerBody.SetHealthThreshold(playerHealth);
            GM.CurrentSceneSettings.IsSpawnLockingEnabled = false;
        }


        public void SetPlayerHealth(int value)
        {
            if (gameObject.activeInHierarchy)
            {
                playerHealth = value;
                GM.CurrentPlayerBody.SetHealthThreshold(value);
            }
        }

        public void SetSosigTeamSize(int team, int value)
        {
            if (gameObject.activeInHierarchy)
            {
                teams[team].maxSosigs = value;

                foreach (TimePeriodOption timePeriod in timePeriodOptions)
                {
                    timePeriod.Teams[team].maxSosigs = value;
                }
            }
        }

        public void SetSosigDelay(float value)
        {
            if (gameObject.activeInHierarchy)
            {
                sosigSpawnFrequency = value;
            }
        }

        public void SetWristMode(int value)
        {
            wristMap.displayMode = (WristMapDisplayMode)value;
        }

        public void SetActiveTimePeriod(TimePeriodOption timePeriod)
        {
            timePeriod.InitializeLoadouts();
            currentTimePeriod = timePeriod;
            teams.Clear();
            teams.AddRange(timePeriod.Teams);
            SetPlayerLoadout(timePeriod.Loadouts[0]);
            UpdateTicketDisplays();
        }


        public void ResetConquest()
        {
            Debug.Log("Resetting Conquest");
            ResetTeams();
            UpdateTicketDisplays();
            currentLevel.ResetLevel();
        }

        public void UpdateTicketDisplays()
        {
            foreach (TicketDisplay ticketDisplay in ticketDisplays)
            {
                ticketDisplay.SetTeamColors(teams[0].teamColor, teams[1].teamColor);
                ticketDisplay.SetTicketValues(teams[0].score, teams[1].score);
            }
        }

        public void ResetTeams()
        {
            foreach (Team team in teams)
            {
                team.KillAllSosigs();
                team.score = startingTickets;
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
            List<ConquestPoint> validPoints = currentLevel.points.Where(point => point.DoesTeamControlPoint(0)).ToList();
            
            if (validPoints.Count > 0)
            {
                playerSpawn.position = validPoints.GetRandom().SpawnAreas.GetRandom().GetPositionInArea();
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


        private bool SpawnSosig(int team)
        {
            List<ConquestPoint> validPoints = currentLevel.points
                .Where(point => point.DoesTeamControlPoint(team) && point.SpawnAreas.Any(spawn => spawn.CanSosigSpawn(team)))
                .ToList();
            if (validPoints.Count <= 0)
            {
                Debug.Log("No Where to spawn!");
                return false;
            }
            
            ConquestPoint selectedPoint = validPoints.GetRandom();

            SosigAPI.SpawnOptions spawnOptions = new SosigAPI.SpawnOptions
            {
                SpawnState = Sosig.SosigOrder.Assault,
                SpawnActivated = true,
                EquipmentMode = SosigAPI.SpawnOptions.EquipmentSlots.All,
                SpawnWithFullAmmo = true,
                IFF = team,
                SosigTargetPosition = selectedPoint.GetNeighboringPointToAttack().AttackPoints.GetRandom().position
            };

            SosigEnemyTemplate template = teams[team].builtSosigTemplates.GetRandom();
            Sosig sosig = selectedPoint.SpawnAreas.GetRandom().SpawnSosig(template, spawnOptions);

            ConquestSosig cSosig = sosig.Links[0].gameObject.AddComponent<ConquestSosig>();
            cSosig.sosig = sosig;
            cSosig.team = teams[team];

            cSosig.EquipSlothingItem(cSosig.team.teamClothingPrefab, 2);

            teams[team].sosigs.Add(cSosig);
            return true;
        }
    }
}

