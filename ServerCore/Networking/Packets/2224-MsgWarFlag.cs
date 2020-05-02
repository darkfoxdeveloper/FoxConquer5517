// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2224 - MsgWarFlag.cs
// Last Edit: 2016/12/02 09:07
// Created: 2016/12/02 09:07

namespace ServerCore.Networking.Packets
{
    public sealed class MsgWarFlag : PacketStructure
    {
        public MsgWarFlag()
            : base(PacketType.MSG_WAR_FLAG, 64, 56)
        {
            
        }

        public MsgWarFlag(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        /// <summary>
        /// Gotta figure out
        /// </summary>
        public WarFlagType Type
        {
            get { return (WarFlagType) ReadUInt(4); }
            set { WriteUInt((uint) value, 4); }
        }

        public bool IsWar
        {
            get { return ReadBoolean(8); }
            set { WriteBoolean(value, 8); }
        }

        public uint Identity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        private int m_pStringStart = 20;

        public uint Amount
        {
            get { return ReadUInt(m_pStringStart); }
            set { WriteUInt(value, m_pStringStart); }
        }

        public void AddToWarRank(uint dwRank, uint dwPoints, string szName)
        {
            int offset = (int)(m_pStringStart + 4 + (Amount * 24));
            Amount += 1;
            Resize((int) (64 + (Amount*24)));
            WriteHeader(Length-8, PacketType.MSG_WAR_FLAG);

            WriteUInt(dwRank, offset);
            WriteUInt(dwPoints, offset + 4);
            WriteString(szName, 16, offset + 8);
        }
    }

    public enum WarFlagType
    {
        WAR_FLAG_BASE_RANK = 1,
        WAR_FLAG_TOP_4 = 2,
        WAR_BASE_DOMINATE = 5,
        GRAB_FLAG_EFFECT = 6,
        BASE_RANK_REQUEST = 7,
        SYNDICATE_REWARD_TAB = 11
    }
}