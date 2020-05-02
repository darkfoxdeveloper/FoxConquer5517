// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1056 - Trade.cs
// Last Edit: 2016/11/23 08:38
// Created: 2016/11/23 08:37

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgTrade : PacketStructure
    {
        public MsgTrade()
            : base(PacketType.MSG_TRADE, 28, 20)
        {

        }

        public MsgTrade(byte[] packet)
            : base(packet)
        {

        }

        public uint Target
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public TradeType Type
        {
            get { return (TradeType)ReadByte(8); }
            set { WriteByte((byte)value, 8); }
        }

        public uint Unknown
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public ushort UnknownLow
        {
            get { return ReadUShort(12); }
            set { WriteUShort(value, 12); }
        }

        public ushort UnknownHigh
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }
    }
}