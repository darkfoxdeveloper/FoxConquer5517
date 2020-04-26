// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1024 - Attribute Points.cs
// Last Edit: 2016/11/23 08:28
// Created: 2016/11/23 08:28

namespace ServerCore.Networking.Packets
{
    public sealed class MsgAllot : PacketStructure
    {
        public MsgAllot(byte[] packet)
            : base(packet)
        {

        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Strength
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint Agility
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint Vitality
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public uint Spirit
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }
    }
}