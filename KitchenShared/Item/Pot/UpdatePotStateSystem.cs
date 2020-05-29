using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 更新锅的状态
    /// </summary>
    [DisableAutoCreation]
    public class UpdatePotStateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .ForEach((Entity entity,
                    ref FireAlertPredictedState fireAlertState,
                    in OwnerPredictedState ownerState) =>
                {
                    if (ownerState.Owner != Entity.Null &&
                        HasComponent<Character>(ownerState.Owner))
                    {
                        fireAlertState.CurTick = 0;
                    }

                }).Run();

            Entities.WithAll<ServerEntity, Burnt>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in PotPredictedState potState) =>
                {
                    if (potState.State != PotState.Burnt)
                        EntityManager.RemoveComponent<Burnt>(entity);
                }).Run();

            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .WithNone<Burnt>()
                .ForEach((Entity entity,
                    in PotPredictedState potState) =>
                {
                    if (potState.State == PotState.Burnt)
                        EntityManager.AddComponentData(entity, new Burnt());
                }).Run();
        }
    }
}