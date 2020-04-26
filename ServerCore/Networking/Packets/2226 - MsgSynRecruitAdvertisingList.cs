// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2226 - MsgSynRecruitAdvertisingList.cs
// Last Edit: 2016/12/07 23:04
// Created: 2016/12/07 22:27
namespace ServerCore.Networking.Packets
{
    public sealed class MsgSynRecruitAdvertisingList : PacketStructure
    {
        public MsgSynRecruitAdvertisingList()
            : base(PacketType.MSG_SYN_RECRUIT_ADVERTISING_LIST, 40, 32)
        {
            
        }

        public MsgSynRecruitAdvertisingList(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        /// <summary>
        /// The starting index.
        /// </summary>
        public uint StartIndex
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        /// <summary>
        /// The amount of displayed records.
        /// </summary>
        public uint Count
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        /// <summary>
        /// The maximum amount of records.
        /// </summary>
        public uint MaxCount
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        /// <summary>
        /// Set 1 when sending the first 2 records.
        /// </summary>
        public uint FirstMatch
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public void Append(uint idSyn, string szDesc, string szSyn, string szLeader, uint dwLevel, uint dwMembers,
            ulong ulFunds)
        {
            int offset = (int) (24 + (Count*344));
            Count += 1;

            Resize((int) (40 + Count*344));
            WriteHeader(Length-8, PacketType.MSG_SYN_RECRUIT_ADVERTISING_LIST);

            WriteUInt(idSyn, offset);
            WriteString(szDesc, 255, offset + 4);
            WriteString(szSyn, 36, offset + 259);
            WriteString(szLeader, 17, offset + 295);
            WriteUInt(dwLevel, offset + 312);
            WriteUInt(dwMembers, offset + 316);
            WriteULong(ulFunds, offset + 320);
            WriteUShort(1, offset + 328);
            WriteUInt(1, offset + 330);
        }
    }
}