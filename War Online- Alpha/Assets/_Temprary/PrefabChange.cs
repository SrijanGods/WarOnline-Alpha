using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabChange : MonoBehaviour {

    public Dropdown dropdown;
    public string prefabToSpawn;
    

	void Start () {


   gameManager connectionScript;
    GameObject gameMan = GameObject.Find("GameManager");
        connectionScript = gameMan.GetComponent<gameManager>();
        prefabToSpawn = dontdestroy.instance.prefab;
       
	}
	
}
