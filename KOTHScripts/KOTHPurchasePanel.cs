using FistVR;
using Gamemodes;
using KOTH;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KOTH
{
    public class KOTHPurchasePanel : MonoBehaviour
    {
        public Transform spawnPos;
        public Text pointText;
        public Supplies suppliesPrefab;
        public ButtonList buttonList;

        public int maxShields = 3;
        public int costSupplies;
        public int costHealth;
        public int costShield;
        public int costRadio;
        public int costTurret;

        [HideInInspector]
        public int points;

        [HideInInspector]
        public KOTHHill hill;

        private List<Supplies> spawnedSupplies = new List<Supplies>();
        private List<GameObject> spawnedShields = new List<GameObject>();

        public void Start()
        {
            ResetButtons();
        }


        public void ResetButtons()
        {
            buttonList.ClearButtons();

            buttonList.AddButton("[" + costSupplies + "] Spawn Supplies", SpawnSupplies, false);
            buttonList.AddButton("[" + costHealth + "] Spawn Health", SpawnHealth, false);
            buttonList.AddButton("[" + costShield + "] Spawn Shield", SpawnShield, false);
            buttonList.AddButton("[" + costRadio + "] Spawn Radio", SpawnRadio, false);
            buttonList.AddButton("[" + costTurret + "] Spawn Turret", SpawnTurret, false);
        }


        public void AddPoints(int i)
        {
            points += i;
            UpdateText();
        }

        public void SetPoints(int i)
        {
            points = i;
            UpdateText();
        }


        public void SpawnSupplies()
        {
            if (points >= costSupplies)
            {
                Supplies supplies = Instantiate(suppliesPrefab, spawnPos.position, spawnPos.rotation);
                supplies.ShowBuildableAreas += hill.ShowBuildableAreas;
                supplies.HideBuildableAreas += hill.HideBuildableAreas;
                spawnedSupplies.Add(supplies);

                AddPoints(-costSupplies);
            }
        }


        public void SpawnHealth()
        {
            if (points >= costHealth)
            {
                Instantiate(IM.OD["PowerUpMeat_Health"].GetGameObject(), spawnPos.position, spawnPos.rotation);
                AddPoints(-costHealth);
            }
        }


        public void SpawnShield()
        {
            if (points >= costShield && spawnedShields.Count < maxShields)
            {
                spawnedShields.Add(Instantiate(IM.OD["deployableshield.tt"].GetGameObject(), spawnPos.position, spawnPos.rotation));
                AddPoints(-costShield);

                if (spawnedShields.Count >= maxShields)
                {
                    buttonList.buttons[2].text.text = "Out Of Stock";
                }
            }
        }

        public void SpawnRadio()
        {
            if (points >= costRadio)
            {
                Instantiate(IM.OD["Dev_Radio"].GetGameObject(), spawnPos.position, spawnPos.rotation);
                AddPoints(-costRadio);
            }
        }


        public void SpawnTurret()
        {
            if (points >= costTurret)
            {
                GameObject turret = Instantiate(IM.OD["Turburgert_Flamethrower"].GetGameObject(), spawnPos.position, spawnPos.rotation);
                AutoMeater turretComp = turret.GetComponent<AutoMeater>();
                turretComp.E.IFFCode = hill.currentTeam;
                hill.turretList.Add(turretComp);

                AddPoints(-costTurret);
            }
        }


        public void UpdateText()
        {
            pointText.text = "Points : " + points;
        }


        public void ClearSpawnedItems()
        {
            ResetButtons();
            ClearSupplies();
            ClearShields();
        }

        public void ClearSupplies()
        {
            foreach (Supplies supplies in spawnedSupplies)
            {
                if (supplies != null)
                {
                    if (supplies.IsHeld)
                    {
                        supplies.ForceBreakInteraction();
                    }

                    else if (supplies.QuickbeltSlot != null)
                    {
                        supplies.SetQuickBeltSlot(null);
                    }

                    Destroy(supplies.gameObject);
                }
            }

            spawnedSupplies.Clear();
        }

        public void ClearShields()
        {
            foreach (GameObject shield in spawnedShields)
            {
                if (shield != null)
                {
                    FVRPhysicalObject physComp = shield.GetComponent<FVRPhysicalObject>();

                    if (physComp.IsHeld)
                    {
                        physComp.ForceBreakInteraction();
                    }

                    else if (physComp.QuickbeltSlot != null)
                    {
                        physComp.SetQuickBeltSlot(null);
                    }

                    Destroy(shield);
                }
            }

            spawnedShields.Clear();
        }

    }
}


