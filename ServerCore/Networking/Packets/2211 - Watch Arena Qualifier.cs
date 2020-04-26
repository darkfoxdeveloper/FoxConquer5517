// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 
// Computer User: PrimeiroLogin
// File Created by:  Felipe Vieira Vendramini
// zfserver v2.5517 - ServerCore - 2211 - Watch Arena Qualifier.cs
// Last Edit: 2017/02/06 11:44
// Created: 2017/02/06 11:26

namespace ServerCore.Networking.Packets
{
    public sealed class MsgArenicWitness : PacketStructure
    {
        public const byte
            RequestView = 0,
            Watchers = 2,
            Leave = 3,
            Fighters = 4;

        public MsgArenicWitness()
            : base(PacketType.MSG_ARENIC_WITNESS, 98, 90)
        {
        }

        public MsgArenicWitness(byte[] pBuffer)
            : base(pBuffer)
        {
        }

        public ushort Action
        {
            get { return ReadUShort(4); }
            set { WriteUShort(value, 4); }
        }

        public uint Param
        {
            get {  return ReadUInt(6); }
            set {  WriteUInt(value, 6); }
        }

        public uint UserIdentity
        {
            get { return ReadUInt(10); }
            set { WriteUInt(value, 10); }
        }

        public uint WatcherCount
        {
            get { return ReadUInt(14); }
            set { WriteUInt(value, 14); }
        }

        public uint Cheers1
        {
            get { return ReadUInt(18); }
            set { WriteUInt(value, 18); }
        }

        public uint Cheers2
        {
            get { return ReadUInt(22); }
            set { WriteUInt(value, 22); }
        }

        public void AppendName(string szName)
        {
            int nOffset = (int) (26 + WatcherCount++ *32);
            WriteString(szName, 16, nOffset);
        }

        public void AppendName(string szName, uint dwLookface)
        {
            int nOffset = (int) (26 + WatcherCount++ *36);
            WriteUInt(dwLookface, nOffset);
            WriteString(szName, 16, nOffset + 4);
        }

        public void AppendName(string szName, uint dwLookface, uint idRole, uint dwLevel, uint dwProf, uint dwRank)
        {
            int nOffset = (int)(26 + WatcherCount++ * 36);
            WriteUInt(dwLookface, nOffset);
            WriteString(szName, 16, nOffset + 4);
            WriteUInt(idRole, nOffset + 20);
            WriteUInt(dwLevel, nOffset + 24);
            WriteUInt(dwProf, nOffset + 28);
            WriteUInt(dwRank, nOffset + 32);
        }
    }
}