using FistVR;
using Sodalite.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamemodes
{
	public class AmmoSupply : MonoBehaviour
	{
		public void SupplyPlayerWithAmmo()
        {
            if (CanPlayerBeSupplied())
            {
				FVRViveHand emptyHand = GM.CurrentMovementManager.Hands.FirstOrDefault(o => o.CurrentInteractable == null);
				FVRPhysicalObject firearm = GM.CurrentMovementManager.Hands.FirstOrDefault(o => o.CurrentInteractable != null).CurrentInteractable as FVRPhysicalObject;

				FVRPhysicalObject ammoObject = SpawnAmmoObjectForGun(firearm, emptyHand.transform.position, emptyHand.transform.rotation);
				if (ammoObject == null) return;

				emptyHand.RetrieveObject(ammoObject);
			}
        }

		private bool CanPlayerBeSupplied()
        {
			return HasEmptyHand() && HasGunInHand();
        }

		private bool HasEmptyHand()
        {
			return GM.CurrentMovementManager.Hands.Any(o => o.CurrentInteractable == null);
		}

		private bool HasGunInHand()
        {
			return GM.CurrentMovementManager.Hands.Any(o => o.CurrentInteractable is FVRFireArm);
		}

		private FVRPhysicalObject SpawnAmmoObjectForGun(FVRPhysicalObject firearm, Vector3 spawnPosition, Quaternion spawnRotation)
        {
			FVRObject ammoObject = GetAmmoObject(firearm.ObjectWrapper);
			if (ammoObject == null) return null;
			return Instantiate(ammoObject.GetGameObject(), spawnPosition, spawnRotation).GetComponent<FVRPhysicalObject>();
        }

		private FVRObject GetAmmoObject(FVRObject fvrObject)
        {
			if(fvrObject.CompatibleMagazines.Count > 0)
            {
				return fvrObject.CompatibleMagazines.GetRandom();
			}

			else if(fvrObject.CompatibleClips.Count > 0)
            {
				return fvrObject.CompatibleClips.GetRandom();
			}

			else if(fvrObject.CompatibleSpeedLoaders.Count > 0)
            {
				return fvrObject.CompatibleSpeedLoaders.GetRandom();
            }

			else if(fvrObject.CompatibleSingleRounds.Count > 0)
            {
				return fvrObject.CompatibleSingleRounds.GetRandom();
            }

			return null;
        }
		
	}
}

