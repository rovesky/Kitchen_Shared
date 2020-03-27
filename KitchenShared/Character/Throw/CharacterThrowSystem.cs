using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterThrowSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref PickupPredictedState pickupState,
                    in TransformPredictedState entityPredictData,
                    in ThrowSetting setting,
                    in UserCommand command) =>
            {
                //  FSLog.Info("PickSystem Update");
                if (!command.Buttons.IsSet(UserCommand.Button.Throw))
                    return;

                if (pickupState.PickupedEntity == Entity.Null)
                    return;

                //TODO 

                Vector3 linear = math.mul(entityPredictData.Rotation, Vector3.forward);
                linear.y = 0.2f;
                linear.Normalize();
                linear *= setting.Velocity;
                EntityManager.AddComponentData(pickupState.PickupedEntity, new ItemDetachFromCharacterRequest
                {
                    LinearVelocity = linear,
                    Pos = entityPredictData.Position +
                          math.mul(entityPredictData.Rotation, new float3(0, 0.2f, 1.3f)),
                    PreOwner = entity
                });
      
                pickupState.PickupedEntity = Entity.Null;
            }).Run();
        }
    }


   
}