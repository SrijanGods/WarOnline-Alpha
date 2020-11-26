using System;
using _Scripts.Controls;
using _Scripts.Photon.Room;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Unity.Jobs;
using Unity.Collections;


namespace _Scripts.Tank.Turrets.Sniper
{
    public class Sniper : MonoBehaviourPun
    {
        public Camera myCamera;

        [Header("Scope")] [SerializeField] private Camera cam;
        [SerializeField] public Transform scope;
        [SerializeField] private Image scopeImage;
        [SerializeField] private float cameraTransitionTime = 1f;
        private Vector3 cameraDefaultPos;
        private Vector3 cameraScopePos;
        private bool cameraMode;
        [SerializeField, Range(0f, 180f)] private float zoomCamFov = 38f;
        [SerializeField, Range(0f, 180f)] private float initCamFov = 64f;
        public float zoomSpeed;
        [SerializeField] private float zoomRotateSpeed = 0.8f;
        public GameObject hitEffect;
        public LayerMask tanksAndObjectsLayer;

        [Header("Turret Stats")] [SerializeField]
        private float damage = 1000f;

        [SerializeField] private float tempDamage = 370f;
        private float range = 10000f;
        private TurretRotation turretRotation;
        private float initRotateSpeed;
        private float damageToLess = 700;

        [Header("Reload System")] [SerializeField]
        private float totalAmmo = 3;

        private bool zoomHeld;
        private bool zoomReleased;
        private bool required;
        private float reloadDuration = 0.25f;
        private float reloadTime;
        private float scopeTime;
        private bool inZoomMode;

        [Header("Sound System")] [FMODUnity.EventRef]
        public string sniperShootSfx = "event:/Sniper";

        FMOD.Studio.EventInstance sniperShootEv;

        [Header("Others")] [SerializeField] private LineRenderer laser;
        [SerializeField] private ParticleSystem barrelFlash;
        private RaycastHit target;

        public TankHealth.TankHealth myTankHealth;
        private Slider _coolDownSlider;
        private Animator animator;
        private bool camMove = false;
        private float currentReloadValue;
        private float expectedReloadValue;
        private float sfxValue;


        private static readonly int Scoped = Animator.StringToHash("Scoped");


        #region Start

        private void Start()
        {
            DBG.BeginMethod("Sniper.Start");
            if (photonView.IsMine || !PhotonNetwork.IsConnected)
            {
                myCamera.gameObject.SetActive(true);
                myCamera.enabled = true;
            }
            else
            {
                myCamera.gameObject.SetActive(false);
            }
            DBG.Log("T1");
            inZoomMode = false;
            required = false;
            //Laser
            laser.enabled = false;

            //Scope
            scopeImage.enabled = false;
            cameraScopePos = scope.localPosition;
            DBG.Log("T2");

            //Rotation
            turretRotation = GetComponent<TurretRotation>();
            initRotateSpeed = turretRotation.rotateSpeed;
            DBG.Log("T3");

            //sfx
            //sniperShootEv = FMODUnity.RuntimeManager.CreateInstance(sniperShootSfx);
            //FMODUnity.RuntimeManager.AttachInstanceToGameObject(sniperShootEv, GetComponent<Transform>(), GetComponent<Rigidbody>());
            //sniperShootEv.start();

            //Interface
            _coolDownSlider = myTankHealth.attackCooldown;
            _coolDownSlider.maxValue = 30f;
            _coolDownSlider.minValue = 0f;
            _coolDownSlider.value = 30f;
            DBG.Log("T4");

            animator = cam.GetComponent<Animator>();
            if(animator != null)
                animator.enabled = false;

            Original_Camera_FieldOfView = cam.fieldOfView;

            OuterReticle = GameObject.Find("OuterReticle");


            //Reload System
            DBG.EndMethod("Sniper.Start");

        }

        #endregion Start

        #region Update

        private void Update()
        {
            // GodsTheGuy_Sniper();

                Sniper_Controller();
        }

        #endregion Update


        void GodsTheGuy_Sniper()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

            if (totalAmmo >= 3)
            {
                sniperShootEv.setParameterByName("ReloadFull", 1f);
            }

            expectedReloadValue = totalAmmo * 10;
            currentReloadValue = _coolDownSlider.value;

            if (SimulatedInput.GetButtonDown(InputCodes.TankFire))
            {
                sniperShootEv.setParameterByName("Firing", 0f);

                if (totalAmmo >= 3)
                {
                    zoomHeld = true;
                }

                if (totalAmmo >= 2.5 && totalAmmo < 3)
                {
                    Shoot(tempDamage);
                }

                if (totalAmmo >= 1.5 && totalAmmo < 2.5)
                {
                    Shoot(tempDamage - tempDamage / 8);
                }
            }

            zoomReleased = SimulatedInput.GetButtonUp(InputCodes.TankFire);

            if (zoomHeld)
            {
                DBG.Log("zoomHeld");
                sniperShootEv.setParameterByName("ReloadFull", 0f);

                required = true;
                //Transition
                if (animator != null)
                {
                    animator.enabled = true;
                    animator.SetBool(Scoped, true);
                }
                sniperShootEv.setParameterByName("Scoping", 1f);

                camMove = true;
                CameraTransition(zoomHeld);
                Scope();

                // Laser
                laser.enabled = true;
                var position = scope.position;
                laser.SetPosition(0, position);
                laser.SetPosition(1, position + scope.forward * range);

                if (Physics.Raycast(position, scope.forward, out var hit, range, tanksAndObjectsLayer))
                {
                    laser.SetPosition(1, hit.point);

                    //Set hit point to target
                    target = hit;
                    //cam.GetComponent<Pro3DCamera.CameraControl>().enabled = false;

                    //Zoom
                    scopeImage.enabled = true;
                    cam.cullingMask = ~(1 << 11);
                    cam.transform.LookAt(hit.point);
                }

                //Rotation
                turretRotation.rotateSpeed = zoomRotateSpeed;
            }

            if (camMove)
            {
                DBG.Log("camMove");

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
                    sniperShootEv.setParameterByName("Scoping", 0f);
                }
            }

            if (zoomReleased && required)
            {
                DBG.Log("zoomReleased  && required");

                damageToLess = 700f;
                inZoomMode = false;
                required = false;
                zoomHeld = false;
                camMove = false;
                //GetComponentInParent<RTCTankController>().engineRunning = true;
                Shoot(damage);

                sniperShootEv.setParameterByName("Scoping", 0f);
                sniperShootEv.setParameterByName("Firing", 1f);

                //Deactivating Animator
                if (animator != null)
                {
                    animator.enabled = false;
                    animator.SetBool(Scoped, false);
                }

                //Unzoom
                CameraTransition(zoomHeld);
                scopeImage.enabled = false;
                laser.enabled = false;
                cam.cullingMask = ~0;
                cam.fieldOfView = initCamFov;
                //cam.GetComponent<Pro3DCamera.CameraControl>().enabled = true;

                //Rotation
                turretRotation.rotateSpeed = initRotateSpeed;

                if (scopeTime >= 1f)
                {
                    scopeTime = 0;
                }
            }

            DBG.Log("can move: " + camMove);

            turretRotation.SniperCamVerticalRotation();

            if (totalAmmo < 3 && inZoomMode == false)
            {
                reloadTime += Time.deltaTime;

                if (reloadTime > reloadDuration)
                {
                    reloadTime = 0f;
                    totalAmmo += 0.0625f;
                    sniperShootEv.setParameterByName("Firing", 0f);
                }
            }

            if (Math.Abs(currentReloadValue - expectedReloadValue) > .001f)
            {
                if (currentReloadValue > expectedReloadValue)
                {
                    _coolDownSlider.value = expectedReloadValue;
                }

                if (currentReloadValue < expectedReloadValue)
                {
                    _coolDownSlider.value += 0.15f;
                }
            }
        }

        [ReadOnly]
        public float Original_Camera_FieldOfView; // set on start
        public float ScopeZoomTimeElapsed = 0.1f;
        public float TimeToReachFullZoom = 4;
        public float TimeToReturnToNormalZoom = 4;
        public bool Scope_Token = false;
        public bool Cancel_Scope_Token = false;
        public bool Shoot_Token = false;
        float Scope_Start_Time = 0;
        float Scope_Ended_Time = 0;

        GameObject OuterReticle;

        public enum States { START, THIRD_PERSON_CAMERA, SCOPING, SHOOT}
        public States State = States.START;
        /// <summary>
        /// This controller uses a Finite State Machine each case is a State. See provided Visual Automaton. by Alexander Z.
        /// </summary>
        public void Sniper_Controller()
        {
            DBG.BeginFSM("Network_Controller");
            switch (State) {
                case States.START:
                   // ScopeZoomTimeElapsed = TimeToReturnToNormalZoom;
                    Scope_Ended_Time = Time.time;
                    State = States.THIRD_PERSON_CAMERA;
                    DBG.LogTransition("START -> THIRD_PERSON_CAMERA");
                    Debug.Assert(GameObject.Find("DBG") != null);
                    break;
                case States.THIRD_PERSON_CAMERA://Default state of turret rotation.
                    ScopeZoomTimeElapsed = Mathf.Clamp(Time.time - Scope_Ended_Time, .01f, TimeToReturnToNormalZoom);//Transition from zoomed to unzoomed is 1f sconds
                    cam.fieldOfView = Mathf.Lerp(Original_Camera_FieldOfView -10, Original_Camera_FieldOfView  , ScopeZoomTimeElapsed/ TimeToReturnToNormalZoom);//Transition from zoomed to unzoomed is TimeToReturnToNormalZoom sconds
                    //ScopeZoomTimeElapsed += Time.deltaTime;
                    if (SimulatedInput.GetButtonDown(InputCodes.TankFire) || Input.GetKey(KeyCode.Z))
                    {
                        Scope_Token = true;
                    }
                    if (Scope_Token.ConsumeToken())
                    {
                        ScopeZoomTimeElapsed = 0.01f;// we gonna zoom from this point in time so we reset to TimeElapsed, not to 0 because we would be divinding by zero
                        Scope_Start_Time = Time.time;//Because we are going from "THIRD_PERSON_CAMERA -> SCOPING" We save time to start counting to meassure damage %
                        scopeImage.enabled = true;
                        Debug.Assert(laser != null);

                        laser.enabled = true;
                        cam.cullingMask = ~(1 << 11);

                        GameObject.FindObjectOfType<RTC_UIDragController>().transform.position = GameObject.Find("LeftHand Aim Drag Position").transform.position;
                        //Stop pressing joystick and deactivate
                        OuterReticle.GetComponentInChildren<RTC_UIJoystickController>().ResetJoystick();
                        OuterReticle.SetActive(false);

                        State = States.SCOPING;
                        DBG.LogTransition("THIRD_PERSON_CAMERA -> SCOPING");
                        GameObject.Find("DBG").GetComponent<Text>().text = "THIRD_PERSON_CAMERA -> SCOPING";

                    }
                    _coolDownSlider.value += (_coolDownSlider.value < _coolDownSlider.maxValue) ? 0.15f : 0;// If cooldown not full increase by
                    break;
                case States.SCOPING:
                    turretRotation.SniperCamVerticalRotation();
                    //turretRotation.DoMovements();
                    // var y = SimulatedInput.GetAxis(InputCodes.MouseLookY);

                    //This gradually zooms
                    ScopeZoomTimeElapsed = Mathf.Clamp(Time.time - Scope_Start_Time, .01f, TimeToReachFullZoom);//Transition from unzoomed to zoomed is [TimeToReachFullZoom]
                    cam.fieldOfView = Mathf.Lerp(Original_Camera_FieldOfView, Original_Camera_FieldOfView - 10f, ScopeZoomTimeElapsed / TimeToReachFullZoom);//Transition from unzoomed to zoomed is [TimeToReachFullZoom]
                   // ScopeZoomTimeElapsed += Time.deltaTime;

                    //lazer
                    var position = scope.position;
                    laser.SetPosition(0, position);
                    laser.SetPosition(1, position + scope.forward * range);

                    if (SimulatedInput.GetButtonUp(InputCodes.TankFire) && Input.GetKey(KeyCode.Z) == false)
                    {
                        Shoot_Token = true;
                        GameObject.Find("DBG").GetComponent<Text>().text += "Shoot_Token: " + Shoot_Token; 
                    }

                    if (Cancel_Scope_Token.ConsumeToken())
                    {
                        ScopeZoomTimeElapsed = 0.1f;// we gonna unzoom from this point in time so we reset to TimeElapsed, not to 0 because we would be divinding by zero
                        scopeImage.enabled = false;
                        Scope_Ended_Time = Time.time;
                        GameObject.FindObjectOfType<RTC_UIDragController>().transform.position = GameObject.Find("RightHand Aim Drag Position").transform.position;

                        State = States.THIRD_PERSON_CAMERA;
                        DBG.LogTransition("SCOPING -> THIRD_PERSON_CAMERA");
                        GameObject.Find("DBG").GetComponent<Text>().text = "SCOPING -> THIRD_PERSON_CAMERA";
                    } else if (Shoot_Token.ConsumeToken())
                    {
                        ScopeZoomTimeElapsed = 0.01f;// we gonna unzoom from this point in time so we reset to TimeElapsed, not to 0 because we would be divinding by zero
                        scopeImage.enabled = false;
                        Scope_Ended_Time = Time.time;
                        GameObject.FindObjectOfType<RTC_UIDragController>().transform.position = GameObject.Find("RightHand Aim Drag Position").transform.position;
                        OuterReticle.SetActive(true);

                        State = States.SHOOT;
                        DBG.LogTransition("SCOPING -> SHOOT");
                        GameObject.Find("DBG").GetComponent<Text>().text = "SCOPING -> SHOOT";

                    }
                    break;
                case States.SHOOT:
                    //sniperShootEv.setParameterByName("Scoping", 0f);
                    //sniperShootEv.setParameterByName("Firing", 1f);

                    if (Time.time - Scope_Start_Time >= 4)
                        Shoot(damage);//full damage if scoped full time
                    else
                        Shoot(damage * (Time.time - Scope_Start_Time) / 4);//fraction of damage, depending on how many seconds of scoping

                    //Unzoom
                    scopeImage.enabled = false;
                    laser.enabled = false;
                    cam.cullingMask = ~0;
                    //cam.fieldOfView = initCamFov;
                    //cam.GetComponent<Pro3DCamera.CameraControl>().enabled = true;

                    //Rotation
                    turretRotation.rotateSpeed = initRotateSpeed;
                    scopeTime = 0;

                    _coolDownSlider.value = 0;

                    State = States.THIRD_PERSON_CAMERA;
                    DBG.LogTransition("SHOOT -> THIRD_PERSON_CAMERA");
                    GameObject.Find("DBG").GetComponent<Text>().text = "SHOOT -> THIRD_PERSON_CAMERA";

                    break;
                default:
                    Debug.LogError("Invalid State, Assign Sniper.State");
                    break;

            }
            DBG.EndFSM();
        }

        #region Shoot

        private void Shoot(float currDamage)
        {
            barrelFlash.Play(true);

            if (Physics.Raycast(scope.position, scope.forward, out var hit, range, tanksAndObjectsLayer))
            {
                laser.SetPosition(1, hit.point);

                //Set hit point to target
                target = hit;
                //cam.GetComponent<Pro3DCamera.CameraControl>().enabled = false;

                //Zoom
                scopeImage.enabled = true;
                cam.cullingMask = ~(1 << 11);
                cam.transform.LookAt(hit.point);
          

                if (target.transform.GetComponent<TankHealth.TankHealth>() == null)//Catch if we dont hit a tank
                    return;

                TankHealth.TankHealth targetHealth = target.transform.GetComponent<TankHealth.TankHealth>();

                inZoomMode = false;

                if (targetHealth)
                {
                    FactionID fID = targetHealth.fid;
                    FactionID myID = myTankHealth.fid;

                    if ((fID.teamIndex == -1 && myID.teamIndex == -1) || fID.teamIndex != myID.teamIndex)
                    {
                        if (fID.myAccID != myID.myAccID)
                        {
                            if (Math.Abs(currDamage - damage) < .005f)
                            {
                                targetHealth.TakeDamage(currDamage - damageToLess, myID.actorNumber);
                            }
                            else
                            {
                                targetHealth.TakeDamage(currDamage, myID.actorNumber);
                            }
                        }
                    }
                }
                else
                {
                    if (target.transform)
                    {
                        photonView.RPC(nameof(HitEffect), RpcTarget.All, target.point, target.normal);
                    }
                }

                // sniperShootEv.setParameterByName("Firing", 0f);

                if (Math.Abs(currDamage - damage) < .001f)
                {
                    totalAmmo = 0;
                }

                if (Math.Abs(currDamage - tempDamage) < .001f)
                {
                    totalAmmo = 1.0f;
                }

                if (Math.Abs(currDamage - (tempDamage - tempDamage / 8)) < .001f)
                {
                    totalAmmo = 0f;
                }

                damageToLess = 700f;
            }
        }

        [PunRPC]
        public void HitEffect(Vector3 pos, Vector3 rot)
        {
            GameObject i = Instantiate(hitEffect, pos, Quaternion.identity);
            i.transform.up = rot;
            Destroy(i, 10f);
        }

        #endregion Shoot

        #region Cameras

        private void CameraTransition(bool isZoom)
        {
            DBG.BeginMethod("CameraTransition");
            if (isZoom)
            {
                if (!cameraMode)
                {
                    cameraDefaultPos = cam.transform.localPosition;
                    cameraMode = true;
                }

                float speed = (Vector3.Distance(cameraDefaultPos, cameraScopePos) / cameraTransitionTime) *
                              Time.deltaTime;
                cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, cameraScopePos, speed);
            }
            else
            {
                cameraMode = false;
            }
            DBG.EndMethod("CameraTransition");
        }

        private void Scope()
        {
            if (camMove)
            {
                // cam.transform.localEulerAngles = new Vector3(0, cam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z);
                //GetComponentInParent<RTCTankController>().engineRunning = false;
                inZoomMode = true;
                camMove = true;
            }
        }

        #endregion Cameras
    }
}