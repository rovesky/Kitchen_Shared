//using System;
//using System.IO;
//using Unity.Entities;
//using UnityEngine;

//namespace Assets.Scripts.ECS
//{

//    public interface IEntityReferenceSerializer
//    {
//        void SerializeReference(ref NetworkWriter writer, string name, Entity entity);
//        void DeserializeReference(ref NetworkReader reader, ref Entity entity);
//    }

//    public struct SerializeContext
//    {
//        public EntityManager entityManager;
//        public Entity entity;
//        public IEntityReferenceSerializer refSerializer;
//        public int tick;
//    }

//    public interface IReplicate
//    {
//        void Serialize(ref SerializeContext context, ref NetworkWriter writer);
//        void Deserialize(ref SerializeContext context, ref NetworkReader reader);
//    }

//    public interface IInterpolateBase
//    {
//    }

//    public interface IIPredictBase
//    {
//    }

//    public interface IInterpolate<T> : IInterpolateBase, IReplicate
//    {
//        void Interpolate(ref T prevState, ref T nextState, float interpVal);
//    }

//    public interface IPredict<T> : IIPredictBase, IReplicate
//    {
//        bool VerifyPrediction(ref T state);
//    }
//}