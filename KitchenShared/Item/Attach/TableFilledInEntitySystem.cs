using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct TableFilledInItemRequest : IComponentData
    {
        public Entity ItemEntity;
    }

    [DisableAutoCreation]
    public class TableFilledInItemSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithStructuralChanges().ForEach((Entity entity,
                ref SlotPredictedState slotState,
                in TableFilledInItemRequest request) =>
            {
                FSLog.Info("TableFilledInItemSystem OnUpdate!");

                slotState.FilledInEntity = request.ItemEntity;
                EntityManager.RemoveComponent<TableFilledInItemRequest>(entity);
            }).Run();
        }
    }
}