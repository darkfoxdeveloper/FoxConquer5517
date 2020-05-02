// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2205 - Arena Check.cs
// Last Edit: 2016/12/22 18:48
// Created: 2016/11/23 09:41
namespace ServerCore.Networking.Packets
{
    public sealed class MsgQualifyingInteractive : PacketStructure
    {
        public MsgQualifyingInteractive()
            : base(PacketType.MSG_QUALIFYING_INTERACTIVE, 64, 56)
        {

        }

        public MsgQualifyingInteractive(byte[] pBuffer)
            : base(pBuffer)
        {

        }

        public ArenaType Type
        {
            get { return (ArenaType)ReadUInt(4); }
            set { WriteUInt((uint)value, 4); }
        }

        public uint Option
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint Identity
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public string Name
        {
            get { return ReadString(16, 16); }
            set { WriteString(value, 16, 16); }
        }

        public uint Rank
        {
            get { return ReadUInt(32); }
            set { WriteUInt(value, 32); }
        }

        public uint Profession
        {
            get { return ReadUInt(36); }
            set { WriteUInt(value, 36); }
        }

        public uint Unknown
        {
            get { return ReadUInt(40); }
            set { WriteUInt(value, 40); }
        }

        public uint ArenaPoints
        {
            get { return ReadUInt(44); }
            set { WriteUInt(value, 44); }
        }

        public uint Level
        {
            get { return ReadUInt(48); }
            set { WriteUInt(value, 48); }
        }
    }

    public enum ArenaType
    {
        ARENA_ICON_ON = 0,
        ARENA_ICON_OFF = 1,
        START_COUNT_DOWN = 2,
        ACCEPT_DIALOG = 3,
        OPPONENT_GAVE_UP = 4,
        BUY_ARENA_POINTS = 5,
        MATCH = 6,
        YOU_ARE_KICKED = 7,
        START_THE_FIGHT = 8,
        DIALOG = 9,
        END_DIALOG = 10,
        END_MATCH_JOIN = 11
    }
}