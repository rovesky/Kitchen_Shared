﻿using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupGroundSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<ServerEntity>().ForEach((Entity entity,
                ref CharacterPickupItem pickupItem,
                ref UserCommand command,
                ref CharacterPredictedState predictData) =>
            {
                if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                    return;

                var worldTick = GetSingleton<WorldTime>().Tick;
                FSLog.Info($"CharacterPickupGroundSystem:{predictData.PickupedEntity},{predictData.TriggeredEntity}");
                if (predictData.PickupedEntity == Entity.Null && predictData.TriggeredEntity != Entity.Null)
                {
                    var triggerData = EntityManager.GetComponentData<TriggerData>(predictData.TriggeredEntity);
               //     FSLog.Info($"CharacterPickupGroundSystem3:{triggerData.Type}");
                    if ((triggerData.Type & (int)TriggerType.Item) == 0)
                        return;
                    
                    FSLog.Info($"PickUpItem,command tick:{command.RenderTick},worldTick:{worldTick}");
                    PickUpItem(entity, ref predictData);
                }
                else if(predictData.PickupedEntity != Entity.Null && predictData.TriggeredEntity == Entity.Null)
                {
                    FSLog.Info($"PutDownItem,tick:{command.RenderTick},worldTick:{worldTick}");
                    PutDownItem(ref predictData);
                }
            });
        }

        private void PutDownItem(ref CharacterPredictedState characterState)
        {
            var entity = characterState.PickupedEntity;
       
            var itemPredictedState = EntityManager.GetComponentData<ItemPredictedState>(entity);
           
            itemPredictedState.Position = characterState.Position + 
                                          math.mul(characterState.Rotation, new float3(0, -0.2f, 1.1f));
            itemPredictedState.Rotation = quaternion.identity;
            itemPredictedState.Owner = Entity.Null;
            itemPredictedState.LinearVelocity = float3.zero;
            EntityManager.SetComponentData(entity, itemPredictedState);

            var replicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(entity);
            replicatedEntityData.PredictingPlayerId = -1;
            EntityManager.SetComponentData(entity, replicatedEntityData);

            if (EntityManager.HasComponent<ServerEntity>(entity))
            {
                FSLog.Info($"{entity} is ServerEntity!");
            }

           //EntityManager.AddComponentData(entity, itemPredictedState.Mass);

            characterState.PickupedEntity = Entity.Null;
        }

        private void PickUpItem(Entity owner, ref CharacterPredictedState characterState)
        {
     
            var entity = characterState.TriggeredEntity;
            var itemPredictedState = EntityManager.GetComponentData<ItemPredictedState>(entity);

            //速度比较快不能pickup
            if (math.distancesq(itemPredictedState.LinearVelocity, float3.zero) > 2.0f)
                return;

            itemPredictedState.Position = new float3(0, -0.2f, 1.0f);
            itemPredictedState.Rotation = quaternion.identity;
            itemPredictedState.LinearVelocity = float3.zero;
            itemPredictedState.Owner = owner;
            EntityManager.SetComponentData(entity, itemPredictedState);

            //变成 Kinematic
            if (EntityManager.HasComponent<PhysicsMass>(entity))
            {
                EntityManager.RemoveComponent<PhysicsMass>(entity);
            }

            var ownerReplicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(owner);
            var replicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(entity);
            replicatedEntityData.PredictingPlayerId = ownerReplicatedEntityData.PredictingPlayerId;
            EntityManager.SetComponentData(entity, replicatedEntityData);

            characterState.PickupedEntity = entity;
          
        }
    }
}