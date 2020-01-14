using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemAttachToCharacterSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Item>().ForEach((Entity entity,
                ref AttachToCharacterRequest pickupRequest,
                ref TriggerPredictedState triggerState,
                ref TransformPredictedState transformPredictedState,
                ref VelocityPredictedState velocityPredictedState,
                ref ItemPredictedState itemPredictedState,
                ref ReplicatedEntityData replicatedEntityData) =>
            {
                FSLog.Info("ItemAttachToCharacterSystem OnUpdate!");
                //速度比较快不能pickup
                if (math.distancesq(velocityPredictedState.Linear, float3.zero) > 2.0f)
                    return;

                triggerState.TriggeredEntity = Entity.Null;
                triggerState.IsAllowTrigger = false;

                itemPredictedState.Owner = pickupRequest.Owner;
              
                transformPredictedState.Position = new float3(0, -0.2f, 0.9f);
                transformPredictedState.Rotation = quaternion.identity;

                velocityPredictedState.Angular = float3.zero;
                velocityPredictedState.Linear = float3.zero;
                velocityPredictedState.MotionType = MotionType.Static;

                replicatedEntityData.PredictingPlayerId = pickupRequest.PredictingPlayerId;
             
                EntityManager.RemoveComponent<AttachToCharacterRequest>(entity);
            });
        }
    }
}