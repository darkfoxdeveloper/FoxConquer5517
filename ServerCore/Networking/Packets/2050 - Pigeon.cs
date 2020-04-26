// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2050 - Pigeon.cs
// Last Edit: 2016/11/23 09:11
// Created: 2016/11/23 09:11

using System.Collections.Generic;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgPigeon : PacketStructure
    {
        public const byte RELEASE_SOON = 1,
            MY_RELEASE = 2,
            BROADCAST_SEND = 3,
            URGENT_15_CPS = 4,
            URGENT_5_CPS = 5;

        public MsgPigeon()
            : base(PacketType.MSG_PIGEON, 24, 16)
        {

        }

        public MsgPigeon(byte[] msg)
            : base(msg)
        {

        }

        public byte Type
        {
            get { return ReadByte(4); }
            set { WriteByte(value, 4); }
        }

        public uint DwParam
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public byte StringCount
        {
            get { return ReadByte(12); }
            set
            {
                int newSize = 16 + value + m_totalStringLength + 8;
                Resize(newSize);
                WriteHeader(newSize - 8, PacketType.MSG_PIGEON);
                WriteByte(value, 12);
            }
        }

        private int m_totalStringLength = 0;

        public void AddString(string text)
        {
            m_totalStringLength += text.Length;
            StringCount += 1;
            var offset = (ushort)(13 + (StringCount - 1) + (m_totalStringLength - text.Length));
            WriteStringWithLength(text, offset);
        }

        public List<string> ToList()
        {
            var temp = new List<string>();

            int offset = 13;
            for (int i = 0; i < StringCount; i++)
            {
                string szTemp = ReadString(ReadByte(offset++), offset);
                temp.Add(szTemp);
                offset += szTemp.Length;
            }

            return temp;
        }
    }
}