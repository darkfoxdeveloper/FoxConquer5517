// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2223 - MsgElitePKGameRankInfo.cs
// Last Edit: 2017/01/09 15:21
// Created: 2017/01/09 14:30
namespace ServerCore.Networking.Packets
{
    public sealed class MsgElitePKGameRankInfo : PacketStructure
    {
        public MsgElitePKGameRankInfo()
            : base(PacketType.MSG_ELITE_PK_GAME_RANK_INFO, 56, 48)
        {
            
        }

        public MsgElitePKGameRankInfo(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public uint Type
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Group
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint GroupStatus
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint Count
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public uint Identity
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public void Append(int nRank, string szName, uint dwLookface, uint idRole)
        {
            int offset = (int) (32 + (Count*36));
            Count += 1;
            Resize((int) (56 + Count*36));
            WriteHeader(Length-8, PacketType.MSG_ELITE_PK_GAME_RANK_INFO);
            WriteInt(nRank, offset);
            WriteString(szName, 16, offset + 4);
            WriteUInt(dwLookface, offset + 20);
            WriteUInt(idRole, offset + 24);
        }
    }
}