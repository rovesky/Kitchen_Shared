using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemUpdatePredictedStateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,
                ref ItemPredictedState predictedData
            ) =>
            {
                // FSLog.Info($"UpdateItemParentSystem:{predictedData.Owner}");
                if (predictedData.Owner != Entity.Null)
                {
                    if (!EntityManager.HasComponent<Parent>(entity))
                    {
                        EntityManager.AddComponentData(entity, new Parent());
                        EntityManager.AddComponentData(entity, new LocalToParent());
                    }

                    var parent = EntityManager.GetComponentData<Parent>(entity);
                    //  FSLog.Info($" parent.Value:{ parent.Value},entity:{entity},translation.Value:{translation.Value}");
                    if (parent.Value == predictedData.Owner)
                        return;
                    parent.Value = predictedData.Owner;
                    var scale = EntityManager.GetComponentData<CompositeScale>(entity);
                    var parentScale = EntityManager.GetComponentData<CompositeScale>(predictedData.Owner);
                    scale.Value.c0.x /= parentScale.Value.c0.x;
                    scale.Value.c1.y /= parentScale.Value.c1.y;
                    scale.Value.c2.z /= parentScale.Value.c2.z;
                    EntityManager.SetComponentData(entity, scale);
                    EntityManager.SetComponentData(entity, parent);
                }
                else
                {
                    if (!EntityManager.HasComponent<Parent>(entity))
                        return;

                    var parent =  EntityManager.GetComponentData<Parent>(entity);
                    var parentScale = EntityManager.GetComponentData<CompositeScale>(parent.Value);
                    var scale = EntityManager.GetComponentData<CompositeScale>(entity);
                    scale.Value.c0.x *= parentScale.Value.c0.x;
                    scale.Value.c1.y *= parentScale.Value.c1.y;
                    scale.Value.c2.z *= parentScale.Value.c2.z;
                    EntityManager.SetComponentData(entity, scale);

                    EntityManager.RemoveComponent<Parent>(entity);
                    EntityManager.RemoveComponent<LocalToParent>(entity);

               
                }
            });
        }
    }
}