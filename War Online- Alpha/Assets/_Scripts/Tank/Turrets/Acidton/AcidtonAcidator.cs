using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Obi;
using UnityEngine.UI;
using Photon.Pun;

public class AcidtonAcidator : MonoBehaviourPun //IPunObservable
{/*
    [Header("Acid Colors")]
    public ObiEmitter acidEmitter;
    public float speed;
    public float lerpTime;
    public float inverseLerpTime;
    

    [Header("Damages")]
    public float damage;
    public GameObject startPoint;
    public GameObject endPoint;
    public float radius;

    
    private bool isFiring;
    private TankHealth enemy;

    private TankHealth tankHealth;
    private GameObject mainCanvas;
    private Slider coolDownSlider;
    private Image fillImage;

    [SerializeField]
    private int acidCapacity = 10;
    [SerializeField]
    private float reloadTime = 1f;
    private float tempAmmo;
    private float uiAmmo;
    private float uiTempAmmo;
    private float uiPreviousAmmo;
    private float reloadLerp;

    private Coroutine reloadCoroutine;
    private Coroutine uiCoroutine;

    void Start()
    {
        tankHealth = GetComponentInParent<TankHealth>();
        mainCanvas = tankHealth.warCanvas;
        GameObject coolDownUI = mainCanvas.transform.Find("CoolDownUI").gameObject;
        coolDownSlider = coolDownUI.GetComponent<Slider>();
        GameObject coolDown = coolDownUI.transform.Find("CoolDown").gameObject;
        fillImage = coolDown.GetComponentInChildren<Image>();

        tempAmmo = acidCapacity;
        uiTempAmmo = acidCapacity;
        uiPreviousAmmo = acidCapacity;
        uiAmmo = acidCapacity;
    }

    private void ProcessFireInput()
    {
        if (Input.GetButton("Fire"))
            Fire();
        else
        {
            isFiring = false;

            if (reloadCoroutine == null)
                if (tempAmmo < acidCapacity)
                {
                    reloadCoroutine = StartCoroutine(Cooldown());
                    uiCoroutine = StartCoroutine(UpdateUI());
                }
        }
    }

    private void Update()
    {
        ProcessFireInput();

        if (reloadCoroutine == null)
            if (tempAmmo <= 0)
            {
                reloadCoroutine = StartCoroutine(Cooldown());
                uiCoroutine = StartCoroutine(UpdateUI());
            }

        if (isFiring)
        {
            Vector3 newStartPos = startPoint.GetComponent<Transform>().position;
            Vector3 newPos = endPoint.GetComponent<Transform>().position;
            Collider[] colliders = Physics.OverlapCapsule(newStartPos, newPos, radius);



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
            //mechanism for Particle Control
            if (acidEmitter.speed >= speed)
            {
                acidEmitter.speed = speed;

            } else if(acidEmitter.speed < speed) {
                
                acidEmitter.speed = Mathf.Lerp(acidEmitter.speed, speed, lerpTime);

            }

        }// if not firing
        else
        {
            if (acidEmitter.speed <= 0f)
            {
                acidEmitter.speed = 0f;

            }
            else if (acidEmitter.speed > 0f)
            {

                acidEmitter.speed = Mathf.Lerp(acidEmitter.speed, 0f, inverseLerpTime);

            }
        }
    }

    private void Damage()
    {
        enemy.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", enemy.GetComponent<PhotonView>().Owner, damage);
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) stream.SendNext(isFiring);
        else this.isFiring = (bool)stream.ReceiveNext();
    }

    private void Fire()
    {
        if (tempAmmo > 0)
        {
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
                StopCoroutine(uiCoroutine);
                reloadLerp = 0;
                reloadCoroutine = null;
                uiCoroutine = null;
            }

            isFiring = true;
            tempAmmo -= Time.unscaledDeltaTime;
            uiAmmo -= Time.unscaledDeltaTime;
            uiPreviousAmmo = uiAmmo;
        }
        else
        {
            tempAmmo = 0;
            isFiring = false;
        }
    }

    private IEnumerator Cooldown()
    {
        while (tempAmmo < acidCapacity)
        {
            yield return new WaitForSecondsRealtime(reloadTime);
            tempAmmo++;

            if (tempAmmo >= acidCapacity)
                tempAmmo = acidCapacity;
        }

        reloadCoroutine = null;
    }

    private IEnumerator UpdateUI()
    {
        uiPreviousAmmo = uiAmmo;
        uiTempAmmo++;

        if (uiTempAmmo >= acidCapacity)
            uiTempAmmo = acidCapacity;

        while (reloadLerp < 1)
        {
            reloadLerp += Time.deltaTime / reloadTime;
            uiAmmo = Mathf.Lerp(uiPreviousAmmo, uiTempAmmo, reloadLerp);
            fillImage.fillAmount = uiAmmo / acidCapacity;

            yield return null;
        }

        reloadLerp = 0;
        uiCoroutine = null;
    }
*/
}

