// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2320 - Subclass Packet.cs
// Last Edit: 2016/11/23 09:45
// Created: 2016/11/23 09:44
namespace ServerCore.Networking.Packets
{
    public enum SubClassActions : ushort
    {
        SWITCH = 0,
        ACTIVATE = 1,
        REQUEST_UPLEV = 2,
        MARTIAL_UPLEV = 3,
        LEARN = 4,
        MARTIAL_PROMOTED = 5,
        INFO = 6,
        SHOW_GUI = 7,
        UPDATE_STUDY = 8
    }

    public enum SubClasses : byte
    {
        NONE = 0,
        MARTIAL_ARTIST = 1,
        WARLOCK = 2,
        CHI_MASTER = 3,
        SAGE = 4,
        APOTHECARY = 5,
        PERFORMER = 6,
        WRANGLER = 9
    }

    public sealed class MsgSubPro : PacketStructure
    {
        public MsgSubPro()
            : base(PacketType.MSG_SUB_PRO, 37, 29)
        {

        }

        public MsgSubPro(byte[] packet)
            : base(packet) { }

        public SubClassActions Action
        {
            get { return (SubClassActions)ReadUShort(4); }
            set { WriteUShort((ushort)value, 4); }
        }

        public SubClasses Subclass
        {
            get { return (SubClasses)ReadByte(6); }
            set { WriteByte((byte)value, 6); }
        }

        public ulong StudyPoints
        {
            get { return ReadULong(6); }
            set { WriteULong(value, 6); }
        }

        public uint AwardedStudy
        {
            get { return ReadUInt(14); }
            set { WriteUInt(value, 14); }
        }

        private byte m_amount = 0;
        public byte Amount
        {
            get { return ReadByte(22); }
            set
            {
                m_amount = value;
                Resize(37 + (value * 3));
                WriteHeader(Length - 8, PacketType.MSG_SUB_PRO);
                WriteByte(m_amount, 22);
            }
        }

        public void Append(SubClasses sClass, byte level, byte phase)
        {
            Amount += 1;
            var offset = (ushort)(26 + (Amount - 1) * 3);
            WriteByte((byte)sClass, offset++);
            WriteByte(level, offset++);
            WriteByte(phase, offset);
        }
    }
}