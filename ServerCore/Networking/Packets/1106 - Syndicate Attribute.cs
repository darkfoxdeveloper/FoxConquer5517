// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1106 - Syndicate Attribute.cs
// Last Edit: 2016/11/23 08:46
// Created: 2016/11/23 08:46

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgSyndicateAttributeInfo : PacketStructure
    {
        public MsgSyndicateAttributeInfo()
            : base(PacketType.MSG_SYNDICATE_ATTRIBUTE_INFO, 100, 92)
        {

        }

        public MsgSyndicateAttributeInfo(byte[] packet)
            : base(packet)
        {

        }

        public uint SyndicateIdentity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public ulong MoneyFund
        {
            get { return ReadULong(12); }
            set { WriteULong(value, 12); }
        }

        public uint EMoneyFund
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public uint MemberAmount
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public SyndicateRank Position
        {
            get { return (SyndicateRank)ReadUShort(28); }
            set { WriteUShort((ushort)value, 28); }
        }

        public string LeaderName
        {
            get { return ReadString(16, 32); }
            set { WriteString(value, 16, 32); }
        }

        public uint RequiredLevel
        {
            get { return ReadUInt(48); }
            set { WriteUInt(value, 48); }
        }

        public uint RequiredMetempsychosis
        {
            get { return ReadUInt(52); }
            set { WriteUInt(value, 52); }
        }

        public uint RequiredProfession
        {
            get { return ReadUInt(56); }
            set { WriteUInt(value, 56); }
        }

        public byte SyndicateLevel
        {
            get { return ReadByte(60); }
            set { WriteByte(value, 60); }
        }

        public uint PositionExpire
        {
            get { return ReadUInt(63); }
            set { WriteUInt(value, 63); }
        }

        /// <summary>
        /// yyyymmdd
        /// </summary>
        public uint EnrollmentDate
        {
            get { return ReadUInt(67); }
            set { WriteUInt(value, 67); }
        }
    }
}