using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{

    public static class ItemAttachUtilities
    {
        public static void ItemAttachToCharacter(EntityManager entityManager, Entity itemEntity, Entity owner,
            int predictingPlayerId)
        {

            var triggerState = entityManager.GetComponentData<TriggerPredictedState>(itemEntity);
            triggerState.TriggeredEntity = Entity.Null;
            triggerState.IsAllowTrigger = false;
            entityManager.SetComponentData(itemEntity, triggerState);

            var itemPredictedState = entityManager.GetComponentData<ItemPredictedState>(itemEntity);
            itemPredictedState.Owner = owner;
            itemPredictedState.PreOwner = Entity.Null;
            entityManager.SetComponentData(itemEntity, itemPredictedState);

            var transformPredictedState = entityManager.GetComponentData<TransformPredictedState>(itemEntity);
            transformPredictedState.Position = new float3(0, -0.2f, 0.9f);
            transformPredictedState.Rotation = quaternion.identity;
            entityManager.SetComponentData(itemEntity, transformPredictedState);


            var velocityPredictedState = entityManager.GetComponentData<VelocityPredictedState>(itemEntity);
            velocityPredictedState.Angular = float3.zero;
            velocityPredictedState.Linear = float3.zero;
            velocityPredictedState.MotionType = MotionType.Static;
            entityManager.SetComponentData(itemEntity, velocityPredictedState);

            var replicatedEntityData = entityManager.GetComponentData<ReplicatedEntityData>(itemEntity);
            replicatedEntityData.PredictingPlayerId = predictingPlayerId;
            entityManager.SetComponentData(itemEntity, replicatedEntityData);
        }



        public static void ItemDetachFromCharacter(EntityManager entityManager, Entity itemEntity,
            Entity preOwner,float3 pos,float3 linearVelocity)
        {
            var triggerState = entityManager.GetComponentData<TriggerPredictedState>(itemEntity);
            triggerState.IsAllowTrigger = true;
            entityManager.SetComponentData(itemEntity, triggerState);

            var itemPredictedState = entityManager.GetComponentData<ItemPredictedState>(itemEntity);
            itemPredictedState.PreOwner = preOwner;
            itemPredictedState.Owner = Entity.Null;
            entityManager.SetComponentData(itemEntity, itemPredictedState);

            var transformPredictedState = entityManager.GetComponentData<TransformPredictedState>(itemEntity);
            transformPredictedState.Position = pos;
            transformPredictedState.Rotation = quaternion.identity;
            entityManager.SetComponentData(itemEntity, transformPredictedState);

            var velocityPredictedState = entityManager.GetComponentData<VelocityPredictedState>(itemEntity);
            velocityPredictedState.Linear = linearVelocity;
            velocityPredictedState.MotionType = MotionType.Dynamic;
            entityManager.SetComponentData(itemEntity, velocityPredictedState);

            var replicatedEntityData = entityManager.GetComponentData<ReplicatedEntityData>(itemEntity);
            replicatedEntityData.PredictingPlayerId = -1;
            entityManager.SetComponentData(itemEntity, replicatedEntityData);
        }


        
        public static void ItemAttachToTable(EntityManager entityManager, Entity itemEntity,Entity tableEntity, float3 slotPos)
        {
            var triggerState = entityManager.GetComponentData<TriggerPredictedState>(itemEntity);
            triggerState.TriggeredEntity = Entity.Null;
            triggerState.IsAllowTrigger = false;
            entityManager.SetComponentData(itemEntity, triggerState);

            var transformPredictedState = entityManager.GetComponentData<TransformPredictedState>(itemEntity);
            transformPredictedState.Position = slotPos;
            transformPredictedState.Rotation = quaternion.identity;
            entityManager.SetComponentData(itemEntity, transformPredictedState);

            var velocityPredictedState = entityManager.GetComponentData<VelocityPredictedState>(itemEntity);
            velocityPredictedState.Linear = float3.zero;
            velocityPredictedState.Angular = float3.zero;
            velocityPredictedState.MotionType = MotionType.Static;
            entityManager.SetComponentData(itemEntity, velocityPredictedState);

            var itemState =  entityManager.GetComponentData<ItemPredictedState>(itemEntity);
            itemState.Owner = tableEntity;
            entityManager.SetComponentData(itemEntity, itemState);
            // entityManager.AddComponentData(itemEntity,new TriggeredSetting());
        }

        public static void ItemDetachFromTable(EntityManager entityManager, Entity itemEntity,Entity tableEntity)
        {
            var itemState =  entityManager.GetComponentData<ItemPredictedState>(itemEntity);
            itemState.Owner = Entity.Null;
            itemState.PreOwner = tableEntity;
            entityManager.SetComponentData(itemEntity, itemState);
            // entityManager.AddComponentData(itemEntity,new TriggeredSetting());
        }

    }
}