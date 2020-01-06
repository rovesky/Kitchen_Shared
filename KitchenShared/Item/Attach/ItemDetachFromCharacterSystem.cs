using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemDetachFromCharacterSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Item>().ForEach((Entity entity,
                ref DetachFromCharacterRequest request,
                ref TransformPredictedState transformPredictedState,
                ref VelocityPredictedState velocityPredictedState,
                ref ItemPredictedState itemPredictedState,
                ref ReplicatedEntityData replicatedEntityData) =>
            {
                FSLog.Info($"ItemDetachFromCharacterSystem OnUpdate!");
              
                itemPredictedState.Owner = Entity.Null;
                itemPredictedState.IsDynamic = true;

                transformPredictedState.Position = request.Pos;
                transformPredictedState.Rotation = quaternion.identity;

                velocityPredictedState.Linear = request.LinearVelocity;
                replicatedEntityData.PredictingPlayerId = -1;
           
                EntityManager.RemoveComponent<DetachFromCharacterRequest>(entity);

            });
        }
    }
}