// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1108 - View Equipment.cs
// Last Edit: 2016/11/23 08:48
// Created: 2016/11/23 08:48

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgItemInfoEx : PacketStructure
    {
        public MsgItemInfoEx()
            : base(PacketType.MSG_ITEM_INFO_EX, 92, 84)
        {

        }

        // Offset 48 is the monster shit that is in chinese and isnt used
        // Offset 54 is locked
        // Offset 68 is the item remaining time
        // Offset 76 does give a new icon to the item???? o_O WriteUInt(410339, 76);
        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }// 4

        public uint TargetIdentity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        } // 8

        public uint Price
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        } // 12

        public uint Itemtype
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        } // 16

        public ushort Durability
        {
            get { return ReadUShort(20); }
            set { WriteUShort(value, 20); }
        } // 20

        public ushort MaximumDurability
        {
            get { return ReadUShort(22); }
            set { WriteUShort(value, 22); }
        } // 22

        public ushort ViewType
        {
            get { return ReadUShort(24); }
            set { WriteUShort(value, 24); }
        } // 24

        public ItemPosition Position
        {
            get { return (ItemPosition) ReadUShort(26); }
            set { WriteUShort((ushort) value, 26); }
        } // 26

        public uint SocketProgress
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        } // 28

        public SocketGem SocketOne
        {
            get { return (SocketGem) ReadByte(32); }
            set { WriteByte((byte) value, 32); }
        } // 32

        public SocketGem SocketTwo
        {
            get { return (SocketGem) ReadByte(33); }
            set { WriteByte((byte) value, 33); }
        } // 33

        public uint Unknown1
        {
            get { return ReadUInt(34); }
            set { WriteUInt(value, 34); }
        } // 34

        public ushort Unknown2
        {
            get { return ReadUShort(38); }
            set { WriteUShort(value, 38); }
        } // 38

        public byte Unknown3
        {
            get { return ReadByte(40); }
            set { WriteByte(value, 40); }
        } // 40

        public byte Plus
        {
            get { return ReadByte(41); }
            set { WriteByte(value, 41); }
        } // 41

        public byte Bless
        {
            get { return ReadByte(42); }
            set { WriteByte(value, 42); }
        } // 42

        public bool Bound
        {
            get { return ReadBoolean(43); }
            set { WriteBoolean(value, 43); }
        } // 43

        public byte Enchant
        {
            get { return ReadByte(44); }
            set { WriteByte(value, 44); }
        } // 44

        public bool Suspicious
        {
            get { return ReadBoolean(52); }
            set { WriteBoolean(value, 52); }
        }// 53

        public bool Locked
        {
            get { return ReadBoolean(54); }
            set { WriteBoolean(value, 54); }
        }

        public ItemColor Color
        {
            get { return (ItemColor) ReadByte(56); }
            set { WriteByte((byte) value, 56); }
        } // 56

        public uint Composition
        {
            get { return ReadUInt(60); }
            set { WriteUInt(value, 60); }
        } // 60 

        public uint RemainingTime
        {
            get { return ReadUInt(68); }
            set { WriteUInt(value, 68); }
        }

        public uint StackAmount
        {
            get { return ReadUInt(72); }
            set { WriteUInt(value, 72); }
        }

        public uint Purification
        {
            get { return ReadUInt(76); }
            set { WriteUInt(value, 76); }
        }
    }
}