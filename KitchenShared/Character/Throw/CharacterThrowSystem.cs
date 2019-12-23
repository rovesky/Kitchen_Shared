using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterThrowSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Character>().ForEach((Entity entity,
                ref ThrowSetting setting,
                ref UserCommand command,
                ref PickupPredictedState pickupState,
                ref TransformPredictedState entityPredictData) =>
            {
                //  FSLog.Info("PickSystem Update");
                if (!command.Buttons.IsSet(UserCommand.Button.Throw))
                    return;

                if (pickupState.PickupedEntity == Entity.Null)
                    return;

                //var pickupedEntity = pickupState.PickupedEntity;
                //var itemEntityPredictedState = EntityManager.GetComponentData<EntityPredictedState>(pickupedEntity);


                //itemEntityPredictedState.Transform.pos = entityPredictData.Transform.pos +
                //    math.mul(entityPredictData.Transform.rot, new float3(0, 0.2f, 0.8f));
                //EntityManager.SetComponentData(pickupedEntity, itemEntityPredictedState);

                //var itemPredictedState = EntityManager.GetComponentData<ItemPredictedState>(pickupedEntity);
                //itemPredictedState.Owner = Entity.Null;
                //EntityManager.SetComponentData(pickupedEntity, itemPredictedState);

                //EntityManager.AddComponentData(pickupedEntity, new PhysicsVelocity());

                //var replicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(pickupedEntity);
                //replicatedEntityData.PredictingPlayerId = -1;
                //EntityManager.SetComponentData(pickupedEntity, replicatedEntityData);

                //if (!EntityManager.HasComponent<ServerEntity>(pickupedEntity))
                //    EntityManager.AddComponentData(pickupedEntity, new ServerEntity());

                //  EntityManager.AddComponentData(pickupedEntity, itemPredictedState.Mass);


                Vector3 linear = math.mul(entityPredictData.Rotation, Vector3.forward);
                linear.y = 0.3f;
                linear.Normalize();
                linear *= setting.Velocity;
                EntityManager.AddComponentData(pickupState.PickupedEntity, new DetachFromCharacterRequest
                {
                    LinearVelocity = linear,
                    Pos = entityPredictData.Position +
                          math.mul(entityPredictData.Rotation, new float3(0, 0.2f, 1.2f))
                });

                pickupState.PickupedEntity = Entity.Null;
            });
        }
    }
}