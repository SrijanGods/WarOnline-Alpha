using Unity.Entities;

namespace _Scripts.Tank.DOTS
{
    public struct TankBody : IComponentData
    {
        public int TeamID, LastAttackedBy;
        public float DeltaHealth;
    }

    [UpdateAfter(typeof(TankProjectileHit))]
    public class TankHealthSync : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach(
                (Entity e, ref TankBody tb) =>
                {
                    if (tb.DeltaHealth > .0000001f)
                    {
                        var t = EntitiesHelper.Eth[e];
                        t.TakeDamage(tb.DeltaHealth, tb.LastAttackedBy);
                        tb.DeltaHealth = 0;
                    }
                }
            );
        }
    }
}