using System.Collections;
using _Scripts.Controls;
using _Scripts.Photon.Room;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Tank.Turrets.Blaster
{
    public class OneShot : MonoBehaviourPun
    {
        public Camera myCamera;

        [Header("Reload Functions")] [SerializeField]
        private float reloadTime;

        private float _ammo = 1;
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

        public TankHealth.TankHealth myTankHealth;
        private Slider _coolDownSlider;
        private bool camMove = false;
        private float currentReloadValue;
        private float expectedReloadValue;

        #region Start&Update

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

            barrelFlash.Stop(true);

            //Interface
            _coolDownSlider = myTankHealth.attackCooldown;
            _coolDownSlider.maxValue = 1f;
            _coolDownSlider.minValue = 0f;
            _coolDownSlider.value = 1f;
        }

        private void Update()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected) return;

            if (SimulatedInput.GetButtonDown(InputCodes.TankFire))
            {
                if (_ammo >= 1)
                {
                    StartCoroutine(nameof(Shoot));
                }
            }

            if (_ammo < 1)
            {
                _ammo += Time.deltaTime / reloadTime;
            }

            _coolDownSlider.value = _ammo;
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

            TankHealth.TankHealth targetHealth = target.transform.gameObject.GetComponent<TankHealth.TankHealth>();

            // Damage if hit any tank
            if (targetHealth && myTankHealth.photonView.IsMine)
            {
                FactionID fID = targetHealth.fid;
                FactionID myID = myTankHealth.fid;

                if (fID.teamIndex != myID.teamIndex)
                {
                    if (fID.myAccID != myID.myAccID)
                    {
                        targetHealth.TakeDamage(damage, myID.actorNumber);
                    }
                }
            }

            photonView.RPC(nameof(HitImpact), RpcTarget.All, target.point, target.normal);
        }

        [PunRPC]
        public void HitImpact(Vector3 position, Vector3 rotation)
        {
            // create impact effect at hit pos
            GameObject i = Instantiate(hitEffect.gameObject, position, Quaternion.identity);
            i.transform.rotation = Quaternion.FromToRotation(Vector3.forward, rotation);
            Destroy(i, 10f);
        }

        #endregion Shoot
    }
}