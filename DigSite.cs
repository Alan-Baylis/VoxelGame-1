using UnityEngine;

public class DigSite : Worksite {
	public bool dig = true;
	ResourceType mainResource;
	CubeBlock workObject;
    const int START_WORKERS_COUNT = 10;
    new public const int MAX_WORKERS = 60;


	override public void WorkUpdate () {
		if (workObject ==null || (workObject.volume == CubeBlock.MAX_VOLUME && dig == false) || (workObject.volume ==0 && dig == true)) {
            StopWork();
			return;
		}
		if (workersCount > 0) {
			workflow += workSpeed ;
			if (workflow >= 1) LabourResult();
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
					if (production == 0) { StopWork(); return;}
				}
			}
			workflow -= production;
        if (dig)
        {
            actionLabel = Localization.GetActionLabel(LocalizationActionLabels.DigInProgress) + " (" + ((int)((1 - (float)workObject.volume / (float)CubeBlock.MAX_VOLUME) * 100)).ToString() + "%)";
        }
        else
        {
            actionLabel = Localization.GetActionLabel(LocalizationActionLabels.PouringInProgress) + " (" + ((int)(((float)workObject.volume / (float)CubeBlock.MAX_VOLUME) * 100)).ToString() + "%)";
        }
	}

	protected override void RecalculateWorkspeed() {
		workSpeed = GameMaster.CalculateWorkspeed(workersCount,WorkType.Digging);
	}

	public void Set(CubeBlock block, bool work_is_dig) {
		workObject = block;
        workObject.SetWorksite(this);
		dig = work_is_dig;
		if (dig) {
			Block b = GameMaster.mainChunk.GetBlock(block.pos.x, block.pos.y + 1, block.pos.z);
			if (b != null && (b.type == BlockType.Surface & b.worksite == null)) {
				CleanSite cs = b.gameObject.AddComponent<CleanSite>();
                TransferWorkers(this, cs);
                cs.Set(b as SurfaceBlock, true);
                if (showOnGUI)
                {
                    cs.ShowOnGUI();
                    showOnGUI = false;
                }
                StopWork();
                return;
			}
			sign = Instantiate(Resources.Load<GameObject> ("Prefs/DigSign")).GetComponent<WorksiteSign>(); 
		}
		else 	sign = Instantiate(Resources.Load<GameObject>("Prefs/PourInSign")).GetComponent<WorksiteSign>();
		sign.worksite = this;
		sign.transform.position = workObject.transform.position + Vector3.up * Block.QUAD_SIZE;
        //FollowingCamera.main.cameraChangedEvent += SignCameraUpdate;

        mainResource = ResourceType.GetResourceTypeById(workObject.material_id);
		if (workersCount < START_WORKERS_COUNT) GameMaster.colonyController.SendWorkers(START_WORKERS_COUNT, this);
        if (!worksitesList.Contains(this)) worksitesList.Add(this);
        if (!subscribedToUpdate)
        {
            GameMaster.realMaster.labourUpdateEvent += WorkUpdate;
            subscribedToUpdate = true;
        }
    }

  //  public void SignCameraUpdate()
   // {
   //     sign.transform.LookAt(FollowingCamera.camPos);
   // }

    override public void StopWork()
    {
        if (destroyed) return;
        else destroyed = true;
        if (workersCount > 0)
        {
            GameMaster.colonyController.AddWorkers(workersCount);
            workersCount = 0;
        }
        if (sign != null)
        {
           // FollowingCamera.main.cameraChangedEvent -= SignCameraUpdate;
            Destroy(sign.gameObject);
        }
        if (workObject != null)
        {            
            if ( workObject.excavatingStatus == 0) workObject.myChunk.AddBlock(new ChunkPos(workObject.pos.x, workObject.pos.y + 1, workObject.pos.z), BlockType.Surface, workObject.material_id, false);
            if (workObject.worksite == this) workObject.ResetWorksite();
        }
        if (worksitesList.Contains(this)) worksitesList.Remove(this);
        if (subscribedToUpdate)
        {
            GameMaster.realMaster.labourUpdateEvent -= WorkUpdate;
            subscribedToUpdate = false;
        }
        if (showOnGUI)
        {
            observer.SelfShutOff();
            showOnGUI = false;
        }
        Destroy(this);
    }

    #region save-load system
    override protected WorksiteSerializer Save() {
		if (workObject == null) {
            StopWork();
			return null;
		}
		WorksiteSerializer ws = GetWorksiteSerializer();
		ws.type = WorksiteType.DigSite;
		ws.workObjectPos = workObject.pos;
		ws.specificData = new byte[]{dig == true ? (byte)1 : (byte)0};
		return ws;
	}
	override protected void Load(WorksiteSerializer ws) {
		LoadWorksiteData(ws);
		Set(GameMaster.mainChunk.GetBlock(ws.workObjectPos) as CubeBlock, ws.specificData[0] == 1);
	}
	#endregion
}
