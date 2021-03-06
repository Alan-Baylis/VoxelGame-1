﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildingObserver : UIObserver {
	Building observingBuilding;
    bool status_connectedToPowerGrid = false, status_energySupplied = false, isHouse = false, canBeUpgraded = false;
	float showingEnergySurplus = 0;
	int showingHousing = 0;
    byte savedLevel = 0;
    Vector2[] savedResourcesValues;
#pragma warning disable 0649
    [SerializeField]RawImage energyImage, housingImage; //fiti
    [SerializeField] Text energyValue, housingValue, upgradeButtonText; // fiti
    [SerializeField] Button upgradeButton; // fiti
    [SerializeField] GameObject upgradeInfoPanel, chargeButton; // fiti
    [SerializeField] GameObject[] resourceCostIndicator; // fiti
    [SerializeField] Button energyButton; // fiti
#pragma warning restore 0649

    private void Awake()
    {
        savedResourcesValues = new Vector2[resourceCostIndicator.Length];
    }

    public static UIBuildingObserver InitializeBuildingObserverScript()
    {
        UIBuildingObserver ub = Instantiate(Resources.Load<GameObject>("UIPrefs/buildingObserver"), UIController.current.rightPanel.transform).GetComponent<UIBuildingObserver>();
        Building.buildingObserver = ub;
        return ub;
    }

    public void SetObservingBuilding(Building b) {
		if (b == null) {
			SelfShutOff();
			return;
		}
		UIStructureObserver us = Structure.structureObserver;
        if (us == null) us = UIStructureObserver.InitializeStructureObserverScript();
        else us.gameObject.SetActive(true);
		observingBuilding = b; isObserving = true;
		us.SetObservingStructure(observingBuilding);
		status_connectedToPowerGrid = b.connectedToPowerGrid;
		if (status_connectedToPowerGrid) {
            if (b.id == Structure.ENERGY_CAPACITOR_1_ID | b.id == Structure.ENERGY_CAPACITOR_2_ID | b.id == Structure.ENERGY_CAPACITOR_3_ID)
            {
                chargeButton.SetActive(true);
                energyValue.enabled = false;
            }
            else { 
            showingEnergySurplus = b.energySurplus;
                status_energySupplied = b.energySupplied;
                if (status_energySupplied)
                {
                    energyButton.GetComponent<RawImage>().uvRect = UIController.GetTextureUV(observingBuilding.isActive ? Icons.PowerOn : Icons.PowerOff);
                    if (showingEnergySurplus <= 0) energyValue.text = string.Format("{0,1:F}", showingEnergySurplus);
                    else energyValue.text = '+' + string.Format("{0,1:F}", showingEnergySurplus);
                }
                else
                {
                    energyValue.text = Localization.GetWord(LocalizedWord.Offline);
                    energyButton.GetComponent<RawImage>().uvRect = UIController.GetTextureUV(Icons.PowerOff);
                }
                energyValue.enabled = true;
                chargeButton.SetActive(false);
            }
			energyImage.enabled = true;
		}
		else {
			energyValue.enabled = false;
			energyImage.enabled = false;
		}
        energyButton.interactable = observingBuilding.canBePowerSwitched;

		if (b is House) {
			isHouse = true;
			showingHousing = (b as House).housing;
			housingValue.text = showingHousing.ToString();
			housingValue.enabled = true;
			housingImage.enabled = true;
		}
		else {
			isHouse = false;
			housingValue.enabled = false;
			housingImage.enabled = false;
		}

        CheckUpgradeAvailability();       
		STATUS_UPDATE_TIME = 1.2f; timer = STATUS_UPDATE_TIME;
	}

	override protected void StatusUpdate() {
		if ( !isObserving ) return;
        if (observingBuilding == null)
        {
            SelfShutOff();
            return;
        }
        else
        {
            if (status_connectedToPowerGrid != observingBuilding.connectedToPowerGrid)
            {
                status_connectedToPowerGrid = observingBuilding.connectedToPowerGrid;
                if (status_connectedToPowerGrid)
                {
                    showingEnergySurplus = observingBuilding.energySurplus;
                    status_energySupplied = observingBuilding.energySupplied;
                    if (status_energySupplied)
                    {
                        if (showingEnergySurplus <= 0) energyValue.text = string.Format("{0,1:F}", showingEnergySurplus);
                        else energyValue.text = '+' + string.Format("{0,1:F}", showingEnergySurplus);
                        energyButton.GetComponent<RawImage>().uvRect = UIController.GetTextureUV(observingBuilding.isActive ? Icons.PowerOn : Icons.PowerOff);
                    }
                    else
                    {
                        energyValue.text = Localization.GetWord(LocalizedWord.Offline);
                        energyButton.GetComponent<RawImage>().uvRect = UIController.GetTextureUV(Icons.PowerOff);
                    }
                    energyValue.enabled = true;
                    energyImage.enabled = true;
                }
                else
                {
                    energyValue.enabled = false;
                    energyImage.enabled = false;
                }
            }
            else
            {
                    if (status_energySupplied != observingBuilding.energySupplied)
                    {
                        status_energySupplied = observingBuilding.energySupplied;
                        if (status_energySupplied)
                        {
                            showingEnergySurplus = observingBuilding.energySurplus;
                            if (showingEnergySurplus <= 0) energyValue.text = string.Format("{0,1:F}", showingEnergySurplus);
                            else energyValue.text = '+' + string.Format("{0,1:F}", showingEnergySurplus);
                            energyButton.GetComponent<RawImage>().uvRect = UIController.GetTextureUV(observingBuilding.isActive ? Icons.PowerOn : Icons.PowerOff);
                        }
                        else
                        {
                            energyValue.text = Localization.GetWord(LocalizedWord.Offline);
                            energyButton.GetComponent<RawImage>().uvRect = UIController.GetTextureUV(Icons.PowerOff);
                        }
                    }
                    else
                    {
                        if (showingEnergySurplus != observingBuilding.energySurplus)
                        {
                            showingEnergySurplus = observingBuilding.energySurplus;
                            if (showingEnergySurplus <= 0) energyValue.text = string.Format("{0,1:F}", showingEnergySurplus);
                            else energyValue.text = '+' + string.Format("{0,1:F}", showingEnergySurplus);
                            energyButton.GetComponent<RawImage>().uvRect = UIController.GetTextureUV(observingBuilding.isActive ? Icons.PowerOn : Icons.PowerOff);
                        }
                    }
            }
            if (isHouse)
            {
                int h = (observingBuilding as House).housing;
                if (showingHousing != h)
                {
                    showingHousing = h;
                    housingValue.text = showingHousing.ToString();
                }
            }
            if (canBeUpgraded) {
                string s = upgradeButtonText.text;
                string answer = string.Empty;
                upgradeButton.interactable = observingBuilding.IsLevelUpPossible(ref answer);
                if (answer != s) {
                    if (answer == string.Empty)
                    {
                        upgradeButtonText.text = Localization.GetWord(LocalizedWord.Upgrade);
                        upgradeButtonText.color = Color.white;
                    }
                    else {
                        upgradeButtonText.text = answer;
                        upgradeButtonText.color = Color.yellow;
                    }
                }
            }
            if (savedLevel != observingBuilding.level) {
                Structure.structureObserver.CheckName();
                CheckUpgradeAvailability();
                if (resourceCostIndicator[0].activeSelf) {
                    RefreshResourcesData();
                }
                savedLevel = observingBuilding.level;
            }
            else
            {
                if (resourceCostIndicator[0].activeSelf) {
                    float[] storageVal = GameMaster.colonyController.storage.standartResources;
                    for (int i = 0; i < resourceCostIndicator.Length; i++) {
                        GameObject g = resourceCostIndicator[i];
                        if (g.activeSelf) {
                            Text t = g.transform.GetChild(0).GetComponent<Text>();
                            t.color = savedResourcesValues[i].y > storageVal[(int)savedResourcesValues[i].x] ? Color.red : Color.white;
                        }
                    }
                }
            }
        }
	}

    virtual public void UpgradeInfoPanelSwitch() {
        if (observingBuilding == null) {
            SelfShutOff();
            return;
        }
        if (upgradeInfoPanel.activeSelf)
        {
            upgradeInfoPanel.SetActive(false);
        }
        else {
            if (observingBuilding.upgradedIndex == -1) return;
            upgradeInfoPanel.SetActive(true);
            RefreshResourcesData();
        }
    }

    void RefreshResourcesData() {
        ResourceContainer[] cost = observingBuilding.GetUpgradeCost();
        if (cost != null && cost.Length != 0)
        {
            float[] storageVolume = GameMaster.colonyController.storage.standartResources;
            for (int i = 0; i < resourceCostIndicator.Length; i++)
            {
                if (i < cost.Length)
                {
                    resourceCostIndicator[i].GetComponent<RawImage>().uvRect = ResourceType.GetTextureRect(cost[i].type.ID);
                    Text t = resourceCostIndicator[i].transform.GetChild(0).GetComponent<Text>();
                    t.text = Localization.GetResourceName(cost[i].type.ID) + " : " + string.Format("{0:0.##}",cost[i].volume);
                    t.color = cost[i].volume > storageVolume[cost[i].type.ID] ? Color.red : Color.white;
                    savedResourcesValues[i] = new Vector2(cost[i].type.ID,cost[i].volume);
                    resourceCostIndicator[i].SetActive(true);
                }
                else resourceCostIndicator[i].SetActive(false);
            }
        }
    }
    void CheckUpgradeAvailability() {
        if (observingBuilding.upgradedIndex != -1)
        {
            canBeUpgraded = true;
            upgradeButton.gameObject.SetActive(true);
            string s = upgradeButtonText.text;
            string answer = string.Empty;
            bool upgradePossibleNow = observingBuilding.IsLevelUpPossible(ref answer);
            upgradeButton.interactable = upgradePossibleNow;
            if (answer != s)
            {
                if (answer == string.Empty)
                {
                    upgradeButtonText.text = Localization.GetWord(LocalizedWord.Upgrade);
                    upgradeButtonText.color = Color.white;
                }
                else
                {
                    upgradeButtonText.text = answer;
                    upgradeButtonText.color = Color.yellow;
                }
            }
            if (savedLevel != observingBuilding.level)
            {
                Structure.structureObserver.CheckName();
                if (resourceCostIndicator[0].activeSelf)
                {
                    RefreshResourcesData();
                }
                savedLevel = observingBuilding.level;
            }
        }
        else
        {
            canBeUpgraded = false;
            upgradeButton.gameObject.SetActive(false);
            upgradeInfoPanel.SetActive(false);
        }
        upgradeInfoPanel.SetActive(false);
    }

    public void Upgrade() {
        if (observingBuilding == null)
        {
            SelfShutOff();
            return;
        }
        else {
            observingBuilding.LevelUp(true);
            if (observingBuilding.upgradedIndex < 0)
            {
                CheckUpgradeAvailability();
            }
        }
    }

    public void PowerToggle() {
        if (observingBuilding == null) {
            SelfShutOff();
            return;
        }
        else
        {
            if ( !observingBuilding.canBePowerSwitched ) return;
            if (!observingBuilding.isActive)
            {
                energyButton.GetComponent<RawImage>().uvRect = UIController.GetTextureUV(Icons.PowerOn);
                observingBuilding.SetActivationStatus(true);                
            }
            else
            {
                energyButton.GetComponent<RawImage>().uvRect = UIController.GetTextureUV(Icons.PowerOff);
                observingBuilding.SetActivationStatus(false);
            }
            StatusUpdate();
            timer = STATUS_UPDATE_TIME / 2f;
        }
    }
    public void Charge()
    {
        ColonyController colony = GameMaster.colonyController;
        if (colony.energyCrystalsCount >= 1)
        {
            if (colony.energyStored != colony.totalEnergyCapacity)
            {
                colony.GetEnergyCrystals(1);
                colony.AddEnergy(GameMaster.ENERGY_IN_CRYSTAL);
            }
        }
        else UIController.current.MakeAnnouncement(Localization.GetAnnouncementString(GameAnnouncements.NotEnoughEnergyCrystals));
    }

	override public void SelfShutOff() {
		isObserving = false;
		Structure.structureObserver.SelfShutOff();
		gameObject.SetActive(false);
	}

	override public void ShutOff() {
		isObserving = false;
		observingBuilding = null;
		Structure.structureObserver.ShutOff();
		gameObject.SetActive(false);
	}
}
