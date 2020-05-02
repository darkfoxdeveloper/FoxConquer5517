// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1023 - Team Action.cs
// Last Edit: 2016/11/23 08:27
// Created: 2016/11/23 08:27

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgTeam : PacketStructure
    {
        public MsgTeam()
            : base(PacketType.MSG_TEAM, 20, 12)
        {

        }

        public MsgTeam(byte[] packet)
            : base(packet)
        {

        }

        public uint Target
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public TeamActionType Type
        {
            get { return (TeamActionType)ReadUInt(4); }
            set { WriteUInt((uint)value, 4); }
        }
    }
}