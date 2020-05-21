using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HullChange : MonoBehaviour {
    public int selection;
    public GameObject[] hulls;
    private int lasttimenumber;
    // Use this for initialization

    private void Awake()
    {
    }

    void Start ()
    {
            //Debug.Log(SceneManager.GetActiveScene().name);
            disableAll();
            hulls[selection].SetActive(true);
            lasttimenumber = selection;
    }
    private void Update()
    {
        if (selection != lasttimenumber)
        {
            disableAll();
            hulls[selection].SetActive(true);
            lasttimenumber = selection;
        }
        else
        {

        }
    }
    void disableAll()
    {
        for(int x = 0; x<hulls.Length; x++)
        {
            hulls[x].SetActive(false);
        }
      
    }

}
