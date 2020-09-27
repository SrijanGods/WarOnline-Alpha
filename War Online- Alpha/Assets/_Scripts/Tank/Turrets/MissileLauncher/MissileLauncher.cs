using System.Collections;
using _Scripts.Controls;
using _Scripts.Photon.Room;
using _Scripts.Tank.Projectile;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Tank.Turrets.MissileLauncher
{
    public class MissileLauncher : ProjectileShooter
    {
        public Camera myCamera;

        // maximum missiles that can be launched at a time
        public int maxLaunchCount = 4;

        // time gap between two consecutive missile launches
        public float consecutiveLaunchGap = .5f;

        // time required to hold for multiple launches and reload time for reloading one missile
        public float requiredHoldTime = .25f, reloadTime = 3f;
        public Transform[] projectilesParents;

        [FMODUnity.EventRef] public string reloadSfx = "event:/TurretMissile", shootSfx = "event:/TurretMissileShoot";
        private FMOD.Studio.EventInstance _reloadEv, _shootEv;

        public ParticleSystem[] shootEffects;

        private float _heldTime, _shootCount;

        private Slider _coolDownSlider;
        private TankHealth.TankHealth _myTankHealth;
        private int _myTeamID;
        private Transform _enemy;

        private void Awake()
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

            _myTankHealth = GetComponentInParent<TankHealth.TankHealth>();
            _coolDownSlider = _myTankHealth.attackCooldown;
            _coolDownSlider.maxValue = maxLaunchCount;
            _coolDownSlider.minValue = 0f;
            _coolDownSlider.value = maxLaunchCount;
            // GameObject coolDown = coolDownUI.transform.Find("CoolDown").gameObject;

            // Projectiles init
            /*for (int i = 0; i < maxProjectilesCount; i++)
            {
                var p = projectilesParents[i % projectilesParents.Length];
                var g = Instantiate(projectilePrefab, p, false);
                var c = g.GetComponent<TankProjectile>();

                InactiveProjectiles.Add(c);
            }*/

            _myTeamID = GetComponentInParent<FactionID>().teamIndex;

            //SFX initialize here
            _reloadEv = FMODUnity.RuntimeManager.CreateInstance(reloadSfx);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(_reloadEv, GetComponent<Transform>(),
                GetComponent<Rigidbody>());
            _shootEv = FMODUnity.RuntimeManager.CreateInstance(shootSfx);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(_shootEv, GetComponent<Transform>(),
                GetComponent<Rigidbody>());
        }

        private void FixedUpdate()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

            // Will launch 1 missile when fire is pressed and released

            // else if fire is held for required time
            // it will launch the max missile launch allowed at the time, according to the cooldown and reload 

            if (SimulatedInput.GetButtonDown(InputCodes.TankFire))
            {
                _heldTime = 0;

                _shootCount = 0;
                _shootCount = (int) Mathf.Clamp(1, 0, _coolDownSlider.value);
            }
            else if (SimulatedInput.GetButton(InputCodes.TankFire))
            {
                _heldTime += Time.fixedDeltaTime;

                if (_heldTime >= requiredHoldTime)
                {
                    _shootCount = (int) Mathf.Clamp(maxLaunchCount, 0, _coolDownSlider.value);

                    _heldTime = 0;
                }
            }

            else if (_shootCount >= 1)
            {
                var c = _shootCount;
                _shootCount = 0;

                // var count = (int) Mathf.Clamp((int) Math.Floor(_heldTime / requiredHoldTime), 0, _coolDownSlider.value);
                // _coolDownSlider.value = maxLaunchCount - count;

                StopAllCoroutines();
                StartCoroutine(nameof(Shoot), c);
            }
        }

        private void GetClosestEnemy()
        {
            var tankHealths = FindObjectsOfType<TankHealth.TankHealth>();

            var closest = float.PositiveInfinity;

            foreach (var tank in tankHealths)
            {
                var t = tank.transform;

                if (tank.myTeam != _myTeamID)
                {
                    var d = Vector3.Distance(transform.position, t.position);

                    if (d < closest)
                    {
                        _enemy = t;
                        closest = d;
                    }
                }
            }
        }

        private IEnumerator Shoot(int c)
        {
            for (var i = 0; i < c; i++)
            {
                _coolDownSlider.value--;

                var p = projectilesParents[i % projectilesParents.Length];

                GameObject g;
                TankProjectile tp;

                if (InactiveProjectiles.Count > 0)
                {
                    tp = InactiveProjectiles[0];
                    g = tp.gameObject;
                }
                else if (ActiveProjectiles.Count < maxProjectilesCount)
                {
                    g = Instantiate(projectilePrefab, p, false);
                    tp = g.GetComponent<TankProjectile>();

                    InactiveProjectiles.Add(tp);
                }
                else continue;

                InactiveProjectiles.Remove(tp);
                ActiveProjectiles.Add(tp);

                tp.transform.SetParent(p, false);
                g.transform.localPosition = Vector3.zero;
                g.transform.localRotation = Quaternion.identity;
                g.transform.SetParent(null, true);
                g.transform.localScale = Vector3.one;

                tp.turretParent = this;
                tp.pvw = photonView;
                tp.Ready();

                _shootEv.start();

                tp.force = force;
                tp.damage = damage;
                tp.dir = p.transform.forward;
                tp.range = range;
                tp.autoAim = autoAim;
                if (autoAim) GetClosestEnemy();
                tp.enemy = _enemy;

                shootEffects[i % projectilesParents.Length].Play(true);
                tp.Launch();
                _shootEv.start();

                yield return new WaitForSeconds(consecutiveLaunchGap);
            }

            StartCoroutine(Reload());
        }

        private IEnumerator Reload()
        {
            var t = 0f;
            _reloadEv.start();

            while (t < reloadTime)
            {
                t += .1f;

                _coolDownSlider.value = (t / reloadTime) * maxProjectilesCount;

                yield return new WaitForSeconds(.1f);
            }

            _coolDownSlider.value = maxProjectilesCount;
        }
    }
}