using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretChange : MonoBehaviour
{
    public int selection;
    public GameObject[] turrets;
    private int lasttimenumber;
    // Use this for initialization

    private void Awake()
    {
        
    }

    void Start()
    {
        disableAll();
        turrets[selection].SetActive(true);
        lasttimenumber = selection;
    }
    void disableAll()
    {
        for (int x = 0; x < turrets.Length; x++)
        {
            turrets[x].SetActive(false);
        }

    }
    private void Update()
    {

        if (selection != lasttimenumber)
        {
            disableAll();
            turrets[selection].SetActive(true);
            lasttimenumber = selection;
        }


    }
}
