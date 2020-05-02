// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1102 - Warehouse Packet.cs
// Last Edit: 2016/11/23 08:43
// Created: 2016/11/23 08:42
namespace ServerCore.Networking.Packets
{
    public enum WarehouseMode : byte
    {
        WH_VIEW = 0, WH_ADDITEM, WH_REMITEM
    }

    public sealed class MsgAccountSoftKb : PacketStructure
    {
        /// <summary>
        /// This method will create the basic packet with no items attached. Use append method
        /// to add items to it.
        /// </summary>
        public MsgAccountSoftKb()
            : base(PacketType.MSG_ACCOUNT_SOFT_KB, 104, 96)
        {

        }

        public MsgAccountSoftKb(byte[] packet)
            : base(packet)
        {

        }

        /// <summary>
        /// The warehouse identity.
        /// </summary>
        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public WarehouseMode Action
        {
            get { return (WarehouseMode)ReadByte(8); }
            set { WriteByte((byte)value, 8); }
        }

        public byte Type
        {
            get { return ReadByte(9); }
            set { WriteByte(value, 9); }
        }

        public byte MaximumAmount
        {
            get { return ReadByte(12); }
            set { WriteByte(value, 12); }
        }

        public uint Identifier
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public byte ItemsCount
        {
            get { return ReadByte(20); }
            set
            {
                Resize(32 + (value * 52));
                WriteHeader(Length - 8, PacketType.MSG_ACCOUNT_SOFT_KB);
                WriteByte(value, 20);
            }
        }

        public uint ItemIdentity
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint Itemtype
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        public byte SocketOne
        {
            get { return ReadByte(33); }
            set { WriteByte(value, 33); }
        }

        public byte SocketTwo
        {
            get { return ReadByte(34); }
            set { WriteByte(value, 34); }
        }

        public byte Plus
        {
            get { return ReadByte(41); }
            set { WriteByte(value, 41); }
        }

        public byte Bless
        {
            get { return ReadByte(42); }
            set { WriteByte(value, 42); }
        }

        public bool Bound
        {
            get { return ReadBoolean(43); }
            set { WriteBoolean(value, 43); }
        }

        public ushort Enchant
        {
            get { return ReadUShort(44); }
            set { WriteUShort(value, 44); }
        }

        public ushort Effect
        {
            get { return ReadUShort(36); }
            set { WriteUShort(value, 36); }
        }

        public bool Locked
        {
            get { return ReadBoolean(50); }
            set { WriteBoolean(value, 50); }
        }

        public bool Suspicious
        {
            get { return ReadBoolean(48); }
            set { WriteBoolean(value, 48); }
        }

        public byte Color
        {
            get { return ReadByte(51); }
            set { WriteByte(value, 51); }
        }

        public uint SocketProgress
        {
            get { return ReadUInt(52); }
            set { WriteUInt(value, 52); }
        }

        public uint AddLevelExp
        {
            get { return ReadUInt(56); }
            set { WriteUInt(value, 56); }
        }

        public uint RemainingTime
        {
            get { return ReadUInt(64); }
            set { WriteUInt(value, 64); }
        }

        public ushort StackAmount
        {
            get { return ReadUShort(68); }
            set { WriteUShort(value, 68); }
        }
    }
}
