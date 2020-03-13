using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemAttachToTableSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<Item>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref TransformPredictedState transformPredictedState,
                    ref VelocityPredictedState  velocityPredictedState,
                    ref TriggerPredictedState triggerState,
                //    ref ItemPredictedState itemState,
                    in ItemAttachToTableRequest request) =>
            {
                FSLog.Info("ItemAttachToTableSystem OnUpdate!");

                EntityManager.RemoveComponent<ItemAttachToTableRequest>(entity);

                triggerState.TriggeredEntity = Entity.Null;
                triggerState.IsAllowTrigger = false;
          
                transformPredictedState.Position = request.SlotPos;
                transformPredictedState.Rotation = quaternion.identity;

                velocityPredictedState.Linear = float3.zero;
                velocityPredictedState.Angular = float3.zero;
                velocityPredictedState.MotionType = MotionType.Static;
               
            }).Run();
        }
    }
}