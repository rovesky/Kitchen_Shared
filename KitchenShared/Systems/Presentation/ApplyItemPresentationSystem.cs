using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ApplyItemPresentationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithStructuralChanges().ForEach((Entity entity,
                ref Translation translation,
                ref Rotation rotation,
                in TransformPredictedState transformState,
                in VelocityPredictedState velocityState,
                in OwnerPredictedState ownerState) =>
            {
                translation.Value = transformState.Position;
                rotation.Value = transformState.Rotation;

                switch (velocityState.MotionType)
                {
                    case MotionType.Dynamic:
                        EntityManager.AddComponentData(entity, new PhysicsVelocity
                        {
                            Linear = velocityState.Linear,
                            Angular = velocityState.Angular
                        });
                        break;
                    case MotionType.Static:
                        EntityManager.RemoveComponent<PhysicsVelocity>(entity);
                        break;
                }

                if (ownerState.Owner != Entity.Null)
                {
                    if (!EntityManager.HasComponent<Parent>(entity))
                    {
                        EntityManager.AddComponentData(entity, new Parent());
                        EntityManager.AddComponentData(entity, new LocalToParent());
                    }

                    var parent = EntityManager.GetComponentData<Parent>(entity);
                  //  FSLog.Info($" parent.Value:{ parent.Value},entity:{entity},translation.Value:{translation.Value}");
                    if (parent.Value == ownerState.Owner)
                       return;
                    parent.Value = ownerState.Owner;
                    EntityManager.SetComponentData(entity, parent);
                }
                else
                {
                    if (!EntityManager.HasComponent<Parent>(entity))
                        return;
                    EntityManager.RemoveComponent<Parent>(entity);
                    EntityManager.RemoveComponent<LocalToParent>(entity);
                }
             
            }).Run();
        }
    }
}