// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2410 - Aura.cs
// Last Edit: 2016/12/08 16:47
// Created: 2016/11/23 09:46
namespace ServerCore.Networking.Packets
{
    public enum IconAction : uint
    {
        REMOVE = 2,
        ADD = 3
    }

    public enum AuraType  : uint
    {
        TYRANT_AURA = 1,
        FEND_AURA = 2,
        METAL_AURA = 3,
        WOOD_AURA = 4,
        WATER_AURA = 5,
        FIRE_AURA = 6,
        EARTH_AURA = 7,
        MAGIC_DEFENDER = 8,
    }

    public sealed class MsgAura : PacketStructure
    {
        public MsgAura(byte[] msg)
            : base(msg)
        {

        }

        public MsgAura()
            : base(PacketType.MSG_AURA, 40, 32)
        {

        }

        public uint Time
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public IconAction Action
        {
            get { return (IconAction)ReadUInt(8); }
            set { WriteUInt((uint)value, 8); }
        }

        public uint EntityIdentity
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint Type
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public uint Level
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public uint Power0
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint Power1
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }
    }
}