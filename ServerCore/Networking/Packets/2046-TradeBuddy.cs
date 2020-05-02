// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2046 - Trade Buddy.cs
// Last Edit: 2016/11/23 09:05
// Created: 2016/11/23 09:05
namespace ServerCore.Networking.Packets
{
    public enum TradePartnerType : byte
    {
        REQUEST_PARTNERSHIP = 0,
        REJECT_REQUEST = 1,
        BREAK_PARTNERSHIP = 4,
        ADD_PARTNER = 5
    }

    public class MsgTradeBuddy : PacketStructure
    {
        public MsgTradeBuddy()
            : base(PacketType.MSG_TRADE_BUDDY, 40, 32)
        {

        }

        public MsgTradeBuddy(byte[] pBuffer)
            : base(pBuffer)
        {

        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public TradePartnerType Type
        {
            get { return (TradePartnerType)ReadByte(8); }
            set { WriteByte((byte)value, 8); }
        }

        public bool Online
        {
            get { return ReadBoolean(9); }
            set { WriteBoolean(value, 9); }
        }

        public int HoursLeft
        {
            get { return ReadInt(10); }
            set { WriteInt(value * 60, 10); }
        }

        public string Name
        {
            get { return ReadString(16, 16); }
            set { WriteString(value, 16, 16); }
        }
    }
}