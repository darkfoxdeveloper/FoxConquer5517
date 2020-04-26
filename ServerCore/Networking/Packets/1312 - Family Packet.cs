// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1312 - Family Packet.cs
// Last Edit: 2016/12/27 20:41
// Created: 2016/11/23 08:56

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// This packet is used to manage the family actions, such as join, quit, ally, enemy and etc.
    /// </summary>
    public sealed class MsgFamily : PacketStructure
    {
        public MsgFamily(byte[] pBuffer)
            : base(pBuffer)
        {

        }

        public MsgFamily()
            : base(PacketType.MSG_FAMILY, 88, 80)
        {

        }

        public FamilyType Type
        {
            get { return (FamilyType)ReadUInt(4); }
            set { WriteUInt((uint)value, 4); }
        }

        public uint Identity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public byte StringCount
        {
            get { return ReadByte(16); }
            set { WriteByte(value, 16); }
        }

        public string Announcement
        {
            get { return ReadString(ReadByte(17), 18); }
            set
            {
                StringCount = 1;
                Resize(88 + value.Length + 2);
                WriteHeader(Length - 8, PacketType.MSG_FAMILY);
                WriteStringWithLength(value, 17);
            }
        }

        public string Name
        {
            get { return ReadString(16, 18); }
            set
            {
                Resize(88 + 18);
                WriteHeader(Length - 8, PacketType.MSG_FAMILY);
                WriteString(value, 16, 18);
            }
        }

        private int m_nTextLength = 0;

        public void AddString(string szString)
        {
            StringCount += 1;
            Resize(88 + m_nTextLength + StringCount);
            WriteHeader(Length - 8, PacketType.MSG_FAMILY);
            int offset = 16 + StringCount + m_nTextLength;
            WriteStringWithLength(szString, offset);
            m_nTextLength += szString.Length;
        }

        public void AddMember(string szName, byte pLevel, ushort pProf, FamilyRank pRank, bool bOnline, uint dwDonation)
        {
            StringCount += 1;

            Resize(88 + (StringCount * 36));
            WriteHeader(Length - 8, PacketType.MSG_FAMILY);

            int offset = 20 + ((StringCount - 1) * 36);
            WriteString(szName, 16, offset);
            WriteByte(pLevel, offset + 16);
            WriteUShort((ushort)pRank, offset + 20);
            WriteBoolean(bOnline, offset + 22);
            WriteUShort(pProf, offset + 24);
            // WriteUShort(0, offset + 28);
            WriteUInt(dwDonation, offset + 32);
        }

        // must set the correct type to ally or enemy
        public void AddRelation(uint idFamily, string szName, string szLeader)
        {
            StringCount += 1;

            Resize(88 + (StringCount * 36));
            WriteHeader(Length - 8, PacketType.MSG_FAMILY);

            int offset = 20 + ((StringCount - 1) * 56);
            WriteUInt((uint)(StringCount + 100), offset);
            WriteString(szName, 16, offset + 4);
            WriteString(szLeader, 16, offset + 40);
        }
    }
}