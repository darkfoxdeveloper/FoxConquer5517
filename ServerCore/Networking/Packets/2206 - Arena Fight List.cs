// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2206 - Arena Fight List.cs
// Last Edit: 2016/11/23 09:42
// Created: 2016/11/23 09:42
namespace ServerCore.Networking.Packets
{
    public sealed class MsgQualifyingFightersList : PacketStructure
    {
        public MsgQualifyingFightersList()
            : base(PacketType.MSG_QUALIFYING_FIGHTERS_LIST, 64, 56)
        {

        }

        public MsgQualifyingFightersList(byte[] pBuffer)
            : base(pBuffer)
        {

        }

        public uint Page
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Unknown
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint MatchesCount
        {
            get { return ReadUInt(12); }
            set
            {
                Resize((int)(64 + (120 * value)));
                WriteHeader(Length - 8, PacketType.MSG_QUALIFYING_FIGHTERS_LIST);
                WriteUInt(value, 12);
            }
        }

        public uint PlayerAmount
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public uint Unknown1
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public uint Showing
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public void AddMatch(uint idRole0, uint dwMesh0, string szName0, uint dwLevel0, uint dwProf0, uint dwRank0,
            uint dwPoints0, uint dwWinsToday0, uint dwLossToday0, uint dwCurrentHonor0, uint dwTotalHonor0,
            uint idRole1, uint dwMesh1, string szName1, uint dwLevel1, uint dwProf1, uint dwRank1, uint dwPoints1,
            uint dwWinsToday1, uint dwLossToday1, uint dwCurrentHonor1, uint dwTotalHonor1)
        {
            // 
            int offset = (int)(28 + (120 * MatchesCount));
            MatchesCount += 1;
            WriteUInt(idRole0, offset);
            WriteUInt(dwMesh0, offset + 4);
            WriteString(szName0, 16, offset + 8);
            WriteUInt(dwLevel0, offset + 24);
            WriteUInt(dwProf0, offset + 28);
            WriteUInt(dwRank0, offset + 36);
            WriteUInt(dwPoints0, offset + 40);
            WriteUInt(dwWinsToday0, offset + 44);
            WriteUInt(dwLossToday0, offset + 48);
            WriteUInt(dwCurrentHonor0, offset + 52);
            WriteUInt(dwTotalHonor0, offset + 56);

            WriteUInt(idRole1, offset + 60);
            WriteUInt(dwMesh1, offset + 64);
            WriteString(szName1, 16, offset + 68);
            WriteUInt(dwLevel1, offset + 84);
            WriteUInt(dwProf1, offset + 88);
            WriteUInt(dwRank1, offset + 96);
            WriteUInt(dwPoints1, offset + 100);
            WriteUInt(dwWinsToday1, offset + 104);
            WriteUInt(dwLossToday1, offset + 108);
            WriteUInt(dwCurrentHonor1, offset + 112);
            WriteUInt(dwTotalHonor1, offset + 116);
        }
    }
}