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
                    state.TriggerEntity= Entity.Null;
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

                    var isRenderTriggered = state.TriggerEntity != Entity.Null &&
                        HasComponent<LocalCharacter>(state.TriggerEntity);

                    if (EntityManager.HasComponent<RenderMesh>(entity))
                    {
                       var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
                        volumeRenderMesh.material =
                            isRenderTriggered ? setting.TriggeredMaterial : setting.OriginMaterial;
                        EntityManager.SetSharedComponentData(entity, volumeRenderMesh);
                        // return;
                    }

                    if (!EntityManager.HasComponent<Presentation>(entity))
                        return;
                    var presentationEntity = EntityManager.GetComponentData<Presentation>(entity).Value;
                    var presentationObject = EntityManager.GetComponentObject<Transform>(presentationEntity).gameObject;
                    var renderers = presentationObject.GetComponentsInChildren<MeshRenderer>();

                    foreach (var renderer in renderers)
                        renderer.material = isRenderTriggered ? setting.TriggeredMaterial : setting.OriginMaterial;
                }).Run();
        }
    }
}