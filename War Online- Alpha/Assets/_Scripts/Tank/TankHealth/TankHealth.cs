﻿using UnityEngine;
using System.Collections;
using _Scripts;
using _Scripts.Photon.Room;
using _Scripts.Tank.DOTS;
using UnityEngine.UI;
using Photon.Pun;
using Unity.Entities;

public class TankHealth : MonoBehaviourPun
{
    [Header("Prefab")] public GameObject entityPrefab;

    [Header("Health")] public float m_StartingHealth = 100f;
    public Color m_FullHealthColor;
    public Color m_ZeroHealthColor;
    public float m_CurrentHealth;

    [Header("UI Stuff")] public GameObject warCanvas, rtcCanvas, teamView, enemyView;
    public Slider m_Slider;
    public Image m_FillImage;

    [Header("GameObjects")] public GameObject actualHull, actualTurret, destroyedHull, destroyedTurret;

    //public GameObject m_ExplosionPrefab;

    private bool spawnCalled;
    [HideInInspector] public int myTeam;
    [HideInInspector] public FactionID fid;
    private string playerTankName;
    private AudioSource m_ExplosionAudio;
    private ParticleSystem m_ExplosionParticles;
    private bool m_Dead;
    private GameManager photonScript;
    private bool destroyCalled;

    private BlobAssetStore _blobAssetStore;
    private EntityManager _entityManager;

    private Entity _instantiatedEntity;

    private void Awake()
    {
        // ECS

        _blobAssetStore = new BlobAssetStore();

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var settings =
            GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);

        var converted =
            GameObjectConversionUtility.ConvertGameObjectHierarchy(entityPrefab, settings);

        _instantiatedEntity = _entityManager.Instantiate(converted);
        _entityManager.SetName(_instantiatedEntity, gameObject.name);
        _entityManager.AddComponentData(_instantiatedEntity, new TankBody
        {
            teamID = myTeam
        });

        _entityManager.AddComponentData(_instantiatedEntity, new SyncEntity
        {
            translation = true, rotation = true
        });

        EntitiesHelper.Tse.Add(_instantiatedEntity, actualHull.transform);
        EntitiesHelper.Eth.Add(_instantiatedEntity, this);

        // fields

        destroyCalled = false;

        m_Dead = false;

        fid = GetComponent<FactionID>();
        myTeam = fid.teamID;

        GameObject warslider = warCanvas.transform.Find("TankHealthUI").gameObject;

        Slider slider = warslider.GetComponent<Slider>();
        m_Slider = slider;
        GameObject health = warslider.transform.Find("Health").gameObject;
        Image healthImage = health.GetComponentInChildren<Image>();
        m_FillImage = healthImage;

        m_CurrentHealth = m_StartingHealth;
        photonScript = FindObjectOfType<GameManager>();

        if (!teamView) teamView = gameObject.transform.Find("TeamCanvas").gameObject;
        if (!enemyView) enemyView = gameObject.transform.Find("EnemyCanvas").gameObject;

        //getting destroyed tank prefabs
        if (!destroyedHull) destroyedHull = gameObject.transform.Find(actualHull.name + "_D").gameObject;
    }

    private void Start()
    {
        if (photonView.IsMine) return;

        teamView.SetActive(false);
        enemyView.SetActive(false);
    }

    /*private void Update()
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
    }*/

    [PunRPC]
    public void UpdateHealth(float health)
    {
        m_CurrentHealth = health;

        SetHealthUI(m_CurrentHealth);

        if (m_CurrentHealth <= 0)
        {
            OnDeath();
        }
    }

    public void TakeDamage(float damage)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.

        m_CurrentHealth -= damage;

        SetHealthUI(m_CurrentHealth);

        photonView.RPC(nameof(UpdateHealth), RpcTarget.Others, m_CurrentHealth);

        if (m_CurrentHealth <= 0)
        {
            OnDeath();
        }
    }

    private void SetHealthUI(float health)
    {
        // Adjust the value and colour of the slider.

        m_Slider.value = health;

        if (health < .05f)
        {
            m_FillImage.color = Color.red;
        }
        else if (health > .95f)
        {
            m_FillImage.color = Color.white;
        }
        else
        {
            m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, health / m_StartingHealth);
        }
    }

    [PunRPC]
    public void SetColorFromFlameThrower(bool isthrowing, Color color)
    {
        if (isthrowing)
        {
            actualHull.GetComponent<MeshRenderer>().material.color =
                Color.Lerp(Color.white, color, Time.deltaTime * 2f);
        }
        else if (actualHull.GetComponentInChildren<MeshRenderer>().material.color != Color.white)
        {
            var currentColor = actualHull.GetComponent<MeshRenderer>().material.color;
            actualHull.GetComponent<MeshRenderer>().material.color =
                Color.Lerp(currentColor, Color.white, Time.deltaTime * 2f);
        }
    }

    [PunRPC]
    private void UIComponentsSet(int teamNo)
    {
        if (teamNo != myTeam || teamNo == 1)
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
            RTC_TankController tankController = gameObject.GetComponent<RTC_TankController>();
            tankController.engineRunning = false;
        }

        StartCoroutine(Destroying());
    }

    private IEnumerator Destroying()
    {
        yield return new WaitForSeconds(1.8f);

        PhotonNetwork.Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!spawnCalled)
        {
            //photonScript.SpawnTank();
            spawnCalled = true;
            Destroy(warCanvas);
        }

        EntitiesHelper.Eth.Remove(_instantiatedEntity);
        _blobAssetStore.Dispose();
    }
}