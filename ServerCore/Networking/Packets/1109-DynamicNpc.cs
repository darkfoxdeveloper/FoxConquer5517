// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1109 - Dynamic Npc.cs
// Last Edit: 2016/11/23 08:49
// Created: 2016/11/23 08:49
namespace ServerCore.Networking.Packets
{
    public sealed class MsgNpcInfoEx : PacketStructure
    {
        public MsgNpcInfoEx()
            : base(PacketType.MSG_NPC_INFO_EX, 40, 32)
        {

        }

        public MsgNpcInfoEx(byte[] packet)
            : base(packet)
        {

        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint MaxLife
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint Life
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public ushort MapX
        {
            get { return ReadUShort(20); }
            set { WriteUShort(value, 20); }
        }

        public ushort MapY
        {
            get { return ReadUShort(22); }
            set { WriteUShort(value, 22); }
        }

        public ushort Lookface
        {
            get { return ReadUShort(24); }
            set { WriteUShort(value, 24); }
        }

        public ushort Flag
        {
            get { return ReadUShort(26); }
            set { WriteUShort(value, 26); }
        }

        public ushort Type
        {
            get { return ReadUShort(28); }
            set { WriteUShort(value, 28); }
        }

        public string Name
        {
            get
            {
                return ReadByte(30) != 0 ? ReadString(ReadByte(31), 32) : null;
            }
            set
            {
                Resize(Length + value.Length);
                WriteHeader(Length - 8, PacketType.MSG_NPC_INFO_EX);
                WriteByte(1, 30);
                WriteStringWithLength(value, 31);
            }
        }
    }
}