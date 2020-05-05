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
                .ForEach((Entity entity,
                    in CatchFirePredictedState catchFireState) =>
                {
                    if (catchFireState.IsCatchFire)
                        EntityManager.AddComponentData(entity, new CatchFire());
                    else
                        EntityManager.RemoveComponent<CatchFire>(entity);
                    

                }).Run();
        }
    }
}