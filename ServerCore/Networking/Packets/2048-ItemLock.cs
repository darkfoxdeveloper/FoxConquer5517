// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2048 - Item Lock.cs
// Last Edit: 2016/11/23 09:10
// Created: 2016/11/23 09:10
namespace ServerCore.Networking.Packets
{
    public enum LockMode : byte
    {
        REQUEST_LOCK = 0,
        REQUEST_UNLOCK = 1,
        UNLOCK_DATE = 2,
        UNLOCKED_ITEM = 3
    }

    public sealed class MsgEquipLock : PacketStructure
    {
        public MsgEquipLock()
            : base(PacketType.MSG_EQUIP_LOCK, 24, 16)
        {

        }

        public MsgEquipLock(byte[] packet)
            : base(packet)
        {

        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public LockMode Mode
        {
            get { return (LockMode)ReadByte(8); }
            set { WriteByte((byte)value, 8); }
        }

        public byte Unknown
        {
            get { return ReadByte(9); }
            set { WriteByte(value, 9); }
        }

        public uint Param
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }
    }
}