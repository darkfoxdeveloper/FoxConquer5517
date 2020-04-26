// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2030 - Npc Information.cs
// Last Edit: 2016/11/23 08:59
// Created: 2016/11/23 08:58
namespace ServerCore.Networking.Packets
{
    public class MsgNpcInfo : PacketStructure
    {
        public MsgNpcInfo()
            : base(PacketType.MSG_NPC_INFO, 36, 28)
        {
            
        }

        public MsgNpcInfo(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Sort
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public ushort MapX
        {
            get { return ReadUShort(12); }
            set { WriteUShort(value, 12); }
        }

        public ushort MapY
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }

        public ushort Lookface
        {
            get { return ReadUShort(16); }
            set { WriteUShort(value, 16); }
        }

        public ushort Kind
        {
            get { return ReadUShort(18); }
            set { WriteUShort(value, 18); }
        }

        public string Name
        {
            get { return ReadString(ReadByte(20), 21); }
            set { WriteStringWithLength(value, 20); }
        }
    }
}