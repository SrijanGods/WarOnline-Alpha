using System.Collections;
using System.Diagnostics.CodeAnalysis;
using _Scripts.Photon.Room;
using _Scripts.Tank.DOTS;
using _Scripts.Tank.Turrets;
using Photon.Pun;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;

namespace _Scripts.Tank.Projectile
{
    public class TankProjectile : MonoBehaviour
    {
        public bool damageAllies;
        public int teamID;

        public float damage, livingTime, range;
        public ParticleSystem particle, destroyedParticle;

        [FMODUnity.EventRef] public string impactSfx = "event:/TurretDuosImpact";

        public GameObject commonParent, entityPrefab, body;

        public float force;
        public Vector3 dir;

        public ProjectileShooter turretParent;

        public Transform enemy;
        public bool autoAim;

        [HideInInspector] public PhotonView pvw;

        private float _livedTime;
        private Vector3 _startPos;
        private bool _dead;

        private FMOD.Studio.EventInstance _impactEv;

        private BlobAssetStore _blobAssetStore;
        private EntityManager _entityManager;

        private Entity _instantiatedEntity;

        private void Awake()
        {
            _impactEv = FMODUnity.RuntimeManager.CreateInstance(impactSfx);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(_impactEv, GetComponent<Transform>(),
                GetComponent<Rigidbody>());
        }

        public void Ready()
        {
            if (!pvw.IsMine && PhotonNetwork.IsConnected) return;

            if (_instantiatedEntity != Entity.Null) return;

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

            _entityManager.AddComponentData(_instantiatedEntity, new SyncGameObject
            {
                translation = true, rotation = true
            });

            EntitiesHelper.Est.Add(_instantiatedEntity, transform);
            EntitiesHelper.Etp.Add(_instantiatedEntity, this);
        }

        private void OnEnable()
        {
            if (destroyedParticle) destroyedParticle.Stop(true);
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void FixedUpdate()
        {
            if (_dead) return;

            _livedTime += Time.fixedDeltaTime;

            if (_livedTime > livingTime)
            {
                OnDie();
                return;
            }

            var cPos = transform.position;
            if (Vector3.Distance(cPos, _startPos) > range)
            {
                OnDie();
                return;
            }

            // Changing the velocity towards the enemy tank

            var newDir = dir;

            if (autoAim && enemy) newDir = enemy.position - transform.position;

            if (newDir != dir)
            {
                var pv = _entityManager.GetComponentData<PhysicsVelocity>(_instantiatedEntity);

                var vel = pv.Linear;

                vel = transform.TransformDirection(vel);

                transform.forward = Vector3.Lerp(transform.forward, newDir.normalized, Time.fixedDeltaTime);

                vel = transform.InverseTransformDirection(vel);

                pv.Linear = vel;

                _entityManager.SetComponentData(_instantiatedEntity, pv);
            }
        }

        public void Launch()
        {
            StartCoroutine(nameof(LaunchStart));
        }

        private IEnumerator LaunchStart()
        {
            if (!pvw.IsMine && PhotonNetwork.IsConnected)
            {
                _dead = false;

                commonParent.SetActive(true);

                if (destroyedParticle) destroyedParticle.Stop(true);
                if (particle) particle.Play(true);
            }
            else
            {
                _livedTime = 0;

                var position = transform.position;
                // ReSharper disable once Unity.InefficientPropertyAccess
                var rotation = transform.rotation;

                _startPos = position;

                _entityManager.SetComponentData(_instantiatedEntity, new DOTS.TankProjectile
                {
                    DamageAllies = damageAllies, TeamID = teamID,
                    Damage = damage, HitCount = 0, MAXHitCount = turretParent.maxHitCount,
                    OwnerActorNumber = turretParent.GetComponentInParent<FactionID>().actorNumber
                });

                _entityManager.SetComponentData(_instantiatedEntity, new Translation
                {
                    Value = position
                });

                _entityManager.SetComponentData(_instantiatedEntity, new Rotation
                {
                    Value = rotation
                });

                _entityManager.SetComponentData(_instantiatedEntity, new LocalToWorld
                {
                    Value = new float4x4(rotation, position)
                });

                var pv = _entityManager.GetComponentData<PhysicsVelocity>(_instantiatedEntity);
                var pm = _entityManager.GetComponentData<PhysicsMass>(_instantiatedEntity);

                pv.Linear = pv.Angular = float3.zero;

                _entityManager.SetComponentData(_instantiatedEntity, pv);

                _entityManager.SetEnabled(_instantiatedEntity, true);

                ComponentExtensions.ApplyLinearImpulse(ref pv, in pm, dir * force);

                _entityManager.SetComponentData(_instantiatedEntity, pv);

                commonParent.SetActive(true);
                body.SetActive(true);

                if (destroyedParticle) destroyedParticle.Stop(true);
                if (particle) particle.Play(true);

                _dead = false;
            }

            yield break;
        }

        public void OnDie()
        {
            turretParent.photonView.RPC(nameof(turretParent.KillProjectile), RpcTarget.All,
                turretParent.ActiveProjectiles.IndexOf(this));
        }

        public void SilentDie()
        {
            if (!pvw.IsMine && PhotonNetwork.IsConnected)
            {
                _dead = true;

                if (particle) particle.Stop(true);
                if (destroyedParticle) destroyedParticle.Stop(true);
            }
            else
            {
                _dead = true;

                _entityManager.SetEnabled(_instantiatedEntity, false);

                var pv = _entityManager.GetComponentData<PhysicsVelocity>(_instantiatedEntity);
                pv.Linear = pv.Angular = float3.zero;
                _entityManager.SetComponentData(_instantiatedEntity, pv);

                if (particle) particle.Stop(true);
                if (destroyedParticle) destroyedParticle.Stop(true);
            }

            body.SetActive(false);

            commonParent.SetActive(false);

            if (!turretParent.ActiveProjectiles.Contains(this)) return;

            turretParent.ActiveProjectiles.Remove(this);
            turretParent.InactiveProjectiles.Add(this);
        }

        public void KillSelf()
        {
            StartCoroutine(nameof(DieStart));
        }

        private IEnumerator DieStart()
        {
            if (!pvw.IsMine && PhotonNetwork.IsConnected)
            {
                _dead = true;

                if (particle) particle.Stop(true);
                if (destroyedParticle) destroyedParticle.Play(true);
            }
            else
            {
                _dead = true;

                _entityManager.SetEnabled(_instantiatedEntity, false);

                var pv = _entityManager.GetComponentData<PhysicsVelocity>(_instantiatedEntity);
                pv.Linear = pv.Angular = float3.zero;
                _entityManager.SetComponentData(_instantiatedEntity, pv);

                if (particle) particle.Stop(true);
                if (destroyedParticle) destroyedParticle.Play(true);
            }

            _impactEv.start();

            body.SetActive(false);

            yield return new WaitWhile(() => destroyedParticle.isPlaying);

            commonParent.SetActive(false);

            if (!turretParent.ActiveProjectiles.Contains(this)) yield break;

            turretParent.ActiveProjectiles.Remove(this);
            turretParent.InactiveProjectiles.Add(this);
        }

        private void OnDestroy()
        {
            EntitiesHelper.Etp.Remove(_instantiatedEntity);
            _blobAssetStore.Dispose();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(commonParent.activeSelf);
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation.eulerAngles);
            }
            else if (stream.IsReading)
            {
                commonParent.SetActive((bool) stream.ReceiveNext());
                transform.position = (Vector3) stream.ReceiveNext();
                transform.rotation = Quaternion.Euler((Vector3) stream.ReceiveNext());
            }
        }
    }
}