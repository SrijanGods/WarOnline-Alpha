using _Scripts.Photon.Game;
using UnityEngine;

public class GlobalValues : MonoBehaviour
{
    public static GlobalValues Instance;

    public static string turret = "FlameThrower", hull = "Dominator";

    public static GameSessionType Session;

    public static GameMode GameMode;

    public static string[] TeamNames = {"Red", "Blue"};

    public static Color[] TeamColors =
            {Color.red, Color.blue},
        FfaColors = {Color.black, Color.blue, Color.green, Color.magenta, Color.red, Color.yellow};

    public static string PlayerPrefab
    {
        get
        {
            var x = hull;

            switch (turret)
            {
                case "Acidton":
                    x += "AT";
                    break;
                case "Blaster":
                    x += "BL";
                    break;
                case "Duos":
                    x += "DS";
                    break;
                case "FlameThrower":
                    x += "FT";
                    break;
                case "MachineGun":
                    x += "MG";
                    break;
                case "MissileLauncher":
                    x += "ML";
                    break;
                case "Sniper":
                    x += "SP";
                    break;
                case "WindChill":
                    x += "WC";
                    break;
                default:
                    x += "DS";
                    break;
            }

            return x;
        }
    }

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