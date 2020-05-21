using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;

public class TankHealth : MonoBehaviourPun
{
    [Header("Health")]
    public float m_StartingHealth = 100f;
    public Color m_FullHealthColor;
    public Color m_ZeroHealthColor;
    public float m_CurrentHealth;

    [Header("UI Stuff")]
    public GameObject warCanvas, teamView, enemyView;
    public Slider m_Slider;
    public Image m_FillImage;

    [Header("GameObjects")]
    public GameObject actualHull;
    public GameObject actualTurret;

    [HideInInspector]
    public GameObject destroyedTurret;

    //public GameObject m_ExplosionPrefab;

    private bool spawnCalled;
    private float myTeam;
    private GameObject destroyedHull;
    private string playerTankName;
    private AudioSource m_ExplosionAudio;
    private ParticleSystem m_ExplosionParticles;
    private bool m_Dead;
    private gameManager photonScript;
    private bool destroyCalled;
    


    private void Awake()
    {
        GameObject canvas = gameObject.transform.Find("WarCanvas").gameObject;
        canvas.SetActive(true);
        warCanvas = canvas;

        teamView = gameObject.transform.Find("TeamCanvas").gameObject;
        enemyView = gameObject.transform.Find("EnemyCanvas").gameObject;
        
        GameObject warslider = canvas.transform.Find("TankHealthUI").gameObject;

        myTeam = GetComponent<FactionID>()._teamID;

        Slider slider = warslider.GetComponent<Slider>();
        m_Slider = slider;
        GameObject health = warslider.transform.Find("Health").gameObject;
        Image healthImage = health.GetComponentInChildren<Image>();
        m_FillImage = healthImage;

        m_Dead = false;

        m_CurrentHealth = m_StartingHealth;
        photonScript = GameObject.Find("GameManager").GetComponent<gameManager>();
       
        //getting destroyed tank prefabs
        destroyedHull = gameObject.transform.Find(actualHull.name + "_D").gameObject;
        

        destroyCalled = false;
        GameObject mainCanvas = GameObject.Find("MainWarCanvas");
        canvas.transform.parent = mainCanvas.transform;

        m_Dead = false;
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            teamView.SetActive(false);
            enemyView.SetActive(false);
        }
    }

    private void Update()
    {

        SetHealthUI(m_CurrentHealth);

        if (m_CurrentHealth <= 0)
        {

            OnDeath();

        }
        
        if (Input.GetKeyDown("u"))
        {
            m_CurrentHealth = 0f;
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.

        m_CurrentHealth -= damage;

        SetHealthUI(m_CurrentHealth);

        photonView.RPC("UpdateHealth", RpcTarget.Others, m_CurrentHealth);

        if (m_CurrentHealth <= 0)
        {
            OnDeath();
        }
    }

    [PunRPC]
    private void SelfDamage(float damage)
    {
        m_CurrentHealth -= damage;

        SetHealthUI(m_CurrentHealth);

        photonView.RPC("UpdateHealth", RpcTarget.Others, m_CurrentHealth);

        if (m_CurrentHealth <= 0)
        {

            OnDeath();

        }
    }

    [PunRPC]
    private void SetHealthUI(float health)
    {
        // Adjust the value and colour of the slider.

        m_Slider.value = health;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, health / m_StartingHealth);

    }

    [PunRPC]
    private void SetColorFromFlameThrower(bool isthrowing, Color color)
    {
        Color currentColor;
        if (isthrowing)
        {
            actualHull.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, color, Time.deltaTime * 2f);
            actualTurret.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, color, Time.deltaTime * 2f); ;
        }
        else if (!isthrowing && actualHull.GetComponentInChildren<MeshRenderer>().material.color != Color.white)
        {
            currentColor = actualHull.GetComponent<MeshRenderer>().material.color;
            actualHull.GetComponent<MeshRenderer>().material.color = Color.Lerp(currentColor, Color.white, Time.deltaTime * 2f);
            actualTurret.GetComponent<MeshRenderer>().material.color = Color.Lerp(currentColor, Color.white, Time.deltaTime * 2f);
        }
    }

    [PunRPC]
    private void UIComponentsSet(float teamNo)
    {
        if(teamNo != myTeam || teamNo == 1)
        {
            
        }
    }

    void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.

        m_Dead = true;


        if (!destroyCalled)
        {
            actualHull.SetActive(false);
            actualTurret.SetActive(false);

            destroyedTurret.SetActive(true);
            destroyedHull.SetActive(true);
            destroyCalled = true;
            RTCTankController tankController = gameObject.GetComponent<RTCTankController>();
            tankController.engineRunning = false;
        }

        StartCoroutine(Destroying());

    }

    private IEnumerator Destroying()
    {
        yield return new WaitForSeconds(1.8f);

        PhotonNetwork.Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        if (!spawnCalled)
        {
            photonScript.SpawnTank();
            spawnCalled = true;
            Destroy(warCanvas);
        }
    }
}