using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{

    public static class ItemAttachUtilities
    {
        public static void ItemAttachToOwner(EntityManager entityManager, Entity item,
            Entity owner, Entity preOwner, float3 pos,quaternion rot)
        {
            var triggerState = entityManager.GetComponentData<TriggerPredictedState>(item);
            triggerState.TriggeredEntity = Entity.Null;
            triggerState.IsAllowTrigger = false;
            entityManager.SetComponentData(item, triggerState);

            var ownerPredictedState = entityManager.GetComponentData<OwnerPredictedState>(item);
            ownerPredictedState.Owner = owner;
            ownerPredictedState.PreOwner = preOwner;
            entityManager.SetComponentData(item, ownerPredictedState);
            
         
            var transformPredictedState = entityManager.GetComponentData<TransformPredictedState>(item);
            transformPredictedState.Position = pos;
            transformPredictedState.Rotation = rot;
            FSLog.Info($"ItemAttachToCharacter,Position:{transformPredictedState.Position}");
            entityManager.SetComponentData(item, transformPredictedState);


            var velocityPredictedState = entityManager.GetComponentData<VelocityPredictedState>(item);
            velocityPredictedState.Angular = float3.zero;
            velocityPredictedState.Linear = float3.zero;
            velocityPredictedState.MotionType = MotionType.Static;
            entityManager.SetComponentData(item, velocityPredictedState);


            if (entityManager.HasComponent<ReplicatedEntityData>(owner))
            {

                var replicatedOwner = entityManager.GetComponentData<ReplicatedEntityData>(owner);
                var replicatedEntityData = entityManager.GetComponentData<ReplicatedEntityData>(item);
                replicatedEntityData.PredictingPlayerId = replicatedOwner.PredictingPlayerId;
                entityManager.SetComponentData(item, replicatedEntityData);
            }
        }
        
   

        public static void ItemAttachToOwner(EntityManager entityManager, Entity item,
            Entity owner, Entity preOwner)
        {

            if (preOwner != Entity.Null)
            {
                if (entityManager.HasComponent<SlotPredictedState>(preOwner))
                {
                    var preOwnerSlotState = entityManager.GetComponentData<SlotPredictedState>(preOwner);
                    preOwnerSlotState.FilledIn = Entity.Null;
                    entityManager.SetComponentData(preOwner, preOwnerSlotState);
                }
                else if (entityManager.HasComponent<MultiSlotPredictedState>(preOwner))
                {
                    var preOwnerSlotState = entityManager.GetComponentData<MultiSlotPredictedState>(preOwner);
                    preOwnerSlotState.Value.TakeOut();
                    entityManager.SetComponentData(preOwner, preOwnerSlotState);

                }
                else if (entityManager.HasComponent<SinkPredictedState>(preOwner))
                {
                    var preOwnerSlotState = entityManager.GetComponentData<SinkPredictedState>(preOwner);
                    preOwnerSlotState.Value.TakeOut();
                    entityManager.SetComponentData(preOwner, preOwnerSlotState);

                }
                else
                {
                    return;
                }
            }

            if (entityManager.HasComponent<SlotPredictedState>(owner))
            {
                var slotState = entityManager.GetComponentData<SlotPredictedState>(owner);
                slotState.FilledIn = item;
                entityManager.SetComponentData(owner, slotState);
            }
            else if (entityManager.HasComponent<MultiSlotPredictedState>(owner))
            {
                var slotState = entityManager.GetComponentData<MultiSlotPredictedState>(owner);
                slotState.Value.FillIn(item);
                entityManager.SetComponentData(owner, slotState);

            }
            else if (entityManager.HasComponent<SinkPredictedState>(owner))
            {
                var slotState = entityManager.GetComponentData<SinkPredictedState>(owner);
                slotState.Value.FillIn(item);
                entityManager.SetComponentData(owner, slotState);
            }
            else
            {
                return;
            }

            var ownerSlot = entityManager.GetComponentData<SlotSetting>(owner);
            var offset = entityManager.GetComponentData<OffsetSetting>(item);

            float3 pos;
            if (entityManager.HasComponent<MultiSlotPredictedState>(owner))
            {
                var slotState = entityManager.GetComponentData<MultiSlotPredictedState>(owner);

                pos = ownerSlot.Pos + ownerSlot.Offset * slotState.Value.Count() + offset.Pos;
            }
            else
            {
                pos = ownerSlot.Pos  + offset.Pos;

            }

            var rot = math.mul(offset.Rot, ownerSlot.Rot);

            ItemAttachToOwner(entityManager, item, owner, preOwner,pos,rot);
        }




        public static void ItemDetachFromOwner(EntityManager entityManager, Entity itemEntity,
            Entity preOwner, float3 pos,quaternion rot, float3 linearVelocity)
        {
            
            if (entityManager.HasComponent<SlotPredictedState>(preOwner))
            {
                var preOwnerSlotState = entityManager.GetComponentData<SlotPredictedState>(preOwner);
                preOwnerSlotState.FilledIn = Entity.Null;
                entityManager.SetComponentData(preOwner, preOwnerSlotState);
            }
            else if (entityManager.HasComponent<MultiSlotPredictedState>(preOwner))
            {
                var preOwnerSlotState = entityManager.GetComponentData<MultiSlotPredictedState>(preOwner);
                preOwnerSlotState.Value.TakeOut();
                entityManager.SetComponentData(preOwner, preOwnerSlotState);
            }
            else if (entityManager.HasComponent<SinkPredictedState>(preOwner))
            {
                var preOwnerSlotState = entityManager.GetComponentData<SinkPredictedState>(preOwner);
                preOwnerSlotState.Value.TakeOut();
                entityManager.SetComponentData(preOwner, preOwnerSlotState);
            }
            else
            {
                return;
            }

      
            var triggerState = entityManager.GetComponentData<TriggerPredictedState>(itemEntity);
            triggerState.IsAllowTrigger = true;
            entityManager.SetComponentData(itemEntity, triggerState);

            var itemPredictedState = entityManager.GetComponentData<OwnerPredictedState>(itemEntity);
            itemPredictedState.PreOwner = preOwner;
            itemPredictedState.Owner = Entity.Null;
            entityManager.SetComponentData(itemEntity, itemPredictedState);

            var offset = entityManager.GetComponentData<OffsetSetting>(itemEntity);
            var transformPredictedState = entityManager.GetComponentData<TransformPredictedState>(itemEntity);
            transformPredictedState.Position = pos;//+ offset.Pos;
            transformPredictedState.Rotation = math.mul(offset.Rot,rot);
            entityManager.SetComponentData(itemEntity, transformPredictedState);

            var velocityPredictedState = entityManager.GetComponentData<VelocityPredictedState>(itemEntity);
            velocityPredictedState.Linear = linearVelocity;
            velocityPredictedState.MotionType = MotionType.Dynamic;
            entityManager.SetComponentData(itemEntity, velocityPredictedState);

            var replicatedEntityData = entityManager.GetComponentData<ReplicatedEntityData>(itemEntity);
            replicatedEntityData.PredictingPlayerId = -1;
            entityManager.SetComponentData(itemEntity, replicatedEntityData);
        }

    }
}