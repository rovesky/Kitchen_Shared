using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterExtinguishStartSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in SlotPredictedState slotState,
                    in UserCommand command) =>
                {
                    //  FSLog.Info("PickSystem Update");
                    if (!command.Buttons.IsSet(UserCommand.Button.Button2))
                        return;

                    var pickupedEntity = slotState.FilledIn;
                    if (pickupedEntity == Entity.Null)
                        return;

                    //非灭火器返回
                    if (!EntityManager.HasComponent<Extinguisher>(pickupedEntity))
                        return;

                    var extinguisherState = EntityManager.GetComponentData<ExtinguisherPredictedState>(pickupedEntity);
                  
                    if(extinguisherState.Distance > 0)
                        return;

                    extinguisherState.Distance = 1;

                    EntityManager.SetComponentData(pickupedEntity,extinguisherState);
                  
                }).Run();
        }
    }

    [DisableAutoCreation]
    public class CharacterExtinguishGrowSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in SlotPredictedState slotState) =>
                {
                    var pickupedEntity = slotState.FilledIn;
                    if (pickupedEntity == Entity.Null)
                        return;

                    //非灭火器返回
                    if (!EntityManager.HasComponent<Extinguisher>(pickupedEntity))
                        return;

                    var extinguisherState = EntityManager.GetComponentData<ExtinguisherPredictedState>(pickupedEntity);
                  
                    if(extinguisherState.Distance == 0 || extinguisherState.Distance  == 3)
                        return;

                    extinguisherState.Distance++;

                    EntityManager.SetComponentData(pickupedEntity,extinguisherState);
                  
                }).Run();
        }
    }

    [DisableAutoCreation]
    public class CharacterExtinguishStopSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in SlotPredictedState slotState,
                    in UserCommand command) =>
                {
                    if (!command.Buttons.IsSet(UserCommand.Button.Button2))
                        return;

                    var pickupedEntity = slotState.FilledIn;
                    if (pickupedEntity == Entity.Null)
                        return;

                    //非灭火器返回
                    if (!EntityManager.HasComponent<Extinguisher>(pickupedEntity))
                        return;

                    var extinguisherState = EntityManager.GetComponentData<ExtinguisherPredictedState>(pickupedEntity);
                    if(extinguisherState.Distance  < 3)
                        return;

                    extinguisherState.Distance = 0;
                    EntityManager.SetComponentData(pickupedEntity,extinguisherState);
                  
                }).Run();
        }

    }

    [DisableAutoCreation]
    public class CharacterExtinguishSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterExtinguishStartSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterExtinguishGrowSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterExtinguishingSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterExtinguishStopSystem>());
     
        }
    }
}