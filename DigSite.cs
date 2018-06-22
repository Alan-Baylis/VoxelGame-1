using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class DigSiteSerializer {
	public WorksiteSerializer worksiteSerializer;
	public int resourceTypeID;
}

public class DigSite : Worksite {
	public bool dig = true;
	ResourceType mainResource;
	CubeBlock workObject;
	const int START_WORKERS_COUNT = 10;


	void Update () {
		if (GameMaster.gameSpeed == 0) return;
		if (workObject ==null || (workObject.volume == CubeBlock.MAX_VOLUME && dig == false) || (workObject.volume ==0 && dig == true)) {
			Destroy(this);
			return;
		}
		if (workersCount > 0) {
			workflow += workSpeed * Time.deltaTime * GameMaster.gameSpeed;
			labourTimer -= Time.deltaTime * GameMaster.gameSpeed;
			if ( labourTimer <= 0 ) {
				if (workflow >= 1) LabourResult();
				labourTimer = GameMaster.LABOUR_TICK;
			}
		}
	}

	void LabourResult() {
			int x = (int) workflow;
			float production = x;
			if (dig) {
				production = workObject.Dig(x, true);
				GameMaster.geologyModule.CalculateOutput(production, workObject, GameMaster.colonyController.storage);
			}
			else {
				production = GameMaster.colonyController.storage.GetResources(mainResource, production);
				if (production != 0) {
					production = workObject.PourIn((int)production);
					if (production == 0) {Destroy(this);return;}
				}
			}
			workflow -= production;	
		actionLabel = Localization.ui_dig_in_progress + " ("+((int) (((float)workObject.volume / (float)CubeBlock.MAX_VOLUME) * 100)).ToString()+"%)";
	}

	protected override void RecalculateWorkspeed() {
		workSpeed = GameMaster.CalculateWorkspeed(workersCount,WorkType.Digging);
	}

	public void Set(CubeBlock block, bool work_is_dig) {
		workObject = block;
		dig = work_is_dig;
		if (dig) {
			Block b = GameMaster.mainChunk.GetBlock(block.pos.x, block.pos.y, block.pos.z);
			if (b != null && b.type == BlockType.Surface) {
				CleanSite cs = b.gameObject.AddComponent<CleanSite>();
				cs.Set(b.gameObject.GetComponent<SurfaceBlock>(), true);
				Destroy(this);
				return;
			}
			sign = Instantiate(Resources.Load<GameObject> ("Prefs/DigSign")).GetComponent<WorksiteSign>(); 
		}
		else 	sign = Instantiate(Resources.Load<GameObject>("Prefs/PourInSign")).GetComponent<WorksiteSign>();
		sign.worksite = this;
		sign.transform.position = workObject.transform.position + Vector3.up * Block.QUAD_SIZE;
		mainResource = ResourceType.GetResourceTypeById(workObject.material_id);
		GameMaster.colonyController.SendWorkers(START_WORKERS_COUNT, this, WorkersDestination.ForWorksite);
		GameMaster.colonyController.AddWorksite(this);
	}

	//---------SAVE   SYSTEM----------------
	override public WorksiteBasisSerializer Save() {
		if (workObject == null) {
			Destroy(this);
			return null;
		}
		WorksiteBasisSerializer wbs = new WorksiteBasisSerializer();
		wbs.type = WorksiteType.DigSite;
		wbs.bool1 = dig;
		wbs.workObjectPos = workObject.pos;
		using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
		{
			new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(stream, GetDigSiteSerializer());
			wbs.data = stream.ToArray();
		}
		return wbs;
	}

	DigSiteSerializer GetDigSiteSerializer() {
		DigSiteSerializer dss = new DigSiteSerializer();
		dss.worksiteSerializer = GetWorksiteSerializer();
		dss.resourceTypeID = mainResource.ID;
		return dss;
	}
	// --------------------------------------------------------
}
