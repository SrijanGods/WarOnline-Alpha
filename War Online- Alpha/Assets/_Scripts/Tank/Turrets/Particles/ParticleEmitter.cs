using System;
using _Scripts.Controls;
using _Scripts.Photon.Room;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Tank.Turrets.Particles
{
    public class ParticleEmitter : MonoBehaviourPun, IPunObservable
    {
        public Camera myCamera;

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
        private Slider _coolDownSlider;
        private float barTime;

        //reload functions
        // [SerializeField] private float ammoRunning = 0.0f;
        [SerializeField] private float ammoReload = 0.0f;

        private float UIseti;
        private float UIsetd;
        private bool increment = false;
        private bool decrement = false;
        private float mainValue = 0f;

        #region Start

        void Start()
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

            particleSmoke.SetParent(particleFire.transform, true);
            particleFire.Stop(true);

            _myTankHealth = GetComponentInParent<TankHealth>();
            _coolDownSlider = _myTankHealth.attackCooldown;
            if (!isFlameThrower)
            {
                _coolDownSlider.maxValue = 13f;
                _coolDownSlider.minValue = 0f;
                _coolDownSlider.value = 13f;
                mainValue = 13f;
            }
            else
            {
                _coolDownSlider.maxValue = 10f;
                _coolDownSlider.minValue = 0f;
                _coolDownSlider.value = 10f;
                mainValue = 10f;
            }

            //FMOD sounds
            flameThrowerEv = FMODUnity.RuntimeManager.CreateInstance(flameThrowersfx);
            flameThrowerEv.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
            flameThrowerEv.start();
        }

        #endregion Start

        #region FireInput

        void ProcessFireInput()
        {
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
            if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

            ProcessFireInput();

            if (Math.Abs(_coolDownSlider.value - ammo) > .005)
            {
                barTime += Time.deltaTime;
                if (barTime >= 0.01f)
                {
                    if (_coolDownSlider.value < ammo)
                    {
                        _coolDownSlider.value += mainValue / 85;
                        barTime = 0f;
                    }

                    if (_coolDownSlider.value > ammo)
                    {
                        _coolDownSlider.value -= mainValue / 85;
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

                        if ((fID.teamIndex != -1 || myID.teamIndex != -1) && fID.teamIndex == myID.teamIndex) continue;

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
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting) stream.SendNext(isFiring);
            else
            {
                isFiring = (bool) stream.ReceiveNext();

                if (isFiring)
                {
                    flameThrowerEv.setParameterByName("Firing", 1f);
                    particleFire.Play(true);
                }
                else
                {
                    flameThrowerEv.setParameterByName("Firing", 0);
                    particleFire.Stop(true);
                }
            }
        }

        #endregion Damage
    }
}