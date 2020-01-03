using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemToTableSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Item>().ForEach((Entity entity,
                ref ItemPredictedState itemState,
                ref TriggerPredictedState triggerState) =>
            {
                if (!itemState.IsDynamic)
                    return;

                if (triggerState.TriggeredEntity == Entity.Null)
                    return;

                var triggeredEntity = triggerState.TriggeredEntity;
                if (!EntityManager.HasComponent<TriggerData>(triggeredEntity))
                    return;

                var triggerData = EntityManager.GetComponentData<TriggerData>(triggeredEntity);
                if ((triggerData.Type & (int) TriggerType.Table) == 0)
                    return;

                var slot = EntityManager.GetComponentData<SlotPredictedState>(triggeredEntity);
                if(slot.FilledInEntity != Entity.Null)
                    return;

                FSLog.Info("ItemToTableSystem OnUpdate!");
                var request = new AttachToTableRequest()
                {
                    ItemEntity = entity,
                    SlotPos = triggerData.SlotPos
                };

                EntityManager.AddComponentData(entity, request);
                EntityManager.AddComponentData(triggeredEntity, request);
            });
        }
    }

    [DisableAutoCreation]
    public class ItemAttachToTableSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Item>().ForEach((Entity entity,
                ref TransformPredictedState transformPredictedState,
                ref VelocityPredictedState  velocityPredictedState,
                ref TriggerPredictedState triggerState,
                ref ItemPredictedState itemState,
                ref AttachToTableRequest request) =>
            {
                triggerState.TriggeredEntity = Entity.Null;
                
                FSLog.Info("ItemAttachToTableSystem OnUpdate!");
                transformPredictedState.Position = request.SlotPos;
                transformPredictedState.Rotation = quaternion.identity;

                velocityPredictedState.Linear = float3.zero;
                velocityPredictedState.Angular = float3.zero;

                itemState.IsDynamic = false;
                //变成 Static
                //     if (EntityManager.HasComponent<PhysicsVelocity>(entity))
                //       EntityManager.RemoveComponent<PhysicsVelocity>(entity);

                EntityManager.RemoveComponent<AttachToTableRequest>(entity);
            });
        }
    }


    [DisableAutoCreation]
    public class AttachToTableSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,
                ref AttachToTableRequest request,
                ref SlotPredictedState slotState) =>
            {
                FSLog.Info("AttachToTableSystem OnUpdate!");
                slotState.FilledInEntity = request.ItemEntity;
                EntityManager.RemoveComponent<AttachToTableRequest>(entity);
            });
        }
    }
}