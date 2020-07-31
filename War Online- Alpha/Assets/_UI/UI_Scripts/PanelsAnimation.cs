using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelsAnimation : MonoBehaviour
{

    public void DisplayTrue()
    {
        GetComponent<Animator>().SetBool("display", true);
    }
    public void DisplayFalse()
    {
        GetComponent<Animator>().SetBool("display", false);
    }



}
