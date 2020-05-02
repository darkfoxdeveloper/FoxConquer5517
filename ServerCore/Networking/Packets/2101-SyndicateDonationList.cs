// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2101 - Syndicate Donation List.cs
// Last Edit: 2016/11/23 09:34
// Created: 2016/11/23 09:34

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgFactionRankInfo : PacketStructure
    {
        /// <summary>
        /// This packet fills the ranking in the Donation board of the guild. There is a subtype for each kind of ranking.
        /// </summary>
        public MsgFactionRankInfo()
            : base(PacketType.MSG_FACTION_RANK_INFO, 24, 16)
        {

        }

        public MsgFactionRankInfo(byte[] pMsg)
            : base(pMsg)
        {

        }

        public ushort Subtype
        {
            get { return ReadUShort(4); }
            set { WriteUShort(value, 4); }
        }

        public ushort Count
        {
            get { return ReadUShort(6); }
            set { WriteUShort(value, 6); }
        }

        public uint MaxCount
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public void AddMember(uint idUser, SyndicateRank pos, uint dwRank, int dwSilver, uint dwEmoney, int dwPk, uint dwGuide, uint dwArsenal,
            uint dwRose, uint dwWhite, uint dwOrchid, uint dwTulip, uint dwTotal, string szName)
        {
            int offset = 16 + (Count * 68);
            Count++;
            Resize(32 + Count * 68);
            WriteHeader(Length-8, PacketType.MSG_FACTION_RANK_INFO);
            WriteUInt((uint) pos, offset);
            WriteUShort((ushort) dwRank, offset + 4);
            WriteInt(dwSilver, offset + 8);
            WriteUInt(dwEmoney, offset + 12);
            WriteInt(dwPk, offset + 20); // pk
            WriteUInt(dwGuide, offset + 16); // Guide
            //WriteUInt(0, offset + 24);
            WriteUInt(dwArsenal, offset + 24);
            WriteUInt(dwRose, offset + 28);
            WriteUInt(dwWhite, offset + 32);
            WriteUInt(dwOrchid, offset + 36);
            WriteUInt(dwTulip, offset + 40);
            WriteUInt(dwTotal, offset + 44);
            WriteString(szName, 16, offset + 48);
        }
    }
}