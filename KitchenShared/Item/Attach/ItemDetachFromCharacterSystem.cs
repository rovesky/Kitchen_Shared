using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemDetachFromCharacterSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<Item>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref TransformPredictedState transformPredictedState,
                    ref VelocityPredictedState velocityPredictedState,
                    ref ItemPredictedState itemPredictedState,
                    ref TriggerPredictedState triggerState,
                    ref ReplicatedEntityData replicatedEntityData,
                    in ItemDetachFromCharacterRequest request) =>
                {
                    FSLog.Info("ItemDetachFromCharacterSystem OnUpdate!");
                    EntityManager.RemoveComponent<ItemDetachFromCharacterRequest>(entity);

                    triggerState.IsAllowTrigger = true;
                    itemPredictedState.PreOwner = request.PreOwner;
                  
                  //  FSLog.Info($"ItemDetachFromCharacterSystem itemPredictedState.TempOwner :{itemPredictedState.TempOwner }," +
                        //       $" itemPredictedState.TempOwnerCD：{ itemPredictedState.TempOwnerCD}");

                    itemPredictedState.Owner = Entity.Null;


                    transformPredictedState.Position = request.Pos;
                    transformPredictedState.Rotation = quaternion.identity;

                    velocityPredictedState.Linear = request.LinearVelocity;
                    velocityPredictedState.MotionType = MotionType.Dynamic;

                    replicatedEntityData.PredictingPlayerId = -1;
                }).Run();
        }
    }
}