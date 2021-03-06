﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum UIMode {View, SurfaceBlockPanel, CubeBlockPanel, StructurePanel, WorksitePanel, GameMenu}

public class UI : MonoBehaviour {
	public LineRenderer lineDrawer;
	public static UI current;

	public UIMode mode{get;private set;}

	public bool showLayerCutButtons = false;
	float buildingsListLength = 0;
	public float leftPanelWidth{get;private set;}
	byte argument, showingBuildingsLevel = 1;
	public Rect rightPanelBox, upPanelBox, acceptBox,systemInfoRect, buildingGridRect;
	string systemInfoString; float systemInfoTimer = 0;
	public bool  showBuildingCreateInfo {get; private set;}
	public bool touchscreenTemporarilyBlocked = false;
	PixelPosByte bufferedPosition; int chosenBuildingIndex = 0;
	ResourceContainer[] bufferedResources;
	SurfaceBlock chosenSurfaceBlock; bool chosenSurfaceBlockIsBorderBlock = false;
	CubeBlock chosenCubeBlock; byte faceIndex = 10;
	Structure chosenStructure;
	Worksite chosenWorksite;
	Crew showingCrew; bool showCrewsOrVesselsList = false, dissmissConfirmation = false;
	Vector2 mousePos, buildingsScrollViewPos = Vector2.zero;
	GameObject quadSelector, structureFrame;

	Texture  grid16_tx, greenSquare_tx, whiteSquare_tx, whiteSpecial_tx, yellowSquare_tx,
	citizen_icon_tx, energy_icon_tx,  energyLightning_icon_tx, buildingQuad_icon_tx, demolishButton_tx,
	layersCut_tx;
	public Texture rightArrow_tx{get;private set;}

	List<Factory> smelteriesList; bool hasSmelteries = false;
	List<Factory> unspecializedFactories; bool hasUnspecializedFactories = false;
	List<Building> showingBuildingsList;
	public const int GUIDEPTH_UI_MAIN = 10, GUIDEPTH_WORKSITE_WINDOW = 9;


	void Awake() {
		current = this;
		mode = UIMode.View;

		showBuildingCreateInfo = false;
		showingBuildingsLevel = 0;

		structureFrame = Instantiate(Resources.Load<GameObject>("Prefs/structureFrame"));
		structureFrame.SetActive(false);
		quadSelector = Instantiate(Resources.Load<GameObject>("Prefs/QuadSelector"));
		quadSelector.SetActive(false);
		GameMaster.realMaster.AddToCameraUpdateBroadcast(gameObject);
		layersCut_tx = Resources.Load<Texture>("Textures/layersCut");
		grid16_tx = Resources.Load<Texture>("Textures/AccessibleGrid");
		greenSquare_tx = Resources.Load<Texture>("Textures/greenSquare");
		whiteSquare_tx = Resources.Load<Texture>("Textures/whiteSquare");
		whiteSpecial_tx = Resources.Load<Texture>("Textures/whiteSpecialSquare");
		yellowSquare_tx = Resources.Load<Texture>("Textures/yellowSquare");
		citizen_icon_tx = Resources.Load<Texture>("Textures/citizen_icon");
		energy_icon_tx = Resources.Load<Texture>("Textures/energy_icon");
		energyLightning_icon_tx = Resources.Load<Texture>("Textures/energyLightning_icon");
		buildingQuad_icon_tx = Resources.Load<Texture>("Textures/buildingQuad_icon");
		demolishButton_tx = Resources.Load<Texture>("Textures/button_demolish");
		rightArrow_tx = Resources.Load<Texture>("Textures/gui_arrowRight");

		argument = 0;

		float k = GameMaster.guiPiece;
		float upPanelHeight = k;
		leftPanelWidth = 2 * k;
		float rightPanelWidth = 8 * k;
		rightPanelBox = new Rect(Screen.width - rightPanelWidth, upPanelHeight, rightPanelWidth, Screen.height - upPanelHeight);
		upPanelBox = new Rect(0,0,Screen.width, upPanelHeight);
		acceptBox = new Rect(Screen.width / 2f - 4 * k, Screen.height / 2f - 2 * k, 8 * k, 4 * k);
		systemInfoRect = new Rect(Screen.width/2f - 6 * k,  2 *k, 12 * k, 4 *k);

		float p = Screen.width/2f / SurfaceBlock.INNER_RESOLUTION;
		float side = p * SurfaceBlock.INNER_RESOLUTION;
		buildingGridRect = new Rect(rightPanelBox.x - side - 2 * p, Screen.height/2f - side/2f, side, side);
	}
	public void Reset() {
		bufferedResources = null;
		DropFocus();
	}

	void Update() {
		mousePos = Input.mousePosition;
		mousePos.y = Screen.height - mousePos.y;
		if (Input.GetMouseButtonDown(0) && !cursorIntersectGUI(mousePos) ) {
			RaycastHit rh;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rh)) {
				DropFocus();
				Structure s = rh.collider.GetComponent<Structure>();
				if (s != null) {
					chosenStructure = s;
					chosenStructure.SetGUIVisible(true);
					mode = UIMode.StructurePanel;
					GameMaster.realMaster.SetLookPoint (chosenStructure.transform.position);
					structureFrame.SetActive(true);
					structureFrame.transform.localScale = new Vector3(chosenStructure.innerPosition.x_size, 1, chosenStructure.innerPosition.z_size);
					structureFrame.transform.position = chosenStructure.transform.position;
				}
				else { // not a structure
					if (rh.collider.transform.parent != null) {
						Block b = rh.collider.transform.parent.GetComponent<Block>();
						if (b != null) {
							float x ,y,z,d = Block.QUAD_SIZE/2f;
							switch (b.type) {
							case BlockType.Cave:
							case BlockType.Surface:
									mode = UIMode.SurfaceBlockPanel;
									chosenSurfaceBlock = b.GetComponent<SurfaceBlock>();
									chosenCubeBlock = chosenSurfaceBlock.myChunk.GetBlock(chosenSurfaceBlock.pos.x, chosenSurfaceBlock.pos.y - 1, chosenSurfaceBlock.pos.z) as CubeBlock;
									x= chosenSurfaceBlock.transform.position.x; y = chosenSurfaceBlock.transform.position.y - d + 0.01f;  z = chosenSurfaceBlock.transform.position.z;
									lineDrawer.SetPositions(new Vector3[5]{new Vector3(x - d, y, z+d), new Vector3(x+d,y,z+d), new Vector3(x+d,y,z-d), new Vector3(x - d, y, z-d), new Vector3(x-d, y,z+d)});
									lineDrawer.material = PoolMaster.lr_green_material;
									lineDrawer.enabled = true;
									GameMaster.realMaster.SetLookPoint (chosenSurfaceBlock.transform.position + Vector3.down * Block.QUAD_SIZE /2f);
									argument = 1;
									if (chosenSurfaceBlock.pos.x == 0 || chosenSurfaceBlock.pos.x == Chunk.CHUNK_SIZE - 1 || chosenSurfaceBlock.pos.z == 0 || chosenSurfaceBlock.pos.z == Chunk.CHUNK_SIZE - 1)
									chosenSurfaceBlockIsBorderBlock = true; else chosenSurfaceBlockIsBorderBlock = false;
									// подборка зданий
								if (showingBuildingsLevel != 0) showingBuildingsList = Structure.GetApplicableBuildingsList(showingBuildingsLevel, chosenSurfaceBlock);
								break;
							case BlockType.Cube:
								chosenCubeBlock = b.GetComponent<CubeBlock>();
									for (byte i =0; i< 6; i++) {
										if (chosenCubeBlock.faces[i] == null) continue;
										if (chosenCubeBlock.faces[i].GetComponent<Collider>() == rh.collider ) {faceIndex = i;break;}
									}
									if (faceIndex != 10) {
										mode = UIMode.CubeBlockPanel;
										switch (faceIndex) {
										case 0: 
											quadSelector.transform.position = chosenCubeBlock.transform.position + Vector3.forward * (Block.QUAD_SIZE/2f + 0.01f);
											quadSelector.transform.rotation = Quaternion.Euler(90,0,0);
											break;
										case 1:
											quadSelector.transform.position = chosenCubeBlock.transform.position + Vector3.right * (Block.QUAD_SIZE/2f + 0.01f);
											quadSelector.transform.rotation = Quaternion.Euler(0,0,-90);
											break;
										case 2:
											quadSelector.transform.position = chosenCubeBlock.transform.position + Vector3.back * (Block.QUAD_SIZE/2f + 0.01f);
											quadSelector.transform.rotation = Quaternion.Euler(-90,0,0);
											break;
										case 3:
											quadSelector.transform.position = chosenCubeBlock.transform.position + Vector3.left * (Block.QUAD_SIZE/2f + 0.01f);
											quadSelector.transform.rotation = Quaternion.Euler(0,0,90);
											break;
										case 4: // up
										if ( chosenCubeBlock.career  ) {
											quadSelector.transform.position = chosenCubeBlock.transform.position + Vector3.up * (Block.QUAD_SIZE/2f + 0.01f);
											quadSelector.transform.rotation = Quaternion.Euler(0,0,0);
										}
										break;
										case 5:
											quadSelector.transform.position = chosenCubeBlock.transform.position + Vector3.down * (Block.QUAD_SIZE/2f + 0.01f);
											quadSelector.transform.rotation = Quaternion.Euler(0,0,180);
											break;
										}
										quadSelector.SetActive(true);
										GameMaster.realMaster.SetLookPoint (quadSelector.transform.position);
									}	
								break;
							}
						}
					}
					else { // no transfrom parent
						WorksiteSign ws = rh.collider.GetComponent<WorksiteSign>();
						if (ws != null) {
							chosenWorksite = ws.worksite;
							chosenWorksite.showOnGUI = true;
							mode = UIMode.WorksitePanel;
						}
					}
			}				
			}
			else DropFocus();
		}
		if (systemInfoTimer > 0) {
			systemInfoTimer -= Time.deltaTime * GameMaster.gameSpeed;
			if (systemInfoTimer <= 0) {
				systemInfoString = null;
			}
		}
	}


	bool cursorIntersectGUI(Vector2 point) {
		if (touchscreenTemporarilyBlocked) return true;
		if ( point.y <= upPanelBox.height || point.x  <= leftPanelWidth ) return true;
		if (mode != UIMode.View) {
			if (point.x >= rightPanelBox.x) return true;
			if (showBuildingCreateInfo) return true;
		}
		return false;
	}

	public void DropFocus() {
		lineDrawer.enabled = false;
		quadSelector.SetActive(false);
		structureFrame.SetActive(false);
		chosenCubeBlock = null; 
		if (chosenStructure != null) {chosenStructure.SetGUIVisible(false);chosenStructure = null; }
		chosenSurfaceBlock = null; 
		if (chosenWorksite != null) {chosenWorksite.showOnGUI = false; chosenWorksite = null;}
		showBuildingCreateInfo = false;
		argument = 0;
		touchscreenTemporarilyBlocked = false;
		buildingsScrollViewPos = Vector2.zero; buildingsListLength = 0;
	}

	void SwitchUIMode(UIMode newMode) {
		switch (newMode) {
		case UIMode.View:
			mode = UIMode.View;
			DropFocus();
			break;
		case UIMode.GameMenu:
			mode = UIMode.GameMenu;
			DropFocus();
			break;
		}
	}

	void ChangeArgument (byte newArg) {
		argument = newArg;
		buildingsScrollViewPos = Vector2.zero; buildingsListLength = 0;
		switch (mode) {
		case UIMode.SurfaceBlockPanel:
			switch (argument) {
			case 1: // main actions menu
				chosenStructure = null;
				showBuildingCreateInfo = false;
				bufferedPosition = PixelPosByte.Empty;
				break;
			case 3:
				chosenBuildingIndex = -1;
				chosenStructure = null;
				showBuildingCreateInfo = false;
				break;
			}
			break;
		case UIMode.StructurePanel:			
			break;
		case UIMode.WorksitePanel:
			switch(argument) {
			case 1:
				chosenCubeBlock = null;
				chosenSurfaceBlock = null;
				lineDrawer.enabled = false;
				quadSelector.SetActive(false);
				break;
			}
			break;
		}
	}

	public void ChangeSystemInfoString(string s) {
		if ( s == null ) return;
		systemInfoString = s;
		systemInfoTimer = 2;
	}

	void OnGUI() {
		GUI.skin = GameMaster.mainGUISkin;
		float k = GameMaster.guiPiece;
		//upLeft infopanel
		GUI.Box(upPanelBox, GUIContent.none);
		ColonyController cc = GameMaster.colonyController; 
		if (cc != null) {
			GUI.DrawTexture(new Rect(0,0, k,k), citizen_icon_tx, ScaleMode.StretchToFill);	
			GUI.Label(new Rect(k,0, 4*k,k), cc.freeWorkers.ToString() + " / " + cc.citizenCount.ToString() + " / "+ cc.totalLivespace.ToString());
			GUI.DrawTexture(new Rect(5*k, 0, k, k), energy_icon_tx, ScaleMode.StretchToFill);
			string energySurplusString = ((int)cc.energySurplus).ToString(); if (cc.energySurplus > 0) energySurplusString = '+' + energySurplusString;
			GUI.Label( new Rect ( 6 * k, 0, 5 * k, k) , ((int)cc.energyStored).ToString() + " / " + ((int)cc.totalEnergyCapacity).ToString() + " ( " + energySurplusString + " )" );
			GUI.DrawTexture ( new Rect ( 11 * k, 0, k, k), PoolMaster.energyCrystal_icon_tx, ScaleMode.StretchToFill ) ;
			GUI.Label ( new Rect ( 12 * k, 0, k, k ), ((int)cc.energyCrystalsCount).ToString() ) ;
		} 
		//Left Panel.
		Rect layerCutRect =new Rect(0, Screen.height - k, leftPanelWidth / 2f,k);
		if (GUI.Button( layerCutRect, layersCut_tx, PoolMaster.GUIStyle_BorderlessButton)) {
			if (showLayerCutButtons) {
				GameMaster.prevCutHeight = GameMaster.layerCutHeight;
				GameMaster.layerCutHeight = Chunk.CHUNK_SIZE;
				GameMaster.mainChunk.LayersCut();
				showLayerCutButtons = false;
			}
			else {
				int p = GameMaster.layerCutHeight;
				GameMaster.layerCutHeight = GameMaster.prevCutHeight;
				if (GameMaster.layerCutHeight != p)  GameMaster.mainChunk.LayersCut();
				showLayerCutButtons = true;
			}
		}
		if (showLayerCutButtons) {
			layerCutRect.x += layerCutRect.width; layerCutRect.y -= 2 * k;
			GUI.Box(new Rect(layerCutRect.x, layerCutRect.y, layerCutRect.width, layerCutRect.height * 3), GUIContent.none);
			if (GUI.Button (layerCutRect, PoolMaster.plusButton_tx) ) {
				GameMaster.layerCutHeight ++;
				if (GameMaster.layerCutHeight> Chunk.CHUNK_SIZE) GameMaster.layerCutHeight= Chunk.CHUNK_SIZE ;
				else GameMaster.mainChunk.LayersCut();
			} 
			layerCutRect.y += k;
			GUI.Label (layerCutRect, GameMaster.layerCutHeight.ToString(), PoolMaster.GUIStyle_CenterOrientedLabel);
			layerCutRect.y += k;
			if (GUI.Button (layerCutRect, PoolMaster.minusButton_tx) ) {
				GameMaster.layerCutHeight --;
				if (GameMaster.layerCutHeight < 0) GameMaster.layerCutHeight = 0 ;
				else GameMaster.mainChunk.LayersCut();
			} 
		}
		//upRight infopanel
		Rect ur = new Rect(Screen.width - 4 *k, 0, 4 *k, upPanelBox.height);
		if (GUI.Button(ur, Localization.menu_gameMenuButton) ) SwitchUIMode(UIMode.GameMenu);
		ur.x -= ur.width;

		if (GUI.Button(ur, Localization.menu_colonyInfo)) {
			if ( !GameMaster.colonyController.showColonyInfo ) {
				GameMaster.colonyController.showColonyInfo = true;
				GameMaster.colonyController.storage.showStorage = false;
			}
			else GameMaster.colonyController.showColonyInfo = false;
		}
		ur.x -= ur.width;
		if (GUI.Button(ur, Localization.ui_storage_name)) {
			if ( !GameMaster.colonyController.storage.showStorage  ) {
				GameMaster.colonyController.storage.showStorage = true;
				GameMaster.colonyController.showColonyInfo = false;
			}
			else {
				GameMaster.colonyController.storage.showStorage = false;
			}
		}	

		//Crew panel
		if (showingCrew != null) {
			float a = Screen.width / 34f, left= Screen.width / 2f - 10 * a, up = 4 *k;
			GUI.Box(new Rect(left, up, 17 * a, 9* a), GUIContent.none);
			bool hasShip = (showingCrew.shuttle != null);
			Crew.GUI_DrawCrewIcon(showingCrew, new Rect(left, up,2 * a, 2*a));
			showingCrew.name = GUI.TextField(new Rect(left + 2 * a,  up, 7 * a, a), showingCrew.name);
			GUI.Label(new Rect(left + 9 * a, up, a,a), showingCrew.level.ToString());
			GUI.DrawTexture(new Rect(left + 2 * a, up + a, 8 * a, a), whiteSquare_tx, ScaleMode.StretchToFill);
			if (showingCrew.experience > 0) GUI.DrawTexture(new Rect(left + a * a, up + a, 8*a * showingCrew.experience / showingCrew.nextExperienceLimit,a), PoolMaster.orangeSquare_tx, ScaleMode.StretchToFill);
			GUI.Label(new Rect(left, up + 2 *a, 5 * a, a), Localization.vessel + " :");
			GUI.Label(new Rect(left, up + 3 * a, 8 * a, a), showingCrew.shuttle.name);
			if (Shuttle.shuttlesList.Count != 0) {
				if (GUI.Button(new Rect(left + 5 * a, up + 2 * a, 5 * a, a), Localization.change)) {
					showCrewsOrVesselsList = !showCrewsOrVesselsList;
				}
			}
			if (showCrewsOrVesselsList) {
				int vn = 0; 
				while (vn < Shuttle.shuttlesList.Count) {
					if (Shuttle.shuttlesList[vn] == null) {
						Shuttle.shuttlesList.RemoveAt(vn);
						continue;
					}
					if (hasShip && Shuttle.shuttlesList[vn] == showingCrew.shuttle) GUI.DrawTexture(new Rect(left, up + (4+vn)*a, 10 * a, a), PoolMaster.orangeSquare_tx, ScaleMode.StretchToFill);
					GUI.DrawTexture(new Rect(left, up + (4 + vn) * a, a,a), showingCrew.shuttle.condition > 0.85f ? PoolMaster.shuttle_good_icon : ( showingCrew.shuttle.condition  < 0.5f ? PoolMaster.shuttle_bad_icon : PoolMaster.shuttle_normal_icon), ScaleMode.StretchToFill );
					if (Shuttle.shuttlesList[vn].crew != null) GUI.DrawTexture(new Rect(left, up + (4 + vn) * a, a,a), showingCrew.stamina < 0.5f ? PoolMaster.crew_bad_icon : ( showingCrew.stamina > 0.85f ? PoolMaster.crew_good_icon : PoolMaster.crew_normal_icon), ScaleMode.StretchToFill);
					if (GUI.Button(new Rect(left + a, up + (4 + vn) * a, 9*a,a), Shuttle.shuttlesList[vn].name)) showingCrew.ChangeShip(Shuttle.shuttlesList[vn]);
					vn++;
				}
			}
			else { // other info
				GUI.Label(new Rect(left, up + 5 * a, 4 * a,a), Localization.crew_membersCount + ": ");
				GUI.Label(new Rect(left + 4*a, up + 5 * a, 3 * a,a), showingCrew.count.ToString());
				if (showingCrew.count > Crew.MIN_MEMBERS_COUNT) {
					if (GUI.Button(new Rect(left + 7 * a, up + 5 * a, a,a), PoolMaster.minusButton_tx)) showingCrew.DismissMember();
				}
				if (showingCrew.count < Crew.MAX_MEMBER_COUNT & GameMaster.colonyController.freeWorkers > 0) {
					if (GUI.Button(new Rect(left + 8 * a, up + 5 * a, a,a), PoolMaster.plusButton_tx)) showingCrew.AddMember();
				}
				if (GUI.Button(new Rect(left + 9*a, up + 5*a,a,a), PoolMaster.redArrow_tx )) {
					dissmissConfirmation = true;
				}
				GUI.Label(new Rect(left, up + 7 * a, 3*a,a), Localization.hangar_crewSalary + ':');
				GUI.Label(new Rect(left + 3 *a, up + 7*a, 2*a,a), string.Format("{0:0.##}",showingCrew.salary));
				GUI.Label(new Rect(left + 5 *a, up + 7*a, 3*a,a), Localization.crew_stamina +':');
				GUI.Label(new Rect(left +8 *a, up + 7*a, 2*a,a), ((int)(showingCrew.stamina * 100)).ToString() + '%');
			}
			GUI.Label(new Rect(left + 10 * a, up, 4 *a,a), Localization.crew_perception +':'); GUI.Label(new Rect(left+14*a, up, 3 *a, a), string.Format("{0:0.###}", showingCrew.perception));
			GUI.Label(new Rect(left + 10 * a, up + a, 4 *a,a), Localization.crew_persistence +':'); GUI.Label(new Rect(left+14*a, up + a, 3 *a, a), string.Format("{0:0.###}", showingCrew.persistence));
			GUI.Label(new Rect(left + 10 * a, up + 2 *a, 4 *a,a), Localization.crew_bravery +':'); GUI.Label(new Rect(left+14*a, up + 2 *a, 3 *a, a), string.Format("{0:0.###}", showingCrew.bravery));
			GUI.Label(new Rect(left + 10 * a, up + 3 *a, 4 *a,a), Localization.crew_techSkills +':'); GUI.Label(new Rect(left+14*a, up + 3*a, 3 *a, a), string.Format("{0:0.###}", showingCrew.techSkills));
			GUI.Label(new Rect(left + 10 * a, up + 4 *a, 4 *a,a), Localization.crew_survivalSkills +':'); GUI.Label(new Rect(left+14*a, up + 4 *a, 3 *a, a), string.Format("{0:0.###}", showingCrew.survivalSkills));
			GUI.Label(new Rect(left + 10 * a, up + 5 *a, 4 *a,a), Localization.crew_teamWork +':'); GUI.Label(new Rect(left+14*a, up + 5*a, 3 *a, a), string.Format("{0:0.###}", showingCrew.teamWork));

			GUI.Label(new Rect(left + 10*a, up + 7*a, 3*a, a), Localization.crew_successfulMissions + ": " + showingCrew.successfulOperations.ToString());
			GUI.Label(new Rect(left + 14*a, up + 7*a, 3*a, a), Localization.crew_totalMissions + ": " + showingCrew.totalOperations.ToString());
			if (GUI.Button(new Rect(left + 9 *a, up + 9*a, 9 *a, a), Localization.ui_close)) HideCrewCard();
		}


		//  RIGHT  PANEL : 
		if (mode != UIMode.View) {
			int guiDepth = GUI.depth;
			GUI.depth = GUIDEPTH_UI_MAIN;
			GUI.Box(rightPanelBox, GUIContent.none);
			Rect rr = new Rect(rightPanelBox.x, rightPanelBox.y, rightPanelBox.width, k);
			if (GUI.Button(rr, "Close Panel")) SwitchUIMode(UIMode.View); // СДЕЛАТЬ  ШТОРКОЙ //???
			rr.y += rr.height;

			switch (mode) {
			case UIMode.SurfaceBlockPanel:
				#region surfaceBlockPanel
				if (chosenSurfaceBlock == null) {
					mode = UIMode.View;
					DropFocus();
					break;
				}
				GUI.Label(rr, "Block (" + chosenSurfaceBlock.pos.x.ToString() + " , " + chosenSurfaceBlock.pos.y + " , " + chosenSurfaceBlock.pos.z + ')');
				rr.y += 2 * rr.height;
				switch (argument) {
				case 1:
					//BUILDING BUTTON
					if (GUI.Button(rr, Localization.ui_build)) ChangeArgument(3); // list of available buildings
					rr.y += rr.height;
					//GATHER BUTTON
					GatherSite gs = chosenSurfaceBlock.GetComponent<GatherSite>();
					if (gs == null) {
						if (GUI.Button(rr, Localization.ui_toGather)) { 
							gs = chosenSurfaceBlock.gameObject.AddComponent<GatherSite>();
							gs.Set(chosenSurfaceBlock);
						}
					}
					else {
						if (GUI.Button(rr, Localization.ui_cancelGathering)) Destroy(gs);
					} 
					rr.y += rr.height;
					//DIG BUTTON
					if (chosenSurfaceBlock.indestructible == false && chosenCubeBlock != null) {
						CleanSite cs = chosenSurfaceBlock.GetComponent<CleanSite>();
						if (cs == null) {
							if (GUI.Button(rr, Localization.ui_dig_block)) {
								if (chosenSurfaceBlock.artificialStructures > 0) ChangeArgument(2);
								else {
									cs = chosenSurfaceBlock.gameObject.AddComponent<CleanSite>();
									cs.Set(chosenSurfaceBlock, true);
								}
							}
						}
					}
					rr.y += rr.height;

					//BLOCK BUILDING BUTTON
					if (GameMaster.colonyController.hq.level > 3) {
						if (GUI.Button(rr, Localization.ui_build +' '+ Localization.block)) ChangeArgument(6); // select material inside
						rr.y += rr.height;
					}
					//COLUMN  BUILDING
					if (GameMaster.colonyController.hq.level > 4 && chosenSurfaceBlock.pos.y < Chunk.CHUNK_SIZE - 1)
					{
						ResourceContainer[] rcc = ResourcesCost.GetCost(Structure.COLUMN_ID);
						if (GUI.Button(rr, Localization.ui_build +' '+ Localization.structureName[Structure.COLUMN_ID])) {
							if (GameMaster.colonyController.storage.CheckBuildPossibilityAndCollectIfPossible( rcc)) {
								float supportPoints = chosenSurfaceBlock.myChunk.CalculateSupportPoints(chosenSurfaceBlock.pos.x, chosenSurfaceBlock.pos.y , chosenSurfaceBlock.pos.z);
								if (supportPoints <= 1) {
									Structure s = Structure.GetNewStructure(Structure.COLUMN_ID);
									s.SetBasement(chosenSurfaceBlock, new PixelPosByte(7,7));
									ChunkPos cpos = chosenSurfaceBlock.pos;
									cpos = new ChunkPos(cpos.x, cpos.y + 1, cpos.z);
								}
								else {
									chosenSurfaceBlock.myChunk.ReplaceBlock(chosenSurfaceBlock.pos, BlockType.Cave, chosenSurfaceBlock.material_id, ResourceType.CONCRETE_ID, false);
								}
								}
								else { systemInfoString = "Not enough resources"; systemInfoTimer = 2;}
						}
						rr.y += rr.height;
						GUI.Box(new Rect(rr.x ,rr.y, rr.width, rr.height * rcc.Length), GUIContent.none);
						foreach (ResourceContainer rcf in rcc) {
							float wx = rr.width * 0.875f; if (wx > rr.height) wx = rr.height;
							if (rcf.volume > GameMaster.colonyController.storage.standartResources[rcf.type.ID] ) GUI.color = Color.red;
							GUI.Label(new Rect(rr.x, rr.y, rr.width * 0.75f, rr.height), rcf.type.name);
							GUI.DrawTexture( new Rect (rr.x + rr.width * 0.75f, rr.y, wx, wx), rcf.type.icon, ScaleMode.ScaleToFit );
							GUI.Label( new Rect(rr.x + rr.width * 0.875f, rr.y, wx,wx), rcf.volume.ToString());
							GUI.color = Color.white;
							rr.y += rr.height;
						}
					}
					// CHANGING MATERIAL
					if (GameMaster.colonyController.gears_coefficient >=2 ) 
					{
						if (GUI.Button(rr, Localization.ui_changeMaterial)) {
							ChangeArgument(8);
						}
						rr.y += rr.height;
					}
					break;

				case 2: // подтверждение на снос при очистке
					GUI.Label(acceptBox, Localization.ui_accept_destruction_on_clearing);
					Rect r2 = new Rect(acceptBox.x, acceptBox.y + acceptBox.height / 2f,acceptBox.width/2f, acceptBox.height/2f);
					if (GUI.Button(r2, Localization.ui_accept)) {
						CleanSite cs2 =  chosenSurfaceBlock.gameObject.AddComponent<CleanSite>();
						cs2.Set(chosenSurfaceBlock, true);
						ChangeArgument(1);
					}
					r2.x += r2.width;
					if (GUI.Button(r2 , Localization.ui_decline)) ChangeArgument(1);
					break;
				case 3: // выбрать доступные для строительства домики
					if (GUI.Button(rr, Localization.menu_cancel)) {
						ChangeArgument(1);			
					}
					rr.y += rr.height;
					//level button

					if (showingBuildingsLevel != 0)	GUI.DrawTexture(new Rect(rr.x + (showingBuildingsLevel - 1) * rr.height, rr.y, rr.height, rr.height), PoolMaster.orangeSquare_tx, ScaleMode.StretchToFill);
					if (GameMaster.colonyController.hq != null) {
						for (byte bl = 1; bl <= GameMaster.colonyController.hq.level; bl++) {
							if (GUI.Button(new Rect(rr.x + (bl - 1) * rr.height, rr.y, rr.height, rr.height), bl.ToString())) {
								showingBuildingsLevel = (byte)bl;
								showingBuildingsList = Structure.GetApplicableBuildingsList(bl, chosenSurfaceBlock);
								ChangeArgument(3);
							}
						}
					}
					rr.y += rr.height;
					int buildingIndex = 0;
					if ( showingBuildingsList != null && showingBuildingsList.Count != 0 ) {
						bool useScroll = (buildingsListLength > Screen.height - rr.y);
						if (useScroll) GUI.BeginScrollView(new Rect(rr.x, rr.y, Screen.width - rr.x, Screen.height - rr.y), buildingsScrollViewPos, new Rect(rr.x, rr.y, Screen.width - rr.x, buildingsListLength));
						buildingsListLength = showingBuildingsList.Count * rr.height;
						foreach (Building bd in showingBuildingsList) {
							if (bd.requiredBasementMaterialId == -1 || bd.requiredBasementMaterialId == chosenSurfaceBlock.material_id) {
								if (GUI.Button(rr, Localization.structureName[bd.id])) {
									if (chosenStructure != bd ) {
										chosenStructure = bd; 
										chosenBuildingIndex = buildingIndex;
										showBuildingCreateInfo = true;
										bufferedResources = ResourcesCost.GetCost(bd.id);
										if (chosenStructure.type != StructureType.MainStructure ) argument = 4; else argument = 3;
										break;
									}
									else { // уже выбрано
										chosenStructure = null;
										showBuildingCreateInfo = false;
										break;
									}
								}
							}
							else {
								Color c = GUI.color; GUI.color = Color.grey;
								GUI.Label(rr, Localization.structureName[bd.id] + " (" + Localization.material_required + ResourceType.resourceTypesArray[bd.requiredBasementMaterialId].name+ " )", PoolMaster.GUIStyle_CenterOrientedLabel);
								GUI.color = c;
							}
						rr.y += rr.height;
						// отрисовка информации о выбранном здании
						if (chosenBuildingIndex == buildingIndex && showBuildingCreateInfo)
						{
							Building chosenBuilding = chosenStructure as Building ;
							GUI.DrawTexture(new Rect(rr.x, rr.y, k, rr.height) , energyLightning_icon_tx, ScaleMode.ScaleToFit);
							float ens =  chosenBuilding.energySurplus;
							string enSurplusString = ens.ToString();
							if (ens > 0) enSurplusString = '+' + enSurplusString;
							GUI.Label( new Rect(rr.x + k, rr.y, rr.width/2f - k, rr.height) , enSurplusString);
							GUI.DrawTexture( new Rect(rr.x + rr.width/2f, rr.y, k, rr.height ) , buildingQuad_icon_tx, ScaleMode.StretchToFill);
							GUI.Label( new Rect(rr.x + rr.width/2f + 2 *k, rr.y, rr.width/2f, rr.height) , chosenStructure.innerPosition.x_size.ToString() + " x " + chosenStructure.innerPosition.z_size.ToString());
							rr.y += rr.height;
								Storage storage = GameMaster.colonyController.storage;
							foreach (ResourceContainer rc in bufferedResources) {
								float wx = rr.width * 0.875f; if (wx > rr.height) wx = rr.height;
								if (rc.volume > storage.standartResources[rc.type.ID] ) GUI.color = Color.red;
								GUI.Label(new Rect(rr.x, rr.y, rr.width * 0.75f, rr.height), rc.type.name);
								GUI.DrawTexture( new Rect (rr.x + rr.width * 0.75f, rr.y, wx, wx), rc.type.icon, ScaleMode.ScaleToFit );
								GUI.Label( new Rect(rr.x + rr.width * 0.875f, rr.y, wx,wx), rc.volume.ToString());
								GUI.color = Color.white;
								rr.y += rr.height;
								buildingsListLength += rr.height;
							}
							//для крупных структур:
							if (chosenStructure.type == StructureType.MainStructure) {
									bool pass = true;
									if (chosenBuilding.borderOnlyConstruction) { 
										if ( !chosenSurfaceBlockIsBorderBlock ) {
											Color c = GUI.color;
											GUI.color = Color.yellow;
											GUI.Label(rr, Localization.ui_buildOnSideOnly, PoolMaster.GUIStyle_CenterOrientedLabel);
											rr.y+=rr.height;
											GUI.color = c;
											pass = false;
										}
										else {
												int side = 0;
												if ( chosenSurfaceBlock.pos.x == 0 ) {
													if (chosenSurfaceBlock.pos.z == 0) side = 2;
												}
												else {
													if (chosenSurfaceBlock.pos.x == Chunk.CHUNK_SIZE - 1) side = 1;
													else side = 3;
												}
												if (chosenSurfaceBlock.myChunk.sideBlockingMap[chosenSurfaceBlock.pos.y, side] == true) {
												Color c = GUI.color;
												GUI.color = Color.red;
												GUI.Label(rr, Localization.ui_heightBlocked, PoolMaster.GUIStyle_CenterOrientedLabel);
												rr.y+=rr.height;
												GUI.color = c;
												pass = false;
											}
										}
									}
									if (pass) {
										if (GUI.Button ( new Rect(rr.x + rr.width/4f, rr.y, rr.width / 2f, rr.height), Localization.ui_build )) {
											PixelPosByte mainStructurePosition = new PixelPosByte(SurfaceBlock.INNER_RESOLUTION / 2 - chosenStructure.innerPosition.x_size/2, SurfaceBlock.INNER_RESOLUTION / 2  -  chosenStructure.innerPosition.z_size/2);
												if (GameMaster.colonyController.storage.CheckBuildPossibilityAndCollectIfPossible( bufferedResources )) {
												if ( chosenSurfaceBlock.IsAnyBuildingInArea( new SurfaceRect (mainStructurePosition.x, mainStructurePosition.y, chosenStructure.innerPosition.x_size, chosenStructure.innerPosition.z_size) ) == false) {
														Structure s = Structure.GetNewStructure(chosenStructure.id);
														s.gameObject.SetActive(true);
														s.SetBasement(chosenSurfaceBlock, mainStructurePosition);
														ChangeArgument(3);
														break;
												}
												else {
													bufferedPosition = mainStructurePosition; 
													ChangeArgument(5);
													break;
												}
											}
											else {
												systemInfoString = "Not enough resources"; systemInfoTimer = 2;
											}
										}
										rr.y += rr.height;
								}								
							}
						}
						buildingIndex++;
					}
						if (useScroll) GUI.EndScrollView();
				}
					// сетка для размещения постройки
					if (argument != 4) GUI.DrawTexture(buildingGridRect, grid16_tx, ScaleMode.StretchToFill); // чтобы не дублировалось
					if (chosenSurfaceBlock.cellsStatus != 0 ) {
						float p = buildingGridRect.width / SurfaceBlock.INNER_RESOLUTION;
						int n = 0;
						while ( n < chosenSurfaceBlock.surfaceObjects.Count) {
							if (chosenSurfaceBlock.surfaceObjects[n] == null) {chosenSurfaceBlock.RequestAnnihilationAtIndex(n); n++; continue;}
							Texture t = PoolMaster.quadSelector_tx;
							switch (chosenSurfaceBlock.surfaceObjects[n].type) {
							case StructureType.HarvestableResources: t = PoolMaster.orangeSquare_tx;break;
							case StructureType.MainStructure: t = whiteSpecial_tx;break;
							case StructureType.Plant: t = greenSquare_tx;break;
							case StructureType.Structure : t = whiteSquare_tx;break;
							}
							SurfaceRect sr= chosenSurfaceBlock.surfaceObjects[n].innerPosition;
							GUI.DrawTexture(new Rect(buildingGridRect.x + sr.x * p, buildingGridRect.y + buildingGridRect.height -  (sr.z+ sr.z_size) * p , p * sr.x_size, p * sr.z_size), t, ScaleMode.StretchToFill);
							n++;
						}
					}	
					break;
				case 4: // размещение на сетке
					{
						if (chosenStructure == null) {
							ChangeArgument(3);
							break;
						}
					GUI.DrawTexture(buildingGridRect, grid16_tx, ScaleMode.StretchToFill); 
					float p = buildingGridRect.width / SurfaceBlock.INNER_RESOLUTION;
					SurfaceRect surpos = chosenStructure.innerPosition;
					for (byte i =0; i < chosenSurfaceBlock.map.GetLength(0); i++) {
						for (byte j = 0; j < chosenSurfaceBlock.map.GetLength(1); j++) {
							if ( i <= SurfaceBlock.INNER_RESOLUTION - chosenStructure.innerPosition.x_size && j <= SurfaceBlock.INNER_RESOLUTION - chosenStructure.innerPosition.z_size) {
								if (GUI.Button(new Rect(buildingGridRect.x + i * p, buildingGridRect.y + buildingGridRect.height - (j+1) *p, p, p), yellowSquare_tx, GameMaster.mainGUISkin.customStyles[1])) {		
									if (GameMaster.colonyController.storage.CheckBuildPossibilityAndCollectIfPossible( bufferedResources )) {
										surpos.x = i; surpos.z = j;
										if ( chosenSurfaceBlock.IsAnyBuildingInArea(surpos) == false) {
												Structure s = Structure.GetNewStructure(chosenStructure.id);
												s.gameObject.SetActive(true);
												s.SetBasement(chosenSurfaceBlock, new PixelPosByte(i,j));
										}
										else {
											bufferedResources = ResourcesCost.GetCost(chosenStructure.id);
											bufferedPosition = new PixelPosByte(surpos.x, surpos.z); 
											ChangeArgument(5);
											break;
										}
									}
									else { systemInfoString = "Not enough resources"; systemInfoTimer = 2;}
								}
							}
						}
					}
					goto case 3;
					}
				case 5: // подтверждение на снос при строительстве
					GUI.Box(acceptBox, Localization.ui_accept_destruction_on_clearing);
					if (GUI.Button (new Rect(acceptBox.x, acceptBox.y + acceptBox.height/2f, acceptBox.width/2f, acceptBox.height/2f), Localization.ui_accept)) {
						if ( bufferedPosition != PixelPosByte.Empty ) {
							Structure s = Structure.GetNewStructure(chosenStructure.id);
							s.SetBasement(chosenSurfaceBlock, bufferedPosition);
							ChangeArgument(3);
							bufferedResources = null;
						}
					} 
					if (GUI.Button (new Rect(acceptBox.x + acceptBox.width/2f, acceptBox.y + acceptBox.height/2f, acceptBox.width/2f, acceptBox.height/2f), Localization.ui_decline)) {
						bufferedPosition = PixelPosByte.Empty;
						if (bufferedResources != null) {
							GameMaster.colonyController.storage.AddResources( bufferedResources );
							bufferedResources = null;
						}
						ChangeArgument(3);
					}
					break;
				case 6:  // CREATING BLOCKS
					if (GUI.Button(rr, Localization.menu_cancel)) {
						ChangeArgument(1);			
					}
					rr.y += 2 * rr.height;
					ResourceType[] blockMaterials = new ResourceType[] {
						ResourceType.Concrete, ResourceType.Dirt, ResourceType.Lumber, ResourceType.metal_E, ResourceType.metal_K, ResourceType.metal_M,
						ResourceType.metal_N, ResourceType.metal_P, ResourceType.metal_S, ResourceType.mineral_F, ResourceType.mineral_L, ResourceType.Plastics,
						ResourceType.Stone
					};
					{
						foreach (ResourceType rt in blockMaterials) {
							GUI.DrawTexture(new Rect(rr.x, rr.y, rr.height, rr.height), rt.icon, ScaleMode.StretchToFill);
							if (GUI.Button(new Rect(rr.x + rr.height, rr.y, rr.width - rr.height, rr.height), rt.name)) {
								BlockBuildingSite bbs = chosenSurfaceBlock.gameObject.GetComponent<BlockBuildingSite>();
								if (bbs == null) bbs = chosenSurfaceBlock.gameObject.AddComponent<BlockBuildingSite>();
								bbs.Set(chosenSurfaceBlock, rt);
								ChangeArgument(1);
							}
							rr.y += rr.height;
						}
					}
					break;
				case 7: //CREATING   COLUMNS
					
					break;
				case 8: // CHANGING MATERIAL
					if (chosenSurfaceBlock == null) {
						ChangeArgument(1);
						return;
					}
					GUI.Label(rr, Localization.material_required + " 256"); rr.y += rr.height;
					foreach (ResourceType rt in ResourceType.materialsForCovering) {
						GUI.DrawTexture( new Rect(rr.x, rr.y, rr.height, rr.height), rt.icon, ScaleMode.StretchToFill );
						if ( GUI.Button (new Rect(rr.x + rr.height, rr.y, rr.width -rr.height, rr.height), rt.name ) ) {
							if ( GameMaster.colonyController.storage.CheckBuildPossibilityAndCollectIfPossible(new ResourceContainer[]{new ResourceContainer(rt,256)}) )
							{
								chosenSurfaceBlock.ReplaceMaterial(rt.ID);
							}
						}
						rr.y += rr.height;
					}
					break;
				}
				break;
				#endregion
			case UIMode.CubeBlockPanel:
				if (chosenCubeBlock == null) {
					mode = UIMode.View;
					DropFocus();
					break;
				}
				#region cubeBlockPanel
				if (faceIndex != 10) { 
							if (GUI.Button(rr, Localization.ui_dig_block)) {
								TunnelBuildingSite tbs = chosenCubeBlock.GetComponent<TunnelBuildingSite>();
								if (tbs == null) {
									tbs = chosenCubeBlock.gameObject.AddComponent<TunnelBuildingSite>();
									tbs.Set(chosenCubeBlock);
								}
								tbs.CreateSign(faceIndex);
								chosenWorksite = tbs;
								mode = UIMode.WorksitePanel;
								ChangeArgument(1);
								return;
						}
				}

				#endregion
				break;
			case UIMode.WorksitePanel:
				if (chosenWorksite == null) {
					mode = UIMode.View;
					DropFocus();
					break;
				}
				// on worksite.cs.OnGUI
				chosenWorksite.gui_ypos = rr.y;
				break;
			case UIMode.StructurePanel:
				if (chosenStructure == null) {
					mode = UIMode.View;
					DropFocus();
					break;
				}
				else {
					if ( !chosenStructure.undestructible ) {
						if (GUI.Button(new Rect(rr.xMax - rr.height, rr.y, rr.height, rr.height), demolishButton_tx)) {
							mode = UIMode.View;
							chosenStructure.Annihilate(false);
							DropFocus();
							return;
						}
					}
				}
				rr.y += rr.height;
				Building b = chosenStructure as Building;
				if ( b != null) {
					rr.y += rr.height;
					GUI.Label(rr,  Localization.structureName[chosenStructure.id]); 
					rr.y += rr.height;

					if (b.connectedToPowerGrid) {
						bool act = GUI.Toggle(new Rect(rr.x, rr.y, rr.width / 2f, rr.height), b.isActive, Localization.ui_activeSelf); 
						rr.y += rr.height;
						if (act != b.isActive) b.SetActivationStatus(act);
						GUI.DrawTexture( new Rect (rr.x, rr.y, rr.height, rr.height), energyLightning_icon_tx, ScaleMode.StretchToFill);
						if (b.isActive && b.energySupplied) {
							string surplusString = b.energySurplus.ToString();
							if (b.energySurplus > 0) surplusString = '+' + surplusString;
							GUI.Label( new Rect(rr.x + rr.height, rr.y, rr.width/2f - rr.height, rr.height),  surplusString );
						}
						else GUI.Label( new Rect(rr.x  + rr.height, rr.y, rr.width/2f - rr.height, rr.height),  "offline" );
					}
					if ( b as House != null) {
						GUI.DrawTexture( new Rect (rr.x  + rr.width/2f  , rr.y, rr.height, rr.height), citizen_icon_tx, ScaleMode.StretchToFill);
						GUI.Label( new Rect(rr.x  + rr.width/2f + rr.height, rr.y, rr.width/2f - rr.height, rr.height),  (b as House).housing.ToString() );
					}
					rr.y += rr.height;

					WorkBuilding wb = b as WorkBuilding;
					if (wb != null) {
						float p = rr.height;
						GUI.Label (new Rect(rr.x , rr.y, p, p), "0" );
						GUI.Label ( new Rect (rr.xMax - p, rr.y, p, p), wb.maxWorkers.ToString(), PoolMaster.GUIStyle_RightOrientedLabel);
						int wcount = (int)GUI.HorizontalSlider(new Rect(rr.x +  p, rr.y, rr.width - 2 * p, p), wb.workersCount, 0, wb.maxWorkers);
						if (wcount != wb.workersCount) {
							if (wcount > wb.workersCount) GameMaster.colonyController.SendWorkers(wcount - wb.workersCount, wb, WorkersDestination.ForWorkBuilding);
							else wb.FreeWorkers(wb.workersCount - wcount);
						}
						rr.y += p;
						p *= 1.5f;
						if ( wb.workersCount > 0 && GUI.Button (new Rect( rr.x, rr.y, p, p ), PoolMaster.minusX10Button_tx)) { wb.FreeWorkers();}
						if ( wb.workersCount > 0 && GUI.Button (new Rect( rr.x + p, rr.y, p, p ), PoolMaster.minusButton_tx)) { wb.FreeWorkers(1);}
						GUI.Label ( new Rect (rr.x + 2 *p, rr.y, rr.width - 4 * p, p), wb.workersCount.ToString(), PoolMaster.GUIStyle_CenterOrientedLabel );
						if ( wb.workersCount != wb.maxWorkers && GUI.Button (new Rect( rr.xMax - 2 *p, rr.y, p, p ), PoolMaster.plusButton_tx) ) { GameMaster.colonyController.SendWorkers(1,wb, WorkersDestination.ForWorkBuilding);}
						if ( wb.workersCount != wb.maxWorkers &&GUI.Button (new Rect( rr.xMax - p, rr.y, p, p ), PoolMaster.plusX10Button_tx)) { GameMaster.colonyController.SendWorkers(wb.maxWorkers - wb.workersCount,wb, WorkersDestination.ForWorkBuilding);}
						rr.y += p;
						GUI.Label ( new Rect (rr.x , rr.y, rr.width , rr.height), string.Format("{0:0.##}",wb.workSpeed) + ' ' + Localization.ui_points_sec, PoolMaster.GUIStyle_CenterOrientedLabel );
						rr.y += rr.height;
					}
				}
				chosenStructure.gui_ypos = rr.y;
				break;
			case UIMode.GameMenu:
				if (GUI.Button(rr, Localization.menu_save)) {
					GameMaster.realMaster.SaveGame("newsave");
					GameMaster.realMaster.AddAnnouncement(Localization.GetGameMessage(GameMessage.GameSaved));
				}
				rr.y += rr.height;
				if (GUI.Button(rr, Localization.menu_load)) {
					if (GameMaster.realMaster.LoadGame("newsave"))	GameMaster.realMaster.AddAnnouncement(Localization.GetGameMessage(GameMessage.GameLoaded));
					else GameMaster.realMaster.AddAnnouncement(Localization.GetGameMessage(GameMessage.LoadingFailed));
				}
				break;
			}
	}
		if (systemInfoString != null) GUI.Label(systemInfoRect, systemInfoString, PoolMaster.GUIStyle_SystemAlert);
	}

	public void AddFactoryToList (Factory f) {
			switch (f.specialization) {
			case FactorySpecialization.Unspecialized:
				if ( !hasUnspecializedFactories ) unspecializedFactories =new List<Factory>();
				hasUnspecializedFactories = true;
				unspecializedFactories.Add(f);
				break;
			case FactorySpecialization.Smeltery:
				if ( !hasSmelteries ) smelteriesList =new List<Factory>();
				hasSmelteries= true;
				smelteriesList.Add(f);
				break;
			}
	}

	public void RemoveFromFactoriesList (Factory f) {
			switch ( f.specialization ) {
			case FactorySpecialization.Smeltery:
				RemoveObjectFromFactoryList( smelteriesList, f, ref hasSmelteries);
				break;
			case FactorySpecialization.Unspecialized:
				RemoveObjectFromFactoryList( unspecializedFactories, f, ref hasUnspecializedFactories);
				break;
			}
	}

	void RemoveObjectFromFactoryList(List<Factory> list, Factory obj, ref bool presenceMarker) {
		if (list == null) {presenceMarker = false; return;}
		int i = 0;
		while ( i < list.Count ) {
			if (list[i] == null || list[i] == obj) list.RemoveAt(i);
			i++;
		}
		if (list.Count == 0) {
			presenceMarker = false;
		}
	}

	public void ShowCrewCard(Crew c) {
		if (c == null) return;
		showingCrew = c;
		touchscreenTemporarilyBlocked = true;
		showCrewsOrVesselsList = false;
	}

	public void HideCrewCard() {
		if (showingCrew == null) return;
		showingCrew = null;
		touchscreenTemporarilyBlocked = false;
		showCrewsOrVesselsList =false;
	}
}
