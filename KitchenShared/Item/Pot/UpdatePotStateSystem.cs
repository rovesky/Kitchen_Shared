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
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref FireAlertPredictedState fireAlertState,
                    in OwnerPredictedState ownerState,
                    in BurntPredictedState burntState) =>
                {
                    if (burntState.IsBurnt)
                        EntityManager.AddComponentData(entity, new Burnt());
                    else
                        EntityManager.RemoveComponent<Burnt>(entity);

                    if (ownerState.Owner != Entity.Null &&
                        EntityManager.HasComponent<Character>(ownerState.Owner))
                    {
                        fireAlertState.CurTick = 0;
                    }

                }).Run();
        }
    }
}