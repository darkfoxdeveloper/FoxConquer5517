// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2209 - Arena Statistic.cs
// Last Edit: 2016/12/07 19:44
// Created: 2016/11/23 09:44
namespace ServerCore.Networking.Packets
{
    public sealed class MsgQualifyingDetailInfo : PacketStructure
    {
        public MsgQualifyingDetailInfo()
            : base(PacketType.MSG_QUALIFYING_DETAIL_INFO, 60, 52)
        {

        }

        public MsgQualifyingDetailInfo(byte[] pBuffer)
            : base(pBuffer)
        {

        }

        public uint Ranking
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Unknown
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public ArenaWaitStatus Status
        {
            get { return (ArenaWaitStatus)ReadUInt(12); }
            set { WriteUInt((uint)value, 12); }
        }

        public uint TotalWins
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public uint TotalLose
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public uint TodayWins
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint TodayLose
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        public uint TotalHonor
        {
            get { return ReadUInt(32); }
            set { WriteUInt(value, 32); }
        }

        public uint CurrentHonor
        {
            get { return ReadUInt(36); }
            set { WriteUInt(value, 36); }
        }

        public uint ArenaPoints
        {
            get { return ReadUInt(40); }
            set { WriteUInt(value, 40); }
        }
    }

    public enum ArenaWaitStatus : uint
    {
        NOT_SIGNED_UP = 0,
        WAITING_FOR_OPPONENT = 1,
        WAITING_INACTIVE = 2
    }
}