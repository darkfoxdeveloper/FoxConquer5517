// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1104 - Flush Skill Exp.cs
// Last Edit: 2016/11/23 08:44
// Created: 2016/11/23 08:44
namespace ServerCore.Networking.Packets
{
    public sealed class SkillExperiencePacket : PacketStructure
    {
        public SkillExperiencePacket()
            : base(PacketType.MSG_FLUSH_EXP, 20, 12)
        {

        }

        public SkillExperiencePacket(byte[] packet)
            : base(packet)
        {

        }

        public uint Experience
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public ushort Identity
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }

        public ushort ExperienceType
        {
            get { return ReadUShort(10); }
            set { WriteUShort(value, 10); }
        }
    }
}