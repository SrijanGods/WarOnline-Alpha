using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateMissiles : MonoBehaviour {
	public Transform[] SpawnPoints = new Transform[4];
	public GameObject OBJ;
	public GameObject[] Missiles = new GameObject[4];
	public bool Ready = false;
	// Use this for initialization
	void Start () {
		for (int i = 0; i <= 3; i++) {
			Missiles[i] = (GameObject)Instantiate (OBJ, SpawnPoints [i].transform.position, SpawnPoints [i].transform.rotation);
		}
		Ready = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
