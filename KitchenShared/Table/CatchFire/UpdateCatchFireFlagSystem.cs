using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 更新燃烧标识
    /// </summary>
    [DisableAutoCreation]
    public class UpdateCatchFireFlagSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .WithNone<CatchFire>()
                .ForEach((Entity entity,
                    in CatchFirePredictedState catchFireState) =>
                {
                    if (catchFireState.IsCatchFire)
                        EntityManager.AddComponentData(entity, new CatchFire());
                }).Run();

            Entities.WithAll<ServerEntity,CatchFire>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in CatchFirePredictedState catchFireState) =>
                {
                    if (!catchFireState.IsCatchFire)
                        EntityManager.RemoveComponent<CatchFire>(entity);

                }).Run();
        }
    }
}