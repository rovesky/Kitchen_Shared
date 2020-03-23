using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemAttachToCharacterSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<Item>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                ref TriggerPredictedState triggerState,
                ref TransformPredictedState transformPredictedState,
                ref VelocityPredictedState velocityPredictedState,
                ref ItemPredictedState itemPredictedState,
                ref ReplicatedEntityData replicatedEntityData,
                in ItemAttachToCharacterRequest pickupRequest) =>
            {
                FSLog.Info("ItemAttachToCharacterSystem OnUpdate!");
                EntityManager.RemoveComponent<ItemAttachToCharacterRequest>(entity);

                triggerState.TriggeredEntity = Entity.Null;
                triggerState.IsAllowTrigger = false;

                itemPredictedState.Owner = pickupRequest.Owner;
                itemPredictedState.TempOwner = Entity.Null;
              
                transformPredictedState.Position = new float3(0, -0.2f, 0.9f);
                transformPredictedState.Rotation = quaternion.identity;

                velocityPredictedState.Angular = float3.zero;
                velocityPredictedState.Linear = float3.zero;
                velocityPredictedState.MotionType = MotionType.Static;

                replicatedEntityData.PredictingPlayerId = pickupRequest.PredictingPlayerId;
             
            }).Run();
        }
    }
}