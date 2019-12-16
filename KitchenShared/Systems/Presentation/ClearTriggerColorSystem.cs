using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ClearTriggerColorSystem : ComponentSystem
    {
        private readonly Dictionary<int,Material> originMaterials = new Dictionary<int, Material>();

        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref TriggerData data) =>
            {
                var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);

                if (!originMaterials.ContainsKey(data.Type))
                {
                    originMaterials.Add(data.Type, volumeRenderMesh.material);
                }
             

                if (volumeRenderMesh.material == originMaterials[data.Type])
                    return;

                volumeRenderMesh.material = originMaterials[data.Type];

                EntityManager.SetSharedComponentData(entity, volumeRenderMesh);
            });

        }
    }
}