//using Unity.Entities;
//using UnityEngine;

//namespace FootStone.Kitchen
//{
//    public class OffsetBehaviour : MonoBehaviour, IConvertGameObjectToEntity
//    {
//        public Vector3 Position;
//        public Vector3 Rotation;

//        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
//            GameObjectConversionSystem conversionSystem)
//        {
//            dstManager.AddComponentData(entity, new OffsetSetting()
//            {
//                Pos = Position,
//                Rot = Quaternion.Euler(Rotation.x,Rotation.y,Rotation.z)
//            });
//        }
//    }
//}
