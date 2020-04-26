// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2064 - Peerage.cs
// Last Edit: 2016/11/23 09:12
// Created: 2016/11/23 09:12

using ServerCore.Common.Enums;

namespace ServerCore.Networking.Packets
{
    public sealed class MsgPeerage : PacketStructure
    {
        private int DATA_OFFSET = 32;

        public MsgPeerage(byte[] buffer)
            : base(buffer)
        {

        }

        public MsgPeerage()
            : base(PacketType.MSG_PEERAGE, 88, 80)
        {

        }

        public MsgPeerage(int size)
            : base(PacketType.MSG_PEERAGE, size + 8, size)
        {

        }

        public NobilityAction Action
        {
            get { return (NobilityAction)ReadUInt(4); }
            set { WriteUInt((uint)value, 4); }
        }

        /// <summary>
        /// ushort offset 8
        /// </summary>
        public ushort Data
        {
            get { return ReadUShort(8); }
            set { WriteUShort(value, 8); }
        }

        /// <summary>
        /// long offset 8
        /// </summary>
        public long DataLong
        {
            get { return ReadLong(8); }
            set { WriteLong(value, 8); }
        }

        /// <summary>
        /// ushort offset 10
        /// </summary>
        public ushort DataShort
        {
            get { return ReadUShort(10); }
            set { WriteUShort(value, 10); }
        }

        public uint DataLow
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint DataHigh
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12); }
        }

        public ushort DataHighLow
        {
            get { return ReadUShort(12); }
            set { WriteUShort(value, 12); }
        }

        public ushort DataHighHigh
        {
            get { return ReadUShort(14); }
            set { WriteUShort(value, 14); }
        }

        /// <summary>
        /// Uint offset 16
        /// </summary>
        public uint Data2
        {
            get { return ReadUInt(16); }
            set { WriteUInt(value, 16); }
        }

        public uint Data3
        {
            get { return ReadUInt(20); }
            set { WriteUInt(value, 20); }
        }

        public uint Data4
        {
            get { return ReadUInt(24); }
            set { WriteUInt(value, 24); }
        }

        private int _dataAmount;
        private int DATA_AMOUNT
        {
            get { return _dataAmount; }
            set
            {
                _dataAmount = value;
                Resize(32 + (value * 48) + 8);
                WriteHeader(Length - 8, PacketType.MSG_PEERAGE);
            }
        }

        public void WriteNobilityData(uint UserId, uint Mesh, string Name, long Donation,
            NobilityLevel RankType, int Ranking)
        {
            DATA_AMOUNT += 1;
            int offset = DATA_OFFSET + ((DATA_AMOUNT - 1) * 48);
            WriteUInt(UserId, offset);
            WriteUInt(Mesh, offset + 4);
            WriteUInt(Mesh, offset + 8);
            WriteString(Name, 16, offset + 12);
            WriteUInt(0, offset + 28);
            WriteLong((long)Donation, offset + 32);
            WriteUInt((uint)RankType, offset + 40);
            WriteInt(Ranking, offset + 44);
        }
    }
}