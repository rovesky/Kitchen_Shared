using Unity.Entities;
using Unity.Rendering;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ClearTriggeredSystem : SystemBase //ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .WithoutBurst()
                .ForEach((Entity entity,
                    ref TriggeredState state) =>
                {
                    state.IsTriggered = false;
                }).Run();
        }
    }

    [DisableAutoCreation]
    public class UpdateTriggeredColorSystem : SystemBase //ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .WithoutBurst()
                .ForEach((Entity entity,
                    in TriggeredState state,
                    in TriggeredSetting setting) =>
                {
                    var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
                    volumeRenderMesh.material = state.IsTriggered ? setting.TriggeredMaterial : setting.OriginMaterial;
                    EntityManager.SetSharedComponentData(entity, volumeRenderMesh);
                }).Run();
        }
    }
}