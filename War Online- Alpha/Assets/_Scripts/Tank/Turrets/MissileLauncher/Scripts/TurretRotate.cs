using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRotate : MonoBehaviour {
	public float speed = 20f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.Z)) {
			this.transform.Rotate (0.0f, speed*Time.deltaTime, 0.0f);
		}
		if (Input.GetKey (KeyCode.X)) {
			this.transform.Rotate (0.0f, -speed*Time.deltaTime, 0.0f);
		}
	}
}
