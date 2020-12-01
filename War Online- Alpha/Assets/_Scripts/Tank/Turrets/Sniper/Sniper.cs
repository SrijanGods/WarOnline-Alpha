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

        [Header("Scope")] 
        [SerializeField] private Camera cam;
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

        [Header("Sound System COMMENTED OUT")]
        [FMODUnity.EventRef]
        public string sniperShootSfx = "event:/Sniper";

        FMOD.Studio.EventInstance sniperShootEv;

        [Header("Others")] 
        [SerializeField] private LineRenderer laser;
        [SerializeField] private ParticleSystem barrelFlash;
        private RaycastHit target;

        public TankHealth.TankHealth myTankHealth;
        private Slider _coolDownSlider;
        private bool camMove = false;
        private float currentReloadValue;
        private float expectedReloadValue;
        private float sfxValue;

        [Header("Inspector set Values for Sniper_Controller() Alexander Z.")] [SerializeField]  

        private Animator camera_sniper_animator;

        public GameObject Tank_Reference ;//Set in the inspector
        public MeshRenderer Body1;
        public MeshRenderer Parts1;
        public MeshRenderer SniperBody1;
        public MeshRenderer SniperParts7;


        private Material OriginalBodyMatte;
        private Material OriginalPartsBase;
        public Material TranslucentBodyMatte;
        public Material TranslucentPartsBase;

        public float NormalizedPercent_ShootConsumption = .5f; //Can be overwritten in the inspector
        //float ScopeZoomingDuration= 10;///You need to set zoomduration in the sniper animator Transition.

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

            camera_sniper_animator = cam.GetComponent<Animator>(); //This is because the Camera is child of SniperTurret
            //ChangeTransitionDuration("THIRD_PERSON_CAMERA", "SCOPE_ZOOMING", 0, ScopeZoomingDuration, camera_sniper_animator);
            //if(animator != null)
            //    animator.enabled = false;

            Original_Camera_FieldOfView = cam.fieldOfView;

            OuterReticle = GameObject.Find("OuterReticle");

            //We save up original materials so that we may swap for translucent when scoping. And back to it.
            OriginalBodyMatte = Body1.material;
            OriginalPartsBase = Parts1.material;
            //OriginalSniperBodyMatte = SniperBody1.material;

           // fire_button = new CustomButton();
           // fire_button.Anchor = 1;
            
           // fire_button.Dimensions = new Vector2(10, 10);
           // fire_button.enabled = true;
           // fire_button.name = "firebutton";
           //// fire_button.image = new Texture2D(10, 10);
           // fire_button.Position = new Vector2(-9, -10);
           //     fire_button.visible_enabled();
           // fire_button.IsVisible = true;
           // fire_button.image = button;
           // fire_button.OnPress += AH;

           // fire_button.Start();
            //Reload System
            DBG.EndMethod("Sniper.Start");

        }
        //CustomButton fire_button; //AUXILIARY CUSTOM BUTTON BECAUSE ROCKETGAMES INPUT FAILED
        //public Texture2D button;
        #endregion Start
        //public void AH()
        //{
        //    Shoot_Token = true;
        //    Debug.Log("HAHAHA");
        //}
        #region Update

        private void Update()
        {
            // GodsTheGuy_Sniper();
            Sniper_Controller_v3();
            //     Sniper_Controller();
        }

        #endregion Update

        //private void OnGUI()
        //{
        //    fire_button.OnGUI();


        //}
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
                //
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
                if (camera_sniper_animator != null)
                {
                    camera_sniper_animator.enabled = true;
                    camera_sniper_animator.SetBool(Scoped, true);
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
                if (camera_sniper_animator != null)
                {
                    camera_sniper_animator.enabled = false;
                    camera_sniper_animator.SetBool(Scoped, false);
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
        public float TimeToReachFullZoom = 4;
        public float TimeToReturnToNormalZoom = 4;
        public bool Scope_Token = false;
        public bool Cancel_Scope_Token = false;
        public bool Shoot_Token = false;
        //float Scope_Start_Time = 0;
        //float Scope_Ended_Time = 0;

        GameObject OuterReticle;

        public enum States { START, THIRD_PERSON_CAMERA, SCOPE_ZOOMING, SCOPE_FULL_ZOOM, SHOOT, MOUNTING_SCOPE,SCOPE_MOUNTED, SCOPE_FULLY_ZOOMED, SHOOT_DELAY}
        public States State = States.START;
        /// <summary>
        /// This controller uses a Finite State Machine each case is a State. See provided Visual Automaton. by Alexander Z.
        /// </summary>
        public void Sniper_Controller()
        {
            DBG.BeginFSM("Sniper_Controller");
            switch (State) {
                case States.START:
                    State = States.THIRD_PERSON_CAMERA;
                    DBG.LogTransition("START -> THIRD_PERSON_CAMERA");
                    Debug.Assert(GameObject.Find("DBG") != null);
                    break;
                case States.THIRD_PERSON_CAMERA://Default state of turret rotation.
                    _coolDownSlider.value += (_coolDownSlider.value < _coolDownSlider.maxValue) ? 0.15f : 0;// If cooldown not full increase by

                    if (SimulatedInput.GetButtonDown(InputCodes.TankFire) || Input.GetKey(KeyCode.Z))
                    {
                       // Debug.Log(" >>>Scope_Token = true");
                        Scope_Token = true;
                    }

                    if (_coolDownSlider.value == _coolDownSlider.maxValue && Scope_Token.ConsumeToken() )
                    {
                        scopeImage.enabled = true;
                        laser.enabled = true;

                        //MainCamera
                        camera_sniper_animator.SetBool("Scope", true);

                        //Make tank translucent
                        Body1.material = TranslucentBodyMatte;
                        SniperBody1.material = TranslucentBodyMatte;
                        Parts1.material = TranslucentPartsBase;
                        SniperParts7.material = TranslucentPartsBase;

                        GameObject.FindObjectOfType<RTC_UIDragController>().transform.position = GameObject.Find("LeftHand Aim Drag Position").transform.position;// This repositions the Drag Area to the Leftt hand positin
                        //Stop pressing joystick and deactivate
                        OuterReticle.GetComponentInChildren<RTC_UIJoystickController>().ResetJoystick();
                        OuterReticle.SetActive(false);

                        State = States.SCOPE_ZOOMING;
                        DBG.LogTransition("THIRD_PERSON_CAMERA -> SCOPE_ZOOMING");
                        GameObject.Find("DBG").GetComponent<Text>().text = "THIRD_PERSON_CAMERA -> SCOPE_ZOOMING";

                    }
                    else if(SimulatedInput.GetButtonUp(InputCodes.TankFire) && Input.GetKey(KeyCode.Z) == false && Scope_Token.ConsumeToken())
                    {
                        State = States.SHOOT;
                        DBG.LogTransition("THIRD_PERSON_CAMERA -> SHOOT");
                        GameObject.Find("DBG").GetComponent<Text>().text = "THIRD_PERSON_CAMERA -> SHOOT";
                    }
                    break;

                case States.SCOPE_ZOOMING:
                    turretRotation.SniperCamVerticalRotation();

                    //Set laser
                    laser.SetPosition(0, scope.position);
                    laser.SetPosition(1, scope.position + scope.forward * range);


                    if (SimulatedInput.GetButtonUp(InputCodes.TankFire) && Input.GetKey(KeyCode.Z) == false)
                    {
                        Shoot_Token = true;
                        GameObject.Find("DBG").GetComponent<Text>().text += "Shoot_Token: " + Shoot_Token;
                    }
                    if (Shoot_Token.ConsumeToken())
                    {
                        scopeImage.enabled = false;
                        GameObject.FindObjectOfType<RTC_UIDragController>().transform.position = GameObject.Find("RightHand Aim Drag Position").transform.position;
                        OuterReticle.SetActive(true);

                        Body1.material = OriginalBodyMatte;
                        SniperBody1.material = OriginalBodyMatte;
                        Parts1.material = OriginalPartsBase;
                        SniperParts7.material = OriginalPartsBase;

                        camera_sniper_animator.SetBool("Scope", false);
                        DBG.Log("Scope? " + camera_sniper_animator.GetBool("Scope"));

                        State = States.SHOOT;
                        DBG.LogTransition("SCOPE_ZOOMING -> SHOOT");
                        GameObject.Find("DBG").GetComponent<Text>().text = "SCOPE_ZOOMING -> SHOOT";
                    }
                    else if (camera_sniper_animator.GetCurrentAnimatorStateInfo(0).IsName("SCOPING"))
                    {
                        State = States.SCOPE_FULL_ZOOM;
                        DBG.LogTransition("SCOPE_ZOOMING -> SCOPE_FULL_ZOOM");
                        GameObject.Find("DBG").GetComponent<Text>().text = "SCOPE_ZOOMING -> SCOPE_FULL_ZOOM";
                    }
                    else
                    {
                        //When you transition normalizedTime==0, so only execute this or bar will go back to Full. Right when you Transition to FULL ZOOM
                        AnimatorTransitionInfo transitioninfo = camera_sniper_animator.GetAnimatorTransitionInfo(0);
                        _coolDownSlider.value = _coolDownSlider.maxValue * (1 - transitioninfo.normalizedTime);
                    }
                    break;
                case States.SCOPE_FULL_ZOOM:
                    turretRotation.SniperCamVerticalRotation();

                      //lazer
                    laser.SetPosition(0, scope.position);
                    laser.SetPosition(1, scope.position + scope.forward * range);

                    if (SimulatedInput.GetButtonUp(InputCodes.TankFire) && Input.GetKey(KeyCode.Z) == false)
                    {
                        Shoot_Token = true;
                        GameObject.Find("DBG").GetComponent<Text>().text += "Shoot_Token: " + Shoot_Token;
                    }

                    if (Shoot_Token.ConsumeToken())
                    {
                        //ScopeZoomTimeElapsed = 0.01f;// we gonna unzoom from this point in time so we reset to TimeElapsed, not to 0 because we would be divinding by zero
                        scopeImage.enabled = false;
                        GameObject.FindObjectOfType<RTC_UIDragController>().transform.position = GameObject.Find("RightHand Aim Drag Position").transform.position;
                        OuterReticle.SetActive(true);

                        Body1.material = OriginalBodyMatte;
                        SniperBody1.material = OriginalBodyMatte;
                        Parts1.material = OriginalPartsBase;
                        SniperParts7.material = OriginalPartsBase;

                        camera_sniper_animator.SetBool("Scope", false);

                        State = States.SHOOT;
                        DBG.LogTransition("SCOPE_FULL_ZOOM -> SHOOT");
                        GameObject.Find("DBG").GetComponent<Text>().text = "SCOPE_FULL_ZOOM -> SHOOT";

                    }
                    break;
                case States.SHOOT:
                    //sniperShootEv.setParameterByName("Scoping", 0f);
                    //sniperShootEv.setParameterByName("Firing", 1f);

                    Shoot(damage * _coolDownSlider.value/_coolDownSlider.maxValue);//fraction of damage, depending on how many seconds of scoping
                    //DBG.Log("_coolDownSlider.value: " + _coolDownSlider.value);
                    //DBG.Log("_coolDownSlider.maxValue/2: " + _coolDownSlider.maxValue * NormalizedPercent_ShootConsumption);

                    _coolDownSlider.value = _coolDownSlider.value - _coolDownSlider.maxValue * NormalizedPercent_ShootConsumption;
                    //DBG.Log("_coolDownSlider.value: " + _coolDownSlider.value);

                    //Unzoom
                    scopeImage.enabled = false;
                    laser.enabled = false;
                    //cam.cullingMask = ~0;
                    //cam.fieldOfView = initCamFov;
                    //cam.GetComponent<Pro3DCamera.CameraControl>().enabled = true;

                    //Rotation
                    turretRotation.rotateSpeed = initRotateSpeed;
                    scopeTime = 0;


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

        public float MinimumShotingCooldown_Normalized = .40f;
        void PointerDownInShootAuxilary() {
            Shoot_Token = true;
        }
        public Button ShootAuxilaryButton;
        private float start_shoot_delay = 0;
        public float Shoot_Delay = 1;
        public void Sniper_Controller_v3()
        {
            DBG.BeginFSM("Sniper_Controller_v3");
            switch (State)
            {
                case States.START:
                    State = States.THIRD_PERSON_CAMERA;
                     
                    DBG.LogTransition("START -> THIRD_PERSON_CAMERA");
                    Debug.Assert(GameObject.Find("DBG") != null);
                    GameObject.Find("DBG").GetComponent<Text>().text = "START -> THIRD_PERSON_CAMERA";
                    break;
                case States.THIRD_PERSON_CAMERA://Default state of turret rotation.
                   // GameObject.Find("DBG").GetComponent<Text>().text = "_coolDownSlider == null? " + (_coolDownSlider== null);

                    _coolDownSlider.value += (_coolDownSlider.value < _coolDownSlider.maxValue) ? 0.15f : 0;// If cooldown not full increase by

                    if (camera_sniper_animator.GetCurrentAnimatorStateInfo(0).IsName("THIRD_PERSON_CAMERA"))
                    {
                        if (_coolDownSlider.value > _coolDownSlider.maxValue * MinimumShotingCooldown_Normalized)//Only shoot is you got more than a percentage of the max cooldown slider. Normalized means the value is betwoo
                        {
                            if (SimulatedInput.GetButtonDown(InputCodes.TankFire))
                            {
                                Shoot_Token = true;
                            }

                            if (_coolDownSlider.value == _coolDownSlider.maxValue && Shoot_Token.ConsumeToken() || Input.GetKey(KeyCode.Z))
                            {
                                scopeImage.enabled = true;
                                laser.enabled = true;

                                //MainCamera
                                camera_sniper_animator.SetBool("Shoot", true);

                                Body1.material = TranslucentBodyMatte;
                                SniperBody1.material = TranslucentBodyMatte;
                                Parts1.material = TranslucentPartsBase;
                                SniperParts7.material = TranslucentPartsBase;

                                GameObject.FindObjectOfType<RTC_UIDragController>().transform.position = GameObject.Find("LeftHand Aim Drag Position").transform.position;// This repositions the Drag Area to the Leftt hand positin

                                OuterReticle.GetComponentInChildren<RTC_UIJoystickController>().ResetJoystick();//Stops pressing joystick and deactivates it
                                OuterReticle.SetActive(false);

                                State = States.MOUNTING_SCOPE;
                                DBG.LogTransition("THIRD_PERSON_CAMERA -> MOUNTING_SCOPE");
                                GameObject.Find("DBG").GetComponent<Text>().text = "THIRD_PERSON_CAMERA -> MOUNTING_SCOPE";
                            }
                            else if (SimulatedInput.GetButtonUp(InputCodes.TankFire) && Input.GetKey(KeyCode.Z) == false && Shoot_Token.ConsumeToken())//Has pressed down. And is now releasing.
                            {
                                camera_sniper_animator.SetBool("Shoot", false);

                                State = States.SHOOT;
                                DBG.LogTransition("THIRD_PERSON_CAMERA -> SHOOT");
                            }
                        }
                    }
                    break;
                case States.MOUNTING_SCOPE:
                    turretRotation.SniperCamVerticalRotation();

                    //Set laser
                    laser.SetPosition(0, scope.position);
                    laser.SetPosition(1, scope.position + scope.forward * range);

                    if (SimulatedInput.GetButtonUp(InputCodes.TankFire) && Input.GetKey(KeyCode.Z) == false)
                    {
                        scopeImage.enabled = false;
                        GameObject.FindObjectOfType<RTC_UIDragController>().transform.position = GameObject.Find("RightHand Aim Drag Position").transform.position;
                        OuterReticle.SetActive(true);

                        Body1.material = OriginalBodyMatte;
                        SniperBody1.material = OriginalBodyMatte;
                        Parts1.material = OriginalPartsBase;
                        SniperParts7.material = OriginalPartsBase;

                        //camera_sniper_animator.SetBool("Shoot", false);
                        //DBG.Log("Scope? " + camera_sniper_animator.GetBool("Scope"));

                        State = States.SHOOT;
                        DBG.LogTransition("MOUNTING_SCOPE -> SHOOT");
                        GameObject.Find("DBG").GetComponent<Text>().text = "MOUNTING_SCOPE -> SHOOT";
                    }
                    else if (camera_sniper_animator.GetCurrentAnimatorStateInfo(0).IsName("SCOPING"))
                    {
                        State = States.SCOPE_MOUNTED;
                        DBG.LogTransition("MOUNTING_SCOPE -> SCOPE_MOUNTED");
                        GameObject.Find("DBG").GetComponent<Text>().text = "MOUNTING_SCOPE -> SCOPE_MOUNTED";
                    }
                    break;
                case States.SCOPE_MOUNTED:
                    transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, transform.localEulerAngles.z);//Set rotation around the X axis to neutral/ straight. Facing the horizon.
                    State = States.SCOPE_ZOOMING;
                    DBG.LogTransition("SCOPE_MOUNTED -> SCOPE_ZOOMING");
                    GameObject.Find("DBG").GetComponent<Text>().text = "SCOPE_MOUNTED -> SCOPE_ZOOMING";
 
                    break;
                case States.SCOPE_ZOOMING:
                    turretRotation.SniperCamVerticalRotation();

                    //Set laser
                    laser.SetPosition(0, scope.position);
                    laser.SetPosition(1, scope.position + scope.forward * range);

                    if (SimulatedInput.GetButtonUp(InputCodes.TankFire) && Input.GetKey(KeyCode.Z) == false)
                    {
                        scopeImage.enabled = false;
                        GameObject.FindObjectOfType<RTC_UIDragController>().transform.position = GameObject.Find("RightHand Aim Drag Position").transform.position;
                        OuterReticle.SetActive(true);

                        Body1.material = OriginalBodyMatte;
                        SniperBody1.material = OriginalBodyMatte;
                        Parts1.material = OriginalPartsBase;
                        SniperParts7.material = OriginalPartsBase;

                        //camera_sniper_animator.SetBool("Shoot", false);

                        State = States.SHOOT;
                        DBG.LogTransition("SCOPE_ZOOMING -> SHOOT");
                        GameObject.Find("DBG").GetComponent<Text>().text = "SCOPE_ZOOMING -> SHOOT";
                    }
                    else if (camera_sniper_animator.GetCurrentAnimatorStateInfo(0).IsName("ZOOMING"))
                    {
                        State = States.SCOPE_FULL_ZOOM;
                        DBG.LogTransition("SCOPE_ZOOMING -> SCOPE_FULL_ZOOM");
                        GameObject.Find("DBG").GetComponent<Text>().text = "SCOPE_ZOOMING -> SCOPE_FULL_ZOOM";
                    }
                    else
                    {
                        //When you transition normalizedTime==0, so only execute this or bar will go back to Full. Right when you Transition to FULL ZOOM
                        AnimatorTransitionInfo transitioninfo = camera_sniper_animator.GetAnimatorTransitionInfo(0);
                        _coolDownSlider.value = _coolDownSlider.maxValue * (1 - transitioninfo.normalizedTime);
                    }
                    break;
                case States.SCOPE_FULL_ZOOM:

                    turretRotation.SniperCamVerticalRotation();

                    //lazer
                    laser.SetPosition(0, scope.position);
                    laser.SetPosition(1, scope.position + scope.forward * range);

                    if (SimulatedInput.GetButtonUp(InputCodes.TankFire))
                    {
                        //ScopeZoomTimeElapsed = 0.01f;// we gonna unzoom from this point in time so we reset to TimeElapsed, not to 0 because we would be divinding by zero
                        scopeImage.enabled = false;
                        GameObject.FindObjectOfType<RTC_UIDragController>().transform.position = GameObject.Find("RightHand Aim Drag Position").transform.position;
                        OuterReticle.SetActive(true);

                        Body1.material = OriginalBodyMatte;
                        SniperBody1.material = OriginalBodyMatte;
                        Parts1.material = OriginalPartsBase;
                        SniperParts7.material = OriginalPartsBase;

                        //camera_sniper_animator.SetBool("Shoot", false);

                        State = States.SHOOT;
                        DBG.LogTransition("SCOPE_FULL_ZOOM -> SHOOT");
                        GameObject.Find("DBG").GetComponent<Text>().text = "SCOPE_FULL_ZOOM -> SHOOT";

                    }

                    break;
                case States.SHOOT:
                    //sniperShootEv.setParameterByName("Scoping", 0f);
                    //sniperShootEv.setParameterByName("Firing", 1f);

                    Shoot(damage * _coolDownSlider.value / _coolDownSlider.maxValue);//fraction of damage, depending on how many seconds of scoping
                    DBG.Log("_coolDownSlider.value: " + _coolDownSlider.value);
                    DBG.Log("_coolDownSlider.maxValue*NormalizedPercent_ShootConsumption " + _coolDownSlider.maxValue * NormalizedPercent_ShootConsumption);

                    _coolDownSlider.value = _coolDownSlider.value - _coolDownSlider.maxValue * NormalizedPercent_ShootConsumption;
                    DBG.Log("_coolDownSlider.value: " + _coolDownSlider.value);

                    //Unzoom
                    scopeImage.enabled = false;
                    laser.enabled = false;
                    //cam.cullingMask = ~0;
                    //cam.fieldOfView = initCamFov;
                    //cam.GetComponent<Pro3DCamera.CameraControl>().enabled = true;
                    camera_sniper_animator.SetBool("Shoot", false);

                    //Rotation
                    turretRotation.rotateSpeed = initRotateSpeed;
                    scopeTime = 0;

                    start_shoot_delay = Time.time;
                    State = States.SHOOT_DELAY;
                    DBG.LogTransition("SHOOT -> SHOOT_DELAY");
                    GameObject.Find("DBG").GetComponent<Text>().text = "SHOOT -> SHOOT_DELAY";
                    break;
                case States.SHOOT_DELAY:
                    transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, transform.localEulerAngles.z);//Set rotation around the X axis to neutral/ straight. Facing the horizon.

                    _coolDownSlider.value += (_coolDownSlider.value < _coolDownSlider.maxValue) ? 0.15f : 0;// If cooldown not full increase by
 
                    if (Time.time - start_shoot_delay >= Shoot_Delay)
                    {
                        State = States.THIRD_PERSON_CAMERA;
                        DBG.LogTransition("SHOOT_DELAY -> THIRD_PERSON_CAMERA");
                        GameObject.Find("DBG").GetComponent<Text>().text = "SHOOT_DELAY -> THIRD_PERSON_CAMERA";
                    }
                    break;
                default:
                    Debug.LogError("Invalid State, Assign Sniper.State");
                    break;

            }
            DBG.EndFSM();
        }

        #region Shoot



        //public static void ChangeTransitionDuration(string stateName, string transitionName, int layerIndex, float desiredDuration, Animator animator)
        //{
        //    // Get a reference to the Animator Controller
        //    UnityEditor.Animations.AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

        //    int layerCount = ac.layers.Length;
        //    if (layerCount <= layerIndex)  // Error chek
        //    {
        //        Debug.Log("The layer index does not exist");
        //       return;
        //    }


        //    UnityEditor.Animations.AnimatorStateMachine sm = ac.layers[layerIndex].stateMachine;
        //    UnityEditor.Animations.ChildAnimatorState[] states = sm.states;
        //    foreach (UnityEditor.Animations.ChildAnimatorState s in states)
        //    {
        //        //Debug.Log(string.Format("State: {0}", s.state.name));
        //        if (s.state.name == stateName)
        //        {
        //            foreach (UnityEditor.Animations.AnimatorStateTransition t in s.state.transitions)
        //            {
        //                if (t.name == transitionName)
        //                {
        //                    Debug.Log(string.Format("Changing {0} duration value", transitionName));
        //                    t.duration = desiredDuration;
        //                }
        //                //Debug.Log(string.Format("{0} transtion: {1}", s.state.name,t.name));
        //            }
        //        }
        //    }
        //}

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

                sniperShootEv.setParameterByName("Firing", 0f);

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
            //DBG.BeginMethod("CameraTransition");
            if (isZoom)
            {
                if (!cameraMode)
                {
                    cameraDefaultPos = cam.transform.localPosition;
                    cameraMode = true;
                }

                float speed = (Vector3.Distance(cameraDefaultPos, cameraScopePos) / cameraTransitionTime) *
                              Time.deltaTime;
                DBG.Log("cameraDefaultPos: "  + cameraDefaultPos);
                DBG.Log("cameraScopePos: " + cameraScopePos);
                DBG.Log("cameraTransitionTime: " + cameraTransitionTime);


                cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, cameraScopePos, speed);
            }
            else
            {
                cameraMode = false;
            }
            //DBG.EndMethod("CameraTransition");
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