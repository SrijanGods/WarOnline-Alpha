using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    public GameObject canvasToReset;
    public int childNumber;

    private void OnEnable()
    {
        GameObject c1 = canvasToReset.transform.GetChild(0).gameObject;
        GameObject c2 = canvasToReset.transform.GetChild(1).gameObject;
        GameObject c3 = canvasToReset.transform.GetChild(2).gameObject;

        c1.SetActive(true);
        c2.SetActive(false);
        c3.SetActive(false);
    }
}
