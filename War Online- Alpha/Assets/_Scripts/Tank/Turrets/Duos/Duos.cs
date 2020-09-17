using System.Collections;
using _Scripts.Controls;
using _Scripts.Tank.Projectile;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.Tank.Turrets.Duos
{
    public class Duos : ProjectileShooter
    {
        public Camera myCamera;

        [FormerlySerializedAs("Sp1")] [Header("GameObjects")]
        public GameObject sp1;

        [FormerlySerializedAs("Sp2")] [Header("GameObjects")]
        public GameObject sp2;

        [Header("Particle Systems")] public ParticleSystem muzzleFlashA, muzzleFlashB;

        [Header("Sound Effects")] [FMODUnity.EventRef]
        public string duosReloadSfx = "event:/TurretDuos", duosShootSfx = "event:/TurretDuosShoot";

        FMOD.Studio.EventInstance _duosReloadEv, _duosShootEv;

        [Header("Cooldown Slider value")] public float decreasePerShoot, increasePerSecond;

        public Collider enemyFinder;

        private bool _right = true, _left;

        private Slider _coolDownSlider;
        private TankHealth _myTankHealth;

        // private int myTeamID;
        private bool _reload;

        // private TouchProcessor tP;

        #region Start&Update

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

            autoAim = false;

            // tP = GetComponentInParent<TouchProcessor>();
            _myTankHealth = GetComponentInParent<TankHealth>();
            _coolDownSlider = _myTankHealth.attackCooldown;
            _coolDownSlider.maxValue = 1f;
            _coolDownSlider.minValue = 0f;
            _coolDownSlider.value = 1f;

            //SFX initialize here
            _duosReloadEv = FMODUnity.RuntimeManager.CreateInstance(duosReloadSfx);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(_duosReloadEv, GetComponent<Transform>(),
                GetComponent<Rigidbody>());
            _duosShootEv = FMODUnity.RuntimeManager.CreateInstance(duosShootSfx);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(_duosShootEv, GetComponent<Transform>(),
                GetComponent<Rigidbody>());
        }

        void FixedUpdate()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

            if (_coolDownSlider.value <= decreasePerShoot)
            {
                muzzleFlashA.Stop(true);
                muzzleFlashB.Stop(true);
            }

            if (SimulatedInput.GetButton(InputCodes.TankFire))
            {
                _reload = true;

                if (_coolDownSlider.value <= decreasePerShoot) return;

                photonView.RPC(nameof(Shoot), RpcTarget.All);
            }
            else
            {
                if (_reload) _duosReloadEv.start();
                _reload = false;
                _coolDownSlider.value += increasePerSecond * Time.fixedDeltaTime;
            }
        }

        #endregion Start&Update

        #region Projectiles Shoot & Sync

        [PunRPC]
        public void Shoot()
        {
            if (_right)
            {
                StartCoroutine(nameof(RightShoot));
            }
            else if (_left)
            {
                StartCoroutine(nameof(LeftShoot));
            }
        }

        IEnumerator RightShoot()
        {
            _coolDownSlider.value -= decreasePerShoot;

            GameObject g;
            TankProjectile tp;

            if (InactiveProjectiles.Count > 0)
            {
                tp = InactiveProjectiles[0];
                InactiveProjectiles.Remove(tp);

                g = tp.gameObject;
                g.transform.SetParent(sp1.transform, false);
            }
            else
            {
                // Break if we have created max projectile instances allowed
                if (ActiveProjectiles.Count >= maxProjectilesCount) yield break;

                g = Instantiate(projectilePrefab, sp1.transform, false);
                tp = g.GetComponent<TankProjectile>();
            }

            ActiveProjectiles.Add(tp);

            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            g.transform.SetParent(null, true);
            g.transform.localScale = Vector3.one;

            tp.turretParent = this;
            tp.pvw = _myTankHealth.photonView;
            tp.Ready();

            // duosShootEv.start();

            tp.force = force;
            tp.damage = damage;
            tp.dir = sp1.transform.forward;
            tp.range = range;
            tp.autoAim = autoAim;
            tp.enemy = null;

            muzzleFlashA.Play(true);

            tp.Launch();

            _right = false;

            yield return new WaitForSeconds(1 / fireRate);

            _left = true;

            _duosShootEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        IEnumerator LeftShoot()
        {
            _coolDownSlider.value -= decreasePerShoot;

            GameObject g;
            TankProjectile tp;

            if (InactiveProjectiles.Count > 0)
            {
                tp = InactiveProjectiles[0];
                InactiveProjectiles.Remove(tp);

                g = tp.gameObject;
                g.transform.SetParent(sp2.transform, false);
            }
            else
            {
                // Break if we have created max projectile instances allowed
                if (ActiveProjectiles.Count >= maxProjectilesCount) yield break;

                g = Instantiate(projectilePrefab, sp2.transform, false);
                tp = g.GetComponent<TankProjectile>();
            }

            ActiveProjectiles.Add(tp);

            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            g.transform.SetParent(null, true);
            g.transform.localScale = Vector3.one;

            tp.turretParent = this;
            tp.pvw = _myTankHealth.photonView;
            tp.Ready();

            _duosShootEv.start();

            tp.force = force;
            tp.damage = damage;
            tp.dir = sp2.transform.forward;
            tp.range = range;
            tp.autoAim = autoAim;
            tp.enemy = null;

            muzzleFlashB.Play(true);

            tp.Launch();

            _left = false;

            yield return new WaitForSeconds(1 / fireRate);

            _right = true;

            _duosShootEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        #endregion IEnum
    }
}