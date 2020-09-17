using System.Collections.Generic;
using _Scripts.Tank.Projectile;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Tank.Turrets
{
    [DisallowMultipleComponent]
    public abstract class ProjectileShooter : MonoBehaviourPun, IPunObservable
    {
        [Header("Projectile Prefab")] public GameObject projectilePrefab;
        [Header("Limit")] public int maxProjectilesCount;
        [Header("Projectile Values")] public int maxHitCount;
        public float fireRate, force, damage, range;
        public bool autoAim;

        public readonly List<TankProjectile> InactiveProjectiles = new List<TankProjectile>(),
            ActiveProjectiles = new List<TankProjectile>();

        private void OnDestroy()
        {
            foreach (var projectile in InactiveProjectiles)
            {
                if (projectile) Destroy(projectile.gameObject);
            }

            foreach (var projectile in ActiveProjectiles)
            {
                if (projectile) Destroy(projectile.gameObject);
            }
        }

        [PunRPC]
        public void KillProjectile(int index)
        {
            ActiveProjectiles[index].KillSelf();
        }

        // for syncing the duos projectiles count and transform in all clients
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // send the count positions and rotations
                var t = new float3x2[ActiveProjectiles.Count];
                for (int i = 0; i < ActiveProjectiles.Count; i++)
                {
                    var tf = ActiveProjectiles[i].transform;
                    var tp = new float3x2(tf.position, tf.rotation.eulerAngles);
                    t[i] = tp;
                }

                stream.SendNext(t);
            }

            if (stream.IsReading)
            {
                var t = (float3x2[]) stream.ReceiveNext();

                // if there are less active projectiles than count then take from inactive array or create more
                if (ActiveProjectiles.Count < t.Length) SpawnProjectiles(t.Length - ActiveProjectiles.Count);

                // if there are more active projectiles than count and deactivate some
                if (ActiveProjectiles.Count > t.Length)
                {
                    for (int i = 0; i < ActiveProjectiles.Count - t.Length; i++)
                    {
                        ActiveProjectiles[0].SilentDie();
                    }
                }

                // lastly set pos and rot
                for (int i = 0; i < ActiveProjectiles.Count; i++)
                {
                    var tf = ActiveProjectiles[i].transform;
                    var pr = t[i];
                    tf.position = pr.c0;
                    tf.eulerAngles = pr.c1;
                }
            }
        }

        private void SpawnProjectiles(int c)
        {
            for (int i = 0; i < c; i++)
            {
                GameObject g;
                TankProjectile tp;

                if (InactiveProjectiles.Count > 0)
                {
                    tp = InactiveProjectiles[0];
                    InactiveProjectiles.Remove(tp);

                    g = tp.gameObject;
                    g.transform.SetParent(null, false);
                }
                else
                {
                    g = Instantiate(projectilePrefab, null, false);
                    tp = g.GetComponent<TankProjectile>();
                }

                g.transform.localPosition = Vector3.zero;
                g.transform.localRotation = Quaternion.identity;
                g.transform.localScale = Vector3.one;

                tp.turretParent = this;
                tp.pvw = photonView;
                tp.Ready();

                tp.force = force;
                tp.damage = damage;
                tp.dir = Vector3.zero;
                tp.range = range;
                tp.autoAim = autoAim;
                tp.enemy = null;

                ActiveProjectiles.Add(tp);
            }
        }
    }
}