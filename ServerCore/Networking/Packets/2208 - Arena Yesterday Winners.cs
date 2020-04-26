// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2208 - Arena Yesterday Winners.cs
// Last Edit: 2016/11/23 09:43
// Created: 2016/11/23 09:43
namespace ServerCore.Networking.Packets
{
    public sealed class MsgQualifyingSeasonRankList : PacketStructure
    {
        public MsgQualifyingSeasonRankList()
            : base(PacketType.MSG_QUALIFYING_SEASON_RANK_LIST, 16, 8)
        {

        }

        public MsgQualifyingSeasonRankList(byte[] pBuffer)
            : base(pBuffer)
        {

        }

        public uint Count
        {
            get { return ReadUInt(4); }
            set
            {
                Resize((int)(16 + (52 * value)));
                WriteHeader(Length - 8, PacketType.MSG_QUALIFYING_SEASON_RANK_LIST);
                WriteUInt(value, 4);
            }
        }

        public void AddPlayer(uint idRole, string szName, uint dwMesh, uint dwLevel, uint dwProf, uint dwScore, uint dwRank,
            uint dwWin, uint dwLose)
        {
            int offset = (int)(8 + (Count * 52));
            Count += 1;

            WriteUInt(idRole, offset);
            WriteString(szName, 16, offset + 4);
            WriteUInt(dwMesh, offset + 20);
            WriteUInt(dwLevel, offset + 24);
            WriteUInt(dwProf, offset + 28);
            WriteUInt(0, offset + 32);
            WriteUInt(dwRank, offset + 36);
            WriteUInt(dwScore, offset + 40);
            WriteUInt(dwWin, offset + 44); // win last
            WriteUInt(dwLose, offset + 48); // lose last
        }
    }
}