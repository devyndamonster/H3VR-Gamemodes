using FistVR;
using Gamemodes;
using Sodalite;
using Sodalite.Api;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;



namespace KOTH
{
    public class KOTHMenuController : MonoBehaviour
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
		public ValueButtonList settingsButtons;


		private static KOTHMenuController instanceRef;
		public static KOTHMenuController instance
		{
			get
			{
				if (instanceRef == null)
				{
					instanceRef = FindObjectOfType<KOTHMenuController>();
				}

				return instanceRef;
			}
			set
			{
				instanceRef = value;
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
			instance = this;

			loadingContainer.SetActive(true);
			loadingBar.gameObject.SetActive(true);

			gamePanel.SetActive(false);
			loadoutPanel.gameObject.SetActive(false);
			KOTHManager.instance.gameObject.SetActive(false);
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
				KOTHManager.instance.gameObject.SetActive(true);

				//Initialize game stuff
				KOTHManager.instance.DelayedInit();
				InitGamemodeButtons();
				InitLevelButtons();
				InitTimePeriodButtons();
				InitLoadoutButtons();

				InitSettings();
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

			foreach(KOTHLevel level in KOTHManager.instance.levels)
            {
				levelButtons.AddButton(level.name, () => { KOTHManager.instance.SetActiveLevel(level); }, true);
			}

			levelButtons.buttons[0].SetSelected();
		}

		public void InitTimePeriodButtons()
		{
			timePeriodButtons.ClearButtons();

			foreach (TimePeriodOption timePeriod in KOTHManager.instance.timePeriodOptions)
			{
				timePeriodButtons.AddButton(
					timePeriod.TimePeriodName, 
					() =>
					{
						timePeriod.InitializeLoadouts();
						KOTHManager.instance.SetActiveTimePeriod(timePeriod);
						InitLoadoutButtons();
					}, 
					true);
			}

			timePeriodButtons.buttons[0].SetSelected();
		}

		public void InitLoadoutButtons()
		{
			loadoutButtons.ClearButtons();

			foreach (PlayerLoadout loadout in KOTHManager.instance.currentTimePeriod.Loadouts)
			{
				loadoutButtons.AddButton(loadout.LoadoutName, () => { KOTHManager.instance.SetPlayerLoadout(loadout); }, true);
			}

			loadoutButtons.buttons[0].SetSelected();
		}


		private void InitSettings()
        {
			Team greenTeam = KOTHManager.instance.teams[0];
			settingsButtons.AddButton(
				"Green Team Size",
				0,
				999,
				greenTeam.maxSosigs,
				1,
				(int value) => { KOTHManager.instance.SetSosigTeamSize(0, value); });

			Team redTeam = KOTHManager.instance.teams[1];
			settingsButtons.AddButton(
				"Red Team Size",
				0,
				999,
				redTeam.maxSosigs,
				1,
				(int value) => { KOTHManager.instance.SetSosigTeamSize(1, value); });

			settingsButtons.AddButton(
				"Respawn Delay",
				1f,
				999f,
				KOTHManager.instance.sosigSpawnFrequency,
				1f,
				(float value) => { KOTHManager.instance.SetSosigDelay(value); });

			settingsButtons.AddButton(
				"Player Health",
				1000,
				100000,
				(int)KOTHManager.instance.playerHealth,
				1000,
				(int value) => { KOTHManager.instance.SetPlayerHealth(value); });

			settingsButtons.AddButton(
				"Wrist Map Mode",
				0,
				2,
				(int)KOTHManager.instance.wristMap.displayMode,
				1,
				(int value) => { KOTHManager.instance.SetWristMode(value); });
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
				KOTHManager.instance.currentPlayerLoadout, 
				KOTHManager.instance.currentTimePeriod.FirearmPools,
				KOTHManager.instance.currentTimePeriod.EquipmentPools);
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

			if (KOTHManager.instance.gameObject.activeInHierarchy)
			{
				KOTHManager.instance.MovePlayerSpawnpoint();
				KOTHManager.instance.OnPlayerFadeComplete();
				GM.CurrentMovementManager.TeleportToPoint(KOTHManager.instance.playerSpawn.position, true, KOTHManager.instance.playerSpawn.forward);
			}

			SteamVR_Fade.Start(Color.clear, 0.5f, false);
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

