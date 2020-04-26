// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 1107 - Syndicate Request.cs
// Last Edit: 2016/11/23 08:47
// Created: 2016/11/23 08:46

using System.Collections.Generic;
using System.Linq;
using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgSyndicate : PacketStructure
    {
        public MsgSyndicate()
            : base(PacketType.MSG_SYNDICATE, 36, 28)
        {

        }

        public MsgSyndicate(string name)
            : base(PacketType.MSG_SYNDICATE, 52, 44)
        {
            Name = name;
        }

        public MsgSyndicate(byte[] packet)
            : base(packet)
        {

        }

        public SyndicateRequest Action
        {
            get { return (SyndicateRequest)ReadUInt(4); }
            set { WriteUInt((uint)value, 4); }
        }

        public uint Param
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint RequiredLevel
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public uint RequiredMetempsychosis
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public uint RequiredProfession
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public byte DwInfo
        {
            get { return ReadByte(24); }
            set { WriteByte(value, 24); }
        }

        public string Name
        {
            get { return ReadString(ReadByte(25), 26); }
            set
            {
                Resize(52);
                WriteHeader(Length - 8, PacketType.MSG_SYNDICATE);
                WriteStringWithLength(value, value.Length);
            }
        }

        public List<string> Positions = new List<string>();

        /// <summary>
        /// This method will fill the buffer with all positions listed on the dictionary.
        /// </summary>
        public void SetList()
        {
            int len = Positions.Sum(pos => pos.Length);
            Resize(28 + 16 + 8 + len);
            WriteHeader(Length - 8, PacketType.MSG_SYNDICATE);
            DwInfo = (byte)Positions.Count;
            ushort start = 25;
            foreach (var Pos in Positions)
            {
                WriteStringWithLength(Pos, start);
                start = (ushort)(start + Pos.Length + 1);
            }
        }
    }
}