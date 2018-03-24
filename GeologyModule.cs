﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeologyModule : MonoBehaviour {
	public readonly float metalK_abundance, metalM_abundance, metalE_abundance, 
	metalN_abundance, metalP_abundance, metalS_abundance,
	mineralF_abundance, mineralL_abundance; // sum must be less than one!

	public GeologyModule() {
		metalK_abundance = 0.01f;
		metalM_abundance = 0.005f; metalE_abundance = 0.003f;
		metalN_abundance = 0.0001f; metalP_abundance = 0.02f; metalS_abundance = 0.0045f;
		mineralF_abundance = 0.02f; mineralL_abundance = 0.02f; // sum must be less than one!
	}

	public void SpreadMinerals(SurfaceBlock surface) {
		int maxObjectsCount = SurfaceBlock.INNER_RESOLUTION * SurfaceBlock.INNER_RESOLUTION / 2;
		GameObject boulderPref = Resources.Load<GameObject>("Structures/Boulder");
		List<Structure> allBoulders = new List<Structure>();

		int bouldersCount = 0;
		if (Random.value < metalK_abundance * 10) {
			bouldersCount = (int)(maxObjectsCount * GameMaster.geologyModule.metalK_abundance * Random.value);
			if (bouldersCount != 0) {						
				for (int i = 0; i< bouldersCount; i++) {
					GameObject g = Instantiate(boulderPref);
					g.transform.GetChild(0).GetComponent<MeshRenderer>().material = PoolMaster.metalK_material;
					HarvestableResource hr = g.GetComponent<HarvestableResource>();
					hr.SetResources(ResourceType.metal_K, 1+ Random.value * 100 * metalK_abundance);
					allBoulders.Add(hr);
				}
			}
		}
		if (Random.value < metalM_abundance * 10) {
			bouldersCount = (int)(maxObjectsCount * GameMaster.geologyModule.metalM_abundance * Random.value);
			if (bouldersCount != 0) {						
				for (int i = 0; i< bouldersCount; i++) {
					GameObject g = Instantiate(boulderPref);
					g.transform.GetChild(0).GetComponent<MeshRenderer>().material = PoolMaster.metalM_material;
					HarvestableResource hr = g.GetComponent<HarvestableResource>();
					hr.SetResources(ResourceType.metal_M, 1+ Random.value * 100 * metalM_abundance);
					allBoulders.Add(hr);
				}
			}
		}
		if (Random.value < metalE_abundance * 10) {
			bouldersCount = (int)(maxObjectsCount * GameMaster.geologyModule.metalE_abundance * Random.value);
			if (bouldersCount != 0) {						
				for (int i = 0; i< bouldersCount; i++) {
					GameObject g = Instantiate(boulderPref);
					g.transform.GetChild(0).GetComponent<MeshRenderer>().material = PoolMaster.metalE_material;
					HarvestableResource hr = g.GetComponent<HarvestableResource>();
					hr.SetResources(ResourceType.metal_E, 1+ Random.value * 100 * metalE_abundance);
					allBoulders.Add(hr);
				}
			}
		}
		if (Random.value < metalN_abundance * 10) {
			bouldersCount = (int)(maxObjectsCount * GameMaster.geologyModule.metalN_abundance * Random.value);
			if (bouldersCount != 0) {						
				for (int i = 0; i< bouldersCount; i++) {
					GameObject g = Instantiate(boulderPref);
					g.transform.GetChild(0).GetComponent<MeshRenderer>().material = PoolMaster.metalN_material;
					HarvestableResource hr = g.GetComponent<HarvestableResource>();
					hr.SetResources(ResourceType.metal_N, 1+ Random.value * 100 * metalN_abundance);
					allBoulders.Add(hr);
				}
			}
		}
		if (Random.value < metalP_abundance * 10) {
			bouldersCount = (int)(maxObjectsCount * GameMaster.geologyModule.metalP_abundance * Random.value);
			if (bouldersCount != 0) {						
				for (int i = 0; i< bouldersCount; i++) {
					GameObject g = Instantiate(boulderPref);
					g.transform.GetChild(0).GetComponent<MeshRenderer>().material = PoolMaster.metalP_material;
					HarvestableResource hr = g.GetComponent<HarvestableResource>();
					hr.SetResources(ResourceType.metal_P, 1+ Random.value * 100 * metalP_abundance);
					allBoulders.Add(hr);
				}
			}
		}
		if (Random.value < metalS_abundance * 10) {
			bouldersCount = (int)(maxObjectsCount * GameMaster.geologyModule.metalS_abundance * Random.value);
			if (bouldersCount != 0) {						
				for (int i = 0; i< bouldersCount; i++) {
					GameObject g = Instantiate(boulderPref);
					g.transform.GetChild(0).GetComponent<MeshRenderer>().material = PoolMaster.metalS_material;
					HarvestableResource hr = g.GetComponent<HarvestableResource>();
					hr.SetResources(ResourceType.metal_S, 1+ Random.value * 100 * metalS_abundance);
					allBoulders.Add(hr);
				}
			}
		}
		boulderPref = Resources.Load<GameObject>("Structures/Pile");
		if (Random.value < mineralF_abundance * 4) {
			bouldersCount = (int)(maxObjectsCount * GameMaster.geologyModule.mineralF_abundance * Random.value);
			if (bouldersCount != 0) {						
				for (int i = 0; i< bouldersCount; i++) {
					GameObject g = Instantiate(boulderPref);
					g.transform.GetChild(0).GetComponent<MeshRenderer>().material = PoolMaster.mineralF_material;
					HarvestableResource hr = g.GetComponent<HarvestableResource>();
					hr.SetResources(ResourceType.metal_S, 1+ Random.value * 100 * mineralF_abundance);
					allBoulders.Add(hr);
				}
			}
		}
		if (Random.value < mineralL_abundance * 4) {
			bouldersCount = (int)(maxObjectsCount * GameMaster.geologyModule.mineralL_abundance * Random.value);
			if (bouldersCount != 0) {						
				for (int i = 0; i< bouldersCount; i++) {
					GameObject g = Instantiate(boulderPref);
					g.transform.GetChild(0).GetComponent<MeshRenderer>().material = PoolMaster.mineralL_material;
					HarvestableResource hr = g.GetComponent<HarvestableResource>();
					hr.SetResources(ResourceType.metal_S, 1+ Random.value * 100 * mineralL_abundance);
					allBoulders.Add(hr);
				}
			}
		}
		if (allBoulders.Count > 0) {
			surface.AddMultipleCellObjects(allBoulders);
		}
		maxObjectsCount = SurfaceBlock.INNER_RESOLUTION * SurfaceBlock.INNER_RESOLUTION - surface.surfaceObjects.Count;
		if (maxObjectsCount > 0) {
			int count = 0;
			if (surface.material_id == PoolMaster.STONE_ID) count = (int)(maxObjectsCount * (0.05f + Random.value * 0.05f)); else count = (int)(Random.value * 0.08f);
			List<PixelPosByte> points = surface.GetRandomPositions(2,2,count);
			if (points.Count > 0) {
				foreach (PixelPosByte p in points) {
					GameObject g = Instantiate(boulderPref);
					g.transform.GetChild(0).GetComponent<MeshRenderer>().material = PoolMaster.stone_material;
					g.transform.localScale = Vector3.one * (1.2f + Random.value * 0.6f);
					HarvestableResource hr = g.GetComponent<HarvestableResource>();
					hr.SetResources(ResourceType.Stone, 4 + Random.value * 10);
					hr.SetBasement(surface, p);
					hr.transform.localRotation = Quaternion.Euler(0, Random.value * 360,0);
				}
			}
		}
	}

	public void CalculateOutput(float production, CubeBlock workObject, Storage storage) {
		if (workObject.naturalFossils > 0) {
			float v = Random.value - GameMaster.LUCK_COEFFICIENT; 
			float m = 0;
			switch (workObject.material_id) {
			case PoolMaster.STONE_ID :
				if (metalK_abundance >= v) {
					m= metalK_abundance * production * (Random.value + 1 + GameMaster.LUCK_COEFFICIENT);
					storage.AddResources(ResourceType.metal_K, m); production -= m;
				}
				if (metalM_abundance >= v) {
					m= metalM_abundance * production * (Random.value +1 + GameMaster.LUCK_COEFFICIENT);
					storage.AddResources(ResourceType.metal_M, m); production -= m;
				}
				if (metalE_abundance >= v) {
					m= metalE_abundance * production * (Random.value + 1 + GameMaster.LUCK_COEFFICIENT);
					storage.AddResources(ResourceType.metal_E, m); production -= m;
				}
				if (metalN_abundance >= v) {
					m= metalN_abundance * production * (Random.value + 1 + GameMaster.LUCK_COEFFICIENT);
					storage.AddResources(ResourceType.metal_N, m); production -= m;
				}
				if (metalP_abundance >= v) {
					m= metalP_abundance * production * (Random.value + 1 + GameMaster.LUCK_COEFFICIENT);
					storage.AddResources(ResourceType.metal_P, m); production -= m;
				}
				if (metalS_abundance >= v) {
					m= metalS_abundance * production * (Random.value + 1 + GameMaster.LUCK_COEFFICIENT);
					storage.AddResources(ResourceType.metal_S, m); production -= m;
				}
				if (mineralF_abundance >= v) {
					m= mineralF_abundance * production * (Random.value + 1 + GameMaster.LUCK_COEFFICIENT);
					storage.AddResources(ResourceType.mineral_F, m); production -= m;
				}
				if (mineralL_abundance >= v) {
					m= mineralL_abundance * production * (Random.value + 1 + GameMaster.LUCK_COEFFICIENT);
					storage.AddResources(ResourceType.mineral_L, m); production -= m;
				}
				if (production > 0) {
					GameMaster.colonyController.storage.AddResources(ResourceType.Stone, production); 
				}
				break;
			case PoolMaster.DIRT_ID:
				if (metalK_abundance >= v) {
					m= metalK_abundance/2f * production * (Random.value + 1 + GameMaster.LUCK_COEFFICIENT);
					GameMaster.colonyController.storage.AddResources(ResourceType.metal_K, m); production -= m;
				}
				if (metalP_abundance >= v) {
					m= metalP_abundance/2f * production * (Random.value + 1 + GameMaster.LUCK_COEFFICIENT);
					GameMaster.colonyController.storage.AddResources(ResourceType.metal_P, m); production -= m;
				}
				if (mineralL_abundance >= v) {
					m= mineralL_abundance/2f * production * (Random.value + 1 + GameMaster.LUCK_COEFFICIENT);
					GameMaster.colonyController.storage.AddResources(ResourceType.mineral_L, m); production -= m;
				}
				if (production > 0) {
					GameMaster.colonyController.storage.AddResources(ResourceType.Dirt, production); 
				}
				break;
			default:
				GameMaster.colonyController.storage.AddResources(ResourceType.GetResourceTypeByMaterialId(workObject.material_id), production); 
				break;
			}
			workObject.naturalFossils -= production;
		}
		else { // no fossils
			GameMaster.colonyController.storage.AddResources(ResourceType.GetResourceTypeByMaterialId(workObject.material_id), production); 
		}
	}
}