using FistVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gamemodes
{
	[CreateAssetMenu(fileName = "New Atlas Config", menuName = "MeatKit/Gamemodes/AtlasConfigTemplate", order = 0)]
	public class AtlasSosigConfigTemplate : ScriptableObject
	{
		public float ViewDistance;
		public float HearingDistance;
		public float MaxFOV;
		public float SearchExtentsModifier;
		public bool DoesAggroOnFriendlyFire;
		public bool HasABrain;
		public bool DoesDropWeaponsOnBallistic;
		public bool CanPickupRanged;
		public bool CanPickupMelee;
		public bool CanPickupOther;
		public int TargetCapacity;
		public float TargetTrackingTime;
		public float NoFreshTargetTime;
		public float AssaultPointOverridesSkirmishPointWhenFurtherThan;
		public float RunSpeed;
		public float WalkSpeed;
		public float SneakSpeed;
		public float CrawlSpeed;
		public float TurnSpeed;
		public float MaxJointLimit;
		public float MovementRotMagnitude;
		public float TotalMustard;
		public float BleedDamageMult;
		public float BleedRateMultiplier;
		public float BleedVFXIntensity;
		public float DamMult_Projectile;
		public float DamMult_Explosive;
		public float DamMult_Melee;
		public float DamMult_Piercing;
		public float DamMult_Blunt;
		public float DamMult_Cutting;
		public float DamMult_Thermal;
		public float DamMult_Chilling;
		public float DamMult_EMP;
		public List<float> LinkDamageMultipliers;
		public List<float> LinkStaggerMultipliers;
		public List<Vector2> StartingLinkIntegrity;
		public List<float> StartingChanceBrokenJoint;
		public float ShudderThreshold;
		public float ConfusionThreshold;
		public float ConfusionMultiplier;
		public float ConfusionTimeMax;
		public float StunThreshold;
		public float StunMultiplier;
		public float StunTimeMax;
		public bool CanBeGrabbed;
		public bool CanBeSevered;
		public bool CanBeStabbed;
		public bool CanBeSurpressed;
		public float SuppressionMult;
		public bool DoesJointBreakKill_Head;
		public bool DoesJointBreakKill_Upper;
		public bool DoesJointBreakKill_Lower;
		public bool DoesSeverKill_Head;
		public bool DoesSeverKill_Upper;
		public bool DoesSeverKill_Lower;
		public bool DoesExplodeKill_Head;
		public bool DoesExplodeKill_Upper;
		public bool DoesExplodeKill_Lower;



		public SosigConfigTemplate GetConfigTemplate()
		{
			SosigConfigTemplate template = (SosigConfigTemplate)ScriptableObject.CreateInstance(typeof(SosigConfigTemplate));

			template.ViewDistance = ViewDistance;
			template.HearingDistance = HearingDistance;
			template.MaxFOV = MaxFOV;
			template.SearchExtentsModifier = SearchExtentsModifier;
			template.DoesAggroOnFriendlyFire = DoesAggroOnFriendlyFire;
			template.HasABrain = HasABrain;
			template.DoesDropWeaponsOnBallistic = DoesDropWeaponsOnBallistic;
			template.CanPickup_Ranged = CanPickupRanged;
			template.CanPickup_Melee = CanPickupMelee;
			template.CanPickup_Other = CanPickupOther;
			template.TargetCapacity = TargetCapacity;
			template.TargetTrackingTime = TargetTrackingTime;
			template.NoFreshTargetTime = NoFreshTargetTime;
			template.AssaultPointOverridesSkirmishPointWhenFurtherThan = AssaultPointOverridesSkirmishPointWhenFurtherThan;
			template.RunSpeed = RunSpeed;
			template.WalkSpeed = WalkSpeed;
			template.SneakSpeed = SneakSpeed;
			template.CrawlSpeed = CrawlSpeed;
			template.TurnSpeed = TurnSpeed;
			template.MaxJointLimit = MaxJointLimit;
			template.MovementRotMagnitude = MovementRotMagnitude;
			template.TotalMustard = TotalMustard;
			template.BleedDamageMult = BleedDamageMult;
			template.BleedRateMultiplier = BleedRateMultiplier;
			template.BleedVFXIntensity = BleedVFXIntensity;
			template.DamMult_Projectile = DamMult_Projectile;
			template.DamMult_Explosive = DamMult_Explosive;
			template.DamMult_Melee = DamMult_Melee;
			template.DamMult_Piercing = DamMult_Piercing;
			template.DamMult_Blunt = DamMult_Blunt;
			template.DamMult_Cutting = DamMult_Cutting;
			template.DamMult_Thermal = DamMult_Thermal;
			template.DamMult_Chilling = DamMult_Chilling;
			template.DamMult_EMP = DamMult_EMP;
			template.LinkDamageMultipliers = LinkDamageMultipliers;
			template.LinkStaggerMultipliers = LinkStaggerMultipliers;
			template.StartingLinkIntegrity = StartingLinkIntegrity;
			template.StartingChanceBrokenJoint = StartingChanceBrokenJoint;
			template.ShudderThreshold = ShudderThreshold;
			template.ConfusionThreshold = ConfusionThreshold;
			template.ConfusionMultiplier = ConfusionMultiplier;
			template.ConfusionTimeMax = ConfusionTimeMax;
			template.StunThreshold = StunThreshold;
			template.StunMultiplier = StunMultiplier;
			template.StunTimeMax = StunTimeMax;
			template.CanBeGrabbed = CanBeGrabbed;
			template.CanBeSevered = CanBeSevered;
			template.CanBeStabbed = CanBeStabbed;
			template.CanBeSurpressed = CanBeSurpressed;
			template.SuppressionMult = SuppressionMult;
			template.DoesJointBreakKill_Head = DoesJointBreakKill_Head;
			template.DoesJointBreakKill_Upper = DoesJointBreakKill_Upper;
			template.DoesJointBreakKill_Lower = DoesJointBreakKill_Lower;
			template.DoesSeverKill_Head = DoesSeverKill_Head;
			template.DoesSeverKill_Upper = DoesSeverKill_Upper;
			template.DoesSeverKill_Lower = DoesSeverKill_Lower;
			template.DoesExplodeKill_Head = DoesExplodeKill_Head;
			template.DoesExplodeKill_Upper = DoesExplodeKill_Upper;
			template.DoesExplodeKill_Lower = DoesExplodeKill_Lower;
			template.UsesLinkSpawns = false;
			template.LinkSpawns = new List<FVRObject>();
			template.LinkSpawnChance = new List<float>();
			template.OverrideSpeech = false;

			return template;
		}

	}
}


