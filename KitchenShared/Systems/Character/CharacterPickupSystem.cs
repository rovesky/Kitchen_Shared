using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupSystem : ComponentSystem
    {
        EntityQuery overlappingGroup;

        protected override void OnCreate()
        {
            overlappingGroup = GetEntityQuery(typeof(OverlappingTrigger));
        }

        protected override void OnUpdate()
        {
            //FSLog.Info("TriggerOperationSystem Update");
            Entities.WithAllReadOnly<Character>().ForEach((Entity entity,
                ref CharacterPickupItem pickupItem,
                ref UserCommand command,
                ref CharacterPredictedState predictData) =>
            {
                if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                    return;

                var isEmpty = predictData.PickupedEntity == Entity.Null;
                var entities = overlappingGroup.ToEntityArray(Allocator.TempJob);
                // FSLog.Info($"overlappingGroup: {entities.Length}");
                foreach (var overlappingEntity in entities)
                {
                    // FSLog.Info(overlappingEntity);
                    var overlappingData = EntityManager.GetComponentData<OverlappingTrigger>(overlappingEntity);
                    if (overlappingData.TriggerEntityIndex != entity.Index)
                        continue;

                    //  FSLog.Info($"overlappingData.TriggerEntityIndex:{overlappingData.TriggerEntityIndex}");
                    var worldTick = GetSingleton<WorldTime>().Tick;
                    var triggerData = EntityManager.GetComponentData<TriggerData>(overlappingEntity);
                    // FSLog.Info($"triggerData.VolumeType:{triggerData.VolumeType}");
                    if ((triggerData.VolumeType & (int) TriggerVolumeType.Table) != 0)
                    {
                        var slot = EntityManager.GetComponentData<SlotPredictedState>(overlappingEntity);
                        // FSLog.Info($"TriggerOperationSystem Update,isEmpty:{isEmpty},slot.FiltInEntity:{slot.FilledInEntity},slot.pos:{slot.SlotPos}");

                        if (isEmpty && slot.FilledInEntity != Entity.Null)
                        {
                            FSLog.Info($"PickUpItem,command tick:{command.RenderTick},worldTick:{worldTick}");
                            PickUpItem(entity, overlappingEntity, ref predictData /*, ref slot*/);
                        }
                        else if (!isEmpty && slot.FilledInEntity == Entity.Null)
                        {
                            FSLog.Info($"PutDownItem,tick:{command.RenderTick},worldTick:{worldTick}");
                            PutDownItem(entity, overlappingEntity, ref predictData /*, ref slot*/);
                        }
                    }

                    break;
                }

                entities.Dispose();

            });
        }

        private void PutDownItem(Entity owner, Entity overlapping,
            ref CharacterPredictedState characterState /*, ref SlotPredictedState slot*/)
        {
            var entity = characterState.PickupedEntity;

            var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(entity);
            physicsVelocity.Linear = float3.zero;
            EntityManager.SetComponentData(entity, physicsVelocity);

            //   var pos = EntityManager.GetComponentData<LocalToWorld>();
            var triggerData = EntityManager.GetComponentData<TriggerData>(overlapping);
            var itemPredictedState = EntityManager.GetComponentData<ItemPredictedState>(entity);
            itemPredictedState.Position = triggerData.SlotPos;
          //  FSLog.Info($"PutDownItem,pos:{slot.SlotPos}");
            itemPredictedState.Rotation = quaternion.identity;
            itemPredictedState.Owner = Entity.Null;
            EntityManager.SetComponentData(entity, itemPredictedState);

            var replicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(entity);
            replicatedEntityData.PredictingPlayerId = -1;
            EntityManager.SetComponentData(entity, replicatedEntityData);

            characterState.PickupedEntity = Entity.Null;

            var slot = EntityManager.GetComponentData<SlotPredictedState>(overlapping);
            slot.FilledInEntity = entity;
            EntityManager.SetComponentData(overlapping, slot);
        }

        private void PickUpItem(Entity owner, Entity overlapping, ref CharacterPredictedState characterState
            /* ,ref SlotPredictedState slot*/)
        {
            var slot = EntityManager.GetComponentData<SlotPredictedState>(overlapping);
            var entity = slot.FilledInEntity;


            var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(entity);
            physicsVelocity.Linear = float3.zero;
            EntityManager.SetComponentData(entity, physicsVelocity);

            var itemPredictedState = EntityManager.GetComponentData<ItemPredictedState>(entity);
            itemPredictedState.Position = new float3(0, 0.2f, 0.8f);
            itemPredictedState.Rotation = quaternion.identity;
            itemPredictedState.Owner = owner;
            EntityManager.SetComponentData(entity, itemPredictedState);


            var ownerReplicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(owner);
            var replicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(entity);
            replicatedEntityData.PredictingPlayerId = ownerReplicatedEntityData.PredictingPlayerId;
            EntityManager.SetComponentData(entity, replicatedEntityData);

            characterState.PickupedEntity = entity;
            slot.FilledInEntity = Entity.Null;
            EntityManager.SetComponentData(overlapping, slot);
        }
    }
}