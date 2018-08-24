﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CubeBlockSerializer {
	public float naturalFossils;
	public byte excavatingStatus;
	public int volume;
	public bool career;
}

public class CubeBlock : Block{
	public MeshRenderer[] faces {get;private set;} // 0 - north, 1 - east, 2 - south, 3 - west, 4 - up, 5 - down
	public float naturalFossils = 0;
	byte excavatingStatus = 0, prevDrawMask = 0; // 0 is 0, 1 is 25, 2 is 50, 3 is 75
	public int volume ;
	public static readonly int MAX_VOLUME;
	public bool career{get;private set;} // изменена ли верхняя поверхность на котлован?

	static CubeBlock() {
		MAX_VOLUME = SurfaceBlock.INNER_RESOLUTION * SurfaceBlock.INNER_RESOLUTION * SurfaceBlock.INNER_RESOLUTION;
	}

	void Awake() {
		visibilityMask = 0; 
		naturalFossils = MAX_VOLUME;
		isTransparent = false;
		volume = MAX_VOLUME; career = false;
	}

	public int PourIn (int blocksCount) {
		if (volume == MAX_VOLUME) return blocksCount;
		if (blocksCount > (MAX_VOLUME - volume)) {
			blocksCount = MAX_VOLUME - volume;
		}
		volume += blocksCount;
		CheckExcavatingStatus();
		return blocksCount;
	}

	public int Dig(int blocksCount, bool show) {
		if (blocksCount > volume) blocksCount = volume;
		volume -= blocksCount;	
		if (show) career = true;
		if (volume == 0) {
			if (career) {
				Block b = myChunk.GetBlock(pos.x, pos.y - 1, pos.z);
				if ( b == null | !(b.type == BlockType.Cube | b.type == BlockType.Cave)) myChunk.DeleteBlock(pos);
				else myChunk.ReplaceBlock(pos, BlockType.Surface, b.material_id, false);
			}
			else myChunk.ReplaceBlock(pos, BlockType.Cave, material_id, false);
		} 
		else if (career) CheckExcavatingStatus();
		return blocksCount;
	}

	public void SetFossilsVolume ( int x) {
		naturalFossils = x;
	}

	public void BlockSet (Chunk f_chunk, ChunkPos f_chunkPos, int f_material_id, bool naturalGeneration) {
		myChunk = f_chunk; transform.parent = f_chunk.transform;
		pos = f_chunkPos; transform.localPosition = new Vector3(pos.x,pos.y,pos.z);
		transform.localRotation = Quaternion.Euler(Vector3.zero);
		material_id = f_material_id;
		type = BlockType.Cube; isTransparent = false;
		if (naturalGeneration) {naturalFossils = MAX_VOLUME;} else naturalFossils = 0;

		gameObject.name = "block "+ pos.x.ToString() + ';' + pos.y.ToString() + ';' + pos.z.ToString();
	}

	public override void ReplaceMaterial(int newId) {
		material_id = newId;
		if (faces != null) {
			foreach (MeshRenderer mr in faces) {
				if (mr == null) continue;
				else mr.material = ResourceType.GetMaterialById(material_id, mr.GetComponent<MeshFilter>());
			}
		}
    }

	override public void SetRenderBitmask(byte x) {
		renderMask = x;
		ChangeFacesStatus();
	}

	override public void SetVisibilityMask (byte x) {
		visibilityMask = x;
		ChangeFacesStatus();
	}

	void ChangeFacesStatus () {
		byte mask = (byte)(renderMask&visibilityMask);
		if (mask == prevDrawMask) return;
		else prevDrawMask = mask;
		byte[] arr = new byte[]{1,2,4,8,16,32};
		if (faces == null) faces = new MeshRenderer[6];
		for (int i = 0; i < 6; i++) {
			if (faces[i] == null) CreateFace(i);
			if (((mask & arr[i]) == 0)) {
				faces[i].enabled = false;
				faces[i].GetComponent<Collider>().enabled = false;
			}
			else {
				faces[i].enabled = true;
				faces[i].GetComponent<Collider>().enabled = true;
			}
		}
    }

	void CreateFace(int i) {
		if (faces == null) faces =new MeshRenderer[6];
		else {if (faces[i] != null) return;}
		GameObject g = GameObject.Instantiate(PoolMaster.quad_pref) as GameObject;
		g.tag = "BlockCollider";
		faces[i] =g.GetComponent <MeshRenderer>();
		g.transform.parent = transform;
		switch (i) {
		case 0: faces[i].name = "north_plane"; faces[i].transform.localRotation = Quaternion.Euler(0, 180, 0); faces[i].transform.localPosition = new Vector3(0, 0, Block.QUAD_SIZE/2f); break;
		case 1: faces[i].transform.localRotation = Quaternion.Euler(0, 270, 0); faces[i].name = "east_plane"; faces[i].transform.localPosition = new Vector3(Block.QUAD_SIZE/2f, 0, 0); break;
		case 2: faces[i].name = "south_plane"; faces[i].transform.localPosition = new Vector3(0, 0, -Block.QUAD_SIZE/2f); break;
		case 3: faces[i].transform.localRotation = Quaternion.Euler(0, 90, 0);faces[i].name = "west_plane"; faces[i].transform.localPosition = new Vector3(-Block.QUAD_SIZE/2f, 0, 0); break;
		case 4: 
				faces[i].transform.localPosition = new Vector3(0, Block.QUAD_SIZE/2f, 0); 
				faces[i].transform.localRotation = Quaternion.Euler(90, 0, 0);
				faces[i].name = "upper_plane"; 
			break;
		case 5: 
			faces[i].transform.localRotation = Quaternion.Euler(-90, 0, 0); 
			faces[i].name = "bottom_plane"; 
			faces[i].transform.localPosition = new Vector3(0, -Block.QUAD_SIZE/2f, 0); 
			//GameObject.Destroy( faces[i].gameObject.GetComponent<MeshCollider>() );
			break;
		}
		faces[i].sharedMaterial = ResourceType.GetMaterialById(material_id, faces[i].GetComponent<MeshFilter>());
		faces[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        faces[i].lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        faces[i].reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
		//if (Block.QUAD_SIZE != 1) faces[i].transform.localScale = Vector3.one * Block.QUAD_SIZE;
		faces[i].enabled = true;
	}

	void CheckExcavatingStatus() {
		if ( volume == 0) {myChunk.DeleteBlock(pos);return;}
		float pc = (float)volume/ (float)MAX_VOLUME;
		if (pc > 0.5f) {				
			if (pc > 0.75f) {				
				if (excavatingStatus != 0) {
					excavatingStatus = 0; 
					if (faces == null || faces[4] == null) CreateFace(4);
                    MeshFilter mf = faces[4].GetComponent<MeshFilter>();
					mf.mesh = PoolMaster.quad_pref.GetComponent<MeshFilter>().mesh;
                    ResourceType.GetMaterialById(material_id, mf);
                }
			}
			else {
				if (excavatingStatus != 1) {
					excavatingStatus = 1;
					if (faces == null || faces[4] == null) CreateFace(4);
                    MeshFilter mf = faces[4].GetComponent<MeshFilter>();
                    mf.mesh = PoolMaster.plane_excavated_025;
                    ResourceType.GetMaterialById(material_id, mf);
                }
			}
		}
		else { // выкопано больше половины
				if (pc > 0.25f) {
				if (excavatingStatus != 2) {
					excavatingStatus = 2;
					if ( faces == null || faces[4] == null) CreateFace(4);
                    MeshFilter mf = faces[4].GetComponent<MeshFilter>();
                    mf.mesh = PoolMaster.plane_excavated_05;
                    ResourceType.GetMaterialById(material_id, mf);
                }
				}
				else {
					if (excavatingStatus != 3) {
						excavatingStatus = 3; 
					if ( faces == null || faces[4] == null) CreateFace(4);
                    MeshFilter mf = faces[4].GetComponent<MeshFilter>();
                    mf.mesh = PoolMaster.plane_excavated_075;
                    ResourceType.GetMaterialById(material_id, mf);
                }
				}
			
		}
	}
   
	#region save-load system
	override public BlockSerializer Save() {
		BlockSerializer bs = GetBlockSerializer();
		using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
		{
			new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(stream, GetCubeBlockSerializer());
			bs.specificData =  stream.ToArray();
		}
		return bs;
	} 

	override public void Load(BlockSerializer bs) {
		LoadBlockData(bs);
		CubeBlockSerializer cbs = new CubeBlockSerializer();
		GameMaster.DeserializeByteArray<CubeBlockSerializer>(bs.specificData, ref cbs);
		LoadCubeBlockData(cbs);
	}

	protected void LoadCubeBlockData(CubeBlockSerializer cbs) {
		career = cbs.career;
		excavatingStatus = cbs.excavatingStatus;
        naturalFossils = cbs.naturalFossils;
        volume = cbs.volume;
        if (career) CheckExcavatingStatus();		
	}
	#endregion

	CubeBlockSerializer GetCubeBlockSerializer() {
		CubeBlockSerializer cbs = new CubeBlockSerializer();
		cbs.naturalFossils =naturalFossils;
		cbs.excavatingStatus = excavatingStatus;
		cbs.volume = volume;
		cbs.career = career;
		return cbs;
	}
}
