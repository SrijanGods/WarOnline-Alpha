using _Scripts.Controls;
using _Scripts.Photon.Room;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Tank.Turrets.MachineGun
{
    public class MachineGunV2 : MonoBehaviour
    {
        public GameObject bulletEffect;

        public float range, actualAmmo, reloadTime, damage, overheatDamage;

        public ParticleSystem muzzleParts;

        public LineRenderer bulletRenderer;

        public Transform bulletStart, barrel;

        public Vector3 barrelRollAxis;

        public float barrelRollSpeed;

        [Header("Sounds")] [FMODUnity.EventRef]
        public string mgShootSfx = "event:/MachineGun";

        FMOD.Studio.EventInstance _mgShootEv;

        private TankHealth _myTankHealth;
        private GameObject _mainCanvas;
        private Slider _coolDownSlider;

        private Image _fillImage;
        // private bool _neededZero;

        private bool _attack;
        private bool _timer;
        private float _ammo, _ammoRunning, _ammoReload, _selfDamage;

        private void Start()
        {
            if (bulletRenderer == null)
            {
                bulletRenderer = GetComponent<LineRenderer>();
            }

            bulletRenderer.enabled = false;

            _ammo = actualAmmo;

            _myTankHealth = GetComponentInParent<TankHealth>();
            _mainCanvas = _myTankHealth.warCanvas;
            GameObject coolDownUI = _mainCanvas.transform.Find("CoolDownUI").gameObject;
            _coolDownSlider = coolDownUI.GetComponent<Slider>();
            _coolDownSlider.minValue = 0f;
            _coolDownSlider.maxValue = actualAmmo;
            _coolDownSlider.value = actualAmmo;
            GameObject coolDown = coolDownUI.transform.Find("CoolDown").gameObject;
            _fillImage = coolDown.GetComponentInChildren<Image>();

            //setting sfx
            _mgShootEv = FMODUnity.RuntimeManager.CreateInstance(mgShootSfx);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(_mgShootEv, GetComponent<Transform>(),
                GetComponent<Rigidbody>());
            _mgShootEv.start();
            _mgShootEv.setParameterByName("SoundLess", 0f);
        }

        private void FixedUpdate()
        {
            if (SimulatedInput.GetButton(InputCodes.TankFire))
            {
                if (_ammo >= 0)
                {
                    _attack = true;
                }
            }

            if (SimulatedInput.GetButtonUp(InputCodes.TankFire))
            {
                if (_ammo >= actualAmmo)
                {
                    _ammo = actualAmmo;
                }

                _attack = false;
            }

            if (_attack)
            {
                _mgShootEv.setParameterByName("Firing", 1f);

                if (_ammo < 0)
                {
                    _selfDamage += Time.fixedDeltaTime;

                    if (_selfDamage > .5f)
                    {
                        _selfDamage = overheatDamage;
                    }

                    muzzleParts.Stop(true);
                    bulletRenderer.enabled = false;
                    BarrelRoll();

                    _mgShootEv.setParameterByName("SoundLess", 1f);
                }
                else
                {
                    bulletRenderer.enabled = true;

                    muzzleParts.Play(true);
                    BarrelRoll();

                    bulletRenderer.SetPosition(0, bulletStart.position);

                    if (Physics.Raycast(bulletStart.position, bulletStart.forward, out var hit, range))
                    {
                        bulletRenderer.SetPosition(1, hit.point);
                        HitDamage(hit);
                    }
                    else
                    {
                        bulletRenderer.SetPosition(1, bulletStart.position + bulletStart.forward * range);
                    }

                    _ammo -= Time.fixedDeltaTime;

                    _mgShootEv.setParameterByName("SoundLess", 0f);
                }
            }
            else
            {
                bulletRenderer.enabled = false;
                muzzleParts.Stop(true);
                _mgShootEv.setParameterByName("Firing", 0f);

                if (_ammo < actualAmmo)
                {
                    _ammoReload += Time.fixedDeltaTime;
                    if (_ammoReload >= 1)
                    {
                        _ammo++;
                        _ammoReload = 0;
                    }
                }
            }

            _coolDownSlider.value = _ammo;
        }

        private void BarrelRoll()
        {
            barrel.Rotate(barrelRollAxis, barrelRollSpeed * Time.deltaTime);
        }

        private void HitDamage(RaycastHit hit)
        {
            GameObject bulletSpot = Instantiate(bulletEffect, hit.point, Quaternion.identity);
            bulletSpot.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            Destroy(bulletSpot, 2f);

            var th = hit.transform.GetComponent<TankHealth>();
            if (!th) th = hit.transform.GetComponentInParent<TankHealth>();

            if (th)
            {
                FactionID fID = th.fid, myID = _myTankHealth.fid;

                if ((fID == null && myID == null) || (fID.teamID == -1 || myID.teamID == -1) ||
                    fID.teamID != myID.teamID)
                {
                    if (fID.myAccID != myID.myAccID)
                    {
                        th.TakeDamage(damage);
                    }
                }
            }
        }
    }
}