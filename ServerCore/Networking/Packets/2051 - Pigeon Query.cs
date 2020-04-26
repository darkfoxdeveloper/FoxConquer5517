// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2051 - Pigeon Query.cs
// Last Edit: 2016/11/23 09:11
// Created: 2016/11/23 09:11

using System;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgPigeonQuery : PacketStructure
    {
        public MsgPigeonQuery(byte[] msg)
            : base(msg)
        {

        }

        public MsgPigeonQuery()
            : base(PacketType.MSG_PIGEON_QUERY, 20, 12)
        {

        }

        public uint Param
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public ushort Total
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }

        public ushort Amount
        {
            get { return ReadUShort(10); }
            set
            {
                Resize((12 + (value * 112) + 8));
                WriteHeader(Length - 8, PacketType.MSG_PIGEON_QUERY);
                WriteUShort(value, 10);
            }
        }

        public void AddBroadcast(uint dwBcId, uint dwPos, uint dwPlayer, string szName,
            uint dwSpentCps, string szMessage)
        {
            if (Amount >= 8)
            {
                Console.WriteLine("System tried to exceed the maximum broadcast queue packet size.");
                return;
            }
            int offset = 12 + Amount * 112;
            Amount += 1;
            WriteUInt(dwBcId, offset);
            WriteUInt(dwPos, offset + 4);
            WriteUInt(dwPlayer, offset + 8);
            WriteString(szName, 16, offset + 12);
            WriteUInt(dwSpentCps, offset + 28);
            WriteString(szMessage, 80, offset + 32);
        }
    }
}
