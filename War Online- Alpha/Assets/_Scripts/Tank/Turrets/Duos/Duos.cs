using System.Collections;
using _Scripts.Controls;
using _Scripts.Tank.Projectile;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Tank.Turrets.Duos
{
    public class Duos : ProjectileShooter
    {
        [Header("GameObjects")] public GameObject Sp1, Sp2;

        [Header("Particle Systems")] public ParticleSystem muzzleFlashA, muzzleFlashB;

        [Header("Sound Effects")] [FMODUnity.EventRef]
        public string duosReloadSfx = "event:/TurretDuos", duosShootSfx = "event:/TurretDuosShoot";

        FMOD.Studio.EventInstance duosReloadEv, duosShootEv;

        [Header("Cooldown Slider value")] public float decreasePerShoot, increasePerSecond;

        public Collider enemyFinder;

        private bool _right = true, _left;

        private Slider coolDownSlider;
        private Image fillImage;
        private TankHealth tankHealth;

        private GameObject mainCanvas;

        // private int myTeamID;
        private bool _reload;

        // private TouchProcessor tP;

        #region Start&Update

        void Start()
        {
            autoAim = false;

            // tP = GetComponentInParent<TouchProcessor>();
            tankHealth = GetComponentInParent<TankHealth>();
            mainCanvas = tankHealth.warCanvas;
            GameObject coolDownUI = mainCanvas.transform.Find("CoolDownUI").gameObject;
            coolDownSlider = coolDownUI.GetComponent<Slider>();
            coolDownSlider.maxValue = 1f;
            coolDownSlider.minValue = 0f;
            coolDownSlider.value = 1f;
            GameObject coolDown = coolDownUI.transform.Find("CoolDown").gameObject;
            fillImage = coolDown.GetComponentInChildren<Image>();

            //SFX initialize here
            duosReloadEv = FMODUnity.RuntimeManager.CreateInstance(duosReloadSfx);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(duosReloadEv, GetComponent<Transform>(),
                GetComponent<Rigidbody>());
            duosShootEv = FMODUnity.RuntimeManager.CreateInstance(duosShootSfx);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(duosShootEv, GetComponent<Transform>(),
                GetComponent<Rigidbody>());
        }

        void FixedUpdate()
        {
            if (coolDownSlider.value <= decreasePerShoot)
            {
                muzzleFlashA.Stop(true);
                muzzleFlashB.Stop(true);
            }

            if (SimulatedInput.GetButton(InputCodes.TankFire))
            {
                _reload = true;

                if (coolDownSlider.value <= decreasePerShoot) return;

                if (_right)
                {
                    StartCoroutine(nameof(RightShoot));
                }
                else if (_left)
                {
                    StartCoroutine(nameof(LeftShoot));
                }
            }
            else
            {
                if (_reload) duosReloadEv.start();
                _reload = false;
                coolDownSlider.value += increasePerSecond * Time.fixedDeltaTime;
            }
        }

        #endregion Start&Update

        /*#region Collision

    private void OnTriggerEnter(Collider otherCollider)
    {
        if (otherCollider.GetComponentInParent<FactionID>() != null)
        {
            if (otherCollider.GetComponentInParent<FactionID>()._teamID != myTeamID && otherCollider.isTrigger == false)
            {
                enemyPos = otherCollider.GetComponentInChildren<Transform>().position;
                changeDir = true;
            }
        }
    }

    private float stayCount = 0.0f;

    private void OnTriggerStay(Collider otherCollider)
    {
        if (stayCount > 1f)
        {
            if (otherCollider.GetComponentInParent<FactionID>() != null)
            {
                if (otherCollider.GetComponentInParent<FactionID>()._teamID != myTeamID ||
                    otherCollider.GetComponentInParent<FactionID>()._teamID == 1 && otherCollider.isTrigger == false)
                {
                    enemyPos = otherCollider.GetComponentInChildren<Transform>().position;
                    changeDir = true;
                }
            }

            stayCount = stayCount - 1f;
        }
        else
        {
            stayCount = stayCount + Time.deltaTime;
        }
    }

    #endregion Collision*/

        #region IEnum

        IEnumerator RightShoot()
        {
            coolDownSlider.value -= decreasePerShoot;

            GameObject g;
            TankProjectile tp;

            if (InactiveProjectiles.Count > 0)
            {
                tp = InactiveProjectiles[0];
                InactiveProjectiles.Remove(tp);

                g = tp.gameObject;
                g.transform.SetParent(Sp1.transform, false);
            }
            else
            {
                // Break if we have created max projectile instances allowed
                if (ActiveProjectiles.Count >= maxProjectilesCount) yield break;

                g = Instantiate(projectilePrefab, Sp1.transform, false);
                tp = g.GetComponent<TankProjectile>();
            }

            ActiveProjectiles.Add(tp);

            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            g.transform.SetParent(null, true);
            g.transform.localScale = Vector3.one;

            tp.turretParent = this;

            // duosShootEv.start();

            tp.force = force;
            tp.damage = damage;
            tp.dir = Sp1.transform.forward;
            tp.range = range;
            tp.autoAim = autoAim;
            tp.enemy = null;

            muzzleFlashA.Play(true);

            tp.Launch();

            _right = false;

            yield return new WaitForSeconds(1 / fireRate);

            _left = true;

            duosShootEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        IEnumerator LeftShoot()
        {
            coolDownSlider.value -= decreasePerShoot;

            GameObject g;
            TankProjectile tp;

            if (InactiveProjectiles.Count > 0)
            {
                tp = InactiveProjectiles[0];
                InactiveProjectiles.Remove(tp);

                g = tp.gameObject;
                g.transform.SetParent(Sp2.transform, false);
            }
            else
            {
                // Break if we have created max projectile instances allowed
                if (ActiveProjectiles.Count >= maxProjectilesCount) yield break;

                g = Instantiate(projectilePrefab, Sp2.transform, false);
                tp = g.GetComponent<TankProjectile>();
            }

            ActiveProjectiles.Add(tp);

            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            g.transform.SetParent(null, true);
            g.transform.localScale = Vector3.one;

            tp.turretParent = this;

            duosShootEv.start();

            tp.force = force;
            tp.damage = damage;
            tp.dir = Sp2.transform.forward;
            tp.range = range;
            tp.autoAim = autoAim;
            tp.enemy = null;

            muzzleFlashB.Play(true);

            tp.Launch();

            _left = false;

            yield return new WaitForSeconds(1 / fireRate);

            _right = true;

            duosShootEv.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        #endregion IEnum
    }
}