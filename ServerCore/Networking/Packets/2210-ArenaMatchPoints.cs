// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2210 - Arena Match Points.cs
// Last Edit: 2016/12/07 22:17
// Created: 2016/12/07 21:25
namespace ServerCore.Networking.Packets
{
    public sealed class MsgArenicScore : PacketStructure
    {
        public MsgArenicScore()
            : base(PacketType.MSG_ARENIC_SCORE, 64, 56)
        {
            
        }

        public uint EntityIdentity1
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public string Name1
        {
            get { return ReadString(16, 8); }
            set { WriteString(value, 16, 8); }
        }

        public uint Damage1
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint EntityIdentity2
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        public string Name2
        {
            get { return ReadString(16, 32); }
            set { WriteString(value, 16, 32); }
        }

        public uint Damage2
        {
            get { return ReadUInt(48); }
            set { WriteUInt(value, 48); }
        }
    }
}