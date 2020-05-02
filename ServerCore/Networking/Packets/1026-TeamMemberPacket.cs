// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1026 - Team Member Packet.cs
// Last Edit: 2016/11/23 08:29
// Created: 2016/11/23 08:29
namespace ServerCore.Networking.Packets
{
    public sealed class MsgTeamMember : PacketStructure
    {
        public MsgTeamMember()
            : base(PacketType.MSG_TEAM_MEMBER, 160, 152)
        {
            WriteByte(1, 5);
        }

        public MsgTeamMember(byte[] packet)
            : base(packet)
        {

        }

        public string Name
        {
            get { return ReadString(16, 8); }
            set { WriteString(value, value.Length, 8); }
        }

        public uint Entity
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint Mesh
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        public ushort MaxLife
        {
            get { return ReadUShort(32); }
            set { WriteUShort(value, 32); }
        }

        public ushort Life
        {
            get { return ReadUShort(34); }
            set { WriteUShort(value, 34); }
        }
    }
}