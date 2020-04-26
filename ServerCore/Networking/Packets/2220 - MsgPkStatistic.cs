// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - 2220 - MsgPkStatistic.cs
// Last Edit: 2017/01/26 23:32
// Created: 2017/01/26 23:21
namespace ServerCore.Networking.Packets
{
    public sealed class MsgPkStatistic : PacketStructure
    {
        public MsgPkStatistic()
            : base(PacketType.MSG_PK_STATISTIC, 44, 36)
        {
            
        }

        public MsgPkStatistic(byte[] pBuffer)
            : base(pBuffer)
        {
            
        }

        public uint Subtype // prolly a type 
        {
            get { return ReadUInt(4); }
            set { WriteUInt(value, 4); }
        }

        public uint Values
        {
            get { return ReadUInt(8); }
            set { WriteUInt(value, 8); }
        }

        public uint MaxValues
        {
            get { return ReadUInt(12); }
            set { WriteUInt(value, 12 ); }
        }

        public void AddTarget(string szName, string szMap, uint lastDieTime, uint dwTimes, ushort usLostExp, ushort usLevel,
            uint dwBattlePower)
        {
            int nOffset = (int)(16 + MaxValues * 64);
            MaxValues += 1;
            Resize((int)(28 + MaxValues * 64));
            WriteHeader(Length-8, PacketType.MSG_PK_STATISTIC);
            WriteString(szName, 16, nOffset);
            WriteUInt(dwTimes, nOffset + 16);
            WriteUInt(lastDieTime, nOffset + 20);
            //WriteUShort(usLostExp, nOffset + 20);
            //WriteUShort(usLevel, nOffset + 22);
            WriteString(szMap, 16, nOffset + 24);
            WriteString(szName, 16, nOffset + 40);
            //WriteUInt(2020, nOffset + 40);
            //WriteUInt(1010, nOffset + 44);
            //WriteUInt(505, nOffset + 48);
            //WriteUInt(4654, nOffset + 52);
            //WriteUInt(6548, nOffset + 56);
            //WriteUInt((uint) Common.UnixTimestamp.Timestamp(), nOffset + 40);
            WriteUInt(dwBattlePower, nOffset + 60);
        }
    }
}