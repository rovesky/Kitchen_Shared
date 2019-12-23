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
            Entities.WithAllReadOnly<Plate, PhysicsVelocity>().ForEach((Entity entity,
                ref TriggerPredictedState triggerState) =>
            {
             
                if (triggerState.TriggeredEntity == Entity.Null)
                    return;

                var triggeredEntity = triggerState.TriggeredEntity;
                if (!EntityManager.HasComponent<TriggerData>(triggeredEntity))
                    return;

                var triggerData = EntityManager.GetComponentData<TriggerData>(triggeredEntity);

                if ((triggerData.Type & (int) TriggerType.Table) == 0)
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
            Entities.WithAllReadOnly<Plate>().ForEach((Entity entity,
                ref TransformPredictedState transformPredictedState,
                ref VelocityPredictedState  velocityPredictedState,
                ref TriggerPredictedState triggerState,
                ref AttachToTableRequest request) =>
            {
                triggerState.TriggeredEntity = Entity.Null;

                FSLog.Info("ItemAttachToTableSystem OnUpdate!");
                transformPredictedState.Position = request.SlotPos;
                transformPredictedState.Rotation = quaternion.identity;
                velocityPredictedState.Linear = float3.zero;

                //变成 Static
                if (EntityManager.HasComponent<PhysicsVelocity>(entity))
                    EntityManager.RemoveComponent<PhysicsVelocity>(entity);

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