// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2033 - Relation Info.cs
// Last Edit: 2016/11/23 09:02
// Created: 2016/11/23 09:02

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgFriendInfo : PacketStructure
    {
        public MsgFriendInfo()
            : base(PacketType.MSG_FRIEND_INFO, 52, 44)
        {

        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Mesh
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public byte Level
        {
            get { return ReadByte(12); }
            set { WriteByte(value, 12); }
        }

        public byte Profession
        {
            get { return ReadByte(13); }
            set { WriteByte(value, 13); }
        }

        public ushort PkPoints
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }

        public ushort SyndicateIdentity
        {
            get { return ReadUShort(16); }
            set { WriteUShort(value, 16); }
        }

        public SyndicateRank SyndicateRank
        {
            get { return (SyndicateRank)ReadUShort(20); }
            set { WriteUShort((ushort)value, 20); }
        }

        public string Mate
        {
            get { return ReadString(16, 26); }
            set { WriteString(value, 16, 26); }
        }

        public bool IsEnemy
        {
            get { return ReadBoolean(42); }
            set { WriteBoolean(value, 42); }
        }
    }
}