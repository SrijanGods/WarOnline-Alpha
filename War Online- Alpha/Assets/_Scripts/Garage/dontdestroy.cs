using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dontdestroy : MonoBehaviour
{
    public GameObject getfrom;
    public string[] hulls;
    public string[] turrets;
    public string prefab = "";
    public static dontdestroy instance;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            DontDestroyOnLoad(gameObject);
        }
        if (SceneManager.GetActiveScene().name != "Lobby")
        {
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            prefab = hulls[getfrom.GetComponent<HullChange>().selection];
            prefab = turrets[getfrom.GetComponent<TurretChange>().selection];
        }
    }
}
