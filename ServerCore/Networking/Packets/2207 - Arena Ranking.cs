// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2207 - Arena Ranking.cs
// Last Edit: 2016/11/23 09:43
// Created: 2016/11/23 09:43
namespace ServerCore.Networking.Packets
{
    public sealed class MsgQualifyingRank : PacketStructure
    {
        public MsgQualifyingRank()
            : base(PacketType.MSG_QUALIFYING_RANK, 56, 48)
        {

        }

        public MsgQualifyingRank(byte[] pBuffer)
            : base(pBuffer)
        {

        }

        public ArenaRankType RankType
        {
            get { return (ArenaRankType)ReadUShort(4); }
            set { WriteUShort((ushort)value, 4); }
        }

        public ushort PageNumber
        {
            get { return ReadUShort(6); }
            set { WriteUShort(value, 6); }
        }

        public uint Count
        {
            get { return ReadUInt(8); }
            set
            {
                WriteUInt(value, 8);
            }
        }

        public uint Showing
        {
            get { return ReadUInt(12); }
            set
            {
                Resize((int)(56 + (32 * value)));
                WriteHeader(Length - 8, PacketType.MSG_QUALIFYING_RANK);
                WriteUInt(value, 12);
            }
        }

        public void AddPlayer(ushort usRank, string szName, ushort usType, uint dwPoints,
            uint dwProf, uint dwLevel, uint dwUnknown)
        {
            int offset = (int)(16 + Showing * 36);
            Showing += 1;

            WriteUShort(usRank, offset);
            WriteString(szName, 16, offset + 2);
            WriteUShort(usType, offset + 18);
            WriteUInt(dwPoints, offset + 20);
            WriteUInt(dwProf, offset + 24);
            WriteUInt(dwLevel, offset + 28);
        }
    }

    public enum ArenaRankType : ushort
    {
        QUALIFIER_RANK,
        HONOR_HISTORY
    }
}