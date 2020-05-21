using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationQuit : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKey("delete"))
        {
            Application.Quit();
        }
    }
    public void OnClick_Quit()
    {
        Application.Quit();
    }
}