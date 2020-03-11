using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct TableFilledInItemRequest : IComponentData
    {
        public Entity ItemEntity;
    }

    [DisableAutoCreation]
    public class TableFilledInItemSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,
                ref TableFilledInItemRequest request,
                ref SlotPredictedState slotState) =>
            {
                FSLog.Info("TableFilledInItemSystem OnUpdate!");
                slotState.FilledInEntity = request.ItemEntity;
                EntityManager.RemoveComponent<TableFilledInItemRequest>(entity);
            });
        }
    }
}