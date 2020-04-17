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
                    in SlotPredictedState slotState,
                    in TransformPredictedState transformState,
                    in ThrowSetting setting,
                    in UserCommand command) =>
                {
                    //  FSLog.Info("PickSystem Update");
                    if (!command.Buttons.IsSet(UserCommand.Button.Throw))
                        return;

                    var pickupedEntity = slotState.FilledInEntity;
                    if (pickupedEntity == Entity.Null)
                        return;
                
                    if (EntityManager.HasComponent<Plate>(pickupedEntity))
                        return;
          
                    Vector3 linear = math.mul(transformState.Rotation, Vector3.forward);
                    linear.y = 0.25f;
                    linear.Normalize();
                    linear *= setting.Velocity;

                    ItemAttachUtilities.ItemDetachFromOwner(EntityManager,
                        pickupedEntity,
                        entity,
                        transformState.Position + math.mul(transformState.Rotation, new float3(0, 0.2f, 1.3f)),
                        linear);

                 
                }).Run();
        }
    }
}


   
