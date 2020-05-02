// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1034 - Detain Item Info.cs
// Last Edit: 2016/11/23 08:32
// Created: 2016/11/23 08:31

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public class MsgDetainItemInfo : PacketStructure
    {
        public MsgDetainItemInfo()
            : base(PacketType.MSG_DETAIN_ITEM_INFO, 120, 112)
        {

        }

        public MsgDetainItemInfo(byte[] pBuffer)
            : base(pBuffer)
        {

        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint ItemIdentity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint Itemtype
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public ushort Durability
        {
            get { return ReadUShort(16); }
            set { WriteUShort(value, 16); }
        }

        public ushort MaximumDurability
        {
            get { return ReadUShort(18); }
            set { WriteUShort(value, 18); }
        }

        public DetainMode Mode
        {
            get { return (DetainMode)ReadUInt(20); }
            set { WriteUInt((uint)value, 20); }
        }

        public uint SocketProgress
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public SocketGem SocketOne
        {
            get { return (SocketGem)ReadByte(28); }
            set { WriteByte((byte)value, 28); }
        }

        public SocketGem SocketTwo
        {
            get { return (SocketGem)ReadByte(29); }
            set { WriteByte((byte)value, 29); }
        }

        public ItemEffect Effect
        {
            get { return (ItemEffect)ReadByte(35); }
            set { WriteByte((byte)value, 35); }
        }

        public byte Plus
        {
            get { return ReadByte(37); }
            set { WriteByte(value, 37); }
        }

        public byte Blessing
        {
            get { return ReadByte(38); }
            set { WriteByte(value, 38); }
        }

        public bool Bound
        {
            get { return ReadBoolean(39); }
            set { WriteBoolean(value, 39); }
        }

        public byte Enchantment
        {
            get { return ReadByte(40); }
            set { WriteByte(value, 40); }
        }

        public bool Suspicious
        {
            get { return ReadBoolean(41); }
            set { WriteBoolean(value, 41); }
        }

        public bool Locked
        {
            get { return ReadBoolean(42); }
            set { WriteBoolean(value, 42); }
        }

        public ItemColor Color
        {
            get { return (ItemColor)ReadByte(44); }
            set { WriteByte((byte)value, 44); }
        }

        public uint OwnerIdentity
        {
            get { return ReadUInt(56); }
            set { WriteUInt(value, 56); }
        }

        public string OwnerName
        {
            get { return ReadString(16, 60); }
            set { WriteString(value, 16, 60); }
        }

        public uint TargetIdentity
        {
            get { return ReadUInt(76); }
            set { WriteUInt(value, 76); }
        }

        public string TargetName
        {
            get { return ReadString(16, 80); }
            set { WriteString(value, 16, 80); }
        }

        public uint Date // yyyymmdd
        {
            get { return ReadUInt(96); }
            set { WriteUInt(value, 96); }
        }

        public bool Expired
        {
            get { return ReadBoolean(100); }
            set { WriteBoolean(value, 100); }
        }

        public uint Cost
        {
            get { return ReadUInt(104); } // 104
            set { WriteUInt(value, 104); } // 104
        }

        public uint DaysPast
        {
            get { return ReadUInt(108); }
            set { WriteUInt(value, 108); }
        }
    }
}