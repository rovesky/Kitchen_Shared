﻿using FootStone.ECS;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    
    [DisableAutoCreation]
    public class ApplyTransformPredictedStateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,
                ref Translation translation,
                ref Rotation rotation,
                in TransformPredictedState transformPredictedState) =>
            {
                translation.Value = transformPredictedState.Position;
                rotation.Value = transformPredictedState.Rotation;
            }).Run();
        }
    }

    [DisableAutoCreation]
    public class ApplyVelocityPredictedStateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithNone<PhysicsVelocity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref VelocityPredictedState velocityPredictedState) =>
                {
                    if (velocityPredictedState.MotionType == MotionType.Dynamic)
                        EntityManager.AddComponentData(entity, new PhysicsVelocity
                        {
                            Linear = velocityPredictedState.Linear,
                            Angular = velocityPredictedState.Angular
                        });

                }).Run();

            Entities
                .WithAll<PhysicsVelocity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref VelocityPredictedState velocityPredictedState) =>
                {
                    if (velocityPredictedState.MotionType == MotionType.Static)
                        EntityManager.RemoveComponent<PhysicsVelocity>(entity);
                }).Run();
        }
    }


    [DisableAutoCreation]
    public class ApplyPredictedStateSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
         //   m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyTransformPredictedStateSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyVelocityPredictedStateSystem>());
         //   m_systemsToUpdate.Add(World.GetOrCreateSystem<ClearSpawnRequestsSystem>());
        }
    }

}
