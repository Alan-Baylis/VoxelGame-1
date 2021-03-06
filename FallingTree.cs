﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTree : MonoBehaviour {
	Quaternion fallRotation;
    public delegate void ReturnFunction(GameObject g);
    public ReturnFunction returnFunction;
	float timer = 10;
	void Awake() {
		fallRotation = Quaternion.LookRotation(Vector3.up, new Vector3(Random.value, 0, Random.value));
	}
	// Update is called once per frame
	void Update () {
		if (transform.rotation != fallRotation) transform.rotation = Quaternion.RotateTowards(transform.rotation, fallRotation, 35 * Time.deltaTime * GameMaster.gameSpeed);
		else {
			transform.Translate(Vector3.down *0.01f* Time.deltaTime * GameMaster.gameSpeed, Space.World);
		}
		timer -= Time.deltaTime * GameMaster.gameSpeed;
		if (timer <= 0) {			
            returnFunction(gameObject);
            Destroy(this);
        }
	}
}