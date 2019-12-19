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
                    EntityManager.SetComponentData(entity, parent);
                }
                else
                {
                    if (!EntityManager.HasComponent<Parent>(entity))
                        return;
                    EntityManager.RemoveComponent<Parent>(entity);
                    EntityManager.RemoveComponent<LocalToParent>(entity);
                }
            });
        }
    }
}