using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class OneShot : MonoBehaviourPun
{
    [Header("Reload Functions")]
    [SerializeField]
    private float reloadTime;
    private float ammo = 1f;
    [SerializeField]
    private float range = 450f;

    [Header("Damage Values")]
    [SerializeField]
    private float damage;
    [SerializeField]
    private ParticleSystem hitEffect;

    [Header("Others")]
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private ParticleSystem barrelFlash;

    [Header("Sound System")]
    [FMODUnity.EventRef]
    public string sniperShootSfx = "event:/Sniper";
    FMOD.Studio.EventInstance sniperShootEv;

    private RaycastHit target;
    private float ammoReload;
    private float timeForBar;
  
    private TankHealth tankHealth;
    private GameObject mainCanvas;
    private Slider coolDownSlider;
    private Image fillImage;
    private Animator animator;
    private bool camMove = false;
    private float currentReloadValue;
    private float expectedReloadValue;

    #region Start&Update
    private void Start()
    {
        barrelFlash.Play(false);

        //Interface
        tankHealth = GetComponentInParent<TankHealth>();
        mainCanvas = tankHealth.warCanvas;
        GameObject coolDownUI = mainCanvas.transform.Find("CoolDownUI").gameObject;
        coolDownSlider = coolDownUI.GetComponent<Slider>();
        GameObject coolDown = coolDownUI.transform.Find("CoolDown").gameObject;
        coolDownSlider.maxValue = 1f;
        coolDownSlider.minValue = 0f;
        coolDownSlider.value = 1f;
        fillImage = coolDown.GetComponentInChildren<Image>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire"))
        {
            if(ammo >= 1)
            {
                Shoot();
            }
        }

        if(ammo < 1f)
        {
            ammoReload += Time.deltaTime;
            if(ammoReload >= reloadTime)
            {
                ammo = 1f;
                ammoReload = 0f;
            }
        }

        if(coolDownSlider.value != ammo)
        {
            if(coolDownSlider.value < ammo)
            {
                timeForBar += Time.deltaTime;
                if (timeForBar >= 0.06)
                {
                    coolDownSlider.value += 0.01f;
                    timeForBar = 0f;
                }
            }
            else
            {
                coolDownSlider.value = 0f;
            }

        }

    }

    #endregion Start&Update

    #region Shoot
    private void Shoot()
    {
        ammo = 0f;

        RaycastHit hit;
        Physics.Raycast(firePoint.position, firePoint.forward, out hit, range);
        target = hit;

        barrelFlash.Play(true);
        TankHealth targetHealth = target.transform.gameObject.GetComponent<TankHealth>();

        if (targetHealth != null)
        {
            FactionID fID = targetHealth.gameObject.GetComponent<FactionID>();
            FactionID myID = gameObject.GetComponentInParent<FactionID>();

            if (fID == null || fID._teamID == 1 || myID._teamID == 1 || fID._teamID != myID._teamID)
            {
                if (fID.myAccID != myID.myAccID)
                {
                    targetHealth.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", target.transform.gameObject.GetComponent<PhotonView>().Owner, damage);
                }
            }
        }
        else
        {
            if(hit.transform != null)
            {
                GameObject i = Instantiate(hitEffect.gameObject, target.point, Quaternion.identity);
                i.transform.rotation = Quaternion.FromToRotation(Vector3.forward, target.normal);
                Destroy(i, 10f);
            }
        }

    }
    #endregion Shoot
}

