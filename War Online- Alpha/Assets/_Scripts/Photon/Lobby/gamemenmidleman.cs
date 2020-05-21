using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamemenmidleman : MonoBehaviour
{
    public GameObject toreplacefrom;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        toreplacefrom.GetComponent<gameManager>().newPlayerPrefab =  dontdestroy.instance.prefab;
    }
}
