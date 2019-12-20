using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupGroundSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<ServerEntity>().ForEach((Entity entity,
                ref CharacterPickup setting,
                ref UserCommand command,
                ref PickupPredictedState pickupState,
                ref TriggerPredictedState triggerState,
                ref EntityPredictedState entityPredictData,
                ref CharacterMovePredictedState characterMoveInternalState) =>
            {
                if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                    return;

                var worldTick = GetSingleton<WorldTime>().Tick;
                FSLog.Info($"CharacterPickupGroundSystem:{pickupState.PickupedEntity},{triggerState.TriggeredEntity}");
                if (pickupState.PickupedEntity == Entity.Null && triggerState.TriggeredEntity != Entity.Null)
                {
                    var triggerData = EntityManager.GetComponentData<TriggerData>(triggerState.TriggeredEntity);
               //     FSLog.Info($"CharacterPickupGroundSystem3:{triggerData.Type}");
                    if ((triggerData.Type & (int)TriggerType.Item) == 0)
                        return;
                    
                    FSLog.Info($"PickUpItem,command tick:{command.RenderTick},worldTick:{worldTick}");
                    PickUpItem(entity,ref triggerState, ref pickupState);
                }
                else if(pickupState.PickupedEntity != Entity.Null && triggerState.TriggeredEntity == Entity.Null)
                {
                    FSLog.Info($"PutDownItem,tick:{command.RenderTick},worldTick:{worldTick}");
                    PutDownItem(ref pickupState, ref entityPredictData, characterMoveInternalState);
                }
            });
        }

        private void PutDownItem(ref PickupPredictedState characterState,
            ref EntityPredictedState entityPredictedState, CharacterMovePredictedState characterMovePredictedState)
        {
            var entity = characterState.PickupedEntity;
       
            var itemEntityPredictedState = EntityManager.GetComponentData<EntityPredictedState>(entity);
            itemEntityPredictedState.Transform.pos = entityPredictedState.Transform.pos + 
                                          math.mul(entityPredictedState.Transform.rot, new float3(0, -0.2f, 1.1f));
            itemEntityPredictedState.Transform.rot = quaternion.identity;
            itemEntityPredictedState.Velocity.Linear = float3.zero;
            EntityManager.SetComponentData(entity, itemEntityPredictedState);

            var itemPredictedState = EntityManager.GetComponentData<ItemPredictedState>(entity);
            itemPredictedState.Owner = Entity.Null;
            EntityManager.SetComponentData(entity, itemPredictedState);

            var replicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(entity);
            replicatedEntityData.PredictingPlayerId = -1;
            EntityManager.SetComponentData(entity, replicatedEntityData);

            //if (EntityManager.HasComponent<ServerEntity>(entity))
            //{
            //    FSLog.Info($"{entity} is ServerEntity!");
            //}
            EntityManager.AddComponentData(entity,new PhysicsVelocity());
            //    EntityManager.AddComponentData(entity, itemPredictedState.Mass);

            characterState.PickupedEntity = Entity.Null;
        }

        private void PickUpItem(Entity owner, ref TriggerPredictedState triggerState, ref PickupPredictedState pickupState)
        {
     
            var entity = triggerState.TriggeredEntity;
            var itemEntityPredictedState = EntityManager.GetComponentData<EntityPredictedState>(entity);

            //速度比较快不能pickup
            if (math.distancesq(itemEntityPredictedState.Velocity.Linear, float3.zero) > 2.0f)
                return;

            itemEntityPredictedState.Transform.pos = new float3(0, -0.2f, 1.0f);
            itemEntityPredictedState.Transform.rot = quaternion.identity;
            itemEntityPredictedState.Velocity.Linear = float3.zero;
            EntityManager.SetComponentData(entity, itemEntityPredictedState);

            var itemPredictedState = EntityManager.GetComponentData<ItemPredictedState>(entity);
            itemPredictedState.Owner = owner;
            EntityManager.SetComponentData(entity, itemPredictedState);

            //变成 Static
            if (EntityManager.HasComponent<PhysicsVelocity>(entity))
            {
                EntityManager.RemoveComponent<PhysicsVelocity>(entity);
            }

            var ownerReplicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(owner);
            var replicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(entity);
            replicatedEntityData.PredictingPlayerId = ownerReplicatedEntityData.PredictingPlayerId;
            EntityManager.SetComponentData(entity, replicatedEntityData);

            pickupState.PickupedEntity = entity;
          
        }
    }
}