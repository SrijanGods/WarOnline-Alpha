using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour {

    public void LoadLeveli(string level)
    {
        Application.LoadLevel(level);
    }
}
