using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace _Scripts.Tank.DOTS
{
    [GenerateAuthoringComponent]
    public struct TankProjectile : IComponentData
    {
        public bool DamageAllies;
        public int TeamID, HitCount, MAXHitCount, OwnerActorNumber;
        public float Damage;
    }

    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class TankProjectileHit : JobComponentSystem
    {
        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;

        [BurstCompile]
        internal struct TankProjectileHitJob : ITriggerEventsJob
        {
            public ComponentDataFromEntity<LocalToWorld> Ltws;
            public ComponentDataFromEntity<TankProjectile> Projectiles;
            public ComponentDataFromEntity<TankBody> Tanks;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity eA = triggerEvent.EntityA, eB = triggerEvent.EntityB;

                bool isBodyATank = Tanks.HasComponent(eA), isBodyBTank = Tanks.HasComponent(eB);

                bool isBodyAProjectile = Projectiles.HasComponent(eA), isBodyBProjectile = Projectiles.HasComponent(eB);

                if (!(isBodyAProjectile || isBodyBProjectile)) return;

                var p = isBodyAProjectile ? eA : eB;
                var c = Projectiles[p];

                c.HitCount += 1;

                Projectiles[p] = c;

                if (!(isBodyATank || isBodyBTank)) return;

                var t = isBodyATank ? eA : eB;

                var ct = Tanks[t];

                // If we can damage ally then proceed else check if the tank hit is enemy then proceed
                // Also damage tank if it is in test mode teamid -1
                if ((c.TeamID == -1 && ct.TeamID == -1) || (c.DamageAllies || c.TeamID != ct.TeamID))
                {
                    if (c.HitCount <= c.MAXHitCount)
                    {
                        ct.DeltaHealth -= c.Damage;
                        ct.LastAttackedBy = c.OwnerActorNumber;
                    }
                }

                Tanks[t] = ct;
            }
        }

        protected override void OnCreate()
        {
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new TankProjectileHitJob
            {
                Ltws = GetComponentDataFromEntity<LocalToWorld>(),
                Projectiles = GetComponentDataFromEntity<TankProjectile>(),
                Tanks = GetComponentDataFromEntity<TankBody>()
            }.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, inputDeps);
        }
    }

    [UpdateAfter(typeof(TankProjectileHit))]
    public class ProjectileHitDestroy : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach(
                (Entity e, ref TankProjectile tp) =>
                {
                    if (tp.HitCount < tp.MAXHitCount) return;

                    var p = EntitiesHelper.Etp[e];
                    p.OnDie();
                    tp.HitCount = 0;
                }
            );
        }
    }
}