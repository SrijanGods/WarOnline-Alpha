using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace _Scripts.Tank.DOTS
{
    public struct SyncGameObject : IComponentData
    {
        public bool translation, rotation;
    }

    public struct SyncEntity : IComponentData
    {
        public bool translation, rotation;
    }

    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class GameObjectSync : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach(
                (Entity e, ref LocalToWorld ltw, ref SyncGameObject sgo) =>
                {
                    var t = EntitiesHelper.Est[e];
                    if (sgo.translation) t.position = ltw.Position;
                    if (sgo.rotation) t.rotation = ltw.Rotation;
                }
            );
        }
    }

    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class EntitySync : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach(
                    (Entity e, ref Translation tr, ref Rotation rt, ref LocalToWorld ltw,
                        ref SyncEntity se) =>
                    {
                        var t = EntitiesHelper.Tse[e];
                        if (se.translation)
                        {
                            var position = t.position;

                            var newWorldPosition = new float4(position, 1);
                            var worldToLocal = math.inverse(ltw.Value);
                            var newLocalPos = tr.Value + math.mul(worldToLocal, newWorldPosition).xyz;

                            tr.Value = newLocalPos;
                            ltw.Value = new float4x4(ltw.Rotation, position);
                        }

                        if (se.rotation)
                        {
                            var rotation = t.rotation;
                            rt.Value = rotation;
                            ltw.Value = new float4x4(rotation, ltw.Position);
                        }
                    }
                );
        }
    }
}