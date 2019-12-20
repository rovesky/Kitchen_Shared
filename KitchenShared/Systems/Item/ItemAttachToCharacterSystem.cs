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
            Entities.WithAllReadOnly<Plate>().ForEach((Entity entity,
                ref AttachToCharacterRequest pickupRequest,
                ref EntityPredictedState entityPredictedState,
                ref ItemPredictedState itemPredictedState,
                ref ReplicatedEntityData replicatedEntityData) =>
            {
                FSLog.Info("ItemAttachToCharacterSystem OnUpdate!");
                //速度比较快不能pickup
                if (math.distancesq(entityPredictedState.Velocity.Linear, float3.zero) > 2.0f)
                    return;

                itemPredictedState.Owner = pickupRequest.Owner;

                entityPredictedState.Transform.pos = new float3(0, -0.2f, 1.0f);
                entityPredictedState.Transform.rot = quaternion.identity;
                entityPredictedState.Velocity.Linear = float3.zero;

                replicatedEntityData.PredictingPlayerId = pickupRequest.PredictingPlayerId;

                //变成 Static
                if (EntityManager.HasComponent<PhysicsVelocity>(entity))
                    EntityManager.RemoveComponent<PhysicsVelocity>(entity);

                EntityManager.RemoveComponent<AttachToCharacterRequest>(entity);
            });
        }
    }
}