using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ClearTriggerColorSystem : ComponentSystem
    {
        private Material originMaterial;

        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref TriggerData data) =>
            {
                var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);

                if (originMaterial == null)
                    originMaterial = volumeRenderMesh.material;

                if (volumeRenderMesh.material == originMaterial)
                    return;

                volumeRenderMesh.material = originMaterial;

                EntityManager.SetSharedComponentData(entity, volumeRenderMesh);
            });

        }
    }
}