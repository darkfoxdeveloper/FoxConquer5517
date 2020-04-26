// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2031 - Npc.cs
// Last Edit: 2016/11/23 09:00
// Created: 2016/11/23 09:00

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public class MsgNpc : PacketStructure
    {
        public MsgNpc()
            : base(PacketType.MSG_NPC, 24, 16)
        {

        }

        public MsgNpc(byte[] pBuffer)
            : base(pBuffer)
        {

        }

        public MsgNpc(uint idObj, uint dwType, NpcActionType pAction, ushort usData)
            : base(PacketType.MSG_NPC, 24, 16)
        {
            Identity = idObj;
            Data = dwType;
            Action = pAction;
            Sort = usData;
        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Data
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public NpcActionType Action
        {
            get { return (NpcActionType)ReadUShort(12); }
            set { WriteUShort((ushort)value, 12); }
        }

        public ushort Sort
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }
    }
}