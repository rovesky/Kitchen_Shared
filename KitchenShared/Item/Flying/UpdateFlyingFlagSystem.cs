using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 更新飞行标识
    /// </summary>
    [DisableAutoCreation]
    public class UpdateFlyingFlagSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .WithNone<Flying>()
                .ForEach((Entity entity,
                    in FlyingPredictedState flyingState) =>
                {
                    if (flyingState.IsFlying)
                    {
                        EntityManager.AddComponentData(entity, new Flying());
                     //   FSLog.Info($"new Flying(),entity:{entity}");
                    }
                }).Run();

            Entities.WithAll<ServerEntity,Flying>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in FlyingPredictedState flyingState) =>
                {
                    if (!flyingState.IsFlying)
                        EntityManager.RemoveComponent<Flying>(entity);

                }).Run();
        }
    }
}