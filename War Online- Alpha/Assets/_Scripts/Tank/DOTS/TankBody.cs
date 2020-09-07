using Unity.Entities;

namespace _Scripts.Tank.DOTS
{
    public struct TankBody : IComponentData
    {
        public int teamID;
        public float deltaHealth;
    }

    [UpdateAfter(typeof(TankProjectileHit))]
    public class TankHealthSync : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach(
                (Entity e, ref TankBody tb) =>
                {
                    if (tb.deltaHealth > .0000001f)
                    {
                        var t = EntitiesHelper.Eth[e];
                        t.TakeDamage(tb.deltaHealth);
                        tb.deltaHealth = 0;
                    }
                }
            );
        }
    }
}