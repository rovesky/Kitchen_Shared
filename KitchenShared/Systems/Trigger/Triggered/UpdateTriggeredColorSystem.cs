using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class UpdateTriggeredColorSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .WithoutBurst()
                .ForEach((Entity entity,
                    ref TriggeredState state) =>
                {
                    state.TriggerEntity = Entity.Null;
                }).Run();


            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in TriggerPredictedState triggerPredictedData
                ) =>
                {
                    if (triggerPredictedData.TriggeredEntity == Entity.Null)
                        return;
                    var triggerEntity = triggerPredictedData.TriggeredEntity;
                    var triggerState = EntityManager.GetComponentData<TriggeredState>(triggerEntity);
                    triggerState.TriggerEntity = entity;
                    EntityManager.SetComponentData(triggerEntity, triggerState);

                }).Run();

            Entities
                .WithStructuralChanges()
                .WithoutBurst()
                .ForEach((Entity entity,
                    in TriggeredState state,
                    in TriggeredSetting setting) =>
                {

                    var isRenderTriggered = state.TriggerEntity != Entity.Null &&
                                            HasComponent<LocalCharacter>(state.TriggerEntity);


                    ChangeRenderder(entity, isRenderTriggered, setting);

                    var bufferEntity = GetBufferFromEntity<Child>();
                    if (!bufferEntity.Exists(entity))
                        return;
                    var buffer = bufferEntity[entity];
                    var children = buffer.ToNativeArray(Allocator.Temp);
                    foreach (var child in children)
                    {
                        // if(isRenderTriggered)
                        //  FSLog.Info($"ChangeRenderder,child:{child.Value}");
                        ChangeRenderder(child.Value, isRenderTriggered, setting);
                    }

                    children.Dispose();

                }).Run();
        }

        private void ChangeRenderder(Entity entity, bool isRenderTriggered, TriggeredSetting setting)
        {
            //tigger的模型本身
            if (EntityManager.HasComponent<RenderMesh>(entity))
            {
                var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
                volumeRenderMesh.material =
                    isRenderTriggered ? setting.TriggeredMaterial : setting.OriginMaterial;
                EntityManager.SetSharedComponentData(entity, volumeRenderMesh);
                // return;
            }

            //trigger的模型本身的显示
            if (!EntityManager.HasComponent<Presentation>(entity))
                return;
            var presentationEntity = EntityManager.GetComponentData<Presentation>(entity).Value;
            var presentationObject = EntityManager.GetComponentObject<Transform>(presentationEntity).gameObject;
            var renderers = presentationObject.GetComponentsInChildren<MeshRenderer>();

            foreach (var renderer in renderers)
                renderer.material = isRenderTriggered ? setting.TriggeredMaterial : setting.OriginMaterial;

        }
    }




    //[DisableAutoCreation]
    //public class UpdateTriggeredColorSystem : SystemBase
    //{

    //    protected override void OnUpdate()
    //    {
    //        //Entities
    //        //    .WithStructuralChanges()
    //        //    .ForEach((Entity entity,
    //        //        in TriggeredSetting setting) =>
    //        //    {
    //        //        Update(entity, setting,false);

    //        //    }).Run();
    //        Entities
    //            .WithStructuralChanges()
    //            .ForEach((in TriggerPredictedState state) =>
    //            {
    //                if (state.TriggeredEntity != state.PreTriggeredEntity)
    //                    Update1(state.PreTriggeredEntity, false);

    //                Update1(state.TriggeredEntity, true);
    //            }).Run();
    //    }

    //    private void Update1(Entity entity,  bool isRenderTriggered)
    //    {
    //        if (entity == Entity.Null)
    //            return;

    //        if (!EntityManager.HasComponent<TriggeredSetting>(entity))
    //            return;

    //        var setting = EntityManager.GetComponentData<TriggeredSetting>(entity);
    //        Update(entity,setting,isRenderTriggered);
    //    }

    //    private void Update(Entity entity,TriggeredSetting setting, bool isRenderTriggered)
    //    {
    //        ChangeRenderder(entity, isRenderTriggered, setting);

    //        var bufferEntity = GetBufferFromEntity<Child>();
    //        if (!bufferEntity.Exists(entity))
    //            return;
    //        var buffer = bufferEntity[entity];
    //        var children = buffer.ToNativeArray(Allocator.Temp);
    //        foreach (var child in children)
    //        {
    //            // if(isRenderTriggered)
    //            //  FSLog.Info($"ChangeRenderder,child:{child.Value}");
    //            ChangeRenderder(child.Value, isRenderTriggered, setting);
    //        }

    //        children.Dispose();
    //    }

    //    private void ChangeRenderder(Entity entity, bool isRenderTriggered, TriggeredSetting setting)
    //    {
    //        //tigger的模型本身
    //        if (EntityManager.HasComponent<RenderMesh>(entity))
    //        {
    //            var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
    //            volumeRenderMesh.material =
    //                isRenderTriggered ? setting.TriggeredMaterial : setting.OriginMaterial;
    //            EntityManager.SetSharedComponentData(entity, volumeRenderMesh);
    //            // return;
    //        }

    //        //trigger的模型本身的显示
    //        if (!EntityManager.HasComponent<Presentation>(entity))
    //            return;
    //        var presentationEntity = EntityManager.GetComponentData<Presentation>(entity).Value;
    //        var presentationObject = EntityManager.GetComponentObject<Transform>(presentationEntity).gameObject;
    //        var renderers = presentationObject.GetComponentsInChildren<MeshRenderer>();

    //        foreach (var renderer in renderers)
    //            renderer.material = isRenderTriggered ? setting.TriggeredMaterial : setting.OriginMaterial;

    //    }
    //}
}