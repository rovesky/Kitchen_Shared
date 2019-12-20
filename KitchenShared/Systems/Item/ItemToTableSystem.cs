using FootStone.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEditor.UIElements;
using UnityEngine.Assertions;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemToTableSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Plate>().ForEach((Entity entity, 
                ref EntityPredictedState entityPredictedState,
                ref TriggerPredictedState triggerState,
                ref PhysicsVelocity velocity) =>
            {
                if (triggerState.TriggeredEntity == Entity.Null)
                    return;

                var triggeredEntity = triggerState.TriggeredEntity;
                if (!EntityManager.HasComponent<TriggerData>(triggeredEntity))
                    return;

                var triggerData = EntityManager.GetComponentData<TriggerData>(triggeredEntity);

                if ((triggerData.Type & (int) TriggerType.Table) == 0)
                    return;

                entityPredictedState.Transform.pos = triggerData.SlotPos;
                entityPredictedState.Transform.rot = quaternion.identity;
                entityPredictedState.Velocity.Linear = float3.zero;
                
                EntityManager.RemoveComponent<PhysicsVelocity>(entity);

                var slot = EntityManager.GetComponentData<SlotPredictedState>(triggeredEntity);
                slot.FilledInEntity = entity;
                EntityManager.SetComponentData(triggeredEntity, slot);
            });
        }
    }
}