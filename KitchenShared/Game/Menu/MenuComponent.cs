using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct Menu : IComponentData,IReplicatedState
    {
        public ushort Index;
        public ushort ProductId;
        public ushort MaterialId1;
        public ushort MaterialId2;
        public ushort MaterialId3;
        public ushort MaterialId4;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Index = reader.ReadUInt16();
            ProductId = reader.ReadUInt16();
            MaterialId1 = reader.ReadUInt16();
            MaterialId2 = reader.ReadUInt16();
            MaterialId3 = reader.ReadUInt16();
            MaterialId4 = reader.ReadUInt16();

        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
           writer.WriteUInt16("Index",Index);
           writer.WriteUInt16("ProductId",ProductId);
           writer.WriteUInt16("MaterialId1",MaterialId1);
           writer.WriteUInt16("MaterialId2",MaterialId2);
           writer.WriteUInt16("MaterialId3",MaterialId3);
           writer.WriteUInt16("MaterialId4",MaterialId4);
      
        }

        public static IReplicatedStateSerializerFactory CreateSerializerFactory()
        {
            return new ReplicatedStateSerializerFactory<Menu>();
        }

        public int MaterialCount()
        {
            var count = 0;

            if (MaterialId1 != 0)
                count++;
            if (MaterialId2 != 0)
                count++;
            if (MaterialId3 != 0)
                count++;
            if (MaterialId4 != 0)
                count++;

            return count;
        }

        public bool HasMaterial(ushort material)
        {
            return MaterialId1 == material 
                   || MaterialId2 == material 
                   || MaterialId3 == material
                   || MaterialId4 == material;

        }
    }
}