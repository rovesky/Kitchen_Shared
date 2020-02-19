using FootStone.ECS;
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
            Entities.WithAllReadOnly<ServerEntity>().ForEach((Entity entity,
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

                Vector3 linear = math.mul(entityPredictData.Rotation, Vector3.forward);
                linear.y = 0.3f;
                linear.Normalize();
                linear *= setting.Velocity;
                EntityManager.AddComponentData(pickupState.PickupedEntity, new DetachFromCharacterRequest
                {
                    LinearVelocity = linear,
                    Pos = entityPredictData.Position +
                          math.mul(entityPredictData.Rotation, new float3(0, 0.2f, 1.3f))
                });

                pickupState.PickupedEntity = Entity.Null;
            });
        }
    }
}