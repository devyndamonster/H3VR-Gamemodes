using FistVR;
using Gamemodes;
using Sodalite;
using Sodalite.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

namespace Gamemodes.Conquest
{
	public class ConquestMenuController : MonoBehaviour
	{

		public GameObject gamePanel;
		public LoadoutEditor loadoutPanel;
		public GameObject loadingContainer;
		public Slider loadingBar;
		public Transform menuPoint;

		public ButtonList gamemodeButtons;
		public ButtonList levelButtons;
		public ButtonList timePeriodButtons;
		public ButtonList loadoutButtons;

		public List<Text> teamText;
		public Text healthText;
		public Text rateText;
		public Text wristText;

		private static ConquestMenuController _instance;
		public static ConquestMenuController Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<ConquestMenuController>();
				}

				return _instance;
			}
			set
			{
				_instance = value;
			}
		}

		[HideInInspector]
		public bool hasPlayerInit = false;

		[HideInInspector]
		public bool hasGameInit = false;

		private float basePointDistance;

		private WristMenuButton menuButton;

		void Awake()
		{
			loadingContainer.SetActive(true);
			loadingBar.gameObject.SetActive(true);

			gamePanel.SetActive(false);
			loadoutPanel.gameObject.SetActive(false);
			ConquestManager.instance.gameObject.SetActive(false);
		}


		void Update()
		{
			if (!hasPlayerInit)
			{
				DelayedPlayerInit();
			}

			else if (!hasGameInit)
			{
				UpdateLoadProgress();
				DelayedGameInit();
			}
		}


		private void DelayedPlayerInit()
		{
			if (GM.CurrentPlayerBody != null && GM.CurrentSceneSettings != null)
			{
				Debug.Log("Delayed Init of Player");
				hasPlayerInit = true;

				//Then initialize player stuff
				basePointDistance = GM.CurrentSceneSettings.MaxPointingDistance;
				SetPlayerSpectator(menuPoint);

				menuButton = new WristMenuButton("Game Menu", StartGoToMenu);
				WristMenuAPI.Buttons.Add(menuButton);
			}
		}


		private void DelayedGameInit()
		{
			if (OtherLoader.LoaderStatus.GetLoaderProgress() >= 1)
			{
				Debug.Log("Delayed Init of Game");
				hasGameInit = true;

				//Deactivate stuff
				loadingContainer.SetActive(false);

				//Activate stuff
				gamePanel.SetActive(true);
				ConquestManager.instance.gameObject.SetActive(true);

				//Initialize game stuff
				ConquestManager.instance.DelayedInit();
				InitGamemodeButtons();
				InitLevelButtons();
				InitTimePeriodButtons();
				InitLoadoutButtons();

				InitSettingsText();
			}
		}


		public void InitGamemodeButtons()
		{
			gamemodeButtons.ClearButtons();

			gamemodeButtons.AddButton("KOTH", () => { }, true);

			gamemodeButtons.buttons[0].SetSelected();
		}

		public void InitLevelButtons()
		{
			levelButtons.ClearButtons();

			foreach (ConquestLevel level in ConquestManager.instance.levels)
			{
				levelButtons.AddButton(level.name, () => { ConquestManager.instance.SetActiveLevel(level); }, true);
			}

			levelButtons.buttons[0].SetSelected();
		}

		public void InitTimePeriodButtons()
		{
			timePeriodButtons.ClearButtons();

			foreach (TimePeriodOption timePeriod in ConquestManager.instance.timePeriodOptions)
			{
				timePeriodButtons.AddButton(
					timePeriod.TimePeriodName,
					() =>
					{
						timePeriod.InitializeLoadouts();
						ConquestManager.instance.SetActiveTimePeriod(timePeriod);
						InitLoadoutButtons();
					},
					true);
			}

			timePeriodButtons.buttons[0].SetSelected();
		}

		public void InitLoadoutButtons()
		{
			loadoutButtons.ClearButtons();

			foreach (PlayerLoadout loadout in ConquestManager.instance.currentTimePeriod.Loadouts)
			{
				loadoutButtons.AddButton(loadout.LoadoutName, () => { ConquestManager.instance.SetPlayerLoadout(loadout); }, true);
			}

			loadoutButtons.buttons[0].SetSelected();
		}


		private void InitSettingsText()
		{
			for (int i = 0; i < teamText.Count; i++)
			{
				teamText[i].text = ConquestManager.instance.teams[i].maxSosigs.ToString();
			}

			healthText.text = ConquestManager.instance.playerHealth.ToString();
			rateText.text = ConquestManager.instance.sosigSpawnFrequency.ToString();
			wristText.text = ConquestManager.instance.wristMap.displayMode.ToString();
		}


		private void UpdateLoadProgress()
		{
			loadingBar.value = OtherLoader.LoaderStatus.GetLoaderProgress();
		}


		void OnDestroy()
		{
			WristMenuAPI.Buttons.Remove(menuButton);
		}


		public void StartGoToMenu(object sender, ButtonClickEventArgs args)
		{
			if (PlayerTracker.currentPlayerState == PlayerState.enteringSpectator) return;
			PlayerTracker.currentPlayerState = PlayerState.enteringSpectator;

			GM.CurrentSceneSettings.DoesDamageGetRegistered = false;
			SteamVR_Fade.Start(Color.black, GM.CurrentSceneSettings.PlayerDeathFade, false);
			Invoke("DelayedGoToMenu", GM.CurrentSceneSettings.PlayerDeathFade);
		}


		public void DelayedGoToMenu()
		{
			SetPlayerSpectator(menuPoint);
			CleanupItems();
			gamePanel.SetActive(true);
			loadoutPanel.gameObject.SetActive(false);

			SteamVR_Fade.Start(Color.clear, 0.5f, false);
		}


		public void SetPlay()
		{
			if (PlayerTracker.currentPlayerState == PlayerState.spawningIn) return;
			PlayerTracker.currentPlayerState = PlayerState.spawningIn;

			SteamVR_Fade.Start(Color.black, GM.CurrentSceneSettings.PlayerDeathFade, false);
			Invoke("SpawnPlayer", GM.CurrentSceneSettings.PlayerDeathFade);
		}


		public void SetEditLoadout()
		{
			gamePanel.SetActive(false);
			loadoutPanel.gameObject.SetActive(true);

			loadoutPanel.InitLoadoutEditor(
				ConquestManager.instance.currentPlayerLoadout,
				ConquestManager.instance.currentTimePeriod.FirearmPools,
				ConquestManager.instance.currentTimePeriod.EquipmentPools);
		}

		public void SetGameMenu()
		{
			gamePanel.SetActive(true);
			loadoutPanel.gameObject.SetActive(false);
		}


		public void SetPlayerSpectator(Transform point)
		{
			PlayerTracker.ClearPlayerItems();
			PlayerTracker.currentPlayerState = PlayerState.spectating;

			GM.CurrentPlayerRoot.localScale = new Vector3(10, 10, 10);
			WristMenuAPI.Instance.transform.localScale = GM.CurrentPlayerRoot.localScale;
			GM.CurrentMovementManager.TeleportToPoint(point.position, true, point.forward);
			GM.CurrentMovementManager.Mode = (FVRMovementManager.MovementMode)10;
			GM.CurrentPlayerBody.SetPlayerIFF(-3);
			GM.CurrentPlayerBody.DisableHitBoxes();
			GM.CurrentSceneSettings.DoesDamageGetRegistered = false;
			GM.CurrentPlayerBody.HealthBar.gameObject.SetActive(false);
			GM.CurrentSceneSettings.MaxPointingDistance = 100;
		}


		public void SpawnPlayer()
		{
			gamePanel.SetActive(false);
			loadoutPanel.gameObject.SetActive(false);

			PlayerTracker.currentPlayerState = PlayerState.playing;

			GM.CurrentPlayerRoot.localScale = new Vector3(1, 1, 1);
			WristMenuAPI.Instance.transform.localScale = GM.CurrentPlayerRoot.localScale;
			GM.CurrentMovementManager.Mode = GM.Options.MovementOptions.CurrentMovementMode;
			GM.CurrentPlayerBody.SetPlayerIFF(0);
			GM.CurrentPlayerBody.EnableHitBoxes();
			GM.CurrentSceneSettings.DoesDamageGetRegistered = true;
			GM.CurrentPlayerBody.HealthBar.gameObject.SetActive(true);
			GM.CurrentSceneSettings.MaxPointingDistance = basePointDistance;

			if (ConquestManager.instance.gameObject.activeInHierarchy)
			{
				ConquestManager.instance.MovePlayerSpawnpoint();
				ConquestManager.instance.OnPlayerFadeComplete();
				GM.CurrentMovementManager.TeleportToPoint(ConquestManager.instance.playerSpawn.position, true, ConquestManager.instance.playerSpawn.forward);
			}

			SteamVR_Fade.Start(Color.clear, 0.5f, false);
		}



		public void IncreasePlayerHealth()
		{
			if (ConquestManager.instance.gameObject.activeInHierarchy)
			{
				ConquestManager.instance.playerHealth = Math.Max(100, ConquestManager.instance.playerHealth + 100);
				GM.CurrentPlayerBody.SetHealthThreshold(ConquestManager.instance.playerHealth);

				healthText.text = ((int)ConquestManager.instance.playerHealth).ToString();
			}
		}

		public void DecreasePlayerHealth()
		{
			if (ConquestManager.instance.gameObject.activeInHierarchy)
			{
				ConquestManager.instance.playerHealth = Math.Max(100, ConquestManager.instance.playerHealth - 100);

				GM.CurrentPlayerBody.SetHealthThreshold(ConquestManager.instance.playerHealth);
				healthText.text = ((int)ConquestManager.instance.playerHealth).ToString();
			}
		}

		public void IncreaseSosigCount(int team)
		{
			if (ConquestManager.instance.gameObject.activeInHierarchy)
			{
				ConquestManager.instance.teams[team].maxSosigs = Math.Max(1, ConquestManager.instance.teams[team].maxSosigs + 1);
				teamText[team].text = ConquestManager.instance.teams[team].maxSosigs.ToString();

				//Update all time periods
				foreach (TimePeriodOption timePeriod in ConquestManager.instance.timePeriodOptions)
				{
					timePeriod.Teams[team].maxSosigs = ConquestManager.instance.teams[team].maxSosigs;
				}
			}
		}

		public void DecreaseSosigCount(int team)
		{
			if (ConquestManager.instance.gameObject.activeInHierarchy)
			{
				ConquestManager.instance.teams[team].maxSosigs = Math.Max(1, ConquestManager.instance.teams[team].maxSosigs - 1);
				teamText[team].text = ConquestManager.instance.teams[team].maxSosigs.ToString();

				//Update all time periods
				foreach (TimePeriodOption timePeriod in ConquestManager.instance.timePeriodOptions)
				{
					timePeriod.Teams[team].maxSosigs = ConquestManager.instance.teams[team].maxSosigs;
				}
			}
		}

		public void IncreaseSosigDelay()
		{
			if (ConquestManager.instance.gameObject.activeInHierarchy)
			{
				ConquestManager.instance.sosigSpawnFrequency = Math.Max(0.1f, ConquestManager.instance.sosigSpawnFrequency + 0.1f);

				rateText.text = ConquestManager.instance.sosigSpawnFrequency.ToString("0.0");
			}
		}

		public void DecreaseSosigDelay()
		{
			if (ConquestManager.instance.gameObject.activeInHierarchy)
			{
				ConquestManager.instance.sosigSpawnFrequency = Math.Max(0.1f, ConquestManager.instance.sosigSpawnFrequency - 0.1f);

				rateText.text = ConquestManager.instance.sosigSpawnFrequency.ToString("0.0");
			}
		}

		public void IncrementWristMode(bool decrease)
		{
			ConquestManager.instance.wristMap.IncrementMapState(decrease);

			wristText.text = ConquestManager.instance.wristMap.displayMode.ToString();
		}


		public void CleanupItems()
		{
			foreach (FVRPhysicalObject item in FindObjectsOfType<FVRPhysicalObject>())
			{
				if (!item.IsHeld && item.QuickbeltSlot == null)
				{
					if (item is FVRFireArm ||
						item is FVRFireArmMagazine ||
						item is FVRFireArmRound ||
						item is FVRFireArmClip ||
						item is Speedloader ||
						item is FVRMeleeWeapon ||
						item is PinnedGrenade ||
						item is FVRFireArmAttachment)
					{
						Destroy(item.gameObject);
					}


					else if (item is GrappleThrowable)
					{
						item.gameObject.GetComponent<GrappleThrowable>().ClearRopeLengths();
						Destroy(item.gameObject);
					}
				}
			}

			foreach (GrappleGunBolt bolt in FindObjectsOfType<GrappleGunBolt>())
			{
				bolt.ClearRopeLengths();
				Destroy(bolt.gameObject);
			}
		}
	}
}

