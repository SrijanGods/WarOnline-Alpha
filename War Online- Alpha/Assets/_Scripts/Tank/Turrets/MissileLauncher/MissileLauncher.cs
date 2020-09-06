using System;
using System.Collections;
using _Scripts.Controls;
using _Scripts.Photon.Room;
using _Scripts.Tank.Projectile;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Tank.Turrets.MissileLauncher
{
    public class MissileLauncher : ProjectileShooter
    {
        // maximum missiles that can be launched at a time
        public int maxLaunchCount = 4;

        // time gap between two consecutive missile launches
        public float consecutiveLaunchGap = .5f;

        // time required to hold for multiple launches and reload time for reloading one missile
        public float requiredHoldTime = .25f, reloadTime = 3f;
        public Transform[] projectilesParents;

        [FMODUnity.EventRef] public string reloadSfx = "event:/TurretMissile", shootSfx = "event:/TurretMissileShoot";
        private FMOD.Studio.EventInstance _reloadEv, _shootEv;

        public ParticleSystem shootEffect;

        private bool _shoot;
        private float _heldTime;

        private Slider _coolDownSlider;
        private TankHealth _tankHealth;
        private GameObject _mainCanvas;
        private int _myTeamID;
        private Transform _enemy;

        private void Awake()
        {
            _tankHealth = GetComponentInParent<TankHealth>();
            _mainCanvas = _tankHealth.warCanvas;
            GameObject coolDownUI = _mainCanvas.transform.Find("CoolDownUI").gameObject;
            _coolDownSlider = coolDownUI.GetComponent<Slider>();
            _coolDownSlider.maxValue = maxLaunchCount;
            _coolDownSlider.minValue = 0f;
            _coolDownSlider.value = maxLaunchCount;
            // GameObject coolDown = coolDownUI.transform.Find("CoolDown").gameObject;
            _myTeamID = GetComponentInParent<FactionID>().teamID;

            //SFX initialize here
            _reloadEv = FMODUnity.RuntimeManager.CreateInstance(reloadSfx);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(_reloadEv, GetComponent<Transform>(),
                GetComponent<Rigidbody>());
            _shootEv = FMODUnity.RuntimeManager.CreateInstance(shootSfx);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(_shootEv, GetComponent<Transform>(),
                GetComponent<Rigidbody>());

            // Projectiles init
            for (int i = 0; i < maxProjectilesCount; i++)
            {
                var p = projectilesParents[i % projectilesParents.Length];
                var g = Instantiate(projectilePrefab, p, false);
                var c = g.GetComponent<TankProjectile>();

                InactiveProjectiles.Add(c);
            }
        }

        private void FixedUpdate()
        {
            // Will launch 1 missile when fire is pressed and released

            // else if fire is held for required time
            // it will launch the max missile launch allowed at the time, according to the cooldown and reload 

            if (SimulatedInput.GetButtonDown(InputCodes.TankFire))
            {
                _heldTime = requiredHoldTime;

                var count = (int) Mathf.Clamp(1, 0, _coolDownSlider.value);
                _coolDownSlider.value = maxLaunchCount - count;
                _shoot = count >= 1;
            }

            if (SimulatedInput.GetButton(InputCodes.TankFire))
            {
                _heldTime += Time.fixedDeltaTime;

                if (_heldTime >= requiredHoldTime)
                {
                    var count = (int) Mathf.Clamp(maxLaunchCount, 0, _coolDownSlider.value);
                    _coolDownSlider.value = maxLaunchCount - count;
                    _shoot = count >= 1;
                }
            }
            /*if (SimulatedInput.GetButton(InputCodes.TankFire))
            {
                _heldTime += Time.fixedDeltaTime;

                var count = (int) Mathf.Clamp((int) Math.Floor(_heldTime / requiredHoldTime), 0, _coolDownSlider.value);
                _coolDownSlider.value = maxLaunchCount - count;
                _shoot = count >= 1;
            }*/

            if (SimulatedInput.GetButtonUp(InputCodes.TankFire))
            {
                if (!_shoot || InactiveProjectiles.Count <= 0) return;

                _shoot = false;

                var count = (int) Mathf.Clamp((int) Math.Floor(_heldTime / requiredHoldTime), 0, _coolDownSlider.value);
                _coolDownSlider.value = maxLaunchCount - count;

                StartCoroutine(nameof(Shoot), count);
                StartCoroutine(nameof(Reload));
            }
        }

        private void GetClosestEnemy()
        {
            var tankHealths = FindObjectsOfType<TankHealth>();

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

        private IEnumerator Shoot(int count)
        {
            count = Mathf.Clamp(count, 0, InactiveProjectiles.Count);

            for (var i = 0; i < count; i++)
            {
                var p = projectilesParents[i % projectilesParents.Length];

                var tp = InactiveProjectiles[0];
                var g = tp.gameObject;

                InactiveProjectiles.Remove(tp);
                ActiveProjectiles.Add(tp);

                tp.transform.SetParent(p, false);
                g.transform.localPosition = Vector3.zero;
                g.transform.localRotation = Quaternion.identity;
                g.transform.SetParent(null, true);
                g.transform.localScale = Vector3.one;

                tp.turretParent = this;

                _shootEv.start();

                tp.force = force;
                tp.damage = damage;
                tp.dir = p.transform.forward;
                tp.range = range;
                tp.autoAim = autoAim;
                if (autoAim) GetClosestEnemy();
                tp.enemy = _enemy;

                shootEffect.Play(true);
                tp.Launch();
                _shootEv.start();

                yield return new WaitForSeconds(consecutiveLaunchGap);
            }
        }

        private IEnumerator Reload()
        {
            var t = 0f;
            _reloadEv.start();

            while (t < reloadTime)
            {
                t += Time.deltaTime;

                _coolDownSlider.value = t / reloadTime;

                yield return new WaitForEndOfFrame();
            }

            _coolDownSlider.value = 1;
        }
    }
}