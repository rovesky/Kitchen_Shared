//using FootStone.ECS;
//using Unity.Entities;
//using Unity.Transforms;

//namespace FootStone.Kitchen
//{
//    [DisableAutoCreation]
//    public class PhysicsPosSystem : ComponentSystem
//    {
//        protected override void OnUpdate()
//        {
//            Entities.WithAllReadOnly<ServerEntity>()
//                .ForEach((Entity entity, 
//                    ref ItemPredictedState predictData,
//                    ref Translation position, 
//                    ref Rotation    rotation) =>
//                {
//                    predictData.Position = position.Value;
//                    predictData.Rotation = rotation.Value;
                  
//                 //   FSLog.Info($"UpdateItemPresentationSystem,Position:{predictData.Position}");
//                });
//        }
//    }
//}