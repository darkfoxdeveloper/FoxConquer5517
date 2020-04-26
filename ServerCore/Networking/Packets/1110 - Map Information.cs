// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1110 - Map Information.cs
// Last Edit: 2016/11/23 08:50
// Created: 2016/11/23 08:50
namespace ServerCore.Networking.Packets
{
    public sealed class MsgMapInfo : PacketStructure
    {
        public MsgMapInfo()
            : base(PacketType.MSG_MAP_INFO, 28, 20)
        {

        }

        public MsgMapInfo(uint mapId, uint mapDoc, ulong flags)
            : base(PacketType.MSG_MAP_INFO, 28, 20)
        {
            MapId = mapId;
            MapDoc = mapDoc;
            Flags = flags;
        }

        public uint MapId
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint MapDoc
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public ulong Flags
        {
            get { return ReadULong(12); }
            set { WriteULong(value, 12); }
        }
    }
}