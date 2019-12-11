using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterThrowSystem : ComponentSystem
    {
      //  private EntityQuery plateQuery;

        //protected override void OnCreate()
        //{
        //    base.OnCreate();
        //  //  plateQuery = GetEntityQuery(typeof(Plate));
        //}

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Character>().ForEach((Entity entity,
                ref CharacterThrowItem pickupItem,
                ref UserCommand command,
                ref CharacterPredictedState predictData) =>
            {
                //  FSLog.Info("PickSystem Update");
                if (!command.Buttons.IsSet(UserCommand.Button.Throw))
                    return;

                if (predictData.PickupedEntity == Entity.Null)
                    return;

                var pickupedEntity = predictData.PickupedEntity;
                var itemPredictedState = EntityManager.GetComponentData<ItemPredictedState>(pickupedEntity);

                Vector3 linear = math.mul(predictData.Rotation, Vector3.forward);
                linear.y = 0.4f;
                linear.Normalize();
                itemPredictedState.Velocity = linear * 18.0f;

                itemPredictedState.Position =
                    predictData.Position + math.mul(predictData.Rotation, new float3(0, 0.2f, 0.8f));

                itemPredictedState.Owner = Entity.Null;
                EntityManager.SetComponentData(pickupedEntity, itemPredictedState);

                var replicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(pickupedEntity);
                replicatedEntityData.PredictingPlayerId = -1;
                EntityManager.SetComponentData(pickupedEntity, replicatedEntityData);

                if (!EntityManager.HasComponent<ServerEntity>(pickupedEntity))
                    EntityManager.AddComponentData(pickupedEntity, new ServerEntity());

                predictData.PickupedEntity = Entity.Null;
            });
        }
    }
}