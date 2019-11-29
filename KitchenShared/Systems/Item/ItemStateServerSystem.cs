using Unity.Entities;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemStateServerSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Plate>().ForEach((Entity entity, ref ItemInterpolatedState state,
                ref Translation translation, ref Rotation rotation) =>
            {
                state.Position = translation.Value;
                state.Rotation = rotation.Value;

                if (EntityManager.HasComponent<Parent>(entity))
                    state.Owner = EntityManager.GetComponentData<Parent>(entity).Value;
                else
                    state.Owner = Entity.Null;
            });
        }
    }
}