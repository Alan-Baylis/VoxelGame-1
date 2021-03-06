﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CubeBlockSerializer {
	public float naturalFossils;
	public int volume;
	public bool career;
}

public class CubeBlock : Block{
	public MeshRenderer[] faces {get;private set;} // 0 - north, 1 - east, 2 - south, 3 - west, 4 - up, 5 - down
	public float naturalFossils = 0;
    public byte excavatingStatus { get; private set; } // 0 is 75%+, 1 is 50%+, 2 is 25%+, 3 is less than 25%
    byte prevDrawMask = 0;
	public int volume ;
	public static readonly int MAX_VOLUME;
	public bool career{get;private set;} // изменена ли верхняя поверхность на котлован?

	static CubeBlock() {
		MAX_VOLUME = SurfaceBlock.INNER_RESOLUTION * SurfaceBlock.INNER_RESOLUTION * SurfaceBlock.INNER_RESOLUTION;
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
        if (volume == 0) return 0;
		if (blocksCount > volume) blocksCount = volume;
		volume -= blocksCount;	
		if (show) career = true;
        if (career) CheckExcavatingStatus();
        else
        {
           if (volume == 0) myChunk.ReplaceBlock(pos, BlockType.Cave, material_id, false);
        }
		return blocksCount;
	}

	public void SetFossilsVolume ( int x) {
		naturalFossils = x;
	}

	public void InitializeCubeBlock (Chunk f_chunk, ChunkPos f_chunkPos, int f_material_id, bool naturalGeneration) {
            visibilityMask = 0;
            excavatingStatus = 0;
            naturalFossils = MAX_VOLUME;
            isTransparent = false;
            volume = MAX_VOLUME; career = false;
            type = BlockType.Cube;

		myChunk = f_chunk;
        pos = f_chunkPos;
        Transform t = transform;
        t.parent = f_chunk.transform;
        t.localPosition = new Vector3(pos.x, pos.y, pos.z);        
		t.localRotation = Quaternion.Euler(Vector3.zero);
        name = "block " + pos.x.ToString() + ';' + pos.y.ToString() + ';' + pos.z.ToString();
        material_id = f_material_id;
        illumination = 255;

        faces = new MeshRenderer[6];
        if (naturalGeneration) { naturalFossils = MAX_VOLUME; }
        else naturalFossils = 0;        
	}

	public override void ReplaceMaterial(int newId) {
        if (newId == material_id) return;
		material_id = newId;
		foreach (MeshRenderer mr in faces) {
			if (mr == null) continue;
			else mr.sharedMaterial = ResourceType.GetMaterialById(material_id, mr.GetComponent<MeshFilter>(), illumination);
		}
    }

	override public void SetRenderBitmask(byte x) {
		renderMask = x;
		ChangeFacesStatus();
	}

	override public void SetVisibilityMask (byte x) {
        byte prevMask = visibilityMask;
        // блоки, отключающиеся полностью, возвращают модели обратно в пул
        if (prevMask == 0 & x != 0) // включение
        {
            visibilityMask = x; 
            for (int i = 0; i < 6; i++)
            {
                if (faces[i] == null) CreateFace(i);
                else faces[i].gameObject.SetActive(true);
            }
            ChangeFacesStatus(); // т.к в случае полного отключение вырубаем не рендереры, а сами объекты
        }
        else
        {
            if (prevMask != 0 & x== 0) // полное выключение
            {                
                visibilityMask = 0;
                if (faces[4] != null) {
                    if (excavatingStatus == 0) { PoolMaster.ReturnQuadToPool(faces[4].gameObject); faces[4] = null; }
                    else faces[4].gameObject.SetActive(false);
                }
                if (faces[0] != null) { PoolMaster.ReturnQuadToPool(faces[0].gameObject); faces[0] = null; }
                if (faces[1] != null) {PoolMaster.ReturnQuadToPool(faces[1].gameObject); faces[1] = null; }
                if (faces[2] != null) {PoolMaster.ReturnQuadToPool(faces[2].gameObject); faces[2] = null; }
                if (faces[3] != null) {PoolMaster.ReturnQuadToPool(faces[3].gameObject); faces[3] = null; }
                if (faces[5] != null) {PoolMaster.ReturnQuadToPool(faces[5].gameObject); faces[5] = null; }
            }
            else
            {
                visibilityMask = x;
                ChangeFacesStatus();
            }
        }		
	}

	void ChangeFacesStatus () {
		byte mask = (byte)(renderMask & visibilityMask);
		if (mask == prevDrawMask) return;
		else prevDrawMask = mask;
		byte[] arr = new byte[]{1,2,4,8,16,32};
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
		GameObject g = PoolMaster.GetQuad();
        g.tag = "BlockCollider";
        Transform t = g.transform;		
        t.parent = transform;
        faces[i] = g.GetComponent <MeshRenderer>();

        byte faceIllumination = 255 ;
		switch (i) {
		case 0: // fwd
                g.name = "north_plane";
                t.localRotation = Quaternion.Euler(0, 180, 0);
                t.localPosition = new Vector3(0, 0, QUAD_SIZE/2f);
                if (pos.z != Chunk.CHUNK_SIZE - 1) faceIllumination = myChunk.lightMap[pos.x, pos.y, pos.z + 1];
                break;
		case 1: // right
                g.name = "east_plane";
                t.localRotation = Quaternion.Euler(0, 270, 0);               
                t.localPosition = new Vector3(QUAD_SIZE/2f, 0, 0);
                if (pos.x != Chunk.CHUNK_SIZE - 1) faceIllumination = myChunk.lightMap[pos.x + 1, pos.y, pos.z];
                break;
		case 2: // back
                g.name = "south_plane";
                t.localRotation = Quaternion.Euler(0, 0, 0);
                t.localPosition = new Vector3(0, 0, -QUAD_SIZE/2f);
                if (pos.z != 0) faceIllumination = myChunk.lightMap[pos.x, pos.y, pos.z - 1];
                break;
		case 3: // left
                g.name = "west_plane";
                t.localRotation = Quaternion.Euler(0, 90, 0);
                t.localPosition = new Vector3(-QUAD_SIZE/2f, 0, 0);
                if (pos.x != 0) faceIllumination = myChunk.lightMap[pos.x - 1, pos.y, pos.z ];
                break;
		case 4: // up
                g.name = "upper_plane";
                t.localPosition = new Vector3(0, QUAD_SIZE/2f, 0); 
				t.localRotation = Quaternion.Euler(90, 0, 0);
                if (pos.y != Chunk.CHUNK_SIZE -1) faceIllumination = myChunk.lightMap[pos.x, pos.y + 1, pos.z ];
                break;
		case 5: // down
                g.name = "bottom_plane";
                t.localRotation = Quaternion.Euler(-90, 0, 0);			
			    t.localPosition = new Vector3(0, -QUAD_SIZE/2f, 0);
                if (pos.y != 0) faceIllumination = myChunk.lightMap[pos.x, pos.y - 1, pos.z ];
                //GameObject.Destroy( faces[i].gameObject.GetComponent<MeshCollider>() );
                break;
		}
		faces[i].sharedMaterial = ResourceType.GetMaterialById(material_id, faces[i].GetComponent<MeshFilter>(), faceIllumination);
		faces[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        faces[i].lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        faces[i].reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
		//if (Block.QUAD_SIZE != 1) faces[i].transform.localScale = Vector3.one * Block.QUAD_SIZE;
		faces[i].enabled = true;
	}

	void CheckExcavatingStatus() {
		if ( volume == 0) {
            if (career) myChunk.DeleteBlock(pos); else myChunk.ReplaceBlock(pos, BlockType.Cave, material_id,false);
            return;
        }
		float pc = volume/ (float)MAX_VOLUME;
		if (pc > 0.5f) {				
			if (pc > 0.75f) {				
				if (excavatingStatus != 0) {
					excavatingStatus = 0; 
					if (faces[4] == null) CreateFace(4);
                    MeshFilter mf = faces[4].GetComponent<MeshFilter>();
                    mf.sharedMesh = PoolMaster.GetOriginalQuadMesh();
                    ResourceType.GetMaterialById(material_id, mf, illumination);
                }
			}
			else {
				if (excavatingStatus != 1) {
					excavatingStatus = 1;
					if (faces[4] == null) CreateFace(4);
                    MeshFilter mf = faces[4].GetComponent<MeshFilter>();
                    mf.sharedMesh = PoolMaster.plane_excavated_025;
                    ResourceType.GetMaterialById(material_id, mf, illumination);
                }
			}
		}
		else { // выкопано больше половины
				if (pc > 0.25f) {
				if (excavatingStatus != 2) {
					excavatingStatus = 2;
					if ( faces[4] == null) CreateFace(4);
                    MeshFilter mf = faces[4].GetComponent<MeshFilter>();
                    mf.sharedMesh = PoolMaster.plane_excavated_05;
                    ResourceType.GetMaterialById(material_id, mf, illumination);
                }
				}
				else {
					if (excavatingStatus != 3) {
						excavatingStatus = 3; 
					if ( faces[4] == null) CreateFace(4);
                    MeshFilter mf = faces[4].GetComponent<MeshFilter>();
                    mf.sharedMesh = PoolMaster.plane_excavated_075;
                    ResourceType.GetMaterialById(material_id, mf, illumination);
                }
				}
			
		}
	}

    override public void SetIllumination()
    {
        illumination = myChunk.lightMap[pos.x, pos.y, pos.z];
        int size = Chunk.CHUNK_SIZE;
        byte[,,] lmap = myChunk.lightMap;
        if (faces[0] != null)
        {
            if (pos.z + 1 >= size) illumination = 255; else illumination = lmap[pos.x, pos.y, pos.z + 1];
            faces[0].sharedMaterial = ResourceType.GetMaterialById(material_id, faces[0].GetComponent<MeshFilter>(), illumination);
        }
        if (faces[1] != null)
        {
            if (pos.x + 1 >= size) illumination = 255; else illumination = lmap[pos.x + 1, pos.y, pos.z];
            faces[1].sharedMaterial = ResourceType.GetMaterialById(material_id, faces[1].GetComponent<MeshFilter>(), illumination);
        }
        if (faces[2] != null)
        {
            if (pos.z - 1 < 0) illumination = 255; else illumination = lmap[pos.x, pos.y, pos.z - 1];
            faces[2].sharedMaterial = ResourceType.GetMaterialById(material_id, faces[2].GetComponent<MeshFilter>(), illumination);
        }
        if (faces[3] != null)
        {
            if (pos.x - 1 < 0) illumination = 255; else illumination = lmap[pos.x - 1, pos.y, pos.z];
            faces[3].sharedMaterial = ResourceType.GetMaterialById(material_id, faces[3].GetComponent<MeshFilter>(), illumination);
        }
        if (faces[4] != null)
        {
            if (pos.y >= size - 1) illumination = 255; else illumination = lmap[pos.x , pos.y + 1, pos.z];
            faces[4].sharedMaterial = ResourceType.GetMaterialById(material_id, faces[4].GetComponent<MeshFilter>(), illumination);
        }
        if (faces[5] != null)
        {
            if (pos.y == 0) illumination = 255; else illumination = lmap[pos.x , pos.y - 1, pos.z];
            faces[5].sharedMaterial = ResourceType.GetMaterialById(material_id, faces[5].GetComponent<MeshFilter>(), illumination);
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
        naturalFossils = cbs.naturalFossils;
        volume = cbs.volume;
        if (career) CheckExcavatingStatus();		
	}
    #endregion

    override public void Annihilate()
    {
        // #block annihilate
        if (destroyed) return;
        else destroyed = true;
        if (worksite != null) worksite.StopWork();
        if (mainStructure != null) mainStructure.Annihilate(true);
        // end
        if (excavatingStatus == 0 & faces[4] != null) PoolMaster.ReturnQuadToPool(faces[4].gameObject);
        if (faces[0] != null) PoolMaster.ReturnQuadToPool(faces[0].gameObject);
        if (faces[1] != null) PoolMaster.ReturnQuadToPool(faces[1].gameObject);
        if (faces[2] != null) PoolMaster.ReturnQuadToPool(faces[2].gameObject);
        if (faces[3] != null) PoolMaster.ReturnQuadToPool(faces[3].gameObject);
        if (faces[5] != null) PoolMaster.ReturnQuadToPool(faces[5].gameObject);
        Destroy(gameObject);
    }

    CubeBlockSerializer GetCubeBlockSerializer() {
		CubeBlockSerializer cbs = new CubeBlockSerializer();
		cbs.naturalFossils =naturalFossils;
		cbs.volume = volume;
		cbs.career = career;
		return cbs;
	}
}
