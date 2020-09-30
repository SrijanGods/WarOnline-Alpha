using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TurretChange : MonoBehaviour
{
    public int selection;
    public GameObject[] turrets;
    public GameObject turret;

    void Start()
    {
        StartCoroutine(UpdateTurret());
    }

    IEnumerator UpdateTurret()
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => GlobalValues.turret != null);
        DisableAll();
        // int selection = Array.FindIndex(turrets, g => g.name == GlobalValues.turret);
        // turrets[selection].SetActive(true);
        turret = Array.Find(turrets, g => g.name == GlobalValues.turret);
        turret.SetActive(true);
    }
    void DisableAll()
    {
        for (int x = 0; x < turrets.Length; x++)
        {
            turrets[x].SetActive(false);
        }

    }
    private void Update()
    {
        StartCoroutine(UpdateTurret());
    }
}
