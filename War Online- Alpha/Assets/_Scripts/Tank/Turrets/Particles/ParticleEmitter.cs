using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ParticleEmitter : MonoBehaviourPun, IPunObservable {

    [Header("Particle System")]
    public ParticleSystem particleFire;
    public ParticleSystem particleSmoke;
    

    [Header("Range & Limits")]
    public float radius = 4f;
    public float damage = 4f;
    public GameObject startPoint;
    public GameObject endPoint;
    public float ammo = 10f;
    public float multiplier = 2f;

    [Header("Special Functions")]
    public bool isFlameThrower;
    public Color flameColor;
    public Color iceColor;

    [Header("Sound Effects")]
    [FMODUnity.EventRef]
    public string flameThrowersfx = "event:/Tanks/Turrets/FlameThrower";
    FMOD.Studio.EventInstance flameThrowerEv;

    //private variables
    private bool isFiring;
    private TankHealth enemy;
    private TankHealth tankHealth;
    private GameObject mainCanvas;
    private Slider coolDownSlider;
    private Vector3 newPos;
    private float barTime;

    //reload functions
    [SerializeField]
    private float ammoRunning = 0.0f;
    [SerializeField]
    private float ammoReload = 0.0f;
    [SerializeField]
    private float range;

    private float UIseti;
    private float UIsetd;
    private bool increment = false;
    private bool decrement = false;
    private float mainValue = 0f;

    #region Start
    void Start()
    {
        particleFire.Stop();
        particleSmoke.Stop();

        tankHealth = GetComponentInParent<TankHealth>();
        mainCanvas = tankHealth.warCanvas;
        GameObject coolDownUI = mainCanvas.transform.Find("CoolDownUI").gameObject;
        coolDownSlider = coolDownUI.GetComponent<Slider>();
        if (!isFlameThrower)
        {
            coolDownSlider.maxValue = 13f;
            coolDownSlider.minValue = 0f;
            coolDownSlider.value = 13f;
            mainValue = 13f;
        }
        else
        {
            coolDownSlider.maxValue = 10f;
            coolDownSlider.minValue = 0f;
            coolDownSlider.value = 10f;
            mainValue = 10f;
        }
        range = Vector3.Distance(startPoint.transform.position, endPoint.transform.position);

        //FMOD sounds
        flameThrowerEv = FMODUnity.RuntimeManager.CreateInstance(flameThrowersfx);
        flameThrowerEv.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
        flameThrowerEv.start();
    }
    #endregion Start

    #region FireInput
    void ProcessFireInput()
    {
        if (Input.GetButton("Fire"))
        {
            isFiring = true;
            flameThrowerEv.setParameterByName("Firing", 1f);
        }
            

        else if (Input.GetButtonUp("Fire"))
        {
            isFiring = false;

            //sfx close
            flameThrowerEv.setParameterByName("Firing", 0f);
            flameThrowerEv.setParameterByName("ReloadFull", 0f);
        }
        else
        {
            isFiring = false;

            //sfx close
            flameThrowerEv.setParameterByName("Firing", 0f);
            flameThrowerEv.setParameterByName("ReloadFull", 0f);
        }
    }
    #endregion FireInput

    #region Update
    void Update()
    {

        ProcessFireInput();

        Transform newStartPos = startPoint.transform;
        RaycastHit hit;
        Physics.Raycast(newStartPos.position, newStartPos.forward, out hit);
        
        if (hit.distance >= range)
        {
            newPos = endPoint.GetComponent<Transform>().position;
        }
        else if (hit.distance < range)
        {
            if (hit.transform.GetComponentInParent<TankHealth>())
            {
                newPos = hit.transform.position + Vector3.forward;
            }
            else
            {
                newPos = hit.transform.position;
            }
        }

        if (coolDownSlider.value != ammo)
        {
            barTime += Time.deltaTime;
            if (barTime >= 0.01f)
            {
                if (coolDownSlider.value < ammo)
                {
                    coolDownSlider.value += mainValue / 85;
                    barTime = 0f;
                }
                if (coolDownSlider.value > ammo)
                {
                    coolDownSlider.value -= mainValue / 85;
                    barTime = 0f;
                }
            }
        }

        if (isFiring && ammo != 0)
        {

            if (ammo > 0)
            {

                Vector3 StartPos = startPoint.GetComponent<Transform>().position;
                Vector3 newPos = endPoint.GetComponent<Transform>().position;
                Collider[] colliders = Physics.OverlapCapsule(StartPos, newPos, radius);

            for (int i = 1; i < colliders.Length; i++)
            {

                Rigidbody targetRigidbody = colliders[i].GetComponentInParent<Rigidbody>();
                if (!targetRigidbody)
                    continue;

                TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();
                if (!targetHealth)
                    continue;

                else if (targetHealth)
                {
                    FactionID fID = targetHealth.gameObject.GetComponent<FactionID>();
                    FactionID myID = gameObject.GetComponentInParent<FactionID>();

                    if (fID == null || fID._teamID == 1 || myID._teamID == null || myID._teamID == 1 || fID._teamID != myID._teamID)
                    {
                        if (fID.myAccID != myID.myAccID)
                        {
                            Damage();
                            enemy = targetHealth;
                        }
                    }
                }

            }

                //reloading function starts here
                 
                ammoRunning += Time.deltaTime;
                if(ammoRunning > 1f)
                {
                    ammo--;
                    ammoRunning = 0f;
                }
            }
            particleFire.Play();
            particleSmoke.Play();

            flameThrowerEv.setParameterByName("Firing", 1f);
        }

        else if (!isFiring || ammo == 0f)
        {
            particleFire.Stop();
            particleSmoke.Stop();

            //sfx close
            flameThrowerEv.setParameterByName("Firing", 0f);

            if (ammo < 10f)
            {
                ammoReload += Time.deltaTime;
                if(ammoReload > 1f)
                {
                    ammo++;
                    ammoReload = 0f;
                }
                flameThrowerEv.setParameterByName("ReloadFull", 0f);
            }
            else if(ammo >= 10f)
            {
                ammo = 10f;
                flameThrowerEv.setParameterByName("ReloadFull", 1f);
            }
        }

    }
    #endregion Update

    #region Damage
    void Damage()
    {
        
        if (enemy != null)
        {
            FactionID fID = enemy.gameObject.GetComponent<FactionID>();
            FactionID myID = gameObject.GetComponentInParent<FactionID>();

            if (fID == null || fID._teamID == 1 || myID._teamID == 1 || fID._teamID != myID._teamID)
            {
                if (fID.myAccID != myID.myAccID)
                {
                    enemy.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", enemy.GetComponent<PhotonView>().Owner, damage);
                    if (isFlameThrower)
                    {
                        enemy.gameObject.GetComponent<PhotonView>().RPC("SetColorFromParticleThrower", enemy.GetComponent<PhotonView>().Owner, true, flameColor);
                    }
                    else
                    {

                    }
                }
            }
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) stream.SendNext(isFiring);
        else this.isFiring = (bool)stream.ReceiveNext();
    }
    #endregion Damage
}
