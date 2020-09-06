using System.Collections;
using _Scripts.Controls;
using _Scripts.Photon.Room;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Tank.Turrets.Blaster
{
    public class OneShot : MonoBehaviour
    {
        [Header("Reload Functions")] [SerializeField]
        private float reloadTime;

        private int _ammo = 1;
        [SerializeField] private float range = 450f;

        [Header("Damage Values")] [SerializeField]
        private float damage;

        [SerializeField] private ParticleSystem hitEffect;

        [Header("Others")] [SerializeField] private Transform firePoint;
        [SerializeField] private ParticleSystem barrelFlash;
        public LayerMask shootObjectslayer;

        [Header("Sound System")] [FMODUnity.EventRef]
        public string sniperShootSfx = "event:/Sniper";

        FMOD.Studio.EventInstance sniperShootEv;

        private RaycastHit target;
        private float ammoReload;
        private float timeForBar;

        private TankHealth _myTankHealth;
        private GameObject mainCanvas;
        private Slider coolDownSlider;
        private Image fillImage;
        private Animator animator;
        private bool camMove = false;
        private float currentReloadValue;
        private float expectedReloadValue;

        #region Start&Update

        private void Start()
        {
            barrelFlash.Play(false);

            //Interface
            _myTankHealth = GetComponentInParent<TankHealth>();
            mainCanvas = _myTankHealth.warCanvas;
            GameObject coolDownUI = mainCanvas.transform.Find("CoolDownUI").gameObject;
            coolDownSlider = coolDownUI.GetComponent<Slider>();
            GameObject coolDown = coolDownUI.transform.Find("CoolDown").gameObject;
            coolDownSlider.maxValue = 1f;
            coolDownSlider.minValue = 0f;
            coolDownSlider.value = 1f;
            fillImage = coolDown.GetComponentInChildren<Image>();
        }

        private void Update()
        {
            if (SimulatedInput.GetButtonDown(InputCodes.TankFire))
            {
                if (_ammo >= 1)
                {
                    StartCoroutine(nameof(Shoot));
                }
            }

            if (_ammo < 1)
            {
                ammoReload += Time.deltaTime;
                if (ammoReload >= reloadTime)
                {
                    _ammo = 1;
                    ammoReload = 0f;
                }
            }

            if (coolDownSlider.value < _ammo)
            {
                timeForBar += Time.deltaTime;
                if (timeForBar >= 0.06)
                {
                    coolDownSlider.value += 0.01f;
                    timeForBar = 0f;
                }
            }
            else
            {
                coolDownSlider.value = 0f;
            }
        }

        #endregion Start&Update

        #region Shoot

        private IEnumerator Shoot()
        {
            _ammo = 0;

            barrelFlash.Play(true);

            // wait until next fixed update to cast ray, physics thing
            yield return new WaitForFixedUpdate();

            // proceed only if the raycast did hit something
            if (!Physics.Raycast(firePoint.position, firePoint.forward, out target, range, shootObjectslayer))
                yield break;

            TankHealth targetHealth = target.transform.gameObject.GetComponent<TankHealth>();

            // Damage if hit any tank
            if (targetHealth && _myTankHealth.photonView.IsMine)
            {
                FactionID fID = targetHealth.fid;
                FactionID myID = _myTankHealth.fid;

                if (fID.teamID != myID.teamID)
                {
                    if (fID.myAccID != myID.myAccID)
                    {
                        targetHealth.TakeDamage(damage);
                        /*targetHealth.gameObject.GetComponent<PhotonView>().RPC("TakeDamage",
                            target.transform.gameObject.GetComponent<PhotonView>().Owner, damage);*/
                    }
                }
            }

            // create impact effect at hit pos
            GameObject i = Instantiate(hitEffect.gameObject, target.point, Quaternion.identity);
            i.transform.rotation = Quaternion.FromToRotation(Vector3.forward, target.normal);
            Destroy(i, 10f);
        }

        #endregion Shoot
    }
}