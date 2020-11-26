using System;
using _Scripts.Controls;
using _Scripts.Photon.Room;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Tank.Turrets.Sniper
{
    public class SniperShooting : MonoBehaviour
    {
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

        private TankHealth.TankHealth _myTankHealth;
        private Slider _coolDownSlider;
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
            _myTankHealth = GetComponentInParent<TankHealth.TankHealth>();
            _coolDownSlider = _myTankHealth.attackCooldown;
            _coolDownSlider.maxValue = 30f;
            _coolDownSlider.minValue = 0f;
            _coolDownSlider.value = 30f;
            animator = cam.GetComponent<Animator>();
            animator.enabled = false;

            //Reload System
        }

        #endregion Start

        #region Update

        private void Update()
        {
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
                sniperShootEv.setParameterByName("ReloadFull", 0f);

                required = true;
                //Transition
                animator.enabled = true;
                animator.SetBool("Scoped", true);

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

            if (zoomReleased && required == true)
            {
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
                animator.enabled = false;
                animator.SetBool("Scoped", false);

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

            if (camMove)
            {
                /*
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
                turretRotation.SniperCamVerticalRotation();
            }
            else
            {
                turretRotation.SniperCamVerticalRotation();
            }

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

            if (currentReloadValue != expectedReloadValue)
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

        #endregion Update

        #region Shoot

        private void Shoot(float currDamage)
        {
            barrelFlash.Play(true);
            TankHealth.TankHealth targetHealth = target.transform.GetComponent<TankHealth.TankHealth>();

            inZoomMode = false;

            if (targetHealth)
            {
                FactionID fID = targetHealth.fid;
                FactionID myID = _myTankHealth.fid;

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
                    GameObject i = Instantiate(hitEffect, target.point, Quaternion.identity);
                    i.transform.up = target.normal;
                    Destroy(i, 10f);
                }
            }

            // sniperShootEv.setParameterByName("Firing", 0f);

            if (currDamage == damage)
            {
                totalAmmo = 0;
            }

            if (currDamage == tempDamage)
            {
                totalAmmo = 1.0f;
            }

            if (currDamage == (tempDamage - tempDamage / 8))
            {
                totalAmmo = 0f;
            }

            damageToLess = 700f;
        }

        #endregion Shoot

        #region Cameras

        private void CameraTransition(bool isZoom)
        {
            if (isZoom)
            {
                if (!cameraMode)
                {
                    cameraDefaultPos = cam.transform.localPosition;
                    cameraMode = isZoom;
                }

                float speed = (Vector3.Distance(cameraDefaultPos, cameraScopePos) / cameraTransitionTime) *
                              Time.deltaTime;
                cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, cameraScopePos, speed);
            }
            else
            {
                cameraMode = isZoom;
            }
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