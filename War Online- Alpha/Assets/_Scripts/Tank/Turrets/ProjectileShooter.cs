using System.Collections.Generic;
using _Scripts.Tank.Projectile;
using UnityEngine;

namespace _Scripts.Tank.Turrets
{
    [DisallowMultipleComponent]
    public abstract class ProjectileShooter : MonoBehaviour
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
    }
}