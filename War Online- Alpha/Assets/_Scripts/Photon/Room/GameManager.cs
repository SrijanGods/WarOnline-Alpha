using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviourPun {

    /*
    [Tooltip("To be honest, this shouldn't be changed at all. But if you do feel like a badass, this controls which scene the player exits to when quitting.")]
    [SerializeField] string exitScene = "StartScene";
    [Tooltip("The prefab to use for representing the player")]
    public string newPlayerPrefab;

    private GameObject Canvas;
    public Transform[] spawnPoints;
    private GameObject playerPrefab;
    private PrefabChange prefabChange;

    private bool firstTimeCalled;
    private bool firstTimeTankCalled;
  

    // Why are we using a static gameManager instance? The tutorial says nothing about this.
    // "It's a surprise tool that will help us later." -- Mickey Mouse
    public static gameManager _instance;
    private void Awake()
    {
        /*if (_instance != null) Destroy(gameObject);
        else if (_instance == null) _instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        firstTimeCalled = false;
        firstTimeTankCalled = false;

        Canvas = Resources.Load<GameObject>("WarCanvas");
        newPlayerPrefab = dontdestroy.instance.prefab;
        if (newPlayerPrefab == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            if (RTC_TankController.LocalPlayerInstance == null && !firstTimeTankCalled)
            {
                SpawnTank();
                firstTimeTankCalled = true;
            }
            else
            {
                Debug.Log("Ignoring scene load for " + SceneManagerHelper.ActiveSceneName);
            }


        }
    }

    //^^gods was here
    #region Instantiating Method

      public void SpawnTank()
    {

        prefabChange = GameObject.Find("Dropdown").GetComponent<PrefabChange>();

        newPlayerPrefab = prefabChange.prefabToSpawn;
        Debug.Log(newPlayerPrefab);

        Debug.Log("We are Instantiating LocalPlayer from " + SceneManagerHelper.ActiveSceneName);

        int spawnNumber = Random.Range(0, spawnPoints.Length);

        GameObject playerPrefab = (GameObject)PhotonNetwork.Instantiate(newPlayerPrefab + " Variant", spawnPoints[spawnNumber].position, spawnPoints[spawnNumber].rotation, 0);
        Debug.Log(playerPrefab);
        
        //Activating Objects

        playerPrefab.GetComponent<RTCTankController>().enabled = true;

        if (playerPrefab.transform.Find("FlameThrower") != null)
        {
            playerPrefab.GetComponentInChildren<ParticleEmitter>().enabled = true;
            GameObject flameThrower = playerPrefab.transform.Find("FlameThrower").gameObject;
            flameThrower.transform.Find("MainCamera").gameObject.SetActive(true);
        }
        else if (playerPrefab.transform.Find("WindChill") != null)
        {
            playerPrefab.GetComponentInChildren<ParticleEmitter>().enabled = true;
            GameObject flameThrower = playerPrefab.transform.Find("WindChill").gameObject;
            flameThrower.transform.Find("MainCamera").gameObject.SetActive(true);
        }
        else if (playerPrefab.transform.Find("Sniper") != null)
        {
            playerPrefab.GetComponentInChildren<snipershooting>().enabled = true;
            GameObject sniper = playerPrefab.transform.Find("Sniper").gameObject;
            sniper.transform.Find("MainCamera").gameObject.SetActive(true);
        }
        else if (playerPrefab.transform.Find("MachineGun") != null)
        {
            playerPrefab.GetComponentInChildren<MachineGun>().enabled = true;
            GameObject machineGun = playerPrefab.transform.Find("MachineGun").gameObject;
            machineGun.transform.Find("MainCamera").gameObject.SetActive(true);
        }
        else if (playerPrefab.transform.Find("Blaster") != null)
        {
            playerPrefab.GetComponentInChildren<OneShot>().enabled = true;
            GameObject machineGun = playerPrefab.transform.Find("Blaster").gameObject;
            machineGun.transform.Find("MainCamera").gameObject.SetActive(true);
        }

        playerPrefab.GetComponent<TankHealth>().enabled = true;
        playerPrefab.GetComponentInChildren<TurretRotation>().enabled = true;

        if (!firstTimeCalled)
        {
            CanvasInstantiate();
            firstTimeCalled = true;
        }
    }

    private void CanvasInstantiate()
    {
        GameObject warCanvas = (GameObject)PhotonNetwork.Instantiate(Canvas.name, Vector3.zero, gameObject.transform.rotation, 0);
        warCanvas.SetActive(true);
    }
 #endregion

    #region Public Methods


    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    #endregion
*/
}