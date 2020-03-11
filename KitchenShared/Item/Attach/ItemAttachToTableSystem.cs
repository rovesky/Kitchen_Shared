using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemAttachToTableSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Item>().ForEach((Entity entity,
                ref AttachToTableRequest request,
                ref TransformPredictedState transformPredictedState,
                ref VelocityPredictedState  velocityPredictedState,
                ref TriggerPredictedState triggerState,
                ref ItemPredictedState itemState) =>
            {
                triggerState.TriggeredEntity = Entity.Null;
                triggerState.IsAllowTrigger = false;
                
                FSLog.Info("ItemAttachToTableSystem OnUpdate!");
                transformPredictedState.Position = request.SlotPos;
                transformPredictedState.Rotation = quaternion.identity;

                velocityPredictedState.Linear = float3.zero;
                velocityPredictedState.Angular = float3.zero;
                velocityPredictedState.MotionType = MotionType.Static;
            
                EntityManager.RemoveComponent<AttachToTableRequest>(entity);
            });
        }
    }


 
}