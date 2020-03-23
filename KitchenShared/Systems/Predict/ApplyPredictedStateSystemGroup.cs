using FootStone.ECS;
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
            Entities.WithStructuralChanges().ForEach((Entity entity,
                ref VelocityPredictedState velocityPredictedState) =>
            {
                switch (velocityPredictedState.MotionType)
                {
                    case MotionType.Dynamic:
                        EntityManager.AddComponentData(entity, new PhysicsVelocity
                        {
                            Linear = velocityPredictedState.Linear,
                            Angular = velocityPredictedState.Angular
                        });
                        break;
                    case MotionType.Static:
                        EntityManager.RemoveComponent<PhysicsVelocity>(entity);
                        break;
                    case MotionType.Kinematic:
                        //EntityManager.SetComponentData(entity, new PhysicsVelocity()
                        //{
                        //    Linear = velocityPredictedState.Linear,
                        //    Angular = velocityPredictedState.Angular
                        //});
                        break;
                }
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
        }
    }

}
