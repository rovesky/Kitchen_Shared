using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 更新灭火器的状态
    /// </summary>
    [DisableAutoCreation]
    public class UpdateExtinguisherStateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref ExtinguisherPredictedState extinguishState,
                    in OwnerPredictedState ownerState
                    ) =>
                {
                  
                    if (ownerState.Owner != Entity.Null &&
                        !EntityManager.HasComponent<Character>(ownerState.Owner))
                    {
                        extinguishState.Distance = 0;
                    }

                }).Run();
        }
    }
}