using System.Runtime.CompilerServices;
using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 放入水槽
    /// </summary>
    [DisableAutoCreation]
    public class CharacterPutDownSinkSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterPutDownSink")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref SlotPredictedState slotState,
                    in TriggerPredictedState triggerState,
                    in UserCommand command) =>
                {
                    //未按键返回
                    if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                        return;

                    //没有触发返回
                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    //触发的不是Sink返回
                    if (!EntityManager.HasComponent<SinkSetting>(triggerEntity))
                        return;


                    var pickupEntity = slotState.FilledIn;
                    if (pickupEntity == Entity.Null)
                        return;
                    if (!EntityManager.HasComponent<PlateDirty>(pickupEntity))
                        return;

                    slotState.FilledIn = Entity.Null;

                    var preOwner = entity;
                    var plateDirty = pickupEntity;
                    var i = 0;
                    while (true)
                    {
                        var nextPlate = PutDownSink(plateDirty, triggerEntity, preOwner,i);
                        if(nextPlate == Entity.Null)
                            break;
                        preOwner = plateDirty;
                        plateDirty = nextPlate;
                        ++i;
                    }

                }).Run();

          
        }

        private Entity PutDownSink(Entity plateDirty,Entity sink,Entity preOwner,int index)
        {
            var slot = EntityManager.GetComponentData<SlotPredictedState>(plateDirty);
            var ret = slot.FilledIn;
            slot.FilledIn = Entity.Null;
            EntityManager.SetComponentData(plateDirty,slot);


            var sinkState = EntityManager.GetComponentData<SinkPredictedState>(sink);
            sinkState.Value.FillIn(plateDirty);
            EntityManager.SetComponentData(sink,sinkState);

                  
            var sinkSetting = EntityManager.GetComponentData<SinkSetting>(sink);
            ItemAttachUtilities.ItemAttachToOwner(EntityManager, 
                plateDirty, sink, preOwner,
                sinkSetting.SlotWashing + new float3(0.1f,0.1f,0f)*index,
                quaternion.identity);

            return ret;

        }
    }
}