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
                ref TransformPredictedState transformPredictedState,
                ref VelocityPredictedState velocityPredictedState,
                ref ItemPredictedState itemPredictedState,
                ref ReplicatedEntityData replicatedEntityData) =>
            {
                FSLog.Info("ItemAttachToCharacterSystem OnUpdate!");
                //速度比较快不能pickup
                if (math.distancesq(velocityPredictedState.Linear, float3.zero) > 2.0f)
                    return;

                itemPredictedState.Owner = pickupRequest.Owner;

                transformPredictedState.Position = new float3(0, -0.2f, 1.0f);
                transformPredictedState.Rotation = quaternion.identity;
                velocityPredictedState.Linear = float3.zero;

                replicatedEntityData.PredictingPlayerId = pickupRequest.PredictingPlayerId;

                //变成 Static
                if (EntityManager.HasComponent<PhysicsVelocity>(entity))
                    EntityManager.RemoveComponent<PhysicsVelocity>(entity);

                EntityManager.RemoveComponent<AttachToCharacterRequest>(entity);
            });
        }
    }
}