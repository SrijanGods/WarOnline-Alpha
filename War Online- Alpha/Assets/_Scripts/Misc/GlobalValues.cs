using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValues : MonoBehaviour
{
    public static GlobalValues Instance;

    public string turret;
    public string hull;

    [HideInInspector]
    public bool loggedIn;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("CurrentTurret"))
        {
            turret = PlayerPrefs.GetString("CurrentTurret");
        }
        else
        {
            PlayerPrefs.SetString("CurrentTurret", "FlameThrower");
            turret = "FlameThrower";
        }

        if (PlayerPrefs.HasKey("CurrentHull"))
        {
            hull = PlayerPrefs.GetString("CurrentHull");
        }
        else
        {
            PlayerPrefs.SetString("CurrentHull", "Dominator");
            hull = "Dominator";
        }
    }
}
