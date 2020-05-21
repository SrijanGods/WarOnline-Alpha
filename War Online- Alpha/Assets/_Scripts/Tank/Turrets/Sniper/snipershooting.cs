using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;

public class snipershooting : MonoBehaviourPun
{
    [Header("Scope")]
    [SerializeField]
    private Camera cam;
    [SerializeField]
    public Transform scope;
    [SerializeField]
    private Image scopeImage;
    [SerializeField]
    private float cameraTransitionTime = 1f;
    private Vector3 cameraDefaultPos;
    private Vector3 cameraScopePos;
    private bool cameraMode;
    [SerializeField, Range(0f, 180f)]
    private float zoomCamFov = 38f;
    [SerializeField, Range(0f, 180f)]
    private float initCamFov = 64f;
    public float zoomSpeed;
    [SerializeField]
    private float zoomRotateSpeed = 0.8f;
    public GameObject hitEffect;

    [Header("Turret Stats")]
    [SerializeField]
    private float damage = 1000f;
    [SerializeField]
    private float tempDamage = 370f;
    private float range = 10000f;
    private TurretRotation turretRotation;
    private float initRotateSpeed;
    private float damageToLess = 700;

    [Header("Reload System")]
    [SerializeField]
    private float totalAmmo = 3;
    private bool zoomHeld;
    private bool zoomReleased;
    private bool required;
    private float reloadDuration = 0.25f;
    private float reloadTime;
    private float scopeTime;
    private bool inZoomMode;

    [Header("Sound System")]
    [FMODUnity.EventRef]
    public string sniperShootSfx = "event:/Sniper";
    FMOD.Studio.EventInstance sniperShootEv;

    [Header("Others")]
    [SerializeField]
    private LineRenderer laser;
    [SerializeField]
    private ParticleSystem barrelFlash;
    private RaycastHit target;
    
    private TankHealth tankHealth;
    private GameObject mainCanvas;
    private Slider coolDownSlider;
    private Image fillImage;
    private Animator animator;
    private bool camMove = false;
    private float currentReloadValue;
    private float expectedReloadValue;
    private float sfxValue;
    

    #region Start
    private void Start()
    {
        inZoomMode = false;
        required = false;
        //Laser
        laser.enabled = false;

        //Scope
        scopeImage.enabled = false;
        cameraScopePos = scope.localPosition;

        //Rotation
        turretRotation = GetComponent<TurretRotation>();
        initRotateSpeed = turretRotation.rotateSpeed;

        //sfx
        //sniperShootEv = FMODUnity.RuntimeManager.CreateInstance(sniperShootSfx);
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(sniperShootEv, GetComponent<Transform>(), GetComponent<Rigidbody>());
        //sniperShootEv.start();

        //Interface
        tankHealth = GetComponentInParent<TankHealth>();
        mainCanvas = tankHealth.warCanvas;
        GameObject coolDownUI = mainCanvas.transform.Find("CoolDownUI").gameObject;
        coolDownSlider = coolDownUI.GetComponent<Slider>();
        coolDownSlider.maxValue = 30f;
        coolDownSlider.minValue = 0f;
        coolDownSlider.value = 30f;
        GameObject coolDown = coolDownUI.transform.Find("CoolDown").gameObject;
        fillImage = coolDown.GetComponentInChildren<Image>();
        animator = cam.GetComponent<Animator>();
        animator.enabled = false;

        //Reload System
    }
    #endregion Start

    #region Update
    private void Update()
    {
        if(totalAmmo >= 3)
        {
            sniperShootEv.setParameterValue("ReloadFull", 1f);
        }
        expectedReloadValue = totalAmmo * 10;
        currentReloadValue = coolDownSlider.value;

        if (Input.GetButtonDown("Fire"))
        {
            sniperShootEv.setParameterValue("Firing", 0f);

            if (totalAmmo >= 3)
            {
                zoomHeld = true;
            }

            if(totalAmmo >= 2.5 && totalAmmo < 3)
            {
                Shoot(tempDamage);
            }
            if(totalAmmo >= 1.5 && totalAmmo < 2.5)
            {
                Shoot(tempDamage - tempDamage/8);
            }
        }

        zoomReleased = Input.GetButtonUp("Fire");

        if (zoomHeld)
        {
            sniperShootEv.setParameterValue("ReloadFull", 0f);

            required = true;
            //Transition
            animator.enabled = true;
            animator.SetBool("Scoped", true);

            sniperShootEv.setParameterValue("Scoping", 1f);

            camMove = true;
            CameraTransition(zoomHeld);
            Scope();

            // Laser
            RaycastHit hit;
            Physics.Raycast(scope.position, scope.forward, out hit, range);
            laser.enabled = true;
            laser.SetPosition(0, scope.position);
            laser.SetPosition(1, hit.point);
            //Set hit point to target
            target = hit;
            cam.GetComponent<Pro3DCamera.CameraControl>().enabled = false;

            //Zoom
            scopeImage.enabled = true;
            cam.cullingMask = ~(1 << 11);
            cam.transform.LookAt(hit.point);

            //Rotation
            turretRotation.rotateSpeed = zoomRotateSpeed;
        }
        if (camMove)
        {
            //Scope
            scopeTime += Time.deltaTime;
            if (scopeTime > zoomSpeed && cam.fieldOfView > zoomCamFov)
            {
                inZoomMode = true;
                cam.fieldOfView -= 1;
                damageToLess -= 16.47f;
                totalAmmo -= 0.0625f;
            }
            else
            {
                sniperShootEv.setParameterValue("Scoping", 0f);
            }
        }

        if (zoomReleased && required == true)
        {
            
            damageToLess = 700f;
            inZoomMode = false;
            required = false;
            zoomHeld = false;
            camMove = false;
            GetComponentInParent<RTCTankController>().engineRunning = true;
            Shoot(damage);

            sniperShootEv.setParameterValue("Scoping", 0f);
            sniperShootEv.setParameterValue("Firing", 1f);

            //Deactivating Animator
            animator.enabled = false;
            animator.SetBool("Scoped", false);

            //Unzoom
            CameraTransition(zoomHeld);
            scopeImage.enabled = false;
            laser.enabled = false;
            cam.cullingMask = ~0;
            cam.fieldOfView = initCamFov;
            cam.GetComponent<Pro3DCamera.CameraControl>().enabled = true;

            //Rotation
            turretRotation.rotateSpeed = initRotateSpeed;

            if (scopeTime >= 1f)
            {
                scopeTime = 0;
            }
            
        }
        if (camMove)
        { /*
            if (Input.GetKey(KeyCode.W) == true)
            {
                scope.transform.Rotate(Vector3.left * Time.deltaTime);
                if (scope.transform.rotation.x < -2f)
                {
                    scope.transform.rotation.x.Equals(-1.9f);
                }
            }
            else if (Input.GetKey(KeyCode.S) == true)
            {
                scope.transform.Rotate(Vector3.right * Time.deltaTime);
                if (firePoint.transform.rotation.x > 0.5f)
                {
                    scope.transform.rotation.x.Equals(0.501f);
                }
            }
            else if(Input.GetKey(KeyCode.A) == true)
            {
                gameObject.transform.Rotate(Vector3.down * Time.deltaTime * 5);
            }
            else if (Input.GetKey(KeyCode.D) == true)
            {
                gameObject.transform.Rotate(Vector3.up * Time.deltaTime * 5);
            }*/
            turretRotation.SniperCamRotation(true);
        }
        else
        {
            turretRotation.SniperCamRotation(false);
        }

        if (totalAmmo < 3 && inZoomMode == false)
        {
            reloadTime += Time.deltaTime;

            if(reloadTime > reloadDuration)
            {
                reloadTime = 0f;
                totalAmmo += 0.0625f;
                sniperShootEv.setParameterValue("Firing", 0f);
            }
        }

        if(currentReloadValue != expectedReloadValue)
        {
            if(currentReloadValue > expectedReloadValue)
            {
                coolDownSlider.value = expectedReloadValue;
            }
            if(currentReloadValue < expectedReloadValue)
            {
                coolDownSlider.value += 0.15f;
            }

        }
    }
    #endregion Update

    #region Shoot
    private void Shoot(float currDamage)
    {
        barrelFlash.Play(true);
        TankHealth targetHealth = target.transform.gameObject.GetComponent<TankHealth>();
        
        inZoomMode = false;

        if (targetHealth != null)
        {

            FactionID fID = targetHealth.gameObject.GetComponent<FactionID>();
            FactionID myID = gameObject.GetComponentInParent<FactionID>();

            if (fID == null || fID._teamID == 1 || myID._teamID == 1 || fID._teamID != myID._teamID)
            {
                if (fID.myAccID != myID.myAccID)
                {
                    if (currDamage == damage)
                    {
                        targetHealth.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", target.transform.gameObject.GetComponent<PhotonView>().Owner, currDamage - damageToLess);
                    }
                    else
                    {
                        targetHealth.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", target.transform.gameObject.GetComponent<PhotonView>().Owner, currDamage);
                    }
                }
            }
            

        }
        else
        {
            if(target.transform != null)
            {
                GameObject i = Instantiate(hitEffect, target.point, Quaternion.identity);
                i.transform.rotation = Quaternion.FromToRotation(Vector3.forward, target.normal);
                Destroy(i, 10f);
            }
        }

       // sniperShootEv.setParameterValue("Firing", 0f);

        if (currDamage == damage)
        {
            totalAmmo = 0;
        }
        if(currDamage == tempDamage)
        {
            totalAmmo = 1.0f;
        }
        if (currDamage == (tempDamage - tempDamage/8))
        {
            totalAmmo = 0f;
        }

        damageToLess = 700f;
    }
    #endregion Shoot

    #region Cameras
    private void CameraTransition(bool isZoom)
    {
        if(isZoom)
        {
            if (!cameraMode)
            {
                cameraDefaultPos = cam.transform.localPosition;
                cameraMode = isZoom;
            }
            
            float speed = (Vector3.Distance(cameraDefaultPos, cameraScopePos) / cameraTransitionTime) * Time.deltaTime;
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, cameraScopePos, speed);
        }
        else
        {
            cameraMode = isZoom;
        }
    }

    private void Scope()
    {
        if(camMove)
        {
            cam.transform.localEulerAngles = new Vector3(0, cam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z);
            GetComponentInParent<RTCTankController>().engineRunning = false;
            inZoomMode = true;
            camMove = true;
        }
    }
    #endregion Cameras
}
