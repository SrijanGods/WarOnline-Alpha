using System.Collections;
using _Scripts.Photon.Game;
using _Scripts.Photon.Room;
using _Scripts.Tank.DOTS;
using Photon.Pun;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Tank.TankHealth
{
    public class TankHealth : MonoBehaviourPun
    {
        [Header("Prefab")] public GameObject entityPrefab;

        [Header("Health")] public float startingHealth = 100f;
        public Color m_FullHealthColor;
        public Color m_ZeroHealthColor;
        public float currentHealth;

        [Header("UI Stuff")] public GameObject warCanvas, rtcCanvas;
        [Header("UI Stuff")] public Image health;
        [Header("UI Stuff")] public Slider attackCooldown;

        [Header("Teams & Enemy UI")] public GameObject othersCanvass;
        public Slider otherShownHealth;
        public Text otherShownName;

        [Header("GameObjects")] public GameObject actualHull, actualTurret, destroyedHull, destroyedTurret;

        //public GameObject m_ExplosionPrefab;

        private bool spawnCalled;
        [HideInInspector] public int myTeam;
        [HideInInspector] public FactionID fid;
        private string _playerTankName;
        private AudioSource m_ExplosionAudio;
        private ParticleSystem m_ExplosionParticles;
        private bool m_Dead;
        private GameManager _photonScript;
        private bool _destroyCalled;

        private BlobAssetStore _blobAssetStore;
        private EntityManager _entityManager;

        private Entity _instantiatedEntity;

        private GameSession _session;

        private void OnEnable()
        {
            _session = FindObjectOfType<GameSession>();

            // ECS

            _blobAssetStore = new BlobAssetStore();

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var settings =
                GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);

            var converted =
                GameObjectConversionUtility.ConvertGameObjectHierarchy(entityPrefab, settings);

            _instantiatedEntity = _entityManager.Instantiate(converted);

#if UNITY_EDITOR
            _entityManager.SetName(_instantiatedEntity, gameObject.name);
#endif

            _entityManager.AddComponentData(_instantiatedEntity, new TankBody
            {
                TeamID = myTeam
            });

            _entityManager.AddComponentData(_instantiatedEntity, new SyncEntity
            {
                translation = true, rotation = true
            });

            EntitiesHelper.Tse.Add(_instantiatedEntity, actualHull.transform);
            EntitiesHelper.Eth.Add(_instantiatedEntity, this);

            // fields

            _destroyCalled = false;

            m_Dead = false;

            fid = GetComponent<FactionID>();
            myTeam = fid.teamIndex;

            currentHealth = startingHealth;
            _photonScript = FindObjectOfType<GameManager>();

            otherShownHealth.maxValue = startingHealth;
            otherShownHealth.value = currentHealth;

            if (photonView.IsMine || !PhotonNetwork.IsConnected)
            {
                rtcCanvas.SetActive(true);
                warCanvas.SetActive(true);
                othersCanvass.SetActive(false);
            }
            else
            {
                rtcCanvas.SetActive(false);
                warCanvas.SetActive(false);
                othersCanvass.SetActive(true);
            }
        }

        [PunRPC]
        public void UpdateHealth(float healthVal, int lastAttackedBy)
        {
            currentHealth = healthVal;

            SetHealthUI(currentHealth);

            if (currentHealth <= 0)
            {
                if (photonView.IsMine)
                    _session.photonView.RPC(nameof(_session.OnPlayerDieRPC), RpcTarget.All,
                        fid.actorNumber, lastAttackedBy);

                OnDeath();
            }
        }

        public void TakeDamage(float damage, int lastAttackedBy)
        {
            // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.

            currentHealth -= damage;

            SetHealthUI(currentHealth);

            photonView.RPC(nameof(UpdateHealth), RpcTarget.Others, currentHealth, lastAttackedBy);

            if (currentHealth <= 0)
            {
                if (photonView.IsMine)
                    _session.photonView.RPC(nameof(_session.OnPlayerDieRPC), RpcTarget.All,
                        fid.actorNumber, lastAttackedBy);

                OnDeath();
            }
        }

        private void SetHealthUI(float healthVal)
        {
            // Adjust the value and colour of the slider.

            health.fillAmount = healthVal;

            otherShownHealth.value = healthVal;

            if (healthVal < .05f)
            {
                health.color = Color.red;
            }
            else if (healthVal > .95f)
            {
                health.color = Color.white;
            }
            else
            {
                health.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, healthVal / startingHealth);
            }
        }

        [PunRPC]
        public void SetColorFromFlameThrower(bool isthrowing, Color color)
        {
            if (isthrowing)
            {
                actualHull.GetComponent<MeshRenderer>().material.color =
                    Color.Lerp(Color.white, color, Time.deltaTime * 2f);
            }
            else if (actualHull.GetComponentInChildren<MeshRenderer>().material.color != Color.white)
            {
                var currentColor = actualHull.GetComponent<MeshRenderer>().material.color;
                actualHull.GetComponent<MeshRenderer>().material.color =
                    Color.Lerp(currentColor, Color.white, Time.deltaTime * 2f);
            }
        }

        void OnDeath()
        {
            // Play the effects for the death of the tank and deactivate it.

            m_Dead = true;


            if (!_destroyCalled)
            {
                actualHull.SetActive(false);
                actualTurret.SetActive(false);

                destroyedTurret.SetActive(true);
                destroyedHull.SetActive(true);
                _destroyCalled = true;
                RTC_TankController tankController = gameObject.GetComponent<RTC_TankController>();
                tankController.engineRunning = false;
            }

            StartCoroutine(nameof(Destroying));
        }

        private IEnumerator Destroying()
        {
            yield return new WaitForSeconds(1.8f);

            PhotonNetwork.Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (!spawnCalled)
            {
                //photonScript.SpawnTank();
                spawnCalled = true;
            }

            EntitiesHelper.Eth.Remove(_instantiatedEntity);
            _blobAssetStore.Dispose();
        }

        private void OnDisable()
        {
            enabled = true;
        }
    }
}