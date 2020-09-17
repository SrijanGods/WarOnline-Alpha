using UnityEngine;

public class GlobalValues : MonoBehaviour
{
    public static GlobalValues Instance;

    public static string turret = "FlameThrower", hull = "Dominator";

    [HideInInspector] public bool loggedIn;

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

        turret = PlayerPrefs.GetString("CurrentTurret", "FlameThrower");

        hull = PlayerPrefs.GetString("CurrentHull", "Dominator");
    }
}