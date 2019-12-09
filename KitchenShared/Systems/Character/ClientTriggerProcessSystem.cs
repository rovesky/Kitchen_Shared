using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace FootStone.Kitchen
{
	[DisableAutoCreation]
	public class ClientTriggerProcessSystem : ComponentSystem
	{
		protected override void OnCreate()
		{
		}

        protected override void OnUpdate()
        {
            //Entities.ForEach((Entity entity, ref OnTriggerEnter enter) =>
            //{
            //    var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
            //    var newMat = new Material(volumeRenderMesh.material)
            //    {
            //        color = Color.white
            //    };
            //    volumeRenderMesh.material = newMat;

            //    PostUpdateCommands.SetSharedComponent(entity, volumeRenderMesh);

            //    EntityManager.RemoveComponent<OnTriggerEnter>(entity);
            //});

            //Entities.ForEach((Entity entity, ref OnTriggerExit enter) =>
            //{
            //    var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
            //    var newMat = new Material(volumeRenderMesh.material)
            //    {
            //        color = new Color(0.945f, 0.635f, 0.184f)
            //    };
            //    volumeRenderMesh.material = newMat;

            //    PostUpdateCommands.SetSharedComponent(entity, volumeRenderMesh);

            //    EntityManager.RemoveComponent<OnTriggerExit>(entity);
            //});



            Entities.ForEach((Entity entity, ref TriggerData data) =>
            {
                var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
                var newMat = new Material(volumeRenderMesh.material)
                {
                   // color = Color.white
                    color = new Color(0.945f, 0.635f, 0.184f)
                };
                volumeRenderMesh.material = newMat;

                PostUpdateCommands.SetSharedComponent(entity, volumeRenderMesh);

               // EntityManager.RemoveComponent<OnTriggerExit>(entity);
            });


            Entities.ForEach((ref CharacterPredictedState predictedState) =>
            {
                if(predictedState.TriggerEntity == Entity.Null)
                    return;

                var entity = predictedState.TriggerEntity;
                var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(entity);
                var newMat = new Material(volumeRenderMesh.material)
                {
                    color = Color.white
                    //color = new Color(0.945f, 0.635f, 0.184f)
                };
                volumeRenderMesh.material = newMat;
                PostUpdateCommands.SetSharedComponent(entity, volumeRenderMesh);

                // EntityManager.RemoveComponent<OnTriggerExit>(entity);
            });
        }

    }
}
