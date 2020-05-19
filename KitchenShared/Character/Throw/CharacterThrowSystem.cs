using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterThrowStartSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref ThrowPredictState throwState,
                    in SlotPredictedState slotState,
                    in TransformPredictedState transformState,
                    in ThrowSetting setting,
                    in UserCommand command) =>
                {
                    //  FSLog.Info("PickSystem Update");
                    if (!command.Buttons.IsSet(UserCommand.Button.Button2))
                        return;

                    var pickupedEntity = slotState.FilledIn;
                    if (pickupedEntity == Entity.Null)
                        return;

                    //非食物返回
                    if (!EntityManager.HasComponent<Food>(pickupedEntity))
                        return;

                    throwState.IsThrowed = true;
                    throwState.CurTick = setting.DelayTick;

                }).Run();
        }
    }


    [DisableAutoCreation]
    public class CharacterThrowEndSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref ThrowPredictState throwState,
                    in TransformPredictedState transformState,
                    in ThrowSetting setting,
                    in  SlotPredictedState slotState) =>
                {
                    if(!throwState.IsThrowed)
                        return;

                    if (throwState.CurTick > 0)
                    {
                        throwState.CurTick--;
                        return;
                    }

                    throwState.IsThrowed = false ;
                    var pickupedEntity = slotState.FilledIn;
                    if (pickupedEntity == Entity.Null)
                        return;

                    Vector3 linear = math.mul(transformState.Rotation, Vector3.forward);
                    linear.y = 0.25f;
                    linear.Normalize();
                    linear *= setting.Velocity;

                    var ownerSlot = EntityManager.GetComponentData<SlotSetting>(entity);
                    var offset = EntityManager.GetComponentData<OffsetSetting>(pickupedEntity);

                    ItemAttachUtilities.ItemDetachFromOwner(EntityManager,
                        pickupedEntity,
                        entity,
                        transformState.Position + math.mul(transformState.Rotation, ownerSlot.Pos + offset.Pos + new float3(0,0.2f,0)),
                        transformState.Rotation,
                        linear);


                }).Run();
        }
    }
}


   
