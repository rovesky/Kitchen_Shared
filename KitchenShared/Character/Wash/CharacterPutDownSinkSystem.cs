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
                    if (!command.Buttons.IsSet(UserCommand.Button.Button1))
                        return;

                    //没有触发返回
                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    //触发的不是Sink返回
                    if (!EntityManager.HasComponent<TableSink>(triggerEntity))
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
                        var nextPlate = PutDownSink(plateDirty, triggerEntity, preOwner);
                        if(nextPlate == Entity.Null)
                            break;
                        preOwner = plateDirty;
                        plateDirty = nextPlate;
                        ++i;
                    }

                }).Run();

          
        }

        private Entity PutDownSink(Entity plateDirty,Entity sink,Entity preOwner)
        {
            var slot = EntityManager.GetComponentData<SlotPredictedState>(plateDirty);
            var ret = slot.FilledIn;
            slot.FilledIn = Entity.Null;
            EntityManager.SetComponentData(plateDirty,slot);

            //var scale = EntityManager.GetComponentData<ScaleSetting>(plateDirty);
            //scale.Scale = new float3(0.6f,0.6f,0.6f);
            //EntityManager.SetComponentData(plateDirty,scale);


            var sinkState = EntityManager.GetComponentData<SinkPredictedState>(sink);
            sinkState.Value.FillIn(plateDirty);
            EntityManager.SetComponentData(sink,sinkState);

            var index = sinkState.Value.Count() - 1;

            var sinkSetting = EntityManager.GetComponentData<TableSink>(sink);
            ItemAttachUtilities.ItemAttachToOwner1(EntityManager, 
                plateDirty, sink, preOwner,
                sinkSetting.SlotWashing+new float3(0f,-0.1f,0f) + new float3(0f,0.1f,0f)*index,
                quaternion.Euler(math.radians(new float3(0,0,-25- 15*index))));

            return ret;

        }
    }
}