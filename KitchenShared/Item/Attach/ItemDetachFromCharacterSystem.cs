using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemDetachFromCharacterSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Plate>().ForEach((Entity entity,
                ref DetachFromCharacterRequest request,
                ref EntityPredictedState entityPredictedState,
                ref ItemPredictedState itemPredictedState,
                ref ReplicatedEntityData replicatedEntityData) =>
            {
                FSLog.Info($"ItemDetachFromCharacterSystem OnUpdate!");
              
                itemPredictedState.Owner = Entity.Null;

                entityPredictedState.Transform.pos = request.Pos;
                entityPredictedState.Transform.rot = quaternion.identity;
                entityPredictedState.Velocity.Linear = float3.zero;
         
                replicatedEntityData.PredictingPlayerId = -1;

                //变成 dynamtic
                if (!EntityManager.HasComponent<PhysicsVelocity>(entity))
                    EntityManager.AddComponent<PhysicsVelocity>(entity);

                EntityManager.RemoveComponent<DetachFromCharacterRequest>(entity);

            });
        }
    }
}