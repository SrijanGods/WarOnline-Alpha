using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Duos : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject Sp1;
    public GameObject Bullet;
    public GameObject Sp2;

    [Header("Forces")]
    public float force;
    public float LerpValue;

    [Header("Particle Systems")]
    public ParticleSystem muzzleFlashA, muzzleFlashB;

    [Header("Sound Effects")]
    [FMODUnity.EventRef]
    public string duosReloadSfx = "event:/TurretDuos";
    FMOD.Studio.EventInstance duosEv;
    [FMODUnity.EventRef]
    public string duosShootSfx = "event:/TurretDuosShoot";
    FMOD.Studio.EventInstance duosShootEv;
    private Collider enemyFinder;
    private bool right = true;
    private bool left = false;

    private Slider coolDownSlider;
    private Image fillImage;
    private TankHealth tankHealth;
    private GameObject mainCanvas;
    private float myTeamID;
    private Vector3 enemyPos;
    private bool changeDir = false;
    private TouchProcessor tP;

    #region Start&Update
    void Start()
    {
        tP = GetComponentInParent<TouchProcessor>();
        tankHealth = GetComponentInParent<TankHealth>();
        mainCanvas = tankHealth.warCanvas;
        GameObject coolDownUI = mainCanvas.transform.Find("CoolDownUI").gameObject;
        coolDownSlider = coolDownUI.GetComponent<Slider>();
        coolDownSlider.maxValue = 1f;
        coolDownSlider.minValue = 0f;
        coolDownSlider.value = 1f;
        GameObject coolDown = coolDownUI.transform.Find("CoolDown").gameObject;
        fillImage = coolDown.GetComponentInChildren<Image>();
        myTeamID = GetComponentInParent<FactionID>()._teamID;

        //SFX initialize here
        duosEv = FMODUnity.RuntimeManager.CreateInstance(duosReloadSfx);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(duosEv, GetComponent<Transform>(), GetComponent<Rigidbody>());
        duosShootEv = FMODUnity.RuntimeManager.CreateInstance(duosShootSfx);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(duosShootEv, GetComponent<Transform>(), GetComponent<Rigidbody>());
        duosEv.start();
    }

    // Update is called once per frame
    void Update()
    {
        print(tP.fire);
        if (tP.fire)
        {
            if (right == true)
            {
                StartCoroutine(Waiting());
            }
            else if (left == true)
            {
                StartCoroutine(Waiting2());
            }
        }
        else
        {
        }
    }
    #endregion Start&Update

    #region Collision
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.GetComponentInParent<FactionID>() != null)
        {
            if(collider.GetComponentInParent<FactionID>()._teamID != myTeamID && collider.isTrigger == false)
            {
                enemyPos = collider.GetComponentInChildren<Transform>().position;
                changeDir = true;
            }
        }
    }

    private float stayCount = 0.0f;

    private void OnTriggerStay(Collider collider)
    {
        if (stayCount > 1f)
        {
            if (collider.GetComponentInParent<FactionID>() != null)
            {
                if (collider.GetComponentInParent<FactionID>()._teamID != myTeamID || collider.GetComponentInParent<FactionID>()._teamID == 1 && collider.isTrigger == false)
                {
                    enemyPos = collider.GetComponentInChildren<Transform>().position;
                    changeDir = true;
                }
            }
            stayCount = stayCount - 1f;
        }
        else
        {
            stayCount = stayCount + Time.deltaTime;
        }

    }
    #endregion Collision

    #region IEnum

    IEnumerator Waiting()
    {
        //coolDownSlider.value = Mathf.Lerp(1f, 0f, LerpValue);
        GameObject G = Instantiate(Bullet, Sp1.transform.position, Sp1.transform.rotation) as GameObject;
        duosShootEv.start();

        G.GetComponent<Rigidbody>().AddForce(G.transform.forward * force);
        if (changeDir)
        {
            G.transform.LookAt(enemyPos);
        }
        muzzleFlashA.Play(true);

        right = false;
        
        yield return new WaitForSeconds(0.5f);
        //coolDownSlider.value = Mathf.Lerp(0f, 1f, LerpValue);
        left = true;
        duosShootEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    IEnumerator Waiting2()
    {
        coolDownSlider.value = Mathf.Lerp(1f, 0f, LerpValue);
        GameObject G = Instantiate(Bullet, Sp2.transform.position, Sp2.transform.rotation) as GameObject;
        duosShootEv.start();
        G.GetComponent<Rigidbody>().AddForce(G.transform.forward * force);
        if (changeDir)
        {
            G.transform.LookAt(enemyPos);
        }
        muzzleFlashB.Play(true);

        left = false;

        yield return new WaitForSeconds(0.5f);
        coolDownSlider.value = Mathf.Lerp(0f, 1f, LerpValue);
        right = true;
        duosShootEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    #endregion IEnum
}


