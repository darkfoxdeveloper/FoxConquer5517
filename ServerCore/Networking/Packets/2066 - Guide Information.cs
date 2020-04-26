// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2066 - Guide Information.cs
// Last Edit: 2016/11/23 09:15
// Created: 2016/11/23 09:14

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public class MsgGuideInfo : PacketStructure
    {
        public MsgGuideInfo()
            : base(PacketType.MSG_GUIDE_INFO, 80, 72)
        {

        }

        public MsgGuideInfo(byte[] pBuffer)
            : base(pBuffer)
        {
            RemainingTime = 999999;
        }

        public uint Type
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Identity
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint TargetIdentity
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint TargetMesh
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public uint SharedBattlePower
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public uint RemainingTime
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        public uint EnroleDate
        {
            get { return ReadUInt(28); }
            set { WriteUInt(value, 28); }
        }

        public byte TargetLevel
        {
            get { return ReadByte(32); }
            set { WriteByte(value, 32); }
        }

        public ProfessionType TargetProfession
        {
            get { return (ProfessionType)ReadByte(33); }
            set { WriteByte((byte)value, 33); }
        }

        public ushort TargetPkPoint
        {
            get { return ReadUShort(34); }
            set { WriteUShort(value, 34); }
        }

        public ushort SyndicateIdentity
        {
            get { return ReadUShort(36); }
            set { WriteUShort(value, 36); }
        }

        public SyndicateRank SyndicatePosition
        {
            get { return (SyndicateRank)ReadByte(39); }
            set { WriteByte((byte)value, 39); }
        }

        public bool TargetOnline
        {
            get { return ReadBoolean(56); }
            set { WriteBoolean(value, 56); }
        }

        public uint ApprenticeExperience
        {
            get { return ReadUInt(56); }
            set { WriteUInt(value, 56); }
        }

        public ushort ApprenticeBlessing
        {
            get { return ReadUShort(64); }
            set { WriteUShort(value, 64); }
        }

        public ushort ApprenticeComposing
        {
            get { return ReadUShort(66); }
            set { WriteUShort(value, 66); }
        }

        public byte StringCount
        {
            get { return ReadByte(76); }
            set
            {
                int newSize = 80 + value + m_totalStringLength + 8;
                Resize(newSize);
                WriteHeader(newSize - 8, PacketType.MSG_GUIDE_INFO);
                WriteByte(value, 76);
            }
        }

        public void AddString(string szName)
        {
            if (StringCount >= 3)
                return;

            m_totalStringLength += szName.Length;
            StringCount += 1;
            var offset = (ushort)(77 + (StringCount - 1) + (m_totalStringLength - szName.Length));
            WriteStringWithLength(szName, offset);
        }

        private int m_totalStringLength = 0;

        public string MentorName
        {
            get
            {
                if (StringCount <= 0 || Length < 77)
                    return string.Empty;
                int txtLength = ReadByte(77);
                if (Length < 77 + txtLength)
                    return string.Empty;
                return ReadString(ReadByte(77), 78);
            }
        }

        public string MentorSpouse
        {
            get
            {
                if (StringCount <= 1 || Length < 78 + MentorName.Length)
                    return string.Empty;
                int txtLength = ReadByte(78 + MentorName.Length);
                int startOffset = 78 + MentorName.Length;
                if (Length < 79 + MentorName.Length + txtLength)
                    return string.Empty;
                return ReadString(ReadByte(startOffset), startOffset + 1);
            }
        }

        public string ApprenticeName
        {
            get
            {
                if (StringCount <= 2 || Length < 79 + MentorName.Length + MentorSpouse.Length)
                    return string.Empty;
                int txtLength = ReadByte(79 + MentorName.Length + MentorSpouse.Length);
                int startOffset = 79 + MentorName.Length + MentorSpouse.Length;
                if (Length < 80 + MentorName.Length + MentorSpouse.Length + txtLength)
                    return string.Empty;
                return ReadString(ReadByte(startOffset), startOffset + 1);
            }
        }
    }
}