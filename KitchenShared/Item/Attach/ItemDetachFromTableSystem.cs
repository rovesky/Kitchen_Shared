using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class DetachFromTableSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,
                ref DetachFromTableRequest request,
                ref SlotPredictedState slotState) =>
            {
                FSLog.Info("DetachFromTableSystem OnUpdate!");
                slotState.FilledInEntity = Entity.Null;
                EntityManager.RemoveComponent<DetachFromTableRequest>(entity);
             
            });
        }
    }
}