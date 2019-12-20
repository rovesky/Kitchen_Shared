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
    public class ItemToCharacterSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Plate>().ForEach((Entity entity,
                ref PickUpRequest        pickupRequest, 
                ref EntityPredictedState entityPredictedState,
                ref ItemPredictedState   itemPredictedState,
                ref ReplicatedEntityData replicatedEntityData) =>
            {
                itemPredictedState.Owner = pickupRequest.Owner;

                entityPredictedState.Transform.pos = new float3(0, -0.2f, 1.0f);
                entityPredictedState.Transform.rot = quaternion.identity;
                entityPredictedState.Velocity.Linear = float3.zero;

                replicatedEntityData.PredictingPlayerId = pickupRequest.PredictingPlayerId;

                //变成 Static
                if (EntityManager.HasComponent<PhysicsVelocity>(entity))
                {
                    EntityManager.RemoveComponent<PhysicsVelocity>(entity);
                }
                EntityManager.RemoveComponent<PickUpRequest>(entity);
            });
        }
    }
}