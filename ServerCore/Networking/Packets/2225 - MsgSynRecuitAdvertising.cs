// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2225 - MsgSynRecuitAdvertising.cs
// Last Edit: 2016/12/07 21:58
// Created: 2016/12/07 21:50
namespace ServerCore.Networking.Packets
{
    public sealed class MsgSynRecuitAdvertising : PacketStructure
    {
        public MsgSynRecuitAdvertising()
             : base(PacketType.MSG_SYN_RECUIT_ADVERTISING, 288, 280)
        {
            
        }

        public MsgSynRecuitAdvertising(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public uint Identity
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public string Description
        {
            get { return ReadString(256, 8); }
            set
            {
                if (value.Length > 255)
                    WriteString(value.Substring(0, 255), 256, 8);
                else 
                    WriteString(value, 255, 8);
            }
        }

        public ulong Amount
        {
            get { return ReadULong(264); }
            set { WriteULong(value, 264); }
        }

        public bool IsAutoRecruit
        {
            get { return ReadBoolean(272); }
            set { WriteBoolean(value, 272); }
        }

        public ushort LevelRequirement
        {
            get { return ReadUShort(274); }
            set { WriteUShort(value, 274); }
        }

        public ushort RebornRequirement
        {
            get { return ReadUShort(276); }
            set { WriteUShort(value, 276); }
        }

        public ushort ProfessionForbid
        {
            get { return ReadUShort(278); }
            set { WriteUShort(value, 278); }
        }

        public ushort GenderForbid
        {
            get { return ReadUShort(280); }
            set { WriteUShort(value, 280); }
        }

        public ushort Unknown
        {
            get { return ReadUShort(282); }
            set { WriteUShort(value, 282); }
        }
    }
}