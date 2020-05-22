﻿using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 更新锅的状态
    /// </summary>
    [DisableAutoCreation]
    public class UpdatePotStateSystem : SystemBase
    {

        //private EndPredictUpdateEntityCommandBufferSystem endPredictUpdateEcbSystem;
        //protected override void OnCreate()
        //{
        //    base.OnCreate();
        //    // Find the ECB system once and store it for later usage
        //    endPredictUpdateEcbSystem = World
        //        .GetOrCreateSystem<EndPredictUpdateEntityCommandBufferSystem>();
        //}

        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .ForEach((Entity entity,
                    ref FireAlertPredictedState fireAlertState,
                    in OwnerPredictedState ownerState) =>
                {
                    if (ownerState.Owner != Entity.Null &&
                        HasComponent<Character>(ownerState.Owner))
                    {
                        fireAlertState.CurTick = 0;
                    }

                }).Run();

          //  var ecb1 = endPredictUpdateEcbSystem.CreateCommandBuffer().ToConcurrent();

            Entities.WithAll<ServerEntity, Burnt>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in PotPredictedState potState) =>
                {
                    if (potState.State != PotState.Burnt)
                        EntityManager.RemoveComponent<Burnt>(entity);
                }).Run();

         //   var ecb2 = endPredictUpdateEcbSystem.CreateCommandBuffer().ToConcurrent();
             Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .WithNone<Burnt>()
                .ForEach((Entity entity,
                    in PotPredictedState potState) =>
                {
                    if (potState.State == PotState.Burnt)
                        EntityManager.AddComponentData(entity,new Burnt());
                }).Run();

          //  Dependency = JobHandle.CombineDependencies(job1, job2, job3);
         //   endPredictUpdateEcbSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}