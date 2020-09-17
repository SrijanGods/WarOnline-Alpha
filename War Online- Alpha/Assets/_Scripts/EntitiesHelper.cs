using System.Collections.Generic;
using _Scripts.Tank.Projectile;
using _Scripts.Tank;
using Unity.Entities;
using UnityEngine;

namespace _Scripts
{
    public static class EntitiesHelper
    {
        // Collection of entities and corresponding tank projectile
        public static readonly Dictionary<Entity, TankProjectile> Etp = new Dictionary<Entity, TankProjectile>();

        // Collection of entities and corresponding tank health
        public static readonly Dictionary<Entity, TankHealth> Eth = new Dictionary<Entity, TankHealth>();

        // Collection of entities and corresponding to sync transform
        public static readonly Dictionary<Entity, Transform> Est = new Dictionary<Entity, Transform>();

        // Collection of entities and corresponding from sync transform
        public static readonly Dictionary<Entity, Transform> Tse = new Dictionary<Entity, Transform>();
    }
}