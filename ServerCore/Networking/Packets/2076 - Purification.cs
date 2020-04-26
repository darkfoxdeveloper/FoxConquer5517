// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2076 - Purification.cs
// Last Edit: 2016/11/23 09:18
// Created: 2016/11/23 09:17
namespace ServerCore.Networking.Packets
{
    public sealed class MsgQuench : PacketStructure
    {
        public MsgQuench()
            : base(PacketType.MSG_QUENCH, 24, 16)
        {

        }

        public MsgQuench(byte[] packet)
            : base(packet)
        {

        }

        public PurificationMode Mode
        {
            get { return (PurificationMode)ReadByte(4); }
            set { WriteByte((byte)value, 4); }
        }

        /// <summary>
        /// The item identity (cq_item id) of the item.
        /// </summary>
        public uint ItemIdentity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        /// <summary>
        /// The identity of the artifact that will be stabilized.
        /// </summary>
        public uint TargetIdentity
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }
    }

    public enum PurificationMode : byte
    {
        ITEM_ARTIFACT = 1, PURIFY = 0, STABILIZE = 2
    }
}