using System;
using System.IO;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    [InternalBufferCapacity(64)]
    public struct UserCommandBuffer : IBufferElementData
    {
        public UserCommand Command;
    }

    [Serializable]
    public struct UserCommand : IComponentData
    {
        public enum Button : uint
        {
            None = 0,
            Pickup = 1 << 0,
            Throw = 1 << 1,
            Jump = 1 << 2,
            Rush = 1 << 3,
            Ability2 = 1 << 4,
            Ability3 = 1 << 5,
            Ability4 = 1 << 6,
            Ability5 = 1 << 7,
            Ability6 = 1 << 8,
            Ability7 = 1 << 9
        }

        public struct ButtonBitField
        {
            public uint Flags;

            public bool IsSet(Button button)
            {
                return (Flags & (uint)button) > 0;
            }

            public void Or(Button button, bool val)
            {
                if (val)
                    Flags = Flags | (uint)button;
            }

            public void Set(Button button, bool val)
            {
                if (val)
                    Flags = Flags | (uint)button;
                else
                {
                    Flags = Flags & ~(uint)button;
                }
            }
        }

        public uint CheckTick;
        public uint RenderTick;
        public ButtonBitField Buttons;
        public Vector3 TargetDir;

        public static UserCommand DefaultCommand = new UserCommand
        {
            CheckTick = 0,
            RenderTick = 0,
            Buttons = default,
            TargetDir = Vector3.zero
        };

        public void Reset()
        {
            //  checkTick = 0;
            //  renderTick = 0;
            Buttons.Flags = 0;
            // targetPos = Vector3.zero;
        }

        //public byte[] ToData()
        //{
        //    MemoryStream memStream = new MemoryStream(100);
        //    BinaryWriter writer = new BinaryWriter(memStream);

        //    writer.Write(CheckTick);
        //    writer.Write(RenderTick);
        //    writer.Write(Buttons.Flags);
        //    writer.Write(TargetDir.x);
        //    writer.Write(TargetDir.y);
        //    writer.Write(TargetDir.z);

        //    return memStream.ToArray();
        //}

        //public void FromData(byte[] data)
        //{
        //    var memStream = new MemoryStream(data);
        //    var reader = new BinaryReader(memStream);

        //    CheckTick = reader.ReadUInt32();
        //    RenderTick = reader.ReadUInt32();
        //    Buttons.Flags = reader.ReadUInt32();
        //    TargetDir.x = reader.ReadSingle();
        //    TargetDir.y = reader.ReadSingle();
        //    TargetDir.z = reader.ReadSingle();           
        //}


        public void Serialize(ref NetworkWriter networkWriter)
        {
            networkWriter.WriteUInt32("checkTick", CheckTick);
            networkWriter.WriteUInt32("renderTick", RenderTick);
            networkWriter.WriteUInt32("buttonFlag", Buttons.Flags);
            networkWriter.WriteVector3Q("targetPos", TargetDir);          
        }

        public void Deserialize(ref NetworkReader networkReader)
        {
            CheckTick = networkReader.ReadUInt32();
            RenderTick = networkReader.ReadUInt32();
            Buttons.Flags = networkReader.ReadUInt32();
            TargetDir = networkReader.ReadVector3Q();       
        }
    }


    public struct CurrentCommand  : IComponentData
    {
        public UserCommand  Command;
    }


    public struct PredictCommands : IComponentData
    {
         public BlobArray<UserCommand> Commands;
    }


}