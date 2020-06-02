using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MachineGun : MonoBehaviour
{

    [Header("Damages")]
    [SerializeField]
    private float damage = 8f;
    [SerializeField]
    private float damagePerTime = 2f;
    public bool isOnline;
    private float delayTime;
    public GameObject bulletEffect;

    [Header("Reload")]
    [SerializeField]
    private float reloadTime = 1f;
    [SerializeField]
    private float actualAmmo = 15f;
    [SerializeField]
    private float overHeatDamage;
    private float barTime;
    private float barValue;

    [Header("Range")]
    [SerializeField]
    private float range = 10000f;
    [SerializeField]
    private float radius = 4f;


    [Header("Barrel")]
    [SerializeField]
    private Transform bulletStartPoint;
    [SerializeField]
    private Transform bulletEndPoint;
    [SerializeField]
    private Transform barrel;
    [SerializeField]//Could be changed to animation.
    private float rollingSpeed = 1000f;
    [SerializeField]
    private ParticleSystem muzzleFlash;

    [Header("Bullet Material Effect")]
    [SerializeField]
    private float XAxis = 0f;
    [SerializeField]
    private float YAxis = 0f;

    [Header("Sounds")]
    [FMODUnity.EventRef]
    public string mgShootSfx = "event:/MachineGun";
    FMOD.Studio.EventInstance mgShootEv;

    private Material bulletMaterial;
    private LineRenderer bulletRenderer;
    private TankHealth enemy;

    private TankHealth tankHealth;
    private GameObject mainCanvas;
    private Slider coolDownSlider;
    private Image fillImage;
    private bool neededzero;
    
    private bool attack = false;
    private bool ceasefire = true;
    private bool timer;
    private float ammo;
    private float ammoRunning;
    private float ammoReload;
    float selfDamage;

    #region Start
    private void Start()
    {
        if (bulletRenderer == null)
        {
            bulletRenderer = GetComponent<LineRenderer>();
        }

        ammo = actualAmmo;

        //setting sfx
        mgShootEv = FMODUnity.RuntimeManager.CreateInstance(mgShootSfx);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(mgShootEv, GetComponent<Transform>(), GetComponent<Rigidbody>());
        mgShootEv.start();
        mgShootEv.setParameterByName("SoundLess", 0f);

        bulletRenderer = GetComponent<LineRenderer>();
        bulletMaterial = bulletRenderer.material;

        tankHealth = GetComponentInParent<TankHealth>();
        mainCanvas = tankHealth.warCanvas;
        GameObject coolDownUI = mainCanvas.transform.Find("CoolDownUI").gameObject;
        coolDownSlider = coolDownUI.GetComponent<Slider>();
        coolDownSlider.minValue = 0f;
        coolDownSlider.maxValue = 13.5f;
        coolDownSlider.value = actualAmmo;
        GameObject coolDown = coolDownUI.transform.Find("CoolDown").gameObject;
        fillImage = coolDown.GetComponentInChildren<Image>();

        attack = false;
    }
    #endregion Start

    #region Update
    private void Update()
    {
        if(ammo >= 0)
        {
            barValue = ammo;
        }
        

        if (Input.GetButton("Fire"))
        {
            if (ammo >= actualAmmo)
            {
                attack = true;
            }
        }
        if(Input.GetButtonUp("Fire") == true)
        {
            if (ammo <= actualAmmo)
            {
                ceasefire = true;
            }
            else if (ammo >= actualAmmo)
            {
                ammo = actualAmmo;
                ceasefire = false;
            }
            
        }
        if(ammo >= actualAmmo)
        {
            ceasefire = false;
            mgShootEv.setParameterByName("SoundLess", 0f);
        }


        if (attack)
        {
            neededzero = true;
            muzzleFlash.Play(true);
            BarrelRoll();

            RaycastHit hit;
            Collider[] colliders;
            if (Physics.Raycast(bulletStartPoint.position, bulletStartPoint.forward, out hit, range))
            {
                colliders = Physics.OverlapCapsule(bulletStartPoint.position, bulletEndPoint.position, radius);

                HitDamage(hit);

                for (int i = 0; i < colliders.Length; i++)
                {
                    Transform targetTransform = colliders[i].GetComponent<Transform>();

                    if (!targetTransform)
                        continue;
                    else if (targetTransform)
                    {
                        TankHealth targetHealth = targetTransform.GetComponentInParent<TankHealth>();
                        if (targetHealth)
                        {
                            FactionID fID = targetHealth.gameObject.GetComponent<FactionID>();
                            FactionID myID = gameObject.GetComponentInParent<FactionID>();

                            if (fID == null || fID._teamID == 1 || myID._teamID == null || myID._teamID == 1 || fID._teamID != myID._teamID)
                            {
                                if (fID.myAccID != myID.myAccID)
                                {
                                    Debug.Log(colliders[i] + "yes");
                                    bulletRenderer.enabled = true;
                                    bulletRenderer.SetPosition(0, bulletStartPoint.position);
                                    bulletRenderer.SetPosition(1, targetHealth.transform.position);
                                    continue;
                                }
                            }

                        }
                        else if (!targetHealth)
                        {
                            Debug.Log(colliders[i] + "No");
                            bulletRenderer.enabled = true;
                            bulletRenderer.SetPosition(0, bulletStartPoint.position);
                            bulletRenderer.SetPosition(1, hit.point);
                            continue;
                        }
                    }
                }
            }

            bulletMaterial.SetTextureOffset("_MainTex", new Vector2(XAxis * Time.deltaTime, YAxis * Time.deltaTime));

            ammoRunning += Time.deltaTime;
            if (ammoRunning > 1f)
            {
                ammo--;
                ammoRunning = 0f;
            }
            barTime += Time.deltaTime;
            if(barTime > 0.1f)
            {
                if (barValue != coolDownSlider.value && coolDownSlider.value >= 0)
                {
                    coolDownSlider.value -= (actualAmmo/100);
                }
                if(coolDownSlider.value < 0 || barValue < 0)
                {
                    coolDownSlider.value = 0f;
                    barValue = 0f;
                }
                barTime = 0f;
            }

            mgShootEv.setParameterByName("Firing", 1f);

        }
       
        if (ceasefire)
        {
            mgShootEv.setParameterByName("Firing", 0f);
            if (neededzero)
            {
                neededzero = false;
                ammo = 0f;
            }
            attack = false;
            muzzleFlash.Stop(true);
            bulletRenderer.enabled = false;
            
            ammoReload += Time.deltaTime;
            if (ammoReload > reloadTime)
            {
                ammo += (actualAmmo/10);
                ammoReload = 0f;
            }
            barTime += Time.deltaTime;
            if (barTime > 0.1f)
            {
                if (barValue != coolDownSlider.value)
                {
                    coolDownSlider.value = Mathf.Lerp(0, ammo, 1);
                }
                barTime = 0f;
            }
            mgShootEv.setParameterByName("SoundLess", 1f);
        }

        if(ammo < 0)
        {
            selfDamage += Time.deltaTime;

            if(selfDamage > .5f)
            {
                selfDamage = overHeatDamage;
            }
        }
    }
    #endregion Update

    #region HitFunctions
    private void BarrelRoll()
    {
        barrel.Rotate(Vector3.forward, rollingSpeed * Time.deltaTime);
    }

    private void HitDamage(RaycastHit bulletCast)
    {
        GameObject bulletSpot = Instantiate(bulletEffect, bulletCast.point, Quaternion.identity);
        bulletSpot.transform.rotation = Quaternion.FromToRotation(Vector3.forward, bulletCast.normal);
        Destroy(bulletSpot, 2f);

        TankHealth targetHealth = bulletCast.transform.GetComponent<TankHealth>();

        if (targetHealth && Time.time >= delayTime)
        {
            FactionID fID = targetHealth.gameObject.GetComponent<FactionID>();
            FactionID myID = gameObject.GetComponentInParent<FactionID>();

            if (fID == null || fID._teamID == 1 || myID._teamID == 1 || fID._teamID != myID._teamID)
            {
                if (fID.myAccID != myID.myAccID)
                {
                    Damage();
                    enemy = targetHealth;
                }
            }

            delayTime = Time.time + 1 / damagePerTime;

        }
        else if (!targetHealth && Time.time >= delayTime)
        {
            TankHealth targetH = bulletCast.transform.gameObject.GetComponentInParent<TankHealth>();
            if (targetH)
            {
                FactionID fID = targetHealth.gameObject.GetComponent<FactionID>();
                FactionID myID = gameObject.GetComponent<FactionID>();

                if (fID == null || fID._teamID == 0 || myID._teamID == 0 || fID._teamID != myID._teamID)
                {
                    if (fID.myAccID != myID.myAccID)
                    {
                        Damage();
                        enemy = targetHealth;
                    }
                }
                delayTime = Time.time + 1 / damagePerTime;
            }
        }
    }
    #endregion HitFunction

    #region Damage
    private void Damage()
    {
        Debug.Log("Damage called");
        if (enemy != null)
        {
            if (isOnline)
            {
                enemy.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", enemy.GetComponent<PhotonView>().Owner, damage);
            }
            else
            {
                enemy.gameObject.GetComponent<TankHealth>().m_CurrentHealth -= damage;

            }
        }
    }

    private void SelfDamage()
    {
        GetComponentInParent<PhotonView>().RPC("SelfDamage", gameObject.GetComponent<PhotonView>().Owner, 50f);
    }

    #endregion Damage

}
