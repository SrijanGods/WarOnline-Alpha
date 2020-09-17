using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HullChange : MonoBehaviour {
    public int selection;
    public GameObject[] hulls;

    void Start ()
    {
        UpdateHull();
    }

    IEnumerator UpdateHull()
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => GlobalValues.hull != null);
        DisableAll();
        // int selection = Array.FindIndex(hulls, g => g.name == GlobalValues.hull);
        // hulls[selection].SetActive(true);
        Array.Find(hulls, g => g.name == GlobalValues.hull).SetActive(true);
    }

    private void Update()
    {
        StartCoroutine(UpdateHull());
    }

    void DisableAll()
    {
        for(int x = 0; x<hulls.Length; x++)
        {
            hulls[x].SetActive(false);
        }
      
    }

}
