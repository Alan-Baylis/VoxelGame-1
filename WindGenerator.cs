﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindGenerator : Building {
	public Transform head, screw;
	Vector3 windDirection;
	bool rotateHead = false, rotateScrew = true;
	const float HEAD_ROTATE_SPEED = 10, SCREW_ROTATE_SPEED = 20;
	float maxSurplus;

	void Awake() {
		PrepareBuilding();
		maxSurplus = energySurplus;
	}

	override public void SetBasement(SurfaceBlock b, PixelPosByte pos) {
		if (b == null) return;
		SetBuildingData(b, pos);
		GameMaster.realMaster.windUpdateList.Add(this);
		WindUpdate(GameMaster.realMaster.windVector);
	}

	void Update() {
		if (GameMaster.gameSpeed == 0) return;
		float t = Time.deltaTime * GameMaster.gameSpeed;
		if ( rotateHead ) {
			head.transform.forward = Vector3.MoveTowards(head.transform.forward, windDirection, HEAD_ROTATE_SPEED * t);
			if (head.transform.forward == windDirection.normalized) rotateHead = false;
		}
		if (rotateScrew) {
			screw.transform.Rotate( Vector3.forward * windDirection.magnitude * SCREW_ROTATE_SPEED * t);
		}
	}

	public void WindUpdate( Vector3 direction) {
		windDirection = direction;
		if (windDirection.magnitude == 0) {
			if ( rotateScrew ) {
				rotateScrew = false;
				energySurplus = 0; 
				if (connectedToPowerGrid) GameMaster.colonyController.RecalculatePowerGrid();
				}
			rotateHead = false;
		}
		else {
			if (rotateScrew == false) {
				rotateScrew = true; 
				energySurplus = maxSurplus * direction.magnitude / GameMaster.realMaster.maxWindPower;
				if (connectedToPowerGrid) GameMaster.colonyController.RecalculatePowerGrid();
			}
			if (transform.forward != windDirection) rotateHead = true; else rotateHead = false;
		}
	}
}
