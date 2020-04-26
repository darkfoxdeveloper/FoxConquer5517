// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1103 - Send Skill.cs
// Last Edit: 2016/11/23 08:44
// Created: 2016/11/23 08:43
namespace ServerCore.Networking.Packets
{
    public sealed class MsgMagicInfo : PacketStructure
    {
        public MsgMagicInfo()
            : base(PacketType.MSG_MAGIC_INFO, 28, 20)
        {

        }

        public MsgMagicInfo(uint experience, ushort level, ushort type)
            : base(PacketType.MSG_MAGIC_INFO, 28, 20)
        {
            Experience = experience;
            Level = level;
            Type = type;
        }

        // hate when i type pubic instead of public...
        public MsgMagicInfo(byte[] packet)
            : base(packet)
        {

        }

        public uint Experience
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public ushort Type
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }

        public ushort Level
        {
            get { return ReadUShort(10); }
            set { WriteUShort(value, 10); }
        }
    }
}