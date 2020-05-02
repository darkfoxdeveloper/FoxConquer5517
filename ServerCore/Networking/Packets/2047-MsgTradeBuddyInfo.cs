// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2047 - MsgTradeBuddyInfo.cs
// Last Edit: 2016/12/07 21:44
// Created: 2016/12/07 21:39

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgTradeBuddyInfo : PacketStructure
    {
        public MsgTradeBuddyInfo()
            : base(PacketType.MSG_TRADE_BUDDY_INFO, 42, 34)
        {
            
        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Lookface
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public byte Level
        {
            get { return ReadByte(12); }
            set { WriteByte(value, 12); }
        }

        public ProfessionType Profession
        {
            get { return (ProfessionType) ReadByte(13); }
            set { WriteByte((byte) value, 13); }
        }

        public ushort PkPoints
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }

        public uint SyndicateIdentity
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public SyndicateRank SyndicateRank
        {
            get { return (SyndicateRank) ReadUInt(20); }
            set { WriteUInt((uint) value, 20); }
        }

        public ushort Unknown
        {
            get { return ReadUShort(24); }
            set { WriteUShort(value, 24); }
        }

        public string Name
        {
            get { return ReadString(16, 26); }
            set { WriteString(value, 16, 26); }
        }
    }
}