using FistVR;
using Sodalite.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Gamemodes
{
	public enum PlayerState
	{
		waitingToRespawn,
		spectating,
		playing,
		enteringSpectator,
		spawningIn
	}


	public static class PlayerTracker
	{
		public static PlayerState currentPlayerState = PlayerState.spectating;

		public static void EquipPlayer(PlayerLoadout loadout)
		{
			ClearPlayerItems();
			EquipHands(loadout);
			EquipQuickbelt(loadout);
		}


		public static void ClearPlayerItems()
		{
			FVRViveHand rightHand = GM.CurrentPlayerBody.RightHand.GetComponent<FVRViveHand>();
			FVRInteractiveObject rightHeld = rightHand.CurrentInteractable;
			if (rightHeld != null)
			{
				rightHeld.ForceBreakInteraction();
				UnityEngine.Object.Destroy(rightHeld.gameObject);
			}

			FVRViveHand leftHand = GM.CurrentPlayerBody.LeftHand.GetComponent<FVRViveHand>();
			FVRInteractiveObject leftHeld = leftHand.CurrentInteractable;
			if (leftHeld != null)
			{
				leftHeld.ForceBreakInteraction();
				UnityEngine.Object.Destroy(leftHeld.gameObject);
			}

			GM.CurrentPlayerBody.WipeQuickbeltContents();
		}


		public static void EquipHands(PlayerLoadout loadout)
		{

			bool leftHandFull = false;
			bool rightHandFull = false;

			if (loadout.rightHandTable != null)
			{
				FVRObject selected = loadout.rightHandTable.GetItem();
				GivePlayerItemRightHand(selected.ItemID);
				rightHandFull = true;

				FVRObject compatibleAmmoObject = null;
				if (selected.CompatibleMagazines.Count > 0)
				{
					//If we don't care about capacity, get a random mag
					if (loadout.rightHandTable.maxMagCapacity < 0 && loadout.rightHandTable.minMagCapacity < 0) compatibleAmmoObject = selected.CompatibleMagazines.GetRandom();

					//Otherwise, try to get a mag within our capacity
					else
					{
						List<FVRObject> mags = selected.CompatibleMagazines.Where(o =>
							(loadout.rightHandTable.maxMagCapacity < 0 || o.MagazineCapacity <= loadout.rightHandTable.maxMagCapacity) &&
							(loadout.rightHandTable.minMagCapacity < 0 || o.MagazineCapacity >= loadout.rightHandTable.minMagCapacity)
							).ToList();

						if (mags.Count > 0) compatibleAmmoObject = mags.GetRandom();
						else compatibleAmmoObject = Sodalite.Api.FirearmAPI.GetSmallestMagazine(selected);
					}
				}

				else if (selected.CompatibleClips.Count > 0)
				{
					compatibleAmmoObject = selected.CompatibleClips.GetRandom();
				}

				else if (selected.CompatibleSpeedLoaders.Count > 0)
				{
					compatibleAmmoObject = selected.CompatibleSpeedLoaders.GetRandom();
				}

				else if (selected.CompatibleSingleRounds.Count > 0)
				{
					compatibleAmmoObject = selected.CompatibleSingleRounds.GetRandom();
				}

				if (compatibleAmmoObject != null)
				{
					if (loadout.magInOtherHand && compatibleAmmoObject.Category == FVRObject.ObjectCategory.Magazine)
					{
						GivePlayerItemLeftHand(compatibleAmmoObject.ItemID);
						leftHandFull = true;
					}

					GivePlayerItemQuickbelt(compatibleAmmoObject.ItemID);
				}


				if (loadout.bespokeInOtherHand && !leftHandFull)
				{
					if (selected.BespokeAttachments.Count > 0)
					{
						GivePlayerItemLeftHand(selected.BespokeAttachments.GetRandom().ItemID);
						leftHandFull = true;
					}
				}
			}


			if (loadout.leftHandTable != null && !leftHandFull)
			{
				FVRObject selected = loadout.leftHandTable.GetItem();
				GivePlayerItemLeftHand(selected.ItemID);

				FVRObject compatibleAmmoObject = null;
				if (selected.CompatibleMagazines.Count > 0)
				{
					//If we don't care about capacity, get a random mag
					if (loadout.leftHandTable.maxMagCapacity < 0 && loadout.leftHandTable.minMagCapacity < 0) compatibleAmmoObject = selected.CompatibleMagazines.GetRandom();

					//Otherwise, try to get a mag within our capacity
					else
					{
						List<FVRObject> mags = selected.CompatibleMagazines.Where(o =>
							(loadout.leftHandTable.maxMagCapacity < 0 || o.MaxCapacityRelated <= loadout.leftHandTable.maxMagCapacity) &&
							(loadout.leftHandTable.minMagCapacity < 0 || o.MinCapacityRelated >= loadout.leftHandTable.minMagCapacity)
							).ToList();

						if (mags.Count > 0) compatibleAmmoObject = mags.GetRandom();
						else compatibleAmmoObject = Sodalite.Api.FirearmAPI.GetSmallestMagazine(selected);
					}
				}

				else if (selected.CompatibleClips.Count > 0)
				{
					compatibleAmmoObject = selected.CompatibleClips.GetRandom();
				}

				else if (selected.CompatibleSpeedLoaders.Count > 0)
				{
					compatibleAmmoObject = selected.CompatibleSpeedLoaders.GetRandom();
				}

				else if (selected.CompatibleSingleRounds.Count > 0)
				{
					compatibleAmmoObject = selected.CompatibleSingleRounds.GetRandom();
				}

				if (compatibleAmmoObject != null)
				{
					if (loadout.magInOtherHand && !rightHandFull)
					{
						GivePlayerItemRightHand(compatibleAmmoObject.ItemID);
						rightHandFull = true;
					}

					GivePlayerItemQuickbelt(compatibleAmmoObject.ItemID);
				}

				if (loadout.bespokeInOtherHand && !rightHandFull)
				{
					if (selected.BespokeAttachments.Count > 0)
					{
						GivePlayerItemRightHand(selected.BespokeAttachments.GetRandom().ItemID);
						rightHandFull = true;
					}
				}
			}
		}


		public static void EquipQuickbelt(PlayerLoadout loadout)
		{
			foreach (LoadoutPool table in loadout.quickbeltTables)
			{
				if (table == null) continue;

				for (int i = 0; i < table.itemsToSpawn; i++)
				{
					FVRObject selected = table.GetItem();

					GivePlayerItemQuickbelt(selected.ItemID);

					if (selected.RequiredSecondaryPieces.Count > 0)
					{
						foreach (FVRObject secondary in selected.RequiredSecondaryPieces)
						{
							GivePlayerItemQuickbelt(secondary.ItemID);
						}
					}

					FVRObject compatibleAmmoObject = null;
					if (selected.CompatibleMagazines.Count > 0)
					{
						compatibleAmmoObject = selected.CompatibleMagazines.GetRandom();
					}

					else if (selected.CompatibleClips.Count > 0)
					{
						compatibleAmmoObject = selected.CompatibleClips.GetRandom();
					}

					else if (selected.CompatibleSpeedLoaders.Count > 0)
					{
						compatibleAmmoObject = selected.CompatibleSpeedLoaders.GetRandom();
					}

					else if (selected.CompatibleSingleRounds.Count > 0)
					{
						compatibleAmmoObject = selected.CompatibleSingleRounds.GetRandom();
					}

					if (compatibleAmmoObject != null)
					{
						GivePlayerItemQuickbelt(compatibleAmmoObject.ItemID);
					}
				}
			}
		}



		public static FVRPhysicalObject GivePlayerItemQuickbelt(string itemID)
		{
			if (!IM.OD.ContainsKey(itemID)) return null;

			Transform playerHead = GM.CurrentPlayerBody.Head;

			GameObject spawned = UnityEngine.Object.Instantiate(IM.OD[itemID].GetGameObject(), playerHead.transform.position + playerHead.transform.forward / 2, playerHead.transform.rotation);

			if (spawned.gameObject.GetComponent<GrappleGun>() != null)
			{
				Debug.Log("Grapple spawned!");
			}

			FVRPhysicalObject[] items = spawned.GetComponents<FVRPhysicalObject>();
			FVRPhysicalObject item = items.FirstOrDefault(o => o.enabled);

			if (items.Length > 1)
			{
				Debug.Log("hey we have two components!");
			}

			foreach (FVRQuickBeltSlot slot in GM.CurrentPlayerBody.QBSlots_Internal.OrderBy(o => o.SizeLimit))
			{
				if (slot.CurObject == null && slot.Type == item.QBSlotType && slot.SizeLimit >= item.Size)
				{
					item.SetQuickBeltSlot(slot);
					break;
				}
			}

			if (spawned.gameObject.GetComponent<GrappleGun>() != null)
			{
				Debug.Log("Is it in a quickbelt? " + (item.m_quickbeltSlot != null).ToString());
			}

			return item;
		}



		public static FVRPhysicalObject GivePlayerItemLeftHand(string itemID)
		{
			if (!IM.OD.ContainsKey(itemID)) return null;

			FVRViveHand hand = GM.CurrentPlayerBody.LeftHand.GetComponent<FVRViveHand>();

			if (hand.CurrentInteractable != null)
			{
				Debug.LogError("Player was holding stuff!");
				return null;
			}

			FVRPhysicalObject item = UnityEngine.Object.Instantiate(IM.OD[itemID].GetGameObject(), hand.transform.position, hand.transform.rotation).GetComponent<FVRPhysicalObject>();

			hand.RetrieveObject(item);

			return item;
		}


		public static FVRPhysicalObject GivePlayerItemRightHand(string itemID)
		{
			if (!IM.OD.ContainsKey(itemID)) return null;

			FVRViveHand hand = GM.CurrentPlayerBody.RightHand.GetComponent<FVRViveHand>();

			if (hand.CurrentInteractable != null)
			{
				Debug.LogError("Player was holding stuff!");
				return null;
			}

			FVRPhysicalObject item = UnityEngine.Object.Instantiate(IM.OD[itemID].GetGameObject(), hand.transform.position, hand.transform.rotation).GetComponent<FVRPhysicalObject>();

			hand.RetrieveObject(item);

			return item;
		}

	}
}


