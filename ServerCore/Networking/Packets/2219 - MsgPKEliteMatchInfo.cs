// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - ServerCore - 2219 - MsgPKEliteMatchInfo.cs
// Last Edit: 2017/02/15 18:40
// Created: 2017/02/15 18:39

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgPKEliteMatchInfo : PacketStructure
    {
        public MsgPKEliteMatchInfo()
            : base(PacketType.MSG_PK_ELITE_MATCH_INFO, 132, 124)
        {
            
        }

        public MsgPKEliteMatchInfo(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public ElitePkMatchType Type
        {
            get { return (ElitePkMatchType) ReadUShort(4);}
            set {  WriteUShort((ushort) value, 4); }
        }

        public ushort Page
        {
            get { return ReadUShort(6); }
            set {  WriteUShort(value, 6); }
        }

        public ushort UnknownUs8
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }

        public ushort MatchCount
        {
            get { return ReadUShort(10); }
            set { WriteUShort(value, 10); }
        }

        public ushort UnknownUs12
        {
            get { return ReadUShort(12); }
            set { WriteUShort(value, 12); }
        }

        public ushort Group
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }

        public ElitePkGuiType InterfaceType
        {
            get { return (ElitePkGuiType) ReadUShort(16); }
            set { WriteUShort((ushort) value, 16); }
        }

        public ushort TimeLeft
        {
            get { return ReadUShort(18); }
            set { WriteUShort(value, 18); }
        }

        public ushort TotalWatchers
        {
            get { return ReadUShort(20); }
            set { WriteUShort(value, 20); }
        }

        public bool OnGoing
        {
            get { return ReadBoolean(20); }
            set { WriteBoolean(value, 20); }
        }

        private int m_nStartOffset = 24;

        public void AppendMatch(uint idMatch, ushort idxMatch, ElitePkRoleStatusFlag flag, UserMatchStatus[] pRoles)
        {
            // 100 length???
            int nOffset = 24 + (MatchCount*100);
            MatchCount += 1;
            Resize(40 + (MatchCount*100));
            WriteHeader(Length - 8, PacketType.MSG_PK_ELITE_MATCH_INFO);
            WriteUInt(idMatch, nOffset);
            WriteUInt((uint) pRoles.Length, nOffset + 4);
            WriteUShort(idxMatch, nOffset + 6);
            WriteUShort((ushort) flag, nOffset + 8);
            for (int i = 0; i < pRoles.Length; i++)
            {
                WriteUInt(pRoles[i].Identity, nOffset + 10 + (i*30));
                WriteUInt(pRoles[i].Lookface, nOffset + 14 + (i*30));
                WriteString(pRoles[i].Name, 16, nOffset + 18 + (i*30));
                WriteUInt((uint) pRoles[i].Flag, nOffset + 34 + (i*30));
                WriteBoolean(pRoles[i].Advance, nOffset + 38 + (i*30));
            }
        }
    }

    public class UserMatchStatus
    {
        public uint Identity;
        public string Name;
        public uint Lookface;
        public uint Wage;
        public uint Cheer;
        public uint Points;
        public ElitePkRoleStatusFlag Flag;
        public bool Advance;

        public void Reset()
        {
            Wage = 0;
            Cheer = 0;
            Points = 0;
            Advance = false;
        }
    }
}