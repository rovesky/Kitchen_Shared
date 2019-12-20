using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterThrowSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Character>().ForEach((Entity entity,
                ref CharacterThrowItem pickupItem,
                ref UserCommand command,
                ref CharacterPredictedState predictData,
                ref EntityPredictedState entityPredictData) =>
            {
                //  FSLog.Info("PickSystem Update");
                if (!command.Buttons.IsSet(UserCommand.Button.Throw))
                    return;

                if (predictData.PickupedEntity == Entity.Null)
                    return;

                var pickupedEntity = predictData.PickupedEntity;
                var itemEntityPredictedState = EntityManager.GetComponentData<EntityPredictedState>(pickupedEntity);

                Vector3 linear = math.mul(entityPredictData.Transform.rot, Vector3.forward);
                linear.y = 0.3f;
                linear.Normalize();
                itemEntityPredictedState.Velocity.Linear = linear * 11.0f;
                itemEntityPredictedState.Transform.pos = entityPredictData.Transform.pos +
                    math.mul(entityPredictData.Transform.rot, new float3(0, 0.2f, 0.8f));
                EntityManager.SetComponentData(pickupedEntity, itemEntityPredictedState);

                var itemPredictedState = EntityManager.GetComponentData<ItemPredictedState>(pickupedEntity);
                itemPredictedState.Owner = Entity.Null;
                EntityManager.SetComponentData(pickupedEntity, itemPredictedState);

                EntityManager.AddComponentData(pickupedEntity, new PhysicsVelocity());

                var replicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(pickupedEntity);
                replicatedEntityData.PredictingPlayerId = -1;
                EntityManager.SetComponentData(pickupedEntity, replicatedEntityData);

                if (!EntityManager.HasComponent<ServerEntity>(pickupedEntity))
                    EntityManager.AddComponentData(pickupedEntity, new ServerEntity());

                //  EntityManager.AddComponentData(pickupedEntity, itemPredictedState.Mass);
            
                predictData.PickupedEntity = Entity.Null;
            });
        }
    }
}