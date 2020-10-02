using _Scripts.Controls;
using _Scripts.Photon.Room;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Tank.Turrets.MachineGun
{
    public class MachineGunV2 : MonoBehaviourPun, IPunObservable
    {
        public Camera myCamera;

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

        public TankHealth.TankHealth myTankHealth;

        private Slider _coolDownSlider;
        // private bool _neededZero;

        private bool _attack;
        private bool _timer;
        private float _ammo, _ammoRunning, _selfDamage;

        private void Start()
        {
            if (photonView.IsMine || !PhotonNetwork.IsConnected)
            {
                myCamera.gameObject.SetActive(true);
                myCamera.enabled = true;
            }
            else
            {
                myCamera.gameObject.SetActive(false);
            }

            if (bulletRenderer == null)
            {
                bulletRenderer = GetComponent<LineRenderer>();
            }

            bulletRenderer.enabled = false;

            _ammo = actualAmmo;

            _coolDownSlider = myTankHealth.attackCooldown;
            _coolDownSlider.minValue = 0f;
            _coolDownSlider.maxValue = actualAmmo;
            _coolDownSlider.value = actualAmmo;

            //setting sfx
            _mgShootEv = FMODUnity.RuntimeManager.CreateInstance(mgShootSfx);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(_mgShootEv, GetComponent<Transform>(),
                GetComponent<Rigidbody>());
            _mgShootEv.start();
            _mgShootEv.setParameterByName("SoundLess", 0f);
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

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
                    _ammo += Time.fixedDeltaTime;
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
            var th = hit.transform.GetComponent<TankHealth.TankHealth>();
            if (!th) th = hit.transform.GetComponentInParent<TankHealth.TankHealth>();

            if (th)
            {
                FactionID fID = th.fid, myID = myTankHealth.fid;

                if ((fID == null && myID == null) || (fID.teamIndex == -1 || myID.teamIndex == -1) ||
                    fID.teamIndex != myID.teamIndex)
                {
                    if (fID.myAccID != myID.myAccID)
                    {
                        th.TakeDamage(damage, myID.actorNumber);
                    }
                }
            }

            photonView.RPC(nameof(HitEffect), RpcTarget.All, hit.point, hit.normal);
        }

        [PunRPC]
        public void HitEffect(Vector3 pos, Vector3 rot)
        {
            GameObject bulletSpot = Instantiate(bulletEffect, pos, Quaternion.identity);
            bulletSpot.transform.up = rot;
            Destroy(bulletSpot, 2f);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(barrel.rotation);

                stream.SendNext(bulletRenderer.enabled);

                var ps = new Vector3[bulletRenderer.positionCount];
                bulletRenderer.GetPositions(ps);
                stream.SendNext(ps);
            }
            else if (stream.IsReading)
            {
                barrel.rotation = (Quaternion) stream.ReceiveNext();

                bulletRenderer.enabled = (bool) stream.ReceiveNext();

                var ps = (Vector3[]) stream.ReceiveNext();

                for (int i = 0; i < ps.Length; i++)
                {
                    bulletRenderer.SetPosition(i, ps[i]);
                }
            }
        }
    }
}