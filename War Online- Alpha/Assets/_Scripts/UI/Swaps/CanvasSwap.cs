using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSwap : MonoBehaviour {

    public GameObject[] NewGO;
    public GameObject[] OldGO;
    public Button btn;
    public bool isNormal = true;
    // Use this for initialization
    void Start () {
        Button btnclick = btn;
        if (isNormal)
        {
            btnclick.onClick.AddListener(TaskOnClick);
        }
	}

    void TaskOnClick()
    {
        foreach(GameObject go in NewGO)
        {
            go.SetActive(true);
        }

        foreach(GameObject go in OldGO)
        {
            go.SetActive(false);
        }
    }

    void OnEnable()
    {
        if(GameObject.FindWithTag("HullTurretSet") != null)
        {
            Destroy(GameObject.FindWithTag("HullTurretSet"));
        }
    }
  
    
}
