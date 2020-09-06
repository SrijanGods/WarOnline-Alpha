using System;
using _Scripts.Controls;
using _Scripts.Photon.Room;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ParticleEmitter : MonoBehaviourPun, IPunObservable
{
    [Header("Particle System")] public ParticleSystem particleFire;
    public Transform particleSmoke;
    public LayerMask tanksLayer;


    [Header("Range & Limits")] public float radius = 4f;
    public float damage = 4f;
    public Transform startPoint, endPoint;
    public float ammo = 10f;
    public float multiplier = 2f;

    [Header("Special Functions")] public bool isFlameThrower;
    public Color flameColor;
    public Color iceColor;

    [Header("Sound Effects")] [FMODUnity.EventRef]
    public string flameThrowersfx = "event:/Tanks/Turrets/FlameThrower";

    FMOD.Studio.EventInstance flameThrowerEv;

    //private variables
    private bool isFiring;
    private TankHealth _myTankHealth;
    private GameObject mainCanvas;
    private Slider coolDownSlider;
    private float barTime;

    //reload functions
    // [SerializeField] private float ammoRunning = 0.0f;
    [SerializeField] private float ammoReload = 0.0f;
    [SerializeField] private float range;

    private float UIseti;
    private float UIsetd;
    private bool increment = false;
    private bool decrement = false;
    private float mainValue = 0f;

    #region Start

    void Start()
    {
        particleSmoke.SetParent(particleFire.transform, true);
        particleFire.Stop(true);

        _myTankHealth = GetComponentInParent<TankHealth>();
        mainCanvas = _myTankHealth.warCanvas;
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

        Debug.Log(coolDownSlider);

        range = Vector3.Distance(startPoint.position, endPoint.position);

        //FMOD sounds
        flameThrowerEv = FMODUnity.RuntimeManager.CreateInstance(flameThrowersfx);
        flameThrowerEv.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
        flameThrowerEv.start();
    }

    #endregion Start

    #region FireInput

    void ProcessFireInput()
    {
        // if (!photonView.IsMine) return;

        if (SimulatedInput.GetButton(InputCodes.TankFire))
        {
            isFiring = true;
            flameThrowerEv.setParameterByName("Firing", 1f);
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

        // Transform newStartPos = startPoint.transform;
        // RaycastHit hit;
        // Physics.Raycast(newStartPos.position, newStartPos.forward, out hit);

        /*
        if (hit.distance >= range)
        {
            newPos = endPoint.GetComponent<Transform>().position;
        }
        else if (hit.distance < range)
        {
            if (hit.transform.GetComponentInParent<TankHealth>() != null)
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
        }
        */


        if (Math.Abs(coolDownSlider.value - ammo) > .005)
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

        if (isFiring && ammo > 0)
        {
            Vector3 startPos = startPoint.position;
            Vector3 endPos = endPoint.position;
            var colliders = new Collider[99];
            var size = Physics.OverlapCapsuleNonAlloc(startPos, endPos, radius, colliders, tanksLayer);

            for (int i = 0; i < size; i++)
            {
                TankHealth targetHealth = colliders[i].GetComponent<TankHealth>();
                if (!targetHealth) continue;

                if (targetHealth)
                {
                    FactionID fID = targetHealth.fid;
                    FactionID myID = _myTankHealth.fid;

                    if ((fID.teamID != -1 || myID.teamID != -1) && fID.teamID == myID.teamID) continue;

                    if (fID.myAccID == myID.myAccID) continue;

                    Damage(targetHealth);
                }
            }

            //reloading function starts here

            ammo -= Time.deltaTime;

            particleFire.Play(true);

            flameThrowerEv.setParameterByName("Firing", 1f);
        }

        else if (!isFiring || ammo <= 0f)
        {
            particleFire.Stop(true);

            //sfx close
            flameThrowerEv.setParameterByName("Firing", 0f);

            if (ammo < 10f)
            {
                ammoReload += Time.deltaTime;
                if (ammoReload > 1f)
                {
                    ammo++;
                    ammoReload = 0f;
                }

                flameThrowerEv.setParameterByName("ReloadFull", 0f);
            }
            else if (ammo >= 10f)
            {
                ammo = 10f;
                flameThrowerEv.setParameterByName("ReloadFull", 1f);
            }
        }
    }

    #endregion Update

    #region Damage

    void Damage(TankHealth enemy)
    {
        // enemy.photonView.RPC("TakeDamage", enemy.GetComponent<PhotonView>().Owner, damage);
        enemy.TakeDamage(damage);
        if (isFlameThrower)
        {
            enemy.photonView.RPC(nameof(enemy.SetColorFromFlameThrower),
                enemy.GetComponent<PhotonView>().Owner, true, flameColor);
        }
        else
        {
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) stream.SendNext(isFiring);
        else isFiring = (bool) stream.ReceiveNext();
    }

    #endregion Damage
}