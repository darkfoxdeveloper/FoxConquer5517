// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1063 - MsgSelfSynMemAwardRank.cs
// Last Edit: 2016/12/02 22:15
// Created: 2016/12/02 10:22
namespace ServerCore.Networking.Packets
{
    public sealed class MsgSelfSynMemAwardRank : PacketStructure
    {
        /// <summary>
        /// This packet is used to fill some rankings related to the guild. Like the CTF page in the Arena button, the ranking
        /// with the top players in the CTF, the top awards and also the guilds with the higher prize for the next event.
        /// </summary>
        public MsgSelfSynMemAwardRank()
            : base(PacketType.MSG_SELF_SYN_MEM_AWARD_RANK, 72, 64)
        {
            
        }

        public MsgSelfSynMemAwardRank(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public ushort Type
        {
            get { return ReadUShort(4); }
            set { WriteUShort(value, 4); }
        }

        /// <summary>
        /// not sure, most of the times the value is 1
        /// </summary>
        public ushort Unknown6
        {
            get { return ReadUShort(6); }
            set { WriteUShort(value, 6); }
        }

        public ushort Unknown8
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }

        public uint Page
        {
            get { return ReadUInt(10); }
            set { WriteUInt(value, 10); }
        }

        public uint ResultNum
        {
            get { return ReadUInt(10); }
            set { WriteUInt(value, 10); }
        }

        public ushort Count
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }

        public uint Exploits
        {
            get { return ReadUInt(18); }
            set { WriteUInt(value, 18); }
        }

        public uint EmoneyPrize
        {
            get { return ReadUInt(18); }
            set { WriteUInt(value, 18); }
        }

        public long MoneyPrize
        {
            get { return ReadLong(22); }
            set { WriteLong(value, 22); }
        }

        public uint SetEmoney
        {
            get { return ReadUInt(22); }
            set { WriteUInt(value, 22); }
        }

        public uint SetMoney
        {
            get { return ReadUInt(26); }
            set { WriteUInt(value, 26); }
        }

        /// <summary>
        /// User ranking into syndicate. used to show exploits of the user
        /// </summary>
        /// <param name="szUsername"></param>
        /// <param name="dwExploit"></param>
        public void AddToRanking(string szUsername, uint dwExploit)
        {
            int offset = (30 + Count * 20);
            Count += 1;
            Resize(48 + Count * 20);
            WriteHeader(Length - 8, PacketType.MSG_SELF_SYN_MEM_AWARD_RANK);
            WriteString(szUsername, 16, offset);
            WriteUInt(dwExploit, offset + 16);
        }

        /// <summary>
        /// Syndicates donation ranking (top prizes)
        /// </summary>
        public void AddToRanking(string szSynName, uint dwEmoney, long money)
        {
            int offset = (30 + Count*52);
            Count += 1;
            Resize(48 + Count * 52);
            WriteHeader(Length - 8, PacketType.MSG_SELF_SYN_MEM_AWARD_RANK);
            WriteUInt(dwEmoney, offset);
            WriteLong(money, offset + 4);
            WriteString(szSynName, 16, offset + 12);
        }

        /// <summary>
        /// CTF Ranking
        /// </summary>
        public void AddToRanking(uint dwRank, uint dwIdentity, string szSynName, long dwMoney, uint dwEmoney, uint dwPoints)
        {
            int offset = (30 + Count*60);
            Count += 1;
            Resize(48 + Count*60);
            WriteHeader(Length-8, PacketType.MSG_SELF_SYN_MEM_AWARD_RANK);
            WriteUInt(dwRank, offset);
            WriteUInt(dwPoints, offset + 4); // exploits
            WriteUInt(dwEmoney, offset + 8); // emoney
            WriteLong(dwMoney, offset + 12); // money
            WriteUInt(dwIdentity, offset + 20);
            WriteString(szSynName, 16, offset + 24);
        }

        /// <summary>
        /// Members ranking into syndicate screen
        /// </summary>
        public void AddToRanking(string szSynName, uint dwPoints, uint dwMembers, long dwSilver, uint dwEmoney)
        {
            int offset = 30 + 36 * Count;
            Count += 1;
            Resize(48 + Count * 36);
            WriteHeader(Length - 8, PacketType.MSG_SELF_SYN_MEM_AWARD_RANK);
            WriteString(szSynName, 16, offset);
            WriteUInt(dwPoints, offset + 16);
            WriteUInt(dwMembers, offset + 20);
            WriteLong(dwSilver, offset + 24);
            WriteUInt(dwEmoney, offset + 32);
        }
    }
}