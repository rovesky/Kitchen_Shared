using Unity.Entities;
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
                in ItemInterpolatedState interpolatedData) =>
            {
                translation.Value = interpolatedData.Position;
                rotation.Value = interpolatedData.Rotation;

                if (interpolatedData.Owner != Entity.Null)
                {
                    if (!EntityManager.HasComponent<Parent>(entity))
                    {
                        EntityManager.AddComponentData(entity, new Parent());
                        EntityManager.AddComponentData(entity, new LocalToParent());
                    }

                    var parent = EntityManager.GetComponentData<Parent>(entity);
                  //  FSLog.Info($" parent.Value:{ parent.Value},entity:{entity},translation.Value:{translation.Value}");
                    if (parent.Value == interpolatedData.Owner)
                       return;
                    parent.Value = interpolatedData.Owner;
                    EntityManager.SetComponentData(entity, parent);
                    
                   
                    var scale = EntityManager.GetComponentData<CompositeScale>(entity);
                    var scaleSetting = EntityManager.GetComponentData<ScaleSetting>(entity);
                    var parentScale = EntityManager.GetComponentData<CompositeScale>(interpolatedData.Owner);
                    scale.Value.c0.x = scaleSetting.Scale.x/parentScale.Value.c0.x;
                    scale.Value.c1.y = scaleSetting.Scale.y/parentScale.Value.c1.y;
                    scale.Value.c2.z = scaleSetting.Scale.z/parentScale.Value.c2.z;
                    EntityManager.SetComponentData(entity, scale);
                }
                else
                {
                    if (!EntityManager.HasComponent<Parent>(entity))
                        return;
                    
                   // var parent =  EntityManager.GetComponentData<Parent>(entity);
                   // var parentScale = EntityManager.GetComponentData<CompositeScale>(parent.Value);
                    var scale = EntityManager.GetComponentData<CompositeScale>(entity);
                    var scaleSetting = EntityManager.GetComponentData<ScaleSetting>(entity);
                    scale.Value.c0.x = scaleSetting.Scale.x;
                    scale.Value.c1.y = scaleSetting.Scale.y;
                    scale.Value.c2.z = scaleSetting.Scale.z;
                    EntityManager.SetComponentData(entity, scale);

                    EntityManager.RemoveComponent<Parent>(entity);
                    EntityManager.RemoveComponent<LocalToParent>(entity);
                }
            }).Run();
        }
    }
}