// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2102 - Syndicate Member.cs
// Last Edit: 2016/11/23 09:35
// Created: 2016/11/23 09:35

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// This packet fills the member list of the guild.
    /// </summary>
    public sealed class MsgSynMemberList : PacketStructure
    {
        public MsgSynMemberList()
            : base(PacketType.MSG_SYN_MEMBER_LIST, 28, 20)
        {

        }

        public MsgSynMemberList(byte[] packet)
            : base(packet)
        {

        }

        public uint Subtype
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint StartIndex
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint Amount
        {
            get { return ReadUInt(12); }
            set
            {
                Resize((int)(28 + value * 48));
                WriteHeader(Length - 8, PacketType.MSG_SYN_MEMBER_LIST);
                WriteUInt(value, 12);
            }
        }

        public void Append(string name, uint dwLookface, uint nobility, ushort level, SyndicateRank rank, uint positionExpire, uint totalDonation,
            bool isOnline, ushort profession, ulong offlineTime)
        {
            Amount += 1;
            uint dwShowNobility = nobility*10;
            if (dwLookface%10 == 3 || dwLookface%10 == 4)
                dwShowNobility += 1;
            else
                dwShowNobility += 2;
            var offset = (int)(16 + ((Amount - 1) * 48));
            WriteString(name, 16, offset);
            WriteUInt(0, offset + 16); // lookface? mess the nobility if filled
            WriteUInt(dwShowNobility, offset + 20);
            WriteUInt(level, offset + 24);
            WriteUShort((ushort)rank, offset + 28);
            WriteUInt(positionExpire, offset + 32);
            WriteUInt(totalDonation, offset + 36);
            WriteBoolean(isOnline, offset + 40);
            WriteUInt(0, offset + 44); // Unknown , changes nothing
        }
    }
}