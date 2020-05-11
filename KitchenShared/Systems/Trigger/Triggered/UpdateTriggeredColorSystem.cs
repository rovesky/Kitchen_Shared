using FootStone.ECS;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

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
                    if (EntityManager.HasComponent<RenderMesh>(entity))
                    {
                        if(state.IsTriggered)
                           FSLog.Info($"entity:{entity},state.IsTriggered:{state.IsTriggered}");
                        var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
                        volumeRenderMesh.material =
                            state.IsTriggered ? setting.TriggeredMaterial : setting.OriginMaterial;
                        EntityManager.SetSharedComponentData(entity, volumeRenderMesh);
                        // return;
                    }

                    if (!EntityManager.HasComponent<Presentation>(entity))
                        return;
                    var presentationEntity = EntityManager.GetComponentData<Presentation>(entity).Value;
                    var presentationObject = EntityManager.GetComponentObject<Transform>(presentationEntity).gameObject;
                    var renderers = presentationObject.GetComponentsInChildren<MeshRenderer>();

                    foreach (var renderer in renderers)
                        renderer.material = state.IsTriggered ? setting.TriggeredMaterial : setting.OriginMaterial;
                }).Run();
        }
    }
}